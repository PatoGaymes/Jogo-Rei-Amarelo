using System;
using System.Collections.Generic;
using Godot;

// Alteração de IA - Revisar
// O que faz: a camada pintada por cima da imagem do jogo que esconde o que o personagem não
//            enxerga — seja névoa ou escuridão, conforme o ambiente da fase.
// Por quê: o mapa é 3D visto de cima, então a câmera mostraria o que está atrás das paredes e do
//          outro lado da sala. Esta camada devolve ao jogador só o que o personagem veria.
//
//          "Névoa de guerra" é o nome do **sistema** (o termo usado em jogos para "o que você não
//          sabe o que tem"). Névoa e escuridão são os dois **ambientes** que ele sabe desenhar —
//          quem decide qual vale é o nó AmbienteDaFase do mapa. Mapa sem esse nó é limpo, e esta
//          camada simplesmente não aparece nem gasta nada.
//
//          Quem decide o que é enxergado é o SensorDeteccao do personagem; quem pinta é
//          Shaders/NevoaDeGuerra.gdshader. Este script é a ponte, e existe para que haja uma fonte
//          de verdade só: mudou o alcance da visão no personagem, a névoa acompanha.
public partial class NevoaDeGuerra : MeshInstance3D
{
	// Alteração de IA - Revisar
	// O que faz: quantas linhas imaginárias saem do personagem, em roda, para medir onde estão
	//            as paredes em cada direção.
	// Por quê: **é o que faz a parede esconder o que está atrás dela.** 128 dá cerca de 3 graus
	//          entre uma linha e outra, o que a olho nu já fica redondo.
	[Export]
	public int QuantidadeDeRaios { get; set; } = 128;

	// Alteração de IA - Revisar
	// O que faz: a folga dada além da parede antes de considerar que algo está escondido.
	// Por quê: a própria parede precisa continuar aparecendo — ela é o que o personagem enxerga.
	[Export]
	public float FolgaDaParede { get; set; } = 0.8f;

	[Export(PropertyHint.Range, "0,6,0.1")]
	public float ForcaDoBorrao { get; set; } = 3.0f;

	// Alteração de IA - Revisar
	// O que faz: o máximo de fontes de luz consideradas ao mesmo tempo, e quantas linhas cada uma
	//            usa para saber onde as paredes a bloqueiam.
	// Por quê: a conta de cada ponto da tela percorre as luzes uma a uma; limitar às 16 mais
	//          perto do jogador mantém o custo fixo, não importa quantas lamparinas o mapa tenha.
	//          Luz parada mede suas paredes uma única vez; só luz que anda (tocha na mão) mede de
	//          novo, e só quando se move.
	private const int MaximoDeLuzes = 16;
	private const int RaiosPorLuz = 64;

	private SensorDeteccao? _sensor;
	private PlayerIsometrico? _jogador;
	private AmbienteDaFase? _ambiente;
	private ShaderMaterial _material = null!;

	// paredes em volta do jogador
	private float[] _paredesDoJogador = Array.Empty<float>();
	private byte[] _bytesDoJogador = Array.Empty<byte>();
	private Image? _imagemDoJogador;
	private ImageTexture? _texturaDoJogador;

	// paredes em volta de cada luz (uma linha da imagem por luz)
	private readonly float[] _paredesDasLuzes = new float[MaximoDeLuzes * RaiosPorLuz];
	private readonly byte[] _bytesDasLuzes = new byte[MaximoDeLuzes * RaiosPorLuz * sizeof(float)];
	private Image? _imagemDasLuzes;
	private ImageTexture? _texturaDasLuzes;
	private readonly FonteDeLuz?[] _luzNaVaga = new FonteDeLuz?[MaximoDeLuzes];
	private readonly Vector3[] _ondeAVagaMediu = new Vector3[MaximoDeLuzes];
	private readonly Vector4[] _posicoesDasLuzes = new Vector4[MaximoDeLuzes];
	private readonly Vector4[] _coresDasLuzes = new Vector4[MaximoDeLuzes];
	private int _quantasLuzes;

	private ImageTexture? _texturaDaNevoa;
	private float _tempo;

	// Alteração de IA - Revisar
	// O que faz: uma única "pergunta ao mundo" reaproveitada para todas as linhas até as paredes,
	//            e a lembrança de onde o jogador estava na última medição.
	// Por quê: **desempenho.** Criar uma pergunta nova para cada uma das 128 linhas, 60 vezes por
	//          segundo, custava mais do que as linhas em si. E parede não anda: se o jogador está
	//          parado, a medida anterior continua certa e não há por que refazê-la. Andando, ela é
	//          refeita um passo sim, um não — 30 vezes por segundo, o que a olho nu é igual.
	private readonly PhysicsRayQueryParameters3D _consulta = new() { HitFromInside = false };
	private Vector3 _ondeMediuOJogador = new(float.MaxValue, 0.0f, 0.0f);
	private float _alcanceMedido;
	private bool _pularEstePasso;

	public override void _Ready()
	{
		// Alteração de IA - Revisar
		// O que faz: monta uma tela plana presa à câmera, que o desenho estica para cobrir a tela.
		// Por quê: a margem enorme impede que ela seja descartada por "estar fora de vista".
		Mesh = new QuadMesh { Size = new Vector2(2.0f, 2.0f) };
		ExtraCullMargin = 16384.0f;
		CastShadow = ShadowCastingSetting.Off;
		Position = new Vector3(0.0f, 0.0f, -0.5f);
		Visible = false;

		var desenho = GD.Load<Shader>("res://Shaders/NevoaDeGuerra.gdshader");
		if (desenho == null)
		{
			GD.PushWarning("NevoaDeGuerra: não achei 'res://Shaders/NevoaDeGuerra.gdshader'.");
			return;
		}

		// desenhada por último, por cima de tudo: lê a imagem já pronta
		_material = new ShaderMaterial { Shader = desenho, RenderPriority = 100 };
		MaterialOverride = _material;

		var achados = GetTree().GetNodesInGroup("player");
		if (achados.Count > 0)
		{
			_jogador = achados[0] as PlayerIsometrico;
			_sensor = (achados[0] as Node)?.GetNodeOrNull<SensorDeteccao>("SensorDeteccao");
		}

		PrepararTexturas();
	}

	// Alteração de IA - Revisar
	// O que faz: prepara as tiras de imagem onde as distâncias até as paredes são entregues ao
	//            desenho, e gera **uma única vez** o desenho de ruído que dá forma à névoa.
	// Por quê: o desenho da tela só consegue consultar números rapidamente se eles vierem como
	//          imagem. E a névoa que se move é esse mesmo ruído deslizando pelo mapa — gerado uma
	//          vez só, no início, em vez de calculado a cada ponto da tela a cada quadro. É isso
	//          que deixa a névoa volátil praticamente de graça.
	private void PrepararTexturas()
	{
		int raios = Mathf.Max(8, QuantidadeDeRaios);
		_paredesDoJogador = new float[raios];
		_bytesDoJogador = new byte[raios * sizeof(float)];
		_imagemDoJogador = Image.CreateEmpty(raios, 1, false, Image.Format.Rf);
		_texturaDoJogador = ImageTexture.CreateFromImage(_imagemDoJogador);

		_imagemDasLuzes = Image.CreateEmpty(RaiosPorLuz, MaximoDeLuzes, false, Image.Format.Rf);
		_texturaDasLuzes = ImageTexture.CreateFromImage(_imagemDasLuzes);

		var ruido = new FastNoiseLite
		{
			NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth,
			Frequency = 0.012f,
			FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
			FractalOctaves = 3,
			Seed = 7
		};
		// "sem emenda": a imagem se repete lado a lado sem deixar costura visível no mapa
		Image imagem = ruido.GetSeamlessImage(256, 256);
		_texturaDaNevoa = ImageTexture.CreateFromImage(imagem);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!EstaAtiva())
		{
			return;
		}

		MedirParedesDoJogador();
		EscolherEMedirLuzes();
	}

	public override void _Process(double delta)
	{
		_ambiente ??= AmbienteDaFase.DaFase(GetTree());

		bool ativa = EstaAtiva();
		Visible = ativa;
		if (!ativa)
		{
			return;
		}

		_tempo += (float)delta;
		var amb = _ambiente!;
		var s = _sensor!;

		_material.SetShaderParameter("modo", amb.Tipo == AmbienteDaFase.TipoDeAmbiente.Escuridao ? 2 : 1);
		_material.SetShaderParameter("jogador", s.GlobalPosition);
		_material.SetShaderParameter("olhando", Mathf.DegToRad(_jogador?.DirecaoOlhando ?? 270.0f));
		_material.SetShaderParameter("meia_abertura", Mathf.DegToRad(s.AberturaVisao * 0.5f));
		_material.SetShaderParameter("passiva_clara", s.VisaoPassiva1);
		_material.SetShaderParameter("passiva_embacada", s.VisaoPassiva2);
		_material.SetShaderParameter("cone_claro", s.AlcanceVisao1);
		_material.SetShaderParameter("cone_embacado", s.AlcanceVisao2);
		_material.SetShaderParameter("cone_vulto", s.AlcanceVisao3);
		_material.SetShaderParameter("paredes", _texturaDoJogador!);
		_material.SetShaderParameter("folga_da_parede", FolgaDaParede);
		_material.SetShaderParameter("forca_do_borrao", ForcaDoBorrao);

		_material.SetShaderParameter("ruido", _texturaDaNevoa!);
		_material.SetShaderParameter("tempo", _tempo);
		_material.SetShaderParameter("cor_da_nevoa", amb.CorDaNevoa);
		_material.SetShaderParameter("densidade_minima", amb.DensidadeMinima);
		_material.SetShaderParameter("densidade_maxima", amb.DensidadeMaxima);
		_material.SetShaderParameter("tamanho_das_massas", Mathf.Max(1.0f, amb.TamanhoDasMassas));
		_material.SetShaderParameter("vento", amb.Vento);

		_material.SetShaderParameter("cor_da_escuridao", amb.CorDaEscuridao);
		_material.SetShaderParameter("luz_ambiente", amb.LuzAmbiente);
		_material.SetShaderParameter("paredes_das_luzes", _texturaDasLuzes!);
		_material.SetShaderParameter("quantas_luzes", _quantasLuzes);
		_material.SetShaderParameter("luzes", _posicoesDasLuzes);
		_material.SetShaderParameter("cores_das_luzes", _coresDasLuzes);
	}

	// Alteração de IA - Revisar
	// O que faz: responde se a camada deve aparecer agora.
	// Por quê: em mapa limpo ela some **e não calcula nada** — nem as linhas até as paredes. É o
	//          que garante que ter o sistema no jogo não custa desempenho onde ele não é usado.
	private bool EstaAtiva()
	{
		return _sensor != null && _texturaDoJogador != null && _texturaDaNevoa != null &&
			   _ambiente != null && _ambiente.EscondeOQueNaoSeVe;
	}

	// Alteração de IA - Revisar
	// O que faz: mede, em cada direção em volta do personagem, a que distância está a parede.
	// Por quê: roda junto com a física porque perguntar ao mundo "tem parede aqui?" fora da hora
	//          da física dá resposta instável.
	private void MedirParedesDoJogador()
	{
		Vector3 olhos = _sensor!.PosicaoDosOlhos;
		float alcance = Mathf.Max(1.0f, _sensor.AlcanceVisao3);

		bool parado = olhos.DistanceTo(_ondeMediuOJogador) < 0.03f && Mathf.IsEqualApprox(alcance, _alcanceMedido);
		if (parado)
		{
			return;   // nada mudou: a medida anterior continua valendo
		}

		_pularEstePasso = !_pularEstePasso;
		bool primeiraVez = _ondeMediuOJogador.X == float.MaxValue;
		if (_pularEstePasso && !primeiraVez)
		{
			return;   // andando: mede um passo sim, um não
		}

		_ondeMediuOJogador = olhos;
		_alcanceMedido = alcance;

		var espaco = GetWorld3D().DirectSpaceState;
		MedirEmRoda(espaco, olhos, alcance, _paredesDoJogador, 0, _paredesDoJogador.Length);

		Buffer.BlockCopy(_paredesDoJogador, 0, _bytesDoJogador, 0, _bytesDoJogador.Length);
		_imagemDoJogador!.SetData(_paredesDoJogador.Length, 1, false, Image.Format.Rf, _bytesDoJogador);
		_texturaDoJogador!.Update(_imagemDoJogador);
	}

	private void MedirEmRoda(PhysicsDirectSpaceState3D espaco, Vector3 origem, float alcance,
							 float[] destino, int inicio, int quantos)
	{
		_consulta.CollisionMask = (uint)_sensor!.CamadaQueBloqueiaVisao;
		_consulta.From = origem;
		for (int i = 0; i < quantos; i++)
		{
			float angulo = Mathf.Tau * i / quantos;
			var rumo = new Vector3(Mathf.Cos(angulo), 0.0f, -Mathf.Sin(angulo));
			_consulta.To = origem + rumo * alcance;

			var achou = espaco.IntersectRay(_consulta);
			float distancia = alcance;
			if (achou.Count > 0)
			{
				var ponto = (Vector3)achou["position"];
				distancia = new Vector2(ponto.X - origem.X, ponto.Z - origem.Z).Length();
			}
			destino[inicio + i] = distancia;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: escolhe as luzes acesas mais perto do jogador e mede as paredes em volta de cada
	//            uma — só quando a luz é nova na lista ou se moveu.
	// Por quê: é o que impede a luz de atravessar parede. Uma fogueira do outro lado do muro não
	//          clareia o lado de cá. Luz parada é medida uma vez e nunca mais; só a tocha que
	//          anda com o personagem é medida de novo, e só quando se move.
	private void EscolherEMedirLuzes()
	{
		_quantasLuzes = 0;
		if (_ambiente!.Tipo != AmbienteDaFase.TipoDeAmbiente.Escuridao || _ambiente.ApagarTodasAsLuzes)
		{
			return;
		}

		Vector3 jogador = _sensor!.GlobalPosition;
		var acesas = new List<FonteDeLuz>();
		foreach (Node no in GetTree().GetNodesInGroup("fonte_de_luz"))
		{
			if (no is FonteDeLuz luz && luz.Acesa && luz.Intensidade > 0.0f && luz.IsVisibleInTree())
			{
				acesas.Add(luz);
			}
		}
		acesas.Sort((a, b) => a.GlobalPosition.DistanceSquaredTo(jogador)
							   .CompareTo(b.GlobalPosition.DistanceSquaredTo(jogador)));

		var espaco = GetWorld3D().DirectSpaceState;
		bool mudou = false;

		for (int i = 0; i < Mathf.Min(acesas.Count, MaximoDeLuzes); i++)
		{
			FonteDeLuz luz = acesas[i];
			Vector3 onde = luz.GlobalPosition;

			if (_luzNaVaga[i] != luz || _ondeAVagaMediu[i].DistanceTo(onde) > 0.1f)
			{
				MedirEmRoda(espaco, onde, Mathf.Max(1.0f, luz.Raio * 1.1f), _paredesDasLuzes,
							i * RaiosPorLuz, RaiosPorLuz);
				_luzNaVaga[i] = luz;
				_ondeAVagaMediu[i] = onde;
				mudou = true;
			}

			_posicoesDasLuzes[i] = new Vector4(onde.X, onde.Y, onde.Z, luz.RaioAgora);
			_coresDasLuzes[i] = new Vector4(luz.CorDaLuz.R, luz.CorDaLuz.G, luz.CorDaLuz.B, luz.Intensidade);
			_quantasLuzes++;
		}

		if (mudou)
		{
			Buffer.BlockCopy(_paredesDasLuzes, 0, _bytesDasLuzes, 0, _bytesDasLuzes.Length);
			_imagemDasLuzes!.SetData(RaiosPorLuz, MaximoDeLuzes, false, Image.Format.Rf, _bytesDasLuzes);
			_texturaDasLuzes!.Update(_imagemDasLuzes);
		}
	}
}

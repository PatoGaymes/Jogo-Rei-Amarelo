using System;
using Godot;

// Alteração de IA - Revisar
// O que faz: liga a névoa da tela ao personagem — a cada instante, avisa ao desenho onde o
//            jogador está, para onde está olhando e até onde ele enxerga.
// Por quê: quem decide o que é enxergado é o `SensorDeteccao` do personagem, e quem pinta é o
//          arquivo de desenho (`Shaders/NevoaDeGuerra.gdshader`). Este script é a ponte entre os
//          dois, e existe para que **exista uma fonte de verdade só**: se a equipe mudar o
//          alcance da visão no personagem, a névoa acompanha sozinha.
//
//          É uma tela plana pendurada na frente da câmera, desenhada por último, por cima de
//          tudo. Ela se cria sozinha em código, sem precisar ser montada na cena.
public partial class NevoaDeGuerra : MeshInstance3D
{
	// Alteração de IA - Revisar
	// O que faz: a cor da névoa.
	// Por quê: **é o que decide se a cena é neblina ou escuridão.** Cinza claro dá névoa de
	//          exterior; quase preto dá porão sem lamparina. A mesma mecânica serve para os dois,
	//          mudando só esta cor — por isso ela fica ajustável e não fixa no desenho.
	[Export]
	public Color CorDaNevoa { get; set; } = new(0.13f, 0.14f, 0.18f);

	// Alteração de IA - Revisar
	// O que faz: o quanto a névoa chega a encobrir, de 0 a 1.
	// Por quê: em 1 o que está fora da visão some por completo. Um pouco abaixo disso deixa
	//          adivinhar contornos, o que ajuda a não se perder no mapa. É um dos números que a
	//          equipe vai querer sentir jogando.
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float ForcaDaNevoa { get; set; } = 0.93f;

	[Export(PropertyHint.Range, "0,6,0.1")]
	public float ForcaDoBorrao { get; set; } = 3.2f;

	// Alteração de IA - Revisar
	// O que faz: liga e desliga a névoa durante o jogo, na tecla F4.
	// Por quê: com a névoa ligada não dá para conferir se o inimigo está fazendo a ronda certa
	//          do outro lado da sala — é justamente o que ela esconde. A tecla permite espiar o
	//          mapa inteiro durante um teste e voltar. **É ferramenta de teste**, diferente da
	//          névoa em si, que é mecânica de jogo.
	[Export]
	public bool Ligada { get; set; } = true;

	// Alteração de IA - Revisar
	// O que faz: quantas linhas imaginárias saem do personagem, em roda, para medir onde estão
	//            as paredes em cada direção.
	// Por quê: **é isto que faz a parede esconder o que está atrás dela.** Sem isso, a névoa
	//          limparia tudo o que estivesse perto e dentro do cone, inclusive o cômodo do outro
	//          lado do muro — que é justamente o problema que ela veio resolver.
	//
	//          Mais linhas deixam o contorno das paredes mais fiel; menos linhas deixam os
	//          cantos serrilhados. 128 dá cerca de 3 graus entre uma e outra, o que a olho nu
	//          já fica redondo.
	[Export]
	public int QuantidadeDeRaios { get; set; } = 128;

	// Alteração de IA - Revisar
	// O que faz: a folga dada além da parede antes de considerar que algo está escondido.
	// Por quê: a própria parede precisa continuar aparecendo — ela é o que o personagem enxerga.
	//          Sem folga, a face dela ficaria escura, o que pareceria defeito. A folga é da ordem
	//          da espessura de uma parede.
	[Export]
	public float FolgaDaParede { get; set; } = 0.8f;

	// Alteração de IA - Revisar
	// O que faz: guarda tudo o que o personagem **já viu alguma vez**, e volta a mostrar esses
	//            lugares de forma apagada quando ele não está mais olhando.
	// Por quê: era o pedido, e é como funciona a cabeça de alguém. Você olhou para a casa: sabe
	//          que ela está ali mesmo de costas, porque casa não anda. Sem isso, virar de lado
	//          apagava o mundo inteiro e o jogador se perdia no próprio mapa.
	//
	//          **Só vale para o cenário.** Inimigos andam, então a memória deles é outra coisa e
	//          fica com cada inimigo (ver VisibilidadeDoInimigo).
	[Export]
	public bool LembrarDoCenario { get; set; } = true;

	// Alteração de IA - Revisar
	// O que faz: o quanto um lugar lembrado aparece, comparado a estar olhando para ele.
	// Por quê: lembrar não é ver. Em 0,45 o lugar fica reconhecível mas claramente mais apagado,
	//          e o jogador entende de relance que aquilo é memória, não informação de agora.
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float PesoDaMemoria { get; set; } = 0.45f;

	// Alteração de IA - Revisar
	// O que faz: o tamanho do quadrado do mapa coberto pela memória, e em quantos pedacinhos ele
	//            é dividido.
	// Por quê: a memória é guardada como uma grade sobre o mapa. Se o mapa for maior que este
	//          quadrado, as bordas não são lembradas; se a grade for fina demais, custa caro sem
	//          aparecer. 80 m em 256 pedaços dá cerca de 31 cm cada, que a olho nu já é liso.
	[Export]
	public float TamanhoDoMapa { get; set; } = 80.0f;

	[Export]
	public Vector2 CentroDoMapa { get; set; } = Vector2.Zero;

	[Export]
	public int ResolucaoDaMemoria { get; set; } = 256;

	// Alteração de IA - Revisar
	// O que faz: de quantos em quantos passos da física a memória é anotada.
	// Por quê: anotar a memória custa percorrer alguns milhares de pedacinhos da grade. Fazer
	//          isso 60 vezes por segundo é desperdício: ninguém percebe a diferença entre anotar
	//          agora ou daqui a três quadros.
	[Export]
	public int PassosEntreAnotacoes { get; set; } = 3;

	private SensorDeteccao? _sensor;
	private PlayerIsometrico? _jogador;
	private ShaderMaterial _material = null!;

	private float[] _distanciaAteAParede = Array.Empty<float>();
	private byte[] _bytesDasDistancias = Array.Empty<byte>();
	private Image? _imagemDasParedes;
	private ImageTexture? _texturaDasParedes;

	private byte[] _memoriaDoCenario = Array.Empty<byte>();
	private Image? _imagemDaMemoria;
	private ImageTexture? _texturaDaMemoria;
	private int _passosDesdeAUltimaAnotacao;
	private bool _memoriaMudou;

	public override void _Ready()
	{
		// Alteração de IA - Revisar
		// O que faz: monta uma tela plana de tamanho 2x2 presa à câmera.
		// Por quê: o desenho reposiciona cada canto para cobrir a tela inteira, então o tamanho
		//          e a posição aqui não importam para o resultado — o que importa é ela nunca
		//          ser descartada por estar fora de vista, e é para isso que serve a margem
		//          enorme abaixo.
		Mesh = new QuadMesh { Size = new Vector2(2.0f, 2.0f) };
		ExtraCullMargin = 16384.0f;
		CastShadow = ShadowCastingSetting.Off;
		Position = new Vector3(0.0f, 0.0f, -0.5f);

		var desenho = GD.Load<Shader>("res://Shaders/NevoaDeGuerra.gdshader");
		if (desenho == null)
		{
			GD.PushWarning("NevoaDeGuerra: não achei 'res://Shaders/NevoaDeGuerra.gdshader'. " +
						   "A névoa fica desligada.");
			Visible = false;
			return;
		}

		// Alteração de IA - Revisar
		// O que faz: manda desenhar esta camada depois de todas as outras.
		// Por quê: ela pinta por cima da imagem já montada. Se fosse desenhada no meio,
		//          leria uma imagem pela metade e partes do cenário apareceriam por cima
		//          da névoa.
		_material = new ShaderMaterial { Shader = desenho, RenderPriority = 100 };
		MaterialOverride = _material;

		// Alteração de IA - Revisar
		// O que faz: procura o personagem pelo grupo "player".
		// Por quê: a névoa é sempre do ponto de vista do jogador. Procurar pelo grupo mantém
		//          isso funcionando mesmo que alguém reorganize os nós do mapa.
		var achados = GetTree().GetNodesInGroup("player");
		if (achados.Count > 0)
		{
			_jogador = achados[0] as PlayerIsometrico;
			_sensor = (achados[0] as Node)?.GetNodeOrNull<SensorDeteccao>("SensorDeteccao");
		}

		if (_sensor == null)
		{
			GD.PushWarning("NevoaDeGuerra: não achei o sensor do jogador. A névoa fica desligada.");
			Visible = false;
			return;
		}

		PrepararMedidaDasParedes();
	}

	// Alteração de IA - Revisar
	// O que faz: prepara a tabelinha onde ficam guardadas as distâncias até a parede em cada
	//            direção — uma tira de imagem de um pixel de altura.
	// Por quê: é a forma de entregar esses números ao desenho da tela. Uma imagem é o único
	//          formato que o desenho consegue consultar rápido, milhares de vezes por quadro.
	//          Cada pixel guarda uma distância, e a posição dele na tira corresponde a uma
	//          direção da roda.
	private void PrepararMedidaDasParedes()
	{
		int quantos = Mathf.Max(8, QuantidadeDeRaios);
		_distanciaAteAParede = new float[quantos];
		_bytesDasDistancias = new byte[quantos * sizeof(float)];
		_imagemDasParedes = Image.CreateEmpty(quantos, 1, false, Image.Format.Rf);
		_texturaDasParedes = ImageTexture.CreateFromImage(_imagemDasParedes);

		int lado = Mathf.Clamp(ResolucaoDaMemoria, 32, 1024);
		_memoriaDoCenario = new byte[lado * lado];
		_imagemDaMemoria = Image.CreateEmpty(lado, lado, false, Image.Format.R8);
		_texturaDaMemoria = ImageTexture.CreateFromImage(_imagemDaMemoria);
	}

	// Alteração de IA - Revisar
	// O que faz: anota na grade de memória tudo o que o personagem está enxergando agora.
	// Por quê: uma vez anotado, nunca é apagado — é o "eu já vi essa parte do mapa". A conta usada
	//          aqui é a mesma da névoa na tela (distância, cone e parede na frente), para o que
	//          fica lembrado ser exatamente o que foi visto, nem mais nem menos.
	private void AnotarNaMemoria(float direcaoOlhandoGraus)
	{
		if (!LembrarDoCenario || _sensor == null || _memoriaDoCenario.Length == 0)
		{
			return;
		}

		int lado = (int)Mathf.Sqrt(_memoriaDoCenario.Length);
		float metros = Mathf.Max(1.0f, TamanhoDoMapa);
		float porPedaco = metros / lado;

		Vector3 onde = _sensor.GlobalPosition;
		float alcance = _sensor.AlcanceVisao3;
		float meiaAbertura = Mathf.DegToRad(_sensor.AberturaVisao * 0.5f);
		float olhando = Mathf.DegToRad(direcaoOlhandoGraus);
		float passiva = _sensor.VisaoPassiva2;

		// só percorre o pedaço da grade que o personagem poderia estar enxergando
		Vector2 canto = CentroDoMapa - Vector2.One * (metros * 0.5f);
		int x0 = Mathf.Clamp((int)((onde.X - alcance - canto.X) / porPedaco), 0, lado - 1);
		int x1 = Mathf.Clamp((int)((onde.X + alcance - canto.X) / porPedaco), 0, lado - 1);
		int z0 = Mathf.Clamp((int)((onde.Z - alcance - canto.Y) / porPedaco), 0, lado - 1);
		int z1 = Mathf.Clamp((int)((onde.Z + alcance - canto.Y) / porPedaco), 0, lado - 1);

		int quantosRaios = _distanciaAteAParede.Length;

		for (int gz = z0; gz <= z1; gz++)
		{
			float mundoZ = canto.Y + (gz + 0.5f) * porPedaco;
			for (int gx = x0; gx <= x1; gx++)
			{
				int posicao = gz * lado + gx;
				if (_memoriaDoCenario[posicao] == 255)
				{
					continue;   // já lembrado: não precisa recalcular
				}

				float mundoX = canto.X + (gx + 0.5f) * porPedaco;
				float dx = mundoX - onde.X;
				float dz = mundoZ - onde.Z;
				float distancia = Mathf.Sqrt(dx * dx + dz * dz);

				if (distancia > alcance)
				{
					continue;
				}

				// parede na frente?
				float angulo = Mathf.Atan2(-dz, dx);
				int raio = Mathf.PosMod((int)Mathf.Round(angulo / Mathf.Tau * quantosRaios), quantosRaios);
				if (distancia > _distanciaAteAParede[raio] + FolgaDaParede)
				{
					continue;
				}

				bool vePerto = distancia <= passiva;
				bool veNoCone = Mathf.Abs(Mathf.AngleDifference(angulo, olhando)) <= meiaAbertura;

				if (vePerto || veNoCone)
				{
					_memoriaDoCenario[posicao] = 255;
					_memoriaMudou = true;
				}
			}
		}

		if (_memoriaMudou)
		{
			_imagemDaMemoria!.SetData(lado, lado, false, Image.Format.R8, _memoriaDoCenario);
			_texturaDaMemoria!.Update(_imagemDaMemoria);
			_memoriaMudou = false;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: esquece tudo o que foi visto.
	// Por quê: ao trocar de fase ou recomeçar, a memória precisa zerar — senão o jogador
	//          começaria a fase nova já conhecendo o mapa da anterior.
	public void EsquecerTudo()
	{
		Array.Clear(_memoriaDoCenario, 0, _memoriaDoCenario.Length);
		_memoriaMudou = true;
	}

	// Alteração de IA - Revisar
	// O que faz: mede, em cada direção em volta do personagem, a que distância está a parede
	//            mais próxima.
	// Por quê: roda junto com a física, e não junto com o desenho, porque perguntar ao mundo
	//          "tem parede nesta direção?" fora da hora certa da física dá resultado instável.
	public override void _PhysicsProcess(double delta)
	{
		if (_sensor == null || !Ligada || _distanciaAteAParede.Length == 0)
		{
			return;
		}

		var espaco = GetWorld3D().DirectSpaceState;
		Vector3 olhos = _sensor.PosicaoDosOlhos;
		float alcance = Mathf.Max(1.0f, _sensor.AlcanceVisao3);
		int quantos = _distanciaAteAParede.Length;

		for (int i = 0; i < quantos; i++)
		{
			float angulo = Mathf.Tau * i / quantos;
			var rumo = new Vector3(Mathf.Cos(angulo), 0.0f, -Mathf.Sin(angulo));

			var consulta = PhysicsRayQueryParameters3D.Create(olhos, olhos + rumo * alcance);
			consulta.CollisionMask = (uint)_sensor.CamadaQueBloqueiaVisao;
			consulta.HitFromInside = false;

			var achou = espaco.IntersectRay(consulta);
			float distancia = alcance;

			if (achou.Count > 0)
			{
				var ponto = (Vector3)achou["position"];
				distancia = new Vector2(ponto.X - olhos.X, ponto.Z - olhos.Z).Length();
			}

			_distanciaAteAParede[i] = distancia;
		}

		Buffer.BlockCopy(_distanciaAteAParede, 0, _bytesDasDistancias, 0, _bytesDasDistancias.Length);
		_imagemDasParedes!.SetData(quantos, 1, false, Image.Format.Rf, _bytesDasDistancias);
		_texturaDasParedes!.Update(_imagemDasParedes);

		_passosDesdeAUltimaAnotacao++;
		if (_passosDesdeAUltimaAnotacao >= Mathf.Max(1, PassosEntreAnotacoes))
		{
			_passosDesdeAUltimaAnotacao = 0;
			AnotarNaMemoria(_jogador?.DirecaoOlhando ?? 270.0f);
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug_nevoa"))
		{
			Ligada = !Ligada;
		}

		if (_sensor == null || _texturaDasParedes == null || _texturaDaMemoria == null)
		{
			return;
		}

		Visible = Ligada;
		if (!Ligada)
		{
			return;
		}

		float olhando = _jogador?.DirecaoOlhando ?? 270.0f;

		_material.SetShaderParameter("jogador", _sensor.GlobalPosition);
		_material.SetShaderParameter("olhando", Mathf.DegToRad(olhando));
		_material.SetShaderParameter("meia_abertura", Mathf.DegToRad(_sensor.AberturaVisao * 0.5f));
		_material.SetShaderParameter("passiva_clara", _sensor.VisaoPassiva1);
		_material.SetShaderParameter("passiva_embacada", _sensor.VisaoPassiva2);
		_material.SetShaderParameter("cone_claro", _sensor.AlcanceVisao1);
		_material.SetShaderParameter("cone_embacado", _sensor.AlcanceVisao2);
		_material.SetShaderParameter("cone_vulto", _sensor.AlcanceVisao3);
		_material.SetShaderParameter("cor_da_nevoa", CorDaNevoa);
		_material.SetShaderParameter("forca_da_nevoa", ForcaDaNevoa);
		_material.SetShaderParameter("forca_do_borrao", ForcaDoBorrao);
		_material.SetShaderParameter("paredes", _texturaDasParedes);
		_material.SetShaderParameter("folga_da_parede", FolgaDaParede);
		_material.SetShaderParameter("memoria", _texturaDaMemoria);
		_material.SetShaderParameter("peso_da_memoria", LembrarDoCenario ? PesoDaMemoria : 0.0f);
		_material.SetShaderParameter("canto_do_mapa", CentroDoMapa - Vector2.One * (TamanhoDoMapa * 0.5f));
		_material.SetShaderParameter("tamanho_do_mapa", Mathf.Max(1.0f, TamanhoDoMapa));
	}
}

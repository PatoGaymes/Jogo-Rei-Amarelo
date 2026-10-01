using System;
using Godot;

// Alteração de IA - Revisar
// O que faz: diz se esta fase é limpa, coberta de névoa ou mergulhada na escuridão — e guarda os
//            ajustes de cada um.
// Por quê: **nem todo mapa tem névoa ou escuridão.** Um mapa sem este nó é limpo: tudo aparece
//          normalmente, sem nada ofuscando. Colocando o nó, o mapa passa a esconder o que o
//          personagem não enxerga. Fica como um nó da fase, e não como ajuste da câmera, porque
//          é uma característica do lugar — a mesma câmera serve para as minas escuras e para a
//          superfície ao ar livre.
//
//          Os dois tipos escondem **de verdade** o que está fora da visão, não só escurecem:
//
//            Escuridão: preto total onde não há luz. É o Don't Starve Together — sem fogo, nem o
//                       próprio personagem aparece. **Tudo o que está iluminado aparece**, para
//                       onde quer que o personagem esteja olhando; só parede esconde.
//
//            Névoa: não é preta, e **não é uniforme**. Ela se move em massas, como neblina de
//                   verdade: num mesmo lugar a névoa pode estar encobrindo 100%, depois 70%,
//                   depois 100% de novo. É o Silent Hill antigo — o que está perto aparece, e
//                   tudo vai sumindo aos poucos com a distância, sem borda nenhuma.
//
// Alteração de IA - Revisar (30/09/2026)
// O que faz: este nó passou a ser também **a fonte única das contas do ambiente**: quanto de névoa
//            existe num ponto, quanta luz chega nele, e o quanto alguém enxerga através disso.
// Por quê: agora os inimigos também são afetados pela névoa e pela escuridão. Se a tela fizesse
//          uma conta e os inimigos outra, o jogador veria uma coisa e o inimigo "veria" outra — um
//          lugar que parece escuro na tela poderia estar claro para o inimigo. Com as contas num
//          lugar só, o que o jogador vê e o que o inimigo percebe seguem a mesma regra.
public partial class AmbienteDaFase : Node
{
	public enum TipoDeAmbiente
	{
		Limpo,      // tudo aparece normalmente
		Nevoa,      // neblina clara que se move e varia de densidade
		Escuridao   // preto total onde não há luz
	}

	[Export]
	public TipoDeAmbiente Tipo { get; set; } = TipoDeAmbiente.Nevoa;

	// ---------------------------------------------------------------- névoa

	[Export]
	public Color CorDaNevoa { get; set; } = new(0.60f, 0.62f, 0.64f);

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: o quão grossa a névoa fica no ponto mais ralo e no mais denso de cada massa de ar.
	//            **1 é a névoa normal**; 0,6 é uma brecha 40% mais rala; 1,6 é uma massa 60% mais
	//            densa.
	// Por quê: é o que faz a névoa ser volátil. Numa distância média, a brecha deixa o lugar
	//          encoberto uns 70%, e a massa densa, perto de 100% — os "70% a 100%" combinados.
	//          Antes estes números eram a porcentagem encoberta direto; mudaram de sentido porque
	//          a névoa agora aumenta aos poucos com a distância, e a densidade passou a dizer o
	//          quão rápido ela fecha.
	[Export(PropertyHint.Range, "0.1,3,0.05")]
	public float DensidadeMinima { get; set; } = 0.6f;

	[Export(PropertyHint.Range, "0.1,3,0.05")]
	public float DensidadeMaxima { get; set; } = 1.6f;

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: um véu leve de névoa que existe até colado no personagem.
	// Por quê: sem ele, a área em volta do personagem ficava perfeitamente limpa, e a névoa parecia
	//          um buraco recortado em volta dele. Com um véu pequeno, o personagem está
	//          **dentro** da névoa, e não num círculo limpo no meio dela. (5% colado nele.)
	[Export(PropertyHint.Range, "0,0.5,0.01")]
	public float NevoaColada { get; set; } = 0.05f;

	// Alteração de IA - Revisar
	// O que faz: o tamanho aproximado de cada "massa" de névoa, em metros, e para onde o vento a
	//            leva, em metros por segundo.
	// Por quê: massas pequenas dão névoa picotada, que parece defeito; grandes demais e a
	//          variação quase não aparece. O vento é o que faz a névoa **passar**, em vez de
	//          ficar pulsando no lugar.
	[Export]
	public float TamanhoDasMassas { get; set; } = 16.0f;

	[Export]
	public Vector2 Vento { get; set; } = new(0.55f, 0.20f);

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: o formato da curva com que a névoa fecha conforme a distância.
	// Por quê: com 1, a névoa já embaça bastante o que está a dois passos; com números maiores o
	//          que está perto fica nítido e a névoa fecha mais adiante. 3 deixa o entorno do
	//          personagem limpo sem criar uma borda visível — a passagem de "vejo" (20%) para "não
	//          vejo" (80%) ocupa uns 5 metros em volta do jogador.
	public const float CurvaDaNevoa = 3.0f;

	// ---------------------------------------------------------------- escuridão

	[Export]
	public Color CorDaEscuridao { get; set; } = new(0.0f, 0.0f, 0.0f);

	// Alteração de IA - Revisar
	// O que faz: um pouco de luz que existe em todo lugar, mesmo sem fogueira.
	// Por quê: em 0 é breu total — o Don't Starve. Um valor pequeno faz um lugar "muito escuro"
	//          em vez de "sem luz nenhuma", onde ainda se adivinham vultos por perto.
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float LuzAmbiente { get; set; } = 0.0f;

	// Alteração de IA - Revisar
	// O que faz: apaga todas as fontes de luz da fase, sem precisar mexer em cada uma.
	// Por quê: é o que permite testar o breu total (a segunda imagem de referência do Don't
	//          Starve). Usado pela tecla F4 no ciclo de testes.
	public bool ApagarTodasAsLuzes { get; set; }

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: o relógio da névoa e o desenho de ruído que dá forma às massas de ar.
	// Por quê: saíram da camada da tela e vieram para cá, porque agora os inimigos também precisam
	//          saber onde a névoa está grossa neste instante. O desenho é gerado **uma vez só**, no
	//          início, e a névoa que se move é esse mesmo desenho deslizando pelo mapa — é o que
	//          deixa a névoa volátil praticamente de graça.
	public float TempoDaNevoa { get; private set; }
	public ImageTexture? TexturaDaNevoa { get; private set; }

	private byte[] _ruido = Array.Empty<byte>();
	private int _ladoDoRuido;

	// Alteração de IA - Revisar
	// O que faz: devolve o ambiente da fase que está rodando, ou nada se a fase for limpa.
	// Por quê: quem precisa saber (a névoa da tela, os inimigos) pergunta aqui, em vez de cada
	//          um procurar o nó do seu jeito.
	public static AmbienteDaFase? DaFase(SceneTree arvore)
	{
		var achados = arvore.GetNodesInGroup("ambiente_da_fase");
		return achados.Count > 0 ? achados[0] as AmbienteDaFase : null;
	}

	public bool EscondeOQueNaoSeVe => Tipo != TipoDeAmbiente.Limpo;

	public override void _EnterTree()
	{
		AddToGroup("ambiente_da_fase");
	}

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: gera, uma vez só, o desenho de ruído que dá forma às massas de névoa — e guarda
	//            uma cópia dos números para os inimigos consultarem.
	// Por quê: era gerado pela camada da tela; veio para cá para a tela e os inimigos lerem
	//          exatamente o mesmo desenho. São as mesmas configurações de antes, então a névoa tem
	//          a mesma cara.
	public override void _Ready()
	{
		GerarRuido();
	}

	private void GerarRuido()
	{
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
		if (imagem.GetFormat() != Image.Format.L8)
		{
			imagem.Convert(Image.Format.L8);
		}

		_ruido = imagem.GetData();
		_ladoDoRuido = imagem.GetWidth();
		TexturaDaNevoa = ImageTexture.CreateFromImage(imagem);
	}

	// Alteração de IA - Revisar
	// O que faz: a tecla F4 percorre os ambientes: limpo → névoa → escuridão → breu total → limpo.
	// Por quê: **ferramenta de teste.** Permite ver o mesmo mapa nos três ambientes sem montar
	//          três mapas. O "breu total" é a escuridão com todas as luzes apagadas.
	public override void _Process(double delta)
	{
		TempoDaNevoa += (float)delta;

		if (!Input.IsActionJustPressed("debug_nevoa"))
		{
			return;
		}

		if (Tipo == TipoDeAmbiente.Limpo)
		{
			Tipo = TipoDeAmbiente.Nevoa;
		}
		else if (Tipo == TipoDeAmbiente.Nevoa)
		{
			Tipo = TipoDeAmbiente.Escuridao;
			ApagarTodasAsLuzes = false;
		}
		else if (!ApagarTodasAsLuzes)
		{
			ApagarTodasAsLuzes = true;
		}
		else
		{
			Tipo = TipoDeAmbiente.Limpo;
			ApagarTodasAsLuzes = false;
		}

		GD.Print($"Ambiente: {NomeDoAmbiente}");
	}

	public string NomeDoAmbiente => Tipo switch
	{
		TipoDeAmbiente.Limpo => "limpo",
		TipoDeAmbiente.Nevoa => "névoa",
		_ => ApagarTodasAsLuzes ? "breu total" : "escuridão"
	};

	// ---------------------------------------------------------------- contas da névoa

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: o quão grossa está a névoa num ponto do mapa **agora**, entre a densidade mínima
	//            e a máxima.
	// Por quê: duas camadas do mesmo ruído, de tamanhos diferentes, levadas pelo vento em direções
	//          diferentes. Somadas, nunca se repetem de um jeito que o olho perceba: uma massa
	//          passa, abre uma brecha, fecha de novo.
	//
	//          **Esta conta é a mesma do desenho da tela** (Shaders/NevoaDeGuerra.gdshader, função
	//          "densidade"). Mudou uma, tem que mudar a outra — senão o inimigo passa a enxergar
	//          através de uma névoa diferente da que o jogador vê.
	public float DensidadeDaNevoaEm(Vector3 ponto)
	{
		float tamanho = Mathf.Max(1.0f, TamanhoDasMassas);
		var xz = new Vector2(ponto.X, ponto.Z);
		Vector2 uv1 = (xz + Vento * TempoDaNevoa) / (tamanho * 3.0f);
		Vector2 uv2 = (xz - new Vector2(Vento.Y, -Vento.X) * TempoDaNevoa * 0.7f) / (tamanho * 1.7f);
		float n = LerRuido(uv1) * 0.65f + LerRuido(uv2) * 0.35f;
		return Mathf.Lerp(DensidadeMinima, DensidadeMaxima, Mathf.SmoothStep(0.30f, 0.70f, n));
	}

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: o quanto se enxerga um ponto através da névoa, de 1 (nítido) a 0 (sumiu).
	//            "alcance" é a distância em que a névoa já fechou bastante para quem olha.
	// Por quê: **é o que faz a visão ser um receptor, e não uma lanterna.** Quem está na névoa não
	//          "acende" uma área: tudo em volta vai sumindo aos poucos conforme se afasta, e some
	//          mais rápido onde a massa de ar está mais grossa. Por isso não existe borda — nem
	//          círculo, nem cone recortado.
	//
	//          Além do dobro do alcance a névoa encobre sempre tudo, mesmo numa brecha, para ela
	//          nunca virar uma janela para o mapa inteiro.
	//
	//          (01/10/2026) Passou a receber um alcance só. Antes eram dois — o da direção do
	//          olhar e o máximo, para a frente — porque o jogador tinha cone de visão.
	public float TransparenciaDaNevoa(Vector3 ponto, float distancia, float alcance)
	{
		if (Tipo != TipoDeAmbiente.Nevoa)
		{
			return 1.0f;
		}

		alcance = Mathf.Max(0.1f, alcance);
		float densidade = DensidadeDaNevoaEm(ponto);
		float espessura = NevoaColada + Mathf.Pow(distancia / alcance, CurvaDaNevoa) * densidade;
		float longe = Mathf.SmoothStep(alcance * 1.9f, alcance * 2.4f, distancia);
		return Mathf.Exp(-espessura) * (1.0f - longe);
	}

	// lê o desenho de ruído do mesmo jeito que a placa de vídeo lê: suavizado e repetindo nas bordas
	private float LerRuido(Vector2 uv)
	{
		int lado = _ladoDoRuido;
		if (lado == 0)
		{
			return 0.5f;
		}

		float x = uv.X * lado - 0.5f;
		float y = uv.Y * lado - 0.5f;
		int x0 = Mathf.FloorToInt(x);
		int y0 = Mathf.FloorToInt(y);
		float fx = x - x0;
		float fy = y - y0;

		float embaixo = Mathf.Lerp(Texel(x0, y0), Texel(x0 + 1, y0), fx);
		float emcima = Mathf.Lerp(Texel(x0, y0 + 1), Texel(x0 + 1, y0 + 1), fx);
		return Mathf.Lerp(embaixo, emcima, fy);
	}

	private float Texel(int x, int y)
	{
		int lado = _ladoDoRuido;
		x = ((x % lado) + lado) % lado;
		y = ((y % lado) + lado) % lado;
		return _ruido[y * lado + x] / 255.0f;
	}

	// ---------------------------------------------------------------- contas da luz

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: o quanto um ponto está iluminado, de 0 (breu) a 1 (claro), somando todas as
	//            fontes de luz acesas que alcançam ele.
	// Por quê: é o que decide se um inimigo consegue ver o jogador no escuro. Uma luz com parede no
	//          meio não conta — a fogueira do outro lado do muro não entrega ninguém.
	//
	//          **O jeito como a luz enfraquece até a borda é o mesmo do desenho da tela** (ver
	//          FatorDaLuz). Assim, se o jogador está num lugar que aparece iluminado na tela, ele
	//          está iluminado para os inimigos também.
	//
	//          Fora da escuridão devolve sempre 1: na névoa e no mapa limpo a luz não muda nada.
	public float LuzEm(Vector3 ponto, PhysicsDirectSpaceState3D espaco, uint paredes)
	{
		if (Tipo != TipoDeAmbiente.Escuridao)
		{
			return 1.0f;
		}

		float total = LuzAmbiente;
		if (ApagarTodasAsLuzes)
		{
			return Mathf.Clamp(total, 0.0f, 1.0f);
		}

		foreach (Node no in GetTree().GetNodesInGroup("fonte_de_luz"))
		{
			if (no is not FonteDeLuz luz || !luz.Acesa || luz.Intensidade <= 0.0f || !luz.IsVisibleInTree())
			{
				continue;
			}

			Vector3 origem = luz.GlobalPosition;
			float distancia = new Vector2(ponto.X - origem.X, ponto.Z - origem.Z).Length();
			float raio = luz.RaioAgora;
			if (distancia >= raio)
			{
				continue;
			}

			var consulta = PhysicsRayQueryParameters3D.Create(origem, ponto, paredes);
			consulta.HitFromInside = false;
			if (espaco.IntersectRay(consulta).Count > 0)
			{
				continue;   // parede entre a luz e o ponto
			}

			total += luz.Intensidade * FatorDaLuz(distancia, raio);
			if (total >= 1.0f)
			{
				break;
			}
		}

		return Mathf.Clamp(total, 0.0f, 1.0f);
	}

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: o quanto uma luz ainda clareia a uma certa distância dela: cheia até quase metade
	//            do raio, enfraquecendo suave até zero na borda.
	// Por quê: é a mesma conta do desenho da tela, escrita num lugar só para a tela e os inimigos
	//          não saírem do passo um do outro.
	public static float FatorDaLuz(float distancia, float raio)
	{
		return 1.0f - Mathf.SmoothStep(raio * 0.45f, raio, distancia);
	}
}

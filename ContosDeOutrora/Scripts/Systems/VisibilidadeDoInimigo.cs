using Godot;

// Alteração de IA - Revisar
// O que faz: ajusta a aparência do inimigo conforme o ambiente da fase e o quanto o jogador o
//            enxerga.
// Por quê: quem **esconde** o inimigo é a camada da névoa de guerra, igual esconde o resto do
//          mapa — assim inimigo e cenário obedecem sempre à mesma regra, e não dá para ver um sem
//          o outro. Este script só cuida de uma coisa que a camada não resolve sozinha: na névoa,
//          o inimigo que está longe tem que virar **silhueta escura**. A névoa é clara; um vulto
//          escuro recortado nela é o que diz "tem alguém ali", como nos Silent Hill antigos. Com
//          as cores normais ele se misturaria ao cinza e sumiria.
//
//          Na escuridão, e em mapa limpo, o inimigo fica com a cor normal: quem decide se ele
//          aparece é a luz.
//
//          Fica separado do cérebro do inimigo (InimigoIA) de propósito: aparência e
//          comportamento não se misturam. Se este script sumir, o inimigo continua rondando e
//          perseguindo igual.
//
//          Até 21/09/2026 este script também deixava um "borrão de memória" no último lugar onde
//          o inimigo tinha sido visto. A ideia foi retirada — ver docs/IDEIAS-ARQUIVADAS.md.
public partial class VisibilidadeDoInimigo : Node
{
	// Alteração de IA - Revisar
	// O que faz: a cor da silhueta do inimigo quando ele está longe, dentro da névoa.
	// Por quê: quase preta, para recortar contra a névoa clara. Quanto mais perto ele chega, mais
	//          essa cor dá lugar à cor de verdade.
	[Export]
	public Color CorDoVulto { get; set; } = new(0.09f, 0.09f, 0.11f, 1.0f);

	[Export]
	public float VelocidadeDaTransicao { get; set; } = 5.0f;

	// Alteração de IA - Revisar
	// O que faz: quando ligado, este inimigo aparece sempre, por cima da névoa e da escuridão.
	// Por quê: a equipe já avisou que **existirão inimigos que a névoa não afeta** — olhos que
	//          brilham no escuro, uma aparição. Marcando a caixa, o inimigo passa a ser desenhado
	//          depois da camada de névoa, e por isso ela não o cobre. Parede continua escondendo.
	[Export]
	public bool IgnoraANevoa { get; set; }

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: o quanto o jogador enxerga este inimigo através da névoa agora, de 0 a 1.
	// Por quê: substituiu o "nível" inteiro (1, 2 ou 3). Com degraus, a silhueta trocava de cor
	//          aos saltos ao cruzar uma faixa; contínuo, ela escurece aos poucos conforme ele se
	//          afasta ou uma massa de névoa passa na frente — e bate com o que a tela mostra.
	public float QuantoOJogadorVe { get; private set; } = 1.0f;

	private SensorDeteccao? _sensorDoJogador;
	private SpriteBase3D? _desenho;
	private AmbienteDaFase? _ambiente;
	private Color _corAtual = Colors.White;

	public override void _Ready()
	{
		foreach (Node filho in GetParent().GetChildren())
		{
			if (filho is SpriteBase3D desenho)
			{
				_desenho = desenho;
				break;
			}
		}
		var achados = GetTree().GetNodesInGroup("player");
		if (achados.Count > 0)
		{
			_sensorDoJogador = (achados[0] as Node)?.GetNodeOrNull<SensorDeteccao>("SensorDeteccao");
		}

		if (IgnoraANevoa && _desenho != null)
		{
			// desenhado depois da camada de névoa (que usa a prioridade 100)
			_desenho.AlphaCut = SpriteBase3D.AlphaCutMode.Disabled;
			_desenho.RenderPriority = 101;
		}
	}

	// roda junto com a física porque pergunta ao mundo se existe parede no meio
	public override void _PhysicsProcess(double delta)
	{
		if (_desenho == null || GetParent() is not Node3D corpo)
		{
			return;
		}

		_ambiente ??= AmbienteDaFase.DaFase(GetTree());

		Color alvo = Colors.White;
		bool naNevoa = _ambiente != null && _ambiente.Tipo == AmbienteDaFase.TipoDeAmbiente.Nevoa;

		if (naNevoa && !IgnoraANevoa && _sensorDoJogador != null)
		{
			// Alteração de IA - Revisar (01/10/2026)
			// O que faz: pergunta o quanto o jogador enxerga este inimigo pelo círculo de visão dele.
			// Por quê: o jogador não tem mais cone de visão, então não importa mais para onde ele
			//          está virado — só a distância e a névoa no caminho.
			QuantoOJogadorVe = _sensorDoJogador.QuantoEnxergaEmVolta(corpo.GlobalPosition, _ambiente);

			// Alteração de IA - Revisar (30/09/2026)
			// O que faz: enxergando bem (acima de 75%), cor normal; enxergando pouco (abaixo de
			//            30%), silhueta escura; entre os dois, uma mistura suave.
			// Por quê: é a forma surgindo na neblina. Longe, a névoa já cobre quase tudo — o vulto
			//          escuro é o que sobra dele. Quando uma massa de névoa rala passa, o vulto
			//          aparece por um instante e some de novo. Parede não precisa ser olhada aqui:
			//          atrás de parede a camada da névoa já cobre o inimigo por inteiro.
			alvo = CorDoVulto.Lerp(Colors.White, Mathf.SmoothStep(0.30f, 0.75f, QuantoOJogadorVe));
		}
		else
		{
			QuantoOJogadorVe = 1.0f;
		}

		_corAtual = _corAtual.Lerp(alvo, Mathf.Clamp((float)delta * VelocidadeDaTransicao, 0.0f, 1.0f));
		_desenho.Modulate = _corAtual;
	}
}

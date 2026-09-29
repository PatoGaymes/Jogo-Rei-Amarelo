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

	public int NivelAtual { get; private set; }

	private SensorDeteccao? _sensorDoJogador;
	private SensorDeteccao? _meuSensor;
	private PlayerIsometrico? _jogador;
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
		_meuSensor = GetParent()?.GetNodeOrNull<SensorDeteccao>("SensorDeteccao");

		var achados = GetTree().GetNodesInGroup("player");
		if (achados.Count > 0)
		{
			_jogador = achados[0] as PlayerIsometrico;
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

		if (naNevoa && !IgnoraANevoa)
		{
			NivelAtual = CalcularNivel(corpo.GlobalPosition);

			// Alteração de IA - Revisar
			// O que faz: de perto (nível 1) cor normal; no meio (nível 2) já meio apagado; longe ou
			//            fora da visão, silhueta escura.
			// Por quê: fora da visão a camada da névoa encobre de 70% a 100%. Nos pontos em que ela
			//          afina, a silhueta escura aparece por um instante e some de novo — é a forma
			//          surgindo na neblina.
			alvo = NivelAtual switch
			{
				1 => Colors.White,
				2 => Colors.White.Lerp(CorDoVulto, 0.45f),
				_ => CorDoVulto
			};
		}
		else
		{
			NivelAtual = 1;
		}

		_corAtual = _corAtual.Lerp(alvo, Mathf.Clamp((float)delta * VelocidadeDaTransicao, 0.0f, 1.0f));
		_desenho.Modulate = _corAtual;
	}

	private int CalcularNivel(Vector3 onde)
	{
		if (_sensorDoJogador == null)
		{
			return 1;
		}
		if (_meuSensor != null && !_sensorDoJogador.TemLinhaDeVisao(_meuSensor))
		{
			return 0;
		}
		return _sensorDoJogador.NivelDeVisaoDe(onde, _jogador?.DirecaoOlhando ?? 270.0f);
	}
}

using Godot;

// Alteração de IA - Revisar
// O que faz: decide como o inimigo aparece na tela conforme o quanto o jogador o enxerga —
//            normal, um pouco apagado, só um vulto escuro — e, quando ele sai de vista, deixa
//            no lugar um **borrão de memória** que vai sumindo aos poucos.
// Por quê: a névoa da tela escurece o cenário por igual, mas um inimigo não pode simplesmente
//          escurecer junto: no limite do alcance o jogador tem que **perceber que tem alguém
//          ali sem saber quem é**.
//
//          E o sumiço não pode ser seco. A cabeça de alguém não apaga o que viu no instante em
//          que vira o rosto: fica a lembrança de onde a pessoa estava. Só que, diferente de uma
//          casa, um inimigo **anda** — então essa lembrança envelhece e deixa de valer. É por
//          isso que ela some sozinha com o tempo, e some na hora se o jogador olhar para lá e
//          não achar ninguém.
//
//          Fica separado do cérebro do inimigo (`InimigoIA`) de propósito: aparência e
//          comportamento não devem se misturar. Se este script sumir, o inimigo continua
//          rondando e perseguindo igual.
public partial class VisibilidadeDoInimigo : Node
{
	[Export]
	public Color CorNivel1 { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

	[Export]
	public Color CorNivel2 { get; set; } = new(0.82f, 0.84f, 0.90f, 0.95f);

	// Alteração de IA - Revisar
	// O que faz: a cor do vulto, no limite do alcance da visão.
	// Por quê: é **mais clara que a névoa**, não mais escura. A névoa é escura, então um vulto
	//          escuro sumiria dentro dela; um vulto claro aparece como uma mancha pálida, que é
	//          exatamente a leitura desejada — tem alguma coisa ali, não dá para ver o quê.
	[Export]
	public Color CorNivel3 { get; set; } = new(0.62f, 0.66f, 0.78f, 0.80f);

	// Alteração de IA - Revisar
	// O que faz: a cor do borrão de memória deixado no último lugar em que o inimigo foi visto.
	// Por quê: mais apagado que o vulto, e sem cor nenhuma, para o jogador distinguir "estou
	//          vendo alguém ali agora" de "lembro que tinha alguém ali".
	[Export]
	public Color CorDaMemoria { get; set; } = new(0.55f, 0.58f, 0.68f, 0.55f);

	// Alteração de IA - Revisar
	// O que faz: quantos segundos a lembrança de onde o inimigo estava leva para sumir sozinha.
	// Por quê: **é o número que decide o quanto o jogador pode confiar na memória.** Curto
	//          demais e o borrão não chega a ajudar; longo demais e o jogador passa a caçar uma
	//          informação velha, achando que o inimigo continua onde não está mais.
	[Export]
	public float TempoDeMemoria { get; set; } = 5.0f;

	[Export]
	public float VelocidadeDaTransicao { get; set; } = 6.0f;

	// Alteração de IA - Revisar
	// O que faz: quando ligado, este inimigo aparece sempre, mesmo dentro da névoa.
	// Por quê: a equipe já avisou que **existirão inimigos que a névoa não afeta**. Deixar o
	//          interruptor pronto evita mexer neste script depois; quem montar o inimigo só
	//          marca a caixa.
	[Export]
	public bool IgnoraANevoa { get; set; }

	public int NivelAtual { get; private set; }
	public float Memoria { get; private set; }
	public Vector3 UltimoLugarVisto { get; private set; }

	private SensorDeteccao? _sensorDoJogador;
	private SensorDeteccao? _meuSensor;
	private PlayerIsometrico? _jogador;
	private Sprite3D? _desenho;
	private Sprite3D? _borrao;
	private Color _corAtual = Colors.White;

	public override void _Ready()
	{
		_desenho = GetParent()?.GetNodeOrNull<Sprite3D>("Sprite3D");
		_meuSensor = GetParent()?.GetNodeOrNull<SensorDeteccao>("SensorDeteccao");

		var achados = GetTree().GetNodesInGroup("player");
		if (achados.Count > 0)
		{
			_jogador = achados[0] as PlayerIsometrico;
			_sensorDoJogador = (achados[0] as Node)?.GetNodeOrNull<SensorDeteccao>("SensorDeteccao");
		}

		CriarBorraoDeMemoria();
	}

	// Alteração de IA - Revisar
	// O que faz: monta a cópia apagada do inimigo que fica no lugar onde ele foi visto pela
	//            última vez.
	// Por quê: o inimigo de verdade continua andando; a lembrança fica parada onde estava. São
	//          duas imagens diferentes, então precisam ser dois desenhos.
	//
	//          Fica marcado para ser desenhado **por cima da névoa** e sem se importar com o que
	//          está na frente. Sem isso a própria névoa apagaria a lembrança — e é justamente
	//          dentro da névoa que ela precisa ser vista. Atravessar parede é proposital: a
	//          lembrança está na cabeça do jogador, não no mundo.
	private void CriarBorraoDeMemoria()
	{
		if (_desenho == null || GetParent() is not Node3D corpo)
		{
			return;
		}

		_borrao = new Sprite3D
		{
			Name = "BorraoDeMemoria",
			Texture = _desenho.Texture,
			PixelSize = _desenho.PixelSize,
			Billboard = _desenho.Billboard,
			TextureFilter = _desenho.TextureFilter,
			Shaded = false,
			AlphaCut = SpriteBase3D.AlphaCutMode.Disabled,
			NoDepthTest = true,
			RenderPriority = 101,
			TopLevel = true,
			Visible = false,
			Modulate = new Color(CorDaMemoria, 0.0f)
		};

		corpo.AddChild(_borrao);
	}

	// Alteração de IA - Revisar
	// O que faz: roda junto com a física, e não com o desenho.
	// Por quê: aqui se pergunta ao mundo se existe parede no meio, e essa pergunta só dá
	//          resposta estável no momento certo da física.
	public override void _PhysicsProcess(double delta)
	{
		if (_desenho == null || GetParent() is not Node3D corpo)
		{
			return;
		}

		float dt = (float)delta;
		NivelAtual = CalcularNivel(corpo.GlobalPosition);

		if (NivelAtual > 0)
		{
			// está vendo: a lembrança vira a posição de agora
			UltimoLugarVisto = corpo.GlobalPosition;
			Memoria = 1.0f;
		}
		else if (Memoria > 0.0f)
		{
			// Alteração de IA - Revisar
			// O que faz: se o jogador está olhando para onde lembra, e não tem ninguém lá, a
			//            lembrança some na hora.
			// Por quê: é o que impede a memória de virar mentira. Olhou, o lugar está vazio —
			//          então o jogador **sabe** que o inimigo saiu dali, e manter o borrão
			//          seria enganá-lo.
			bool olhandoParaOLugar = _sensorDoJogador != null &&
									 _sensorDoJogador.EnxergaOPonto(UltimoLugarVisto,
																	_jogador?.DirecaoOlhando ?? 270.0f);

			Memoria = olhandoParaOLugar
				? 0.0f
				: Mathf.Max(0.0f, Memoria - dt / Mathf.Max(0.1f, TempoDeMemoria));
		}

		AtualizarDesenho(dt);
		AtualizarBorrao();
	}

	private void AtualizarDesenho(float dt)
	{
		Color alvo = NivelAtual switch
		{
			1 => CorNivel1,
			2 => CorNivel2,
			3 => CorNivel3,
			_ => new Color(CorNivel3.R, CorNivel3.G, CorNivel3.B, 0.0f)
		};

		_corAtual = _corAtual.Lerp(alvo, Mathf.Clamp(dt * VelocidadeDaTransicao, 0.0f, 1.0f));
		_desenho!.Modulate = _corAtual;

		// um desenho totalmente transparente ainda custa para ser desenhado
		_desenho.Visible = _corAtual.A > 0.02f;
	}

	private void AtualizarBorrao()
	{
		if (_borrao == null)
		{
			return;
		}

		// enquanto o inimigo aparece de verdade, não faz sentido mostrar a lembrança dele
		bool mostrar = NivelAtual == 0 && Memoria > 0.01f;
		_borrao.Visible = mostrar;

		if (!mostrar)
		{
			return;
		}

		_borrao.GlobalPosition = UltimoLugarVisto + new Vector3(0.0f, 0.95f, 0.0f);
		_borrao.FlipH = _desenho!.FlipH;

		// some junto com a lembrança, e não de uma vez
		_borrao.Modulate = new Color(CorDaMemoria, CorDaMemoria.A * Memoria);
	}

	private int CalcularNivel(Vector3 onde)
	{
		if (IgnoraANevoa || _sensorDoJogador == null)
		{
			return 1;
		}

		// Alteração de IA - Revisar
		// O que faz: parede no meio esconde o inimigo, mesmo dentro do alcance da visão.
		// Por quê: sem isto, o jogador enxergaria o inimigo através do muro sempre que ele
		//          estivesse no cone — que é exatamente o problema que a névoa veio resolver.
		//          Usa a mesma checagem de parede da detecção, então os dois lados enxergam o
		//          mundo pelas mesmas regras.
		if (_meuSensor != null && !_sensorDoJogador.TemLinhaDeVisao(_meuSensor))
		{
			return 0;
		}

		return _sensorDoJogador.NivelDeVisaoDe(onde, _jogador?.DirecaoOlhando ?? 270.0f);
	}
}

using Godot;

// Alteração de IA - Revisar
// O que faz: controla o personagem andando pelo mapa 3D.
// Por quê: o mapa é 3D, mas o personagem é um desenho 2D em pé, como uma folha de papel —
//          é o estilo do Don't Starve. Então ele se move nas três dimensões, mas o que
//          aparece na tela é sempre uma imagem plana virada para a câmera.
public partial class PlayerIsometrico : CharacterBody3D, IPersonagemQueOlha
{
	// Alteração de IA - Revisar
	// O que faz: as duas formas de agachar.
	// Por quê: **isto existe para o módulo de configurações que virá depois.** Segurar e
	//          alternar agradam a pessoas diferentes, e a escolha é do jogador, não nossa.
	//          Deixar as duas prontas agora significa que a tela de configurações só vai
	//          precisar escrever neste campo — nada de mexer no controle do personagem.
	public enum FormaDeAgachar
	{
		Segurar,   // agachado só enquanto a tecla estiver pressionada
		Alternar   // aperta para agachar, aperta de novo para levantar
	}

	[Export]
	public FormaDeAgachar ModoDeAgachar { get; set; } = FormaDeAgachar.Segurar;

	// Alteração de IA - Revisar
	// O que faz: velocidade máxima de caminhada.
	// Por quê: fica com [Export] para a equipe testar valores no painel do Godot com o jogo
	//          rodando, sem mexer no código.
	[Export]
	public float VelocidadeMaxima { get; set; } = 5.0f;

	// Alteração de IA - Revisar
	// O que faz: o quão rápido o personagem alcança a velocidade máxima ao começar a andar.
	// Por quê: sair direto na velocidade final deixa o movimento travado e robótico.
	[Export]
	public float Aceleracao { get; set; } = 40.0f;

	// Alteração de IA - Revisar
	// O que faz: o quão rápido ele para quando solta a tecla.
	// Por quê: é maior que a aceleração de propósito — parar mais rápido do que arranca faz
	//          o controle parecer mais preciso nas mãos do jogador.
	[Export]
	public float Desaceleracao { get; set; } = 55.0f;

	// Alteração de IA - Revisar
	// O que faz: força que puxa o personagem para baixo.
	// Por quê: o jogo não tem pulo, mas sem gravidade ele flutuaria ao passar por qualquer
	//          desnível do terreno.
	[Export]
	public float Gravidade { get; set; } = 24.0f;

	// Alteração de IA - Revisar
	// O que faz: o quanto ele anda mais devagar enquanto está agachado.
	// Por quê: **é o preço de agachar, e o único motivo para levantar.** Sem esse preço,
	//          agachar seria sempre melhor que andar em pé e o jogador passaria o jogo inteiro
	//          agachado — o que transformaria a mecânica numa tecla obrigatória em vez de uma
	//          escolha. Com o preço, a decisão vira "vale a pena ir devagar aqui?".
	//          Em 1.0 não há penalidade nenhuma, caso a equipe prefira assim.
	[Export]
	public float FatorDeVelocidadeAgachado { get; set; } = 0.45f;

	// Alteração de IA - Revisar
	// O que faz: o quanto o desenho do personagem abaixa e achata ao agachar, e a rapidez disso.
	// Por quê: o jogador precisa **ver** que está agachado, senão só descobre pelo resultado.
	//          Com a arte provisória, abaixar e achatar um pouco já lê como "abaixou". Quando
	//          houver desenho de agachado, isto some e vira troca de imagem.
	[Export]
	public float AbaixamentoDoDesenho { get; set; } = 0.30f;

	[Export]
	public float AchatamentoDoDesenho { get; set; } = 0.82f;

	[Export]
	public float VelocidadeDeAgachar { get; set; } = 9.0f;

	// Alteração de IA - Revisar
	// O que faz: informa se o personagem está agachado agora.
	// Por quê: o desenho de teste mostra isso na tela, e futuras mecânicas (passar por vãos
	//          baixos, por exemplo) vão precisar perguntar.
	public bool Agachado { get; private set; }

	private CameraIsometrica? _camera;
	// Alteração de IA - Revisar
	// O que faz: o desenho do personagem, seja uma imagem parada ou uma animação.
	// Por quê: com a chegada da animação de andar, o desenho deixou de ser sempre uma imagem
	//          única. Guardar pelo tipo em comum aos dois deixa o agachar funcionar igual.
	private SpriteBase3D? _sprite;
	private SensorDeteccao? _sensor;
	private float _alturaNormalDoDesenho;
	private float _quantoEstaAgachado;

	// Alteração de IA - Revisar
	// O que faz: guarda para que lado o personagem está virado, em graus — que é sempre o lado
	//            para onde ele anda (parado, fica virado para onde andou por último).
	// Por quê: comparar esta direção com o ângulo da câmera é o que diz se o desenho mostra o
	//          personagem de frente, de costas ou de lado (ver AnimacaoDirecional).
	//          (01/10/2026) Antes, o personagem olhava para onde o mouse apontava. Saiu junto com o
	//          cone de visão do jogador — e com isso acabou o "moonwalk": mouse para um lado,
	//          andando para o outro, o desenho mostrava o personagem de frente andando de costas.
	public float DirecaoOlhando { get; private set; } = 270.0f;

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: quanta luz chega no personagem agora, de 0 a 1 — e o ambiente da fase, guardado.
	// Por quê: ver o comentário onde é medida, em _PhysicsProcess.
	public float LuzNoPersonagem { get; private set; } = 1.0f;

	private AmbienteDaFase? _ambiente;
	private FonteDeLuz? _lanterna;

	// Alteração de IA - Revisar (30/09/2026)
	// O que faz: acha a lanterna do personagem — a primeira fonte de luz presa a ele.
	// Por quê: procurada pelo tipo, e não pelo nome, para funcionar com tocha, lampião ou o que a
	//          fase der a ele, sem depender de como o nó foi batizado.
	public FonteDeLuz? AcharLanterna()
	{
		if (_lanterna != null && IsInstanceValid(_lanterna))
		{
			return _lanterna;
		}

		foreach (Node filho in GetChildren())
		{
			if (filho is FonteDeLuz luz)
			{
				_lanterna = luz;
				return luz;
			}
		}
		return null;
	}

	public override void _Ready()
	{
		foreach (Node filho in GetChildren())
		{
			if (filho is SpriteBase3D desenho)
			{
				_sprite = desenho;
				break;
			}
		}

		// Alteração de IA - Revisar
		// O que faz: garante que o personagem tenha quem escolha o desenho certo para cada
		//            direção (frente, costas, lado) e toque a animação de andar.
		// Por quê: criado em código, e não montado na cena, para não se perder quando o editor
		//          do Godot regravar o arquivo da cena — o que já aconteceu três vezes.
		if (GetNodeOrNull<AnimacaoDirecional>("AnimacaoDirecional") == null)
		{
			AddChild(new AnimacaoDirecional { Name = "AnimacaoDirecional" });
		}
		_sensor = GetNodeOrNull<SensorDeteccao>("SensorDeteccao");

		if (_sprite != null)
		{
			_alturaNormalDoDesenho = _sprite.Position.Y;
		}

		// Alteração de IA - Revisar
		// O que faz: procura a câmera do mapa pelo grupo "camera_isometrica".
		// Por quê: o personagem precisa saber para onde a câmera está olhando, senão não
		//          consegue descobrir o que é "andar para frente" na visão do jogador.
		//          Procurar pelo grupo evita depender do caminho dos nós dentro da cena.
		var encontradas = GetTree().GetNodesInGroup("camera_isometrica");
		if (encontradas.Count > 0)
		{
			_camera = encontradas[0] as CameraIsometrica;
		}

		if (_camera == null)
		{
			GD.PushWarning("PlayerIsometrico: câmera não encontrada. O personagem vai andar " +
						   "nas direções fixas do mundo, ignorando para onde a câmera aponta. " +
						   "Confira se a câmera está no grupo 'camera_isometrica'.");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Alteração de IA - Revisar
		// O que faz: lê as teclas de andar e monta a direção em duas dimensões.
		// Por quê: X é esquerda/direita e Y é frente/trás vistos da tela — ainda não é a
		//          direção no mundo 3D. A conversão acontece logo abaixo.
		Vector2 comando = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

		// Alteração de IA - Revisar
		// O que faz: agacha do jeito escolhido — segurando a tecla ou apertando para alternar.
		// Por quê: as duas formas existem porque a escolha é do jogador. O padrão é segurar,
		//          que evita esquecer que está agachado; quem preferir alternar muda no futuro
		//          menu de configurações, sem tocar em código.
		if (ModoDeAgachar == FormaDeAgachar.Alternar)
		{
			if (Input.IsActionJustPressed("agachar"))
			{
				Agachado = !Agachado;
			}
		}
		else
		{
			Agachado = Input.IsActionPressed("agachar");
		}

		// quem faz as contas de percepção é o sensor; aqui só avisamos o estado
		if (_sensor != null)
		{
			_sensor.Agachado = Agachado;
		}

		// Alteração de IA - Revisar (30/09/2026)
		// O que faz: a tecla L acende e apaga a lanterna do personagem, se ele carregar uma.
		// Por quê: agora os inimigos também dependem de luz para ver. A lanterna ilumina o caminho,
		//          mas **ilumina o próprio personagem** — com ela acesa, ele é visto de longe no
		//          escuro. Apagar é a forma de se esconder na sombra. Sem poder apagar, não dava nem
		//          para testar a escuridão contra os inimigos.
		if (Input.IsActionJustPressed("lanterna"))
		{
			FonteDeLuz? lanterna = AcharLanterna();
			if (lanterna != null)
			{
				lanterna.Acesa = !lanterna.Acesa;
			}
		}

		// Alteração de IA - Revisar (30/09/2026)
		// O que faz: mede quanta luz chega no personagem agora, de 0 (breu) a 1 (claro).
		// Por quê: é a mesma conta que o inimigo faz para decidir se o vê. Fica exposto para o
		//          desenho de teste mostrar e, no futuro, para a interface avisar o jogador de que
		//          ele está visível — como a "joia de luz" dos jogos de furtividade.
		_ambiente ??= AmbienteDaFase.DaFase(GetTree());
		LuzNoPersonagem = _ambiente?.LuzEm(GlobalPosition + Vector3.Up, GetWorld3D().DirectSpaceState,
			(uint)(_sensor?.CamadaQueBloqueiaVisao ?? 8)) ?? 1.0f;

		// Alteração de IA - Revisar
		// O que faz: converte o comando do teclado na direção correspondente dentro do mundo,
		//            levando em conta para onde a câmera está apontando **neste momento**.
		// Por quê: ESTE É O PONTO MAIS IMPORTANTE DESTE ARQUIVO.
		//
		//          O jogador espera que o W leve o personagem "para longe da câmera", seja
		//          qual for o ângulo em que ela está. Se o movimento usasse as direções fixas
		//          do mundo, bastaria girar a câmera com Q ou E para o W passar a mover o
		//          personagem de lado, ou para trás — e o controle ficaria impossível de usar.
		//
		//          Por isso a direção do teclado é girada pelo mesmo ângulo da câmera antes
		//          de virar movimento de verdade.
		Vector3 direcao = Vector3.Zero;
		if (comando != Vector2.Zero)
		{
			float anguloCamera = _camera != null ? _camera.AnguloAtual : 270.0f;

			// A câmera a 270 graus olha para o norte, que é a posição inicial. Este acerto
			// alinha o "para frente" do teclado com o "para longe da câmera" que o jogador vê.
			//
			// O sinal é negativo de propósito: a roda de ângulos da câmera cresce no sentido
			// anti-horário, e a rotação aqui precisa acontecer no sentido contrário para
			// acompanhá-la. Sem esse sinal, funciona com a câmera ao norte ou ao sul, mas
			// inverte quando ela está a leste ou a oeste — e o W passa a andar na direção
			// da câmera em vez de para longe dela.
			float radianos = -Mathf.DegToRad(anguloCamera - 270.0f);

			float cos = Mathf.Cos(radianos);
			float sen = Mathf.Sin(radianos);

			direcao = new Vector3(
				comando.X * cos - comando.Y * sen,
				0.0f,
				comando.X * sen + comando.Y * cos
			).Normalized();
		}

		// Alteração de IA - Revisar
		// O que faz: aproxima a velocidade atual da desejada, aos poucos.
		// Por quê: com comando, o alvo é a velocidade máxima naquela direção e usamos a
		//          aceleração; sem comando, o alvo é parar e usamos a desaceleração.
		Vector3 velocidade = Velocity;
		float velocidadeAgora = Agachado
			? VelocidadeMaxima * Mathf.Max(0.05f, FatorDeVelocidadeAgachado)
			: VelocidadeMaxima;
		Vector3 alvoHorizontal = direcao * velocidadeAgora;
		float taxa = direcao != Vector3.Zero ? Aceleracao : Desaceleracao;

		velocidade.X = Mathf.MoveToward(velocidade.X, alvoHorizontal.X, taxa * (float)delta);
		velocidade.Z = Mathf.MoveToward(velocidade.Z, alvoHorizontal.Z, taxa * (float)delta);

		// Alteração de IA - Revisar
		// O que faz: aplica gravidade só enquanto o personagem não está no chão.
		// Por quê: parar de somar ao encostar no chão evita que o valor cresça sem limite
		//          enquanto ele está parado.
		if (!IsOnFloor())
		{
			velocidade.Y -= Gravidade * (float)delta;
		}
		else if (velocidade.Y < 0.0f)
		{
			velocidade.Y = 0.0f;
		}

		Velocity = velocidade;
		MoveAndSlide();

		AtualizarDirecaoVisual(direcao);
		AtualizarDesenhoAgachado((float)delta);
	}

	// Alteração de IA - Revisar
	// O que faz: abaixa e achata o desenho aos poucos ao agachar, e devolve ao normal ao levantar.
	// Por quê: aos poucos, e não de um quadro para o outro, porque o salto seco parece defeito.
	//          É só aparência — quem muda a percepção é o sensor, e os dois são independentes de
	//          propósito: se a animação travar, a mecânica continua correta.
	private void AtualizarDesenhoAgachado(float dt)
	{
		if (_sprite == null)
		{
			return;
		}

		_quantoEstaAgachado = Mathf.MoveToward(_quantoEstaAgachado, Agachado ? 1.0f : 0.0f,
											   dt * VelocidadeDeAgachar);

		Vector3 posicao = _sprite.Position;
		posicao.Y = _alturaNormalDoDesenho - AbaixamentoDoDesenho * _quantoEstaAgachado;
		_sprite.Position = posicao;

		Vector3 tamanho = _sprite.Scale;
		tamanho.Y = Mathf.Lerp(1.0f, AchatamentoDoDesenho, _quantoEstaAgachado);
		_sprite.Scale = tamanho;
	}

	// Alteração de IA - Revisar
	// O que faz: vira o personagem para o lado em que ele está andando.
	// Por quê: quem escolhe o desenho (frente, costas ou lado) é a AnimacaoDirecional, comparando
	//          esta direção com o ângulo da câmera — a mesma técnica usada no Doom.
	//
	//          (01/10/2026) Voltou a ser sempre a direção da caminhada: a mira pelo mouse foi
	//          retirada junto com o cone de visão do jogador. Parado, ele continua virado para
	//          onde andou por último.
	private void AtualizarDirecaoVisual(Vector3 direcao)
	{
		if (_sprite == null || direcao == Vector3.Zero)
		{
			return;
		}

		DirecaoOlhando = Mathf.PosMod(Mathf.RadToDeg(Mathf.Atan2(-direcao.Z, direcao.X)), 360.0f);

		// Alteração de IA - Revisar
		// O que faz: espelhar e escolher o desenho saiu daqui.
		// Por quê: agora é trabalho da AnimacaoDirecional, que além de espelhar escolhe entre
		//          frente, costas e lado. Deixar os dois mexendo no mesmo desenho faria um
		//          desfazer o que o outro fez a cada quadro.
	}
}

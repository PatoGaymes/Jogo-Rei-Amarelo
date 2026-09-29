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
	// O que faz: liga e desliga a mira pelo mouse.
	// Por quê: com ela desligada o personagem volta a olhar para onde anda, que é o
	//          comportamento antigo. Serve para comparar os dois durante os testes e para o
	//          caso de a equipe decidir depois que a mira no mouse não é para todo momento do
	//          jogo (numa cena de diálogo, por exemplo).
	[Export]
	public bool ApontarComOMouse { get; set; } = true;

	// Alteração de IA - Revisar
	// O que faz: informa se o personagem está agachado agora.
	// Por quê: o desenho de teste mostra isso na tela, e futuras mecânicas (passar por vãos
	//          baixos, por exemplo) vão precisar perguntar.
	public bool Agachado { get; private set; }

	// o ponto do chão para onde o cursor está apontando — usado pelo desenho de teste
	public Vector3 PontoParaOndeOlha { get; private set; }

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
	// O que faz: guarda para que lado o personagem está virado, em graus.
	// Por quê: quando a arte definitiva chegar, o personagem terá desenhos diferentes para
	//          cada direção. Comparar esta direção com o ângulo da câmera é o que diz qual
	//          desenho mostrar. Com a arte provisória, por enquanto só vira o desenho.
	public float DirecaoOlhando { get; private set; } = 270.0f;

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

		ApontarOlharParaOMouse();
		AtualizarDirecaoVisual(direcao);
		AtualizarDesenhoAgachado((float)delta);
	}

	// Alteração de IA - Revisar
	// O que faz: vira o olhar do personagem para onde o cursor do mouse está apontando no chão.
	// Por quê: separa **para onde ele anda** de **para onde ele olha**. Antes o personagem
	//          olhava sempre na direção em que andava, o que impedia andar para um lado
	//          vigiando o outro — que é justamente o que a névoa exige, já que só se enxerga
	//          o que está dentro do cone de visão.
	//
	//          A câmera **não gira junto**: ela continua nos 8 ângulos fixos, girada só por
	//          Q e E. Quem gira é o personagem. Misturar as duas coisas deixaria o
	//          enquadramento instável e enjoativo.
	//
	//          Como funciona: joga-se uma linha imaginária da câmera, passando pelo cursor,
	//          até o chão na altura dos pés do personagem. O ponto em que ela encosta é
	//          "para onde o jogador está apontando".
	private void ApontarOlharParaOMouse()
	{
		if (_camera == null || !ApontarComOMouse)
		{
			return;
		}

		Vector2 cursor = GetViewport().GetMousePosition();
		Vector3 origem = _camera.ProjectRayOrigin(cursor);
		Vector3 rumo = _camera.ProjectRayNormal(cursor);

		// a linha precisa estar descendo para cruzar o chão; se estiver na horizontal, desiste
		if (Mathf.Abs(rumo.Y) < 0.0001f)
		{
			return;
		}

		float quanto = (GlobalPosition.Y - origem.Y) / rumo.Y;
		if (quanto <= 0.0f)
		{
			return;
		}

		Vector3 alvo = origem + rumo * quanto;
		Vector3 ate = alvo - GlobalPosition;
		ate.Y = 0.0f;

		// cursor praticamente em cima do personagem: mantém o olhar onde estava, para não
		// ficar girando à toa com qualquer tremida do mouse
		if (ate.LengthSquared() < 0.04f)
		{
			return;
		}

		DirecaoOlhando = Mathf.PosMod(Mathf.RadToDeg(Mathf.Atan2(-ate.Z, ate.X)), 360.0f);
		PontoParaOndeOlha = alvo;
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
	// O que faz: guarda para que lado o personagem está indo e vira o desenho dele.
	// Por quê: por enquanto só espelha a imagem para a esquerda ou para a direita, porque a
	//          arte provisória tem um desenho só.
	//
	//          Quando a arte definitiva chegar, com desenhos para 8 direções, é aqui que
	//          entra a escolha do desenho certo: compara-se a direção do personagem com o
	//          ângulo da câmera para descobrir se ele está sendo visto de frente, de costas
	//          ou de lado. É a mesma técnica usada no Doom.
	private void AtualizarDirecaoVisual(Vector3 direcao)
	{
		if (_sprite == null)
		{
			return;
		}

		// Alteração de IA - Revisar
		// O que faz: com a mira no mouse ligada, a direção do olhar **já foi decidida** pelo
		//            cursor e não é mais sobrescrita pela direção da caminhada.
		// Por quê: é o que permite andar para um lado vigiando o outro. Sem esta condição, o
		//          personagem voltaria a encarar para onde anda no instante em que desse um
		//          passo, e a mira no mouse não valeria de nada enquanto ele estivesse andando.
		if (!ApontarComOMouse)
		{
			if (direcao == Vector3.Zero)
			{
				return;
			}
			DirecaoOlhando = Mathf.PosMod(Mathf.RadToDeg(Mathf.Atan2(-direcao.Z, direcao.X)), 360.0f);
		}

		// Alteração de IA - Revisar
		// O que faz: espelhar e escolher o desenho saiu daqui.
		// Por quê: agora é trabalho da AnimacaoDirecional, que além de espelhar escolhe entre
		//          frente, costas e lado. Deixar os dois mexendo no mesmo desenho faria um
		//          desfazer o que o outro fez a cada quadro.
	}
}

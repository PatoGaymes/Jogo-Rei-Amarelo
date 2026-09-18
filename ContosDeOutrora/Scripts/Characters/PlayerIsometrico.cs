using Godot;

// Alteração de IA - Revisar
// O que faz: controla o personagem andando pelo mapa 3D.
// Por quê: o mapa é 3D, mas o personagem é um desenho 2D em pé, como uma folha de papel —
//          é o estilo do Don't Starve. Então ele se move nas três dimensões, mas o que
//          aparece na tela é sempre uma imagem plana virada para a câmera.
public partial class PlayerIsometrico : CharacterBody3D
{
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

	private CameraIsometrica? _camera;
	private Sprite3D? _sprite;

	// Alteração de IA - Revisar
	// O que faz: guarda para que lado o personagem está virado, em graus.
	// Por quê: quando a arte definitiva chegar, o personagem terá desenhos diferentes para
	//          cada direção. Comparar esta direção com o ângulo da câmera é o que diz qual
	//          desenho mostrar. Com a arte provisória, por enquanto só vira o desenho.
	public float DirecaoOlhando { get; private set; } = 270.0f;

	public override void _Ready()
	{
		_sprite = GetNodeOrNull<Sprite3D>("Sprite3D");

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
		Vector3 alvoHorizontal = direcao * VelocidadeMaxima;
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
		if (direcao == Vector3.Zero || _sprite == null)
		{
			return;
		}

		// ângulo para onde o personagem anda, na mesma roda de graus usada pela câmera
		DirecaoOlhando = Mathf.PosMod(Mathf.RadToDeg(Mathf.Atan2(-direcao.Z, direcao.X)), 360.0f);

		float anguloCamera = _camera != null ? _camera.AnguloAtual : 270.0f;

		// diferença entre para onde ele anda e de onde a câmera olha
		float relativo = Mathf.PosMod(DirecaoOlhando - anguloCamera, 360.0f);

		// entre 0 e 180 significa que ele está indo para a direita da tela
		_sprite.FlipH = relativo > 180.0f;
	}
}

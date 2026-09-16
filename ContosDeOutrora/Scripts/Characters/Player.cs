using Godot;

// Alteração de IA - Revisar
// O que faz: controla o personagem durante a exploração — anda para a esquerda e para a
//            direita, e vira o desenho para o lado em que está andando.
// Por quê: é a movimentação descrita no GDD, onde a exploração acontece só para frente e
//          para trás dentro de um corredor. Não existe pulo nem movimento para cima ou
//          para baixo, por isso o personagem só se move no eixo horizontal.
public partial class Player : CharacterBody2D
{
	// Alteração de IA - Revisar
	// O que faz: velocidade máxima de caminhada, em pontos por segundo.
	// Por quê: fica com [Export] para a equipe testar valores direto no painel do Godot,
	//          com o jogo rodando, sem precisar mexer no código nem recompilar.
	[Export]
	public float VelocidadeMaxima { get; set; } = 400.0f;

	// Alteração de IA - Revisar
	// O que faz: o quão rápido o personagem sai do zero até a velocidade máxima.
	// Por quê: se ele saísse já na velocidade final, o movimento pareceria travado e
	//          robótico. Ganhar velocidade aos poucos deixa a caminhada mais natural.
	//          Valor alto = arranca rápido. Valor baixo = arranca devagar.
	[Export]
	public float Aceleracao { get; set; } = 2500.0f;

	// Alteração de IA - Revisar
	// O que faz: o quão rápido o personagem para quando solta a tecla.
	// Por quê: é propositalmente maior que a aceleração, para o personagem parar mais
	//          rápido do que arranca. Isso faz o controle parecer mais preciso nas mãos
	//          do jogador.
	[Export]
	public float Desaceleracao { get; set; } = 3500.0f;

	// Alteração de IA - Revisar
	// O que faz: força que puxa o personagem para baixo, mantendo ele encostado no chão.
	// Por quê: o jogo não tem pulo, mas sem nada puxando para baixo o personagem ficaria
	//          flutuando se a cena o posicionasse um pouco acima do chão. Isso também
	//          deixa o caminho aberto para corredores com desnível no futuro.
	[Export]
	public float Gravidade { get; set; } = 1800.0f;

	// Alteração de IA - Revisar
	// O que faz: guarda o desenho do personagem. O "?" avisa que este campo pode estar vazio.
	// Por quê: ele só é preenchido quando a cena entra no jogo (no _Ready abaixo), e não no
	//          momento em que o objeto é criado. Sem o "?", o compilador reclama disso.
	private Sprite2D? _sprite;

	public override void _Ready()
	{
		// Alteração de IA - Revisar
		// O que faz: guarda uma referência ao desenho do personagem para poder virá-lo depois.
		// Por quê: buscar o nó uma vez só, aqui no início, é mais rápido do que procurá-lo
		//          a cada quadro do jogo.
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Alteração de IA - Revisar
		// O que faz: lê os comandos de andar e devolve -1 (esquerda), 0 (parado) ou 1 (direita).
		// Por quê: usa os nomes "move_left" e "move_right" configurados em Project Settings →
		//          Input Map. Assim, trocar a tecla ou adicionar controle não exige mexer aqui.
		float direcao = Input.GetAxis("move_left", "move_right");

		// Alteração de IA - Revisar
		// O que faz: aproxima a velocidade atual da velocidade desejada, aos poucos.
		// Por quê: quando há comando, o alvo é a velocidade máxima naquela direção e usamos a
		//          aceleração. Quando não há comando, o alvo é zero e usamos a desaceleração.
		//          MoveToward garante que a velocidade nunca passe do alvo.
		float velocidadeAlvo = direcao * VelocidadeMaxima;
		float taxa = direcao != 0.0f ? Aceleracao : Desaceleracao;

		Vector2 velocidade = Velocity;
		velocidade.X = Mathf.MoveToward(velocidade.X, velocidadeAlvo, taxa * (float)delta);

		// Alteração de IA - Revisar
		// O que faz: aplica a gravidade apenas enquanto o personagem não está no chão.
		// Por quê: parar de somar gravidade ao encostar no chão evita que o valor cresça
		//          indefinidamente enquanto ele está parado.
		if (!IsOnFloor())
		{
			velocidade.Y += Gravidade * (float)delta;
		}

		Velocity = velocidade;
		MoveAndSlide();

		// Alteração de IA - Revisar
		// O que faz: vira o desenho para o lado em que o personagem está andando.
		// Por quê: só vira quando há comando. Se virasse também ao parar, o personagem
		//          daria uma guinada estranha no instante em que a velocidade chega a zero.
		if (_sprite != null && direcao != 0.0f)
		{
			_sprite.FlipH = direcao < 0.0f;
		}
	}
}

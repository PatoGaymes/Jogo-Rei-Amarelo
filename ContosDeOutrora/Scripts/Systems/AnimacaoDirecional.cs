using Godot;

// Alteração de IA - Revisar
// O que faz: qualquer personagem que tenha uma direção de olhar — jogador ou inimigo.
// Por quê: a animação precisa saber para onde o personagem está virado para escolher o desenho
//          certo. Os dois tipos de personagem já guardam essa direção; esta "combinação" deixa a
//          animação funcionar com qualquer um deles sem saber qual é.
public interface IPersonagemQueOlha
{
	float DirecaoOlhando { get; }
}

// Alteração de IA - Revisar
// O que faz: escolhe qual desenho mostrar — de frente, de costas ou de lado — conforme para onde
//            o personagem está virado **em relação à câmera**, e toca a animação de andar enquanto
//            ele se move.
// Por quê: o personagem é uma folha de papel em pé dentro do mundo 3D, e a câmera gira em 8
//          ângulos. "Virado para o norte" pode ser visto de frente, de costas ou de lado,
//          dependendo de onde a câmera está. O que decide o desenho não é a direção no mapa, é a
//          diferença entre para onde ele olha e de onde a câmera olha. É a técnica do Doom.
//
//          Só existe desenho de um lado; o outro é o mesmo desenho espelhado. Por isso os quadros
//          precisam vir com **os pés no centro** — senão o personagem pularia de lugar ao espelhar
//          (ver inserir/README.md).
//
//          Funciona também com desenho parado (imagem única): nesse caso só espelha.
public partial class AnimacaoDirecional : Node
{
	// Alteração de IA - Revisar
	// O que faz: o nome da ação cujas animações serão tocadas. Os desenhos precisam se chamar
	//            "<ação>_frente", "<ação>_costas" e "<ação>_lado".
	// Por quê: hoje só existe "andar". Quando chegarem correr, agachar ou atacar, basta seguir o
	//          mesmo padrão de nome e trocar este campo — nada de código novo.
	[Export]
	public string Acao { get; set; } = "andar";

	// Alteração de IA - Revisar
	// O que faz: diz se o desenho de lado foi feito olhando para a direita.
	// Por quê: o espelhamento depende disso. Os quadros do Desgarrado olham para a direita; se
	//          outro personagem vier desenhado olhando para a esquerda, é só desmarcar.
	[Export]
	public bool LadoOlhaParaADireita { get; set; } = true;

	// Alteração de IA - Revisar
	// O que faz: a velocidade de caminhada em que a animação toca no ritmo original do desenho.
	// Por quê: andando mais rápido que isso a animação acelera; mais devagar (agachado, por
	//          exemplo), desacelera. Sem esse acerto os pés "patinam" no chão: o corpo anda uma
	//          distância e as pernas mostram outra. Se os pés parecerem deslizar, é este número.
	[Export]
	public float VelocidadeDeReferencia { get; set; } = 3.0f;

	// Alteração de IA - Revisar
	// O que faz: quanto o ângulo precisa passar do limite entre duas vistas para o desenho trocar.
	// Por quê: sem essa folga, andando bem na divisa entre "de frente" e "de lado" (na diagonal,
	//          ou com a câmera no meio de um giro) o desenho ficaria trocando sem parar.
	[Export]
	public float FolgaParaTrocarDeVista { get; set; } = 8.0f;

	public enum Vista
	{
		Frente,
		Lado,
		Costas
	}

	public Vista VistaAtual { get; private set; } = Vista.Frente;
	public bool Espelhado { get; private set; }
	public bool Andando { get; private set; }

	private SpriteBase3D? _desenho;
	private AnimatedSprite3D? _animado;
	private CharacterBody3D? _corpo;
	private IPersonagemQueOlha? _olhar;
	private CameraIsometrica? _camera;

	public override void _Ready()
	{
		_corpo = GetParent() as CharacterBody3D;
		_olhar = GetParent() as IPersonagemQueOlha;

		foreach (Node filho in GetParent().GetChildren())
		{
			if (filho is SpriteBase3D desenho)
			{
				_desenho = desenho;
				break;
			}
		}
		_animado = _desenho as AnimatedSprite3D;

		var cameras = GetTree().GetNodesInGroup("camera_isometrica");
		if (cameras.Count > 0)
		{
			_camera = cameras[0] as CameraIsometrica;
		}
	}

	public override void _Process(double delta)
	{
		if (_desenho == null || _olhar == null)
		{
			return;
		}

		float anguloCamera = _camera?.AnguloAtual ?? 270.0f;

		// Alteração de IA - Revisar
		// O que faz: mede para onde o personagem olha **visto da câmera**. 0 é olhando para a
		//            câmera, 90 é para a direita da tela, 180 é de costas, 270 é para a esquerda.
		float relativo = Mathf.PosMod(_olhar.DirecaoOlhando - anguloCamera, 360.0f);
		EscolherVista(relativo);

		float velocidade = 0.0f;
		if (_corpo != null)
		{
			velocidade = new Vector2(_corpo.Velocity.X, _corpo.Velocity.Z).Length();
		}
		Andando = velocidade > 0.15f;

		_desenho.FlipH = Espelhado;

		if (_animado != null)
		{
			TocarAnimacao(velocidade);
		}
	}

	// Alteração de IA - Revisar
	// O que faz: decide entre frente, lado e costas, com uma folga nas divisas.
	// Por quê: cada vista cobre um quarto da volta (90 graus). A folga só vale para **sair** da
	//          vista atual — é o que impede o desenho de piscar entre duas vistas na divisa.
	private void EscolherVista(float relativo)
	{
		Vista nova = VistaPara(relativo, 0.0f);

		if (nova != VistaAtual && VistaPara(relativo, FolgaParaTrocarDeVista) == VistaAtual)
		{
			nova = VistaAtual;   // ainda dentro da folga: mantém
		}

		VistaAtual = nova;

		// de lado, olhando para a direita da tela (entre 0 e 180) ou para a esquerda
		bool paraADireita = relativo > 0.0f && relativo < 180.0f;
		Espelhado = VistaAtual == Vista.Lado && (paraADireita != LadoOlhaParaADireita);
	}

	private static Vista VistaPara(float relativo, float folga)
	{
		// a folga alarga as vistas de lado para dentro das outras duas
		if (relativo >= 45.0f - folga && relativo < 135.0f + folga)
		{
			return Vista.Lado;
		}
		if (relativo >= 225.0f - folga && relativo < 315.0f + folga)
		{
			return Vista.Lado;
		}
		if (relativo >= 135.0f && relativo < 225.0f)
		{
			return Vista.Costas;
		}
		return Vista.Frente;
	}

	// Alteração de IA - Revisar
	// O que faz: toca a animação da vista atual no ritmo da caminhada, ou para no primeiro quadro
	//            quando o personagem está parado.
	// Por quê: ao trocar de vista no meio do passo, o quadro em que a perna estava é mantido.
	//          Sem isso, cada mudança de direção (ou giro da câmera) recomeçaria o passo do zero e
	//          as pernas dariam um tranco.
	private void TocarAnimacao(float velocidade)
	{
		string nome = $"{Acao}_{NomeDaVista(VistaAtual)}";
		if (_animado!.SpriteFrames == null || !_animado.SpriteFrames.HasAnimation(nome))
		{
			return;
		}

		if (_animado.Animation != nome)
		{
			int quadro = _animado.Frame;
			float progresso = _animado.FrameProgress;
			_animado.Animation = nome;
			_animado.SetFrameAndProgress(quadro, progresso);
		}

		if (Andando)
		{
			_animado.SpeedScale = Mathf.Clamp(velocidade / Mathf.Max(0.1f, VelocidadeDeReferencia), 0.25f, 3.0f);
			if (!_animado.IsPlaying())
			{
				_animado.Play(nome);
			}
		}
		else
		{
			// parado: o primeiro quadro serve de pose de descanso até chegar a animação própria
			_animado.Stop();
			_animado.Frame = 0;
		}
	}

	private static string NomeDaVista(Vista vista) => vista switch
	{
		Vista.Costas => "costas",
		Vista.Lado => "lado",
		_ => "frente"
	};
}

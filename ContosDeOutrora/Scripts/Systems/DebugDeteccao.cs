using Godot;

// Alteração de IA - Revisar
// O que faz: desenha no chão, em linhas coloridas, os raios de percepção de um personagem —
//            e, no caso do inimigo, mostra também em que modo ele está.
// Por quê: os raios são invisíveis no jogo. Sem poder vê-los, testar o sistema de furtividade
//          vira adivinhação: não dá para saber se o inimigo não percebeu o jogador porque a
//          conta está certa ou porque está errada. Com o desenho, o erro aparece na tela.
//
//          Isto é **ferramenta de teste**: fica desligado no jogo publicado.
public partial class DebugDeteccao : Node3D
{
	// Alteração de IA - Revisar
	// O que faz: liga e desliga o desenho.
	// Por quê: a tecla F3 alterna durante o jogo, para conferir uma situação específica sem
	//          precisar parar e recompilar.
	[Export]
	public bool Mostrar { get; set; } = true;

	[Export]
	public bool EhInimigo { get; set; } = false;

	// Alteração de IA - Revisar
	// O que faz: as cores de cada raio.
	// Por quê: cada um significa uma coisa, e cor é a forma mais rápida de distinguir sem ler
	//          legenda. Amarelo = desconfia, laranja = tem certeza, vermelho = combate,
	//          azul = campo de visão.
	private static readonly Color CorRaio1 = new(1.0f, 0.85f, 0.2f, 0.9f);
	private static readonly Color CorRaio2 = new(1.0f, 0.45f, 0.1f, 0.9f);
	private static readonly Color CorEncontro = new(1.0f, 0.15f, 0.15f, 0.9f);
	private static readonly Color CorVisao = new(0.3f, 0.7f, 1.0f, 0.9f);

	private SensorDeteccao _sensor = null!;
	private InimigoIA? _inimigo;
	private PlayerIsometrico? _jogador;
	private MeshInstance3D _desenho = null!;
	private ImmediateMesh _malha = null!;
	private Label3D _texto = null!;

	public override void _Ready()
	{
		_sensor = GetParent().GetNode<SensorDeteccao>("SensorDeteccao");
		_inimigo = GetParent() as InimigoIA;
		_jogador = GetParent() as PlayerIsometrico;

		// Alteração de IA - Revisar
		// O que faz: prepara um material que ignora luz e aparece por cima do cenário.
		// Por quê: as linhas precisam ser vistas mesmo atrás de uma parede ou no escuro —
		//          é justamente nessas situações que dá vontade de conferir os raios.
		_malha = new ImmediateMesh();
		_desenho = new MeshInstance3D
		{
			Mesh = _malha,
			CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
		};
		var material = new StandardMaterial3D
		{
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
			VertexColorUseAsAlbedo = true,
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
			NoDepthTest = true
		};
		_desenho.MaterialOverride = material;
		AddChild(_desenho);

		// Alteração de IA - Revisar
		// O que faz: a altura em que o texto de teste flutua sobre o personagem.
		// Por quê: precisa caber entre a cabeça do personagem e o balão do "?", que fica em
		//          3.1. Com a barra de suspeita o texto passou a ter três linhas, e na altura
		//          antiga a última linha caía por cima do sprite.
		_texto = new Label3D
		{
			Position = new Vector3(0.0f, 2.5f, 0.0f),
			Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
			FontSize = 32,
			NoDepthTest = true,
			Modulate = Colors.White
		};
		AddChild(_texto);
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug_raios"))
		{
			Mostrar = !Mostrar;
		}

		_desenho.Visible = Mostrar;
		_texto.Visible = Mostrar;

		if (!Mostrar)
		{
			return;
		}

		Redesenhar();
		AtualizarTexto();
	}

	private void Redesenhar()
	{
		_malha.ClearSurfaces();
		_malha.SurfaceBegin(Mesh.PrimitiveType.Lines);

		// as linhas são desenhadas em volta do personagem, um palmo acima do chão
		Vector3 centro = Vector3.Up * 0.05f;

		Circulo(centro, _sensor.Raio1Efetivo, CorRaio1);

		// Alteração de IA - Revisar
		// O que faz: o raio interno só é desenhado para o jogador.
		// Por quê: por decisão de design, os inimigos não têm esse raio — desenhá-lo neles
		//          daria a impressão errada de que têm.
		if (!EhInimigo)
		{
			Circulo(centro, _sensor.Raio2Efetivo, CorRaio2);
		}
		else
		{
			Circulo(centro, _sensor.RaioEncontro, CorEncontro);
		}

		float olhando = EhInimigo
			? (_inimigo?.DirecaoOlhando ?? 270.0f)
			: (_jogador?.DirecaoOlhando ?? 270.0f);

		Cone(centro, _sensor.AlcanceVisaoEfetivo, _sensor.AberturaVisao, olhando, CorVisao);

		_malha.SurfaceEnd();
	}

	// Alteração de IA - Revisar
	// O que faz: desenha um círculo deitado no chão, feito de segmentos de reta.
	// Por quê: não existe "desenhar círculo" em 3D — o que existe são linhas. 48 segmentos
	//          já ficam redondos o suficiente a olho nu e são baratos de desenhar.
	private void Circulo(Vector3 centro, float raio, Color cor, int lados = 48)
	{
		for (int i = 0; i < lados; i++)
		{
			float a1 = Mathf.Tau * i / lados;
			float a2 = Mathf.Tau * (i + 1) / lados;
			_malha.SurfaceSetColor(cor);
			_malha.SurfaceAddVertex(centro + new Vector3(Mathf.Cos(a1) * raio, 0, Mathf.Sin(a1) * raio));
			_malha.SurfaceSetColor(cor);
			_malha.SurfaceAddVertex(centro + new Vector3(Mathf.Cos(a2) * raio, 0, Mathf.Sin(a2) * raio));
		}
	}

	// Alteração de IA - Revisar
	// O que faz: desenha o campo de visão como uma "fatia de pizza" apontada para onde o
	//            personagem está olhando.
	// Por quê: mostra as duas coisas que importam do cone: até onde ele alcança e o quanto é
	//          aberto. As duas linhas retas das bordas deixam claro o limite lateral.
	private void Cone(Vector3 centro, float alcance, float aberturaGraus, float direcaoGraus, Color cor)
	{
		float meia = Mathf.DegToRad(aberturaGraus * 0.5f);
		float dir = Mathf.DegToRad(direcaoGraus);

		Vector3 Ponto(float ang) => centro + new Vector3(Mathf.Cos(ang) * alcance, 0, -Mathf.Sin(ang) * alcance);

		// as duas bordas
		foreach (float lado in new[] { -meia, meia })
		{
			_malha.SurfaceSetColor(cor);
			_malha.SurfaceAddVertex(centro);
			_malha.SurfaceSetColor(cor);
			_malha.SurfaceAddVertex(Ponto(dir + lado));
		}

		// o arco que fecha a ponta
		const int passos = 24;
		for (int i = 0; i < passos; i++)
		{
			float a1 = dir - meia + (2 * meia) * i / passos;
			float a2 = dir - meia + (2 * meia) * (i + 1) / passos;
			_malha.SurfaceSetColor(cor);
			_malha.SurfaceAddVertex(Ponto(a1));
			_malha.SurfaceSetColor(cor);
			_malha.SurfaceAddVertex(Ponto(a2));
		}
	}

	// Alteração de IA - Revisar
	// O que faz: escreve sobre a cabeça do inimigo em que modo ele está e, quando está sendo
	//            observado, quanto falta para ele reconhecer o jogador.
	// Por quê: sem isso, dá para ver os raios se tocando mas não dá para saber o que o
	//          inimigo concluiu daquilo. O texto mostra a decisão, não só a geometria.
	private void AtualizarTexto()
	{
		if (_inimigo == null)
		{
			_texto.Text = "jogador";
			_texto.Modulate = Colors.White;
			return;
		}

		string modo = _inimigo.ModoAtual switch
		{
			InimigoIA.Modo.Ronda => "RONDA",
			InimigoIA.Modo.Alerta => "OUVIU ALGO",
			InimigoIA.Modo.Busca => "BUSCA",
			InimigoIA.Modo.Perseguicao => "PERSEGUIÇÃO",
			InimigoIA.Modo.Idle => "PARADO",
			_ => "?"
		};

		_texto.Modulate = _inimigo.ModoAtual switch
		{
			InimigoIA.Modo.Ronda => new Color(0.6f, 1.0f, 0.6f),
			InimigoIA.Modo.Alerta => new Color(1.0f, 0.75f, 0.2f),
			InimigoIA.Modo.Busca => new Color(1.0f, 0.9f, 0.3f),
			InimigoIA.Modo.Perseguicao => new Color(1.0f, 0.3f, 0.3f),
			InimigoIA.Modo.Idle => new Color(0.6f, 0.6f, 0.9f),
			_ => Colors.White
		};

		// Alteração de IA - Revisar
		// O que faz: durante o alerta, mostra em que etapa da reação ele está.
		// Por quê: sem isso dá para ver que ele parou, mas não se está estranhando, olhando
		//          para os lados ou já virando para o som — que é justamente o que precisa
		//          ser conferido ao ajustar os tempos.
		if (_inimigo.ModoAtual == InimigoIA.Modo.Alerta)
		{
			_texto.Text = $"{modo}\n{_inimigo.EtapaDoAlertaTexto}\n{Barra()}";
			return;
		}

		_texto.Text = $"{modo}\n{Barra()}";
	}

	// Alteração de IA - Revisar
	// O que faz: escreve a suspeita como porcentagem e como uma barrinha feita de blocos.
	// Por quê: o número diz o valor exato, útil para ajustar; a barrinha mostra de relance se
	//          está subindo ou descendo, sem precisar ler. Junto vai a origem da suspeita
	//          ("ouvindo", "vendo", "perto") — sem ela dá para ver a barra subir mas não saber
	//          qual dos três alcances está causando isso, que é metade do trabalho de ajustar.
	private string Barra()
	{
		if (_inimigo == null)
		{
			return "";
		}

		float por = _inimigo.Suspeita;
		int cheios = Mathf.RoundToInt(por / 10.0f);
		string desenho = new string('=', cheios) + new string('.', 10 - cheios);
		string fonte = _inimigo.FonteDaSuspeita;

		return fonte.Length > 0
			? $"[{desenho}] {por:0}% {fonte}"
			: $"[{desenho}] {por:0}%";
	}
}

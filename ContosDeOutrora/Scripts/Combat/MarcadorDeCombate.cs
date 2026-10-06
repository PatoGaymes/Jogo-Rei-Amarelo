using Godot;

// Alteração de IA - Revisar
// O que faz: o que aparece sobre a cabeça de cada lutador durante o combate — a barra de vida, o
//            nome, os efeitos que ele carrega e os números de dano e cura que sobem e somem.
// Por quê: é o retorno visual do combate. Sem ele o jogador não sabe se acertou, quanto tirou, nem
//          por que o inimigo não se mexeu (atordoado, enraizado). Desenhado por cima da névoa e das
//          paredes, como a interface — ele não faz parte do cenário.
public partial class MarcadorDeCombate : Node3D
{
	private const float LarguraDaBarra = 0.8f;

	private Sprite3D _fundo = null!;
	private Sprite3D _vida = null!;
	private Label3D _nome = null!;
	private Label3D _efeitos = null!;
	private Label3D _intencao = null!;
	private Color _corDoLado;

	// Alteração de IA - Revisar
	// O que faz: cria o marcador na altura certa, com a cor do lado (azul aliado, vermelho inimigo).
	public static MarcadorDeCombate Criar(Combatente quem, float altura)
	{
		var marcador = new MarcadorDeCombate
		{
			Name = "MarcadorDeCombate",
			Position = new Vector3(0.0f, altura, 0.0f),
			_corDoLado = quem.Lado == LadoDoCombate.Aliados
				? new Color(0.35f, 0.65f, 1.0f)
				: new Color(1.0f, 0.32f, 0.30f),
		};
		marcador.Montar();
		return marcador;
	}

	private void Montar()
	{
		// uma textura branca de 64 x 5 pontos serve de barra: escalada, fica do tamanho certo
		var branco = Image.CreateEmpty(64, 5, false, Image.Format.Rgba8);
		branco.Fill(Colors.White);
		var textura = ImageTexture.CreateFromImage(branco);
		float tamanhoDoPonto = LarguraDaBarra / 64.0f;

		_fundo = NovoSprite(textura, tamanhoDoPonto, new Color(0.05f, 0.05f, 0.07f, 0.85f), 120);
		_vida = NovoSprite(textura, tamanhoDoPonto, _corDoLado, 121);
		// a barra de vida mostra só um pedaço da textura, e é deslocada para começar sempre na
		// ponta esquerda do fundo (ver Atualizar)
		_vida.RegionEnabled = true;
		_vida.RegionRect = new Rect2(0.0f, 0.0f, 64.0f, 5.0f);
		AddChild(_fundo);
		AddChild(_vida);

		_nome = NovoTexto(28, Colors.White);
		_nome.Position = new Vector3(0.0f, 0.13f, 0.0f);
		AddChild(_nome);

		_efeitos = NovoTexto(24, new Color(1.0f, 0.88f, 0.55f));
		_efeitos.Position = new Vector3(0.0f, 0.30f, 0.0f);
		AddChild(_efeitos);

		_intencao = NovoTexto(26, new Color(1.0f, 0.55f, 0.95f));
		_intencao.Position = new Vector3(0.0f, 0.48f, 0.0f);
		AddChild(_intencao);
	}

	private static Sprite3D NovoSprite(Texture2D textura, float tamanho, Color cor, int prioridade)
	{
		return new Sprite3D
		{
			Texture = textura,
			PixelSize = tamanho,
			Modulate = cor,
			Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
			Shaded = false,
			NoDepthTest = true,
			RenderPriority = prioridade,
			AlphaCut = SpriteBase3D.AlphaCutMode.Disabled,
		};
	}

	private static Label3D NovoTexto(int tamanho, Color cor)
	{
		return new Label3D
		{
			FontSize = tamanho,
			OutlineSize = 8,
			Modulate = cor,
			PixelSize = 0.004f,
			Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
			NoDepthTest = true,
			RenderPriority = 122,
			OutlineRenderPriority = 121,
		};
	}

	// Alteração de IA - Revisar
	// O que faz: atualiza a barra, o nome e os efeitos a partir do estado do lutador.
	public void Atualizar(Combatente quem)
	{
		float fracao = quem.VidaMaxima > 0 ? Mathf.Clamp((float)quem.Vida / quem.VidaMaxima, 0.0f, 1.0f) : 0.0f;
		float largura = Mathf.Max(0.5f, 64.0f * fracao);
		_vida.RegionRect = new Rect2(0.0f, 0.0f, largura, 5.0f);
		_vida.Offset = new Vector2(-32.0f + largura * 0.5f, 0.0f);
		_nome.Text = $"{quem.Nome}  {quem.Vida}/{quem.VidaMaxima}";
		_efeitos.Text = quem.TextoDosEfeitos();
		Visible = quem.Vivo;
	}

	// Alteração de IA - Revisar
	// O que faz: mostra (ou apaga) a intenção revelada de um inimigo — "→ Khalid".
	public void MostrarIntencao(string? texto)
	{
		_intencao.Text = texto ?? "";
	}

	// Alteração de IA - Revisar
	// O que faz: um número (ou palavra) que sobe da cabeça e some — dano, cura, "errou".
	public void Flutuar(string texto, Color cor)
	{
		var numero = NovoTexto(44, cor);
		numero.Text = texto;
		numero.Position = new Vector3(0.0f, 0.2f, 0.0f);
		AddChild(numero);

		Tween animacao = numero.CreateTween();
		animacao.SetParallel();
		animacao.TweenProperty(numero, "position:y", 0.95f, 1.0f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
		animacao.TweenProperty(numero, "modulate:a", 0.0f, 1.0f).SetDelay(0.35f);
		animacao.Chain().TweenCallback(Callable.From(numero.QueueFree));
	}
}

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Alteração de IA - Revisar
// O que faz: o menu de equipe (tecla T). Por enquanto mostra só o que importa agora: a lista dos
//            personagens e a **formação de início de luta**, numa mini-região de hexágonos onde dá
//            para arrastar cada personagem para outra casa.
// Por quê: pedido do PO em 06/10/2026, "bem simples, justamente para testar". A frente da formação
//          (para cima no menu) é virada para os inimigos quando a luta começa. Se a casa prevista
//          tiver parede, árvore ou outro personagem no mapa, o personagem fica na casa livre mais
//          próxima — os obstáculos contam.
//          O menu pausa o jogo enquanto está aberto. Montado em código, sem arquivo de cena.
public partial class MenuDeEquipe : CanvasLayer
{
	[Signal]
	public delegate void FechouEventHandler();

	public override void _Ready()
	{
		Layer = 20;
		ProcessMode = ProcessModeEnum.Always;

		var fundo = new ColorRect { Color = new Color(0.0f, 0.0f, 0.0f, 0.62f) };
		fundo.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		AddChild(fundo);

		// o painel fica sempre no meio da tela, em qualquer tamanho de janela
		var centro = new CenterContainer();
		centro.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		fundo.AddChild(centro);

		var painel = new PanelContainer { CustomMinimumSize = new Vector2(1240, 840) };
		painel.AddThemeStyleboxOverride("panel", Estilos.Painel(0.94f));
		centro.AddChild(painel);

		var colunas = new VBoxContainer();
		colunas.AddThemeConstantOverride("separation", 14);
		painel.AddChild(colunas);

		colunas.AddChild(Estilos.Texto("Equipe", 40, Colors.White));
		colunas.AddChild(Estilos.Texto(
			"Formação de início de luta — arraste os personagens entre os hexágonos. A frente (para cima) fica " +
			"virada para os inimigos quando a luta começa. Se a casa tiver parede, árvore ou alguém no mapa, " +
			"o personagem fica na livre mais próxima.", 20, new Color(0.8f, 0.82f, 0.88f), largura: 1180));

		var meio = new HBoxContainer();
		meio.AddThemeConstantOverride("separation", 30);
		meio.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		colunas.AddChild(meio);

		var lista = new VBoxContainer();
		lista.CustomMinimumSize = new Vector2(380, 0);
		lista.AddThemeConstantOverride("separation", 10);
		meio.AddChild(lista);

		Equipe? equipe = Equipe.Instancia;
		if (equipe != null)
		{
			foreach (Equipe.Membro membro in equipe.Membros)
			{
				lista.AddChild(LinhaDoMembro(membro, membro == equipe.Lider));
			}
		}

		var editor = new EditorDeFormacao { CustomMinimumSize = new Vector2(780, 620) };
		meio.AddChild(editor);

		var rodape = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.End };
		colunas.AddChild(rodape);
		var fechar = new Button { Text = "Fechar (T)", CustomMinimumSize = new Vector2(220, 52) };
		fechar.AddThemeFontSizeOverride("font_size", 24);
		fechar.Pressed += () => EmitSignal(SignalName.Fechou);
		rodape.AddChild(fechar);
	}

	// Alteração de IA - Revisar
	// O que faz: uma linha da lista — retrato, nome, classe, vida e se é o líder ou se morreu.
	private static Control LinhaDoMembro(Equipe.Membro membro, bool lider)
	{
		var linha = new HBoxContainer();
		linha.AddThemeConstantOverride("separation", 12);

		var retrato = new TextureRect
		{
			Texture = Retratos.De(membro.Ficha),
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			CustomMinimumSize = new Vector2(70, 90),
			Modulate = membro.Morto ? new Color(0.4f, 0.4f, 0.4f) : Colors.White,
		};
		linha.AddChild(retrato);

		var textos = new VBoxContainer();
		string marca = lider ? "  (líder)" : "";
		string estado = membro.Morto ? "morto" : $"Vida {membro.Vida}/{membro.Ficha.Vida}";
		textos.AddChild(Estilos.Texto($"{membro.Ficha.Nome}{marca}", 24, lider ? new Color(1.0f, 0.86f, 0.4f) : Colors.White));
		textos.AddChild(Estilos.Texto($"{membro.Ficha.Classe} · {estado}", 18, new Color(0.75f, 0.77f, 0.82f)));
		linha.AddChild(textos);
		return linha;
	}

	public override void _UnhandledInput(InputEvent evento)
	{
		if (evento.IsActionPressed("pause") || evento.IsActionPressed("ui_cancel"))
		{
			EmitSignal(SignalName.Fechou);
			GetViewport().SetInputAsHandled();
		}
	}
}

// Alteração de IA - Revisar
// O que faz: a mini-região de hexágonos do menu de equipe, com os personagens que dá para arrastar.
// Por quê: desenhada à mão (sem cena) para ser simples. Os hexágonos são os mesmos da grade do
//          combate — de topo reto, com a frente para cima — então o que se vê aqui é exatamente
//          como a equipe entra na luta, só que virada para os inimigos.
public partial class EditorDeFormacao : Control
{
	private const float TamanhoDaCasa = 64.0f;

	private Equipe.Membro? _arrastando;
	private Vector2 _posicaoDoMouse;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Stop;
	}

	// o centro da mini-região, abaixo da faixa onde fica o aviso da frente
	private Vector2 Centro => new(Size.X * 0.5f, 46.0f + TamanhoDaCasa * Mathf.Sqrt(3.0f) * (Equipe.RaioDaFormacao + 0.5f));

	private Vector2 PontoDaCasa(Hex h) => Centro + Hex.CentroNoPlano(h, TamanhoDaCasa);

	private Hex CasaDoPonto(Vector2 ponto) => Hex.DoPonto(ponto - Centro, TamanhoDaCasa);

	private static IEnumerable<Hex> CasasDaRegiao()
	{
		int raio = Equipe.RaioDaFormacao;
		for (int q = -raio; q <= raio; q++)
		{
			for (int r = Math.Max(-raio, -q - raio); r <= Math.Min(raio, -q + raio); r++)
			{
				yield return new Hex(q, r);
			}
		}
	}

	public override void _Draw()
	{
		var fonte = ThemeDB.FallbackFont;

		// a seta da frente
		DrawString(fonte, new Vector2(0.0f, 26.0f), "^  FRENTE — lado dos inimigos  ^", HorizontalAlignment.Center, Size.X, 22,
			new Color(1.0f, 0.55f, 0.45f));

		Equipe? equipe = Equipe.Instancia;
		Hex? sobOMouse = Hex.Distancia(CasaDoPonto(_posicaoDoMouse), new Hex(0, 0)) <= Equipe.RaioDaFormacao
			? CasaDoPonto(_posicaoDoMouse)
			: null;

		foreach (Hex h in CasasDaRegiao())
		{
			Vector2[] pontas = Pontas(PontoDaCasa(h), TamanhoDaCasa * 0.95f);
			bool destacada = _arrastando != null && sobOMouse.HasValue && sobOMouse.Value == h;
			DrawColoredPolygon(pontas, destacada ? new Color(0.45f, 0.75f, 1.0f, 0.35f) : new Color(1.0f, 1.0f, 1.0f, 0.06f));
			var contorno = new Vector2[7];
			Array.Copy(pontas, contorno, 6);
			contorno[6] = pontas[0];
			DrawPolyline(contorno, new Color(1.0f, 1.0f, 1.0f, 0.35f), 2.0f, true);
		}

		if (equipe == null)
		{
			return;
		}

		foreach (Equipe.Membro membro in equipe.Membros)
		{
			if (membro == _arrastando)
			{
				continue;
			}
			DesenharPersonagem(membro, PontoDaCasa(membro.Formacao), membro == equipe.Lider, fonte);
		}

		if (_arrastando != null)
		{
			DesenharPersonagem(_arrastando, _posicaoDoMouse, _arrastando == equipe.Lider, fonte);
		}
	}

	// Alteração de IA - Revisar
	// O que faz: desenha um personagem numa casa — um disco azul (como no combate), o retrato e o
	//            nome embaixo; o líder ganha uma estrela.
	private void DesenharPersonagem(Equipe.Membro membro, Vector2 ponto, bool lider, Font fonte)
	{
		Color disco = membro.Morto ? new Color(0.3f, 0.3f, 0.3f, 0.8f) : new Color(0.25f, 0.55f, 1.0f, 0.55f);
		DrawColoredPolygon(Pontas(ponto, TamanhoDaCasa * 0.82f), disco);

		Texture2D? retrato = Retratos.De(membro.Ficha);
		if (retrato != null)
		{
			float altura = TamanhoDaCasa * 1.25f;
			float largura = altura * retrato.GetWidth() / Mathf.Max(1.0f, retrato.GetHeight());
			var area = new Rect2(ponto + new Vector2(-largura * 0.5f, -altura * 0.72f), new Vector2(largura, altura));
			DrawTextureRect(retrato, area, false, membro.Morto ? new Color(0.4f, 0.4f, 0.4f) : Colors.White);
		}

		string nome = membro.Ficha.Nome.Split(' ')[0] + (lider ? " (líder)" : "");
		DrawString(fonte, ponto + new Vector2(-TamanhoDaCasa, TamanhoDaCasa * 0.62f), nome, HorizontalAlignment.Center,
			TamanhoDaCasa * 2.0f, 20, lider ? new Color(1.0f, 0.86f, 0.4f) : Colors.White);
	}

	private static Vector2[] Pontas(Vector2 centro, float raio)
	{
		var pontas = new Vector2[6];
		for (int i = 0; i < 6; i++)
		{
			float angulo = Mathf.DegToRad(60.0f * i);
			pontas[i] = centro + new Vector2(Mathf.Cos(angulo), Mathf.Sin(angulo)) * raio;
		}
		return pontas;
	}

	// Alteração de IA - Revisar
	// O que faz: o arrastar e soltar — pega o personagem da casa clicada e, ao soltar, põe na casa
	//            debaixo do mouse (trocando com quem estiver lá).
	public override void _GuiInput(InputEvent evento)
	{
		Equipe? equipe = Equipe.Instancia;
		if (equipe == null)
		{
			return;
		}

		if (evento is InputEventMouseMotion movimento)
		{
			_posicaoDoMouse = movimento.Position;
			if (_arrastando != null)
			{
				QueueRedraw();
			}
			return;
		}

		if (evento is not InputEventMouseButton botao || botao.ButtonIndex != MouseButton.Left)
		{
			return;
		}

		_posicaoDoMouse = botao.Position;
		Hex casa = CasaDoPonto(botao.Position);

		if (botao.Pressed)
		{
			_arrastando = equipe.Membros.FirstOrDefault(m => m.Formacao == casa && !m.Morto);
		}
		else if (_arrastando != null)
		{
			equipe.MoverNaFormacao(_arrastando, casa);
			_arrastando = null;
		}
		QueueRedraw();
		AcceptEvent();
	}
}

// Alteração de IA - Revisar
// O que faz: o retrato de cada personagem para os menus — a imagem provisória, recortada sem a
//            sobra transparente em volta.
// Por quê: as imagens provisórias têm margens transparentes de tamanhos diferentes; sem recortar,
//          uns apareceriam minúsculos e outros enormes. O recorte é feito uma vez e guardado.
public static class Retratos
{
	private static readonly Dictionary<FichaDeCombatente, Texture2D?> Guardados = new();

	public static Texture2D? De(FichaDeCombatente ficha)
	{
		if (Guardados.TryGetValue(ficha, out Texture2D? pronto))
		{
			return pronto;
		}

		Texture2D? imagem = ficha.Imagem;
		Texture2D? resultado = imagem;
		Image? pixels = imagem?.GetImage();
		if (imagem != null && pixels != null)
		{
			if (pixels.IsCompressed())
			{
				pixels.Decompress();
			}
			Rect2I usado = pixels.GetUsedRect();
			if (usado.Size.X > 0 && usado.Size.Y > 0)
			{
				resultado = new AtlasTexture { Atlas = imagem, Region = new Rect2(usado.Position, usado.Size) };
			}
		}
		Guardados[ficha] = resultado;
		return resultado;
	}
}

// Alteração de IA - Revisar
// O que faz: o visual comum dos painéis e textos das telas montadas em código (combate, equipe,
//            fim de jogo).
// Por quê: um lugar só para a cor e a borda dos painéis — quando a arte da interface chegar, é
//          trocar aqui.
public static class Estilos
{
	public static StyleBoxFlat Painel(float opacidade = 0.86f)
	{
		return new StyleBoxFlat
		{
			BgColor = new Color(0.07f, 0.07f, 0.09f, opacidade),
			BorderColor = new Color(0.85f, 0.72f, 0.35f, 0.55f),
			BorderWidthLeft = 2,
			BorderWidthRight = 2,
			BorderWidthTop = 2,
			BorderWidthBottom = 2,
			CornerRadiusTopLeft = 8,
			CornerRadiusTopRight = 8,
			CornerRadiusBottomLeft = 8,
			CornerRadiusBottomRight = 8,
			ContentMarginLeft = 18,
			ContentMarginRight = 18,
			ContentMarginTop = 14,
			ContentMarginBottom = 14,
		};
	}

	public static Label Texto(string texto, int tamanho, Color cor, float largura = 0.0f)
	{
		var rotulo = new Label { Text = texto, MouseFilter = Control.MouseFilterEnum.Ignore };
		rotulo.AddThemeFontSizeOverride("font_size", tamanho);
		rotulo.AddThemeColorOverride("font_color", cor);
		if (largura > 0.0f)
		{
			rotulo.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			rotulo.CustomMinimumSize = new Vector2(largura, 0.0f);
		}
		return rotulo;
	}
}

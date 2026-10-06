using Godot;
using System.Collections.Generic;
using System.Linq;

// Alteração de IA - Revisar
// O que faz: a interface do combate — a ordem da iniciativa no alto, o painel de quem está na vez,
//            os botões das 5 ações (Atacar, Habilidades, Defender, Itens, Fugir) e o de encerrar o
//            turno, a lista de habilidades, uma dica do que fazer, o registro do que aconteceu e o
//            quadro de quem está debaixo do mouse.
// Por quê: provisória e funcional — o visual definitivo vem com a arte da interface. Montada em
//          código, sem arquivo de cena, para não depender de algo que o editor do Godot possa regravar.
public partial class InterfaceDeCombate : CanvasLayer
{
	private const int LinhasDoRegistro = 11;

	private GerenciadorDeCombate _combate = null!;
	private HBoxContainer _ordem = null!;
	private Label _nomeDaVez = null!;
	private Label _numerosDaVez = null!;
	private Label _turnoDaVez = null!;
	private Label _efeitosDaVez = null!;
	private Button _atacar = null!;
	private Button _habilidades = null!;
	private Button _defender = null!;
	private Button _itens = null!;
	private Button _fugir = null!;
	private Button _encerrar = null!;
	private PanelContainer _painelDeHabilidades = null!;
	private VBoxContainer _listaDeHabilidades = null!;
	private Label _dica = null!;
	private VBoxContainer _registro = null!;
	private Label _aviso = null!;
	private PanelContainer _quadroSobOMouse = null!;
	private Label _textoSobOMouse = null!;
	private Combatente? _sobOMouse;
	private Tween? _animacaoDoAviso;

	public void Conectar(GerenciadorDeCombate combate)
	{
		_combate = combate;
		Layer = 10;
		Montar();
	}

	private void Montar()
	{
		var raiz = new Control { MouseFilter = Control.MouseFilterEnum.Ignore };
		raiz.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		AddChild(raiz);

		// a ordem da iniciativa, no alto e no meio
		var painelDaOrdem = Painel(raiz);
		Ancorar(painelDaOrdem, 0.5f, 0.0f, 0.5f, 0.0f, Control.GrowDirection.Both, Control.GrowDirection.End);
		painelDaOrdem.OffsetTop = 12;
		_ordem = new HBoxContainer();
		_ordem.AddThemeConstantOverride("separation", 10);
		painelDaOrdem.AddChild(_ordem);

		// o painel de quem está na vez, embaixo à esquerda
		var painelDaVez = Painel(raiz);
		Ancorar(painelDaVez, 0.0f, 1.0f, 0.0f, 1.0f, Control.GrowDirection.End, Control.GrowDirection.Begin);
		painelDaVez.OffsetLeft = 16;
		painelDaVez.OffsetBottom = -16;
		painelDaVez.CustomMinimumSize = new Vector2(470, 0);
		var vez = new VBoxContainer();
		painelDaVez.AddChild(vez);
		_nomeDaVez = Estilos.Texto("", 30, Colors.White);
		_numerosDaVez = Estilos.Texto("", 22, new Color(0.85f, 0.87f, 0.92f));
		_turnoDaVez = Estilos.Texto("", 22, new Color(0.85f, 0.87f, 0.92f));
		_efeitosDaVez = Estilos.Texto("", 20, new Color(1.0f, 0.85f, 0.5f), largura: 430);
		vez.AddChild(_nomeDaVez);
		vez.AddChild(_numerosDaVez);
		vez.AddChild(_turnoDaVez);
		vez.AddChild(_efeitosDaVez);

		// os botões, embaixo, à direita do painel da vez (sem cobrir o painel)
		var painelDosBotoes = Painel(raiz);
		Ancorar(painelDosBotoes, 0.0f, 1.0f, 0.0f, 1.0f, Control.GrowDirection.End, Control.GrowDirection.Begin);
		painelDosBotoes.OffsetLeft = 506;
		painelDosBotoes.OffsetBottom = -16;
		var botoes = new HBoxContainer();
		botoes.AddThemeConstantOverride("separation", 10);
		painelDosBotoes.AddChild(botoes);
		_atacar = Botao(botoes, "Atacar", () => { FecharHabilidades(); if (_combate.DaVez?.Ficha.AtaqueBasico is Habilidade b) { _combate.EscolherHabilidade(b); } });
		_habilidades = Botao(botoes, "Habilidades", AlternarHabilidades);
		_defender = Botao(botoes, "Defender", () => { FecharHabilidades(); _combate.Defender(); });
		_itens = Botao(botoes, "Itens", () => { FecharHabilidades(); _combate.UsarItem(); });
		_fugir = Botao(botoes, "Fugir", () => { FecharHabilidades(); _combate.Fugir(); });
		_encerrar = Botao(botoes, "Encerrar turno", () => { FecharHabilidades(); _combate.EncerrarTurno(); });
		_encerrar.CustomMinimumSize = new Vector2(230, 64);

		// a dica, logo acima dos botões
		_dica = Estilos.Texto("", 22, new Color(0.95f, 0.95f, 1.0f));
		_dica.HorizontalAlignment = HorizontalAlignment.Center;
		raiz.AddChild(_dica);
		Ancorar(_dica, 0.0f, 1.0f, 0.0f, 1.0f, Control.GrowDirection.End, Control.GrowDirection.Begin);
		_dica.OffsetLeft = 516;
		_dica.OffsetBottom = -112;

		// a lista de habilidades, acima da dica
		_painelDeHabilidades = Painel(raiz);
		Ancorar(_painelDeHabilidades, 0.0f, 1.0f, 0.0f, 1.0f, Control.GrowDirection.End, Control.GrowDirection.Begin);
		_painelDeHabilidades.OffsetLeft = 506;
		_painelDeHabilidades.OffsetBottom = -150;
		_listaDeHabilidades = new VBoxContainer();
		_painelDeHabilidades.AddChild(_listaDeHabilidades);
		_painelDeHabilidades.Visible = false;

		// o registro, à direita
		var painelDoRegistro = Painel(raiz);
		Ancorar(painelDoRegistro, 1.0f, 0.0f, 1.0f, 0.0f, Control.GrowDirection.Begin, Control.GrowDirection.End);
		painelDoRegistro.OffsetRight = -16;
		painelDoRegistro.OffsetTop = 120;
		painelDoRegistro.CustomMinimumSize = new Vector2(560, 0);
		_registro = new VBoxContainer();
		painelDoRegistro.AddChild(_registro);

		// quem está debaixo do mouse, à esquerda no alto
		_quadroSobOMouse = Painel(raiz);
		Ancorar(_quadroSobOMouse, 0.0f, 0.0f, 0.0f, 0.0f, Control.GrowDirection.End, Control.GrowDirection.End);
		_quadroSobOMouse.OffsetLeft = 16;
		_quadroSobOMouse.OffsetTop = 120;
		_textoSobOMouse = Estilos.Texto("", 21, Colors.White, largura: 400);
		_quadroSobOMouse.AddChild(_textoSobOMouse);
		_quadroSobOMouse.Visible = false;

		// o aviso grande ("Combate!", "Vitória!")
		_aviso = Estilos.Texto("", 72, new Color(1.0f, 0.86f, 0.4f));
		_aviso.HorizontalAlignment = HorizontalAlignment.Center;
		_aviso.AddThemeConstantOverride("outline_size", 12);
		_aviso.AddThemeColorOverride("font_outline_color", Colors.Black);
		raiz.AddChild(_aviso);
		Ancorar(_aviso, 0.5f, 0.2f, 0.5f, 0.2f, Control.GrowDirection.Both, Control.GrowDirection.Both);
	}

	private static PanelContainer Painel(Control pai)
	{
		var painel = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Stop };
		painel.AddThemeStyleboxOverride("panel", Estilos.Painel());
		pai.AddChild(painel);
		return painel;
	}

	private static void Ancorar(Control c, float esquerda, float topo, float direita, float base_,
								Control.GrowDirection crescerNaHorizontal, Control.GrowDirection crescerNaVertical)
	{
		c.AnchorLeft = esquerda;
		c.AnchorTop = topo;
		c.AnchorRight = direita;
		c.AnchorBottom = base_;
		c.GrowHorizontal = crescerNaHorizontal;
		c.GrowVertical = crescerNaVertical;
	}

	private static Button Botao(Container pai, string texto, System.Action aoApertar)
	{
		var botao = new Button { Text = texto, CustomMinimumSize = new Vector2(170, 64), FocusMode = Control.FocusModeEnum.None };
		botao.AddThemeFontSizeOverride("font_size", 24);
		botao.Pressed += aoApertar;
		pai.AddChild(botao);
		return botao;
	}

	// Alteração de IA - Revisar
	// O que faz: redesenha tudo a partir do estado do combate.
	public void AtualizarTudo()
	{
		if (_combate == null || _ordem == null)
		{
			return;
		}
		AtualizarOrdem();
		AtualizarVez();
		AtualizarBotoes();
		MostrarSobOMouse(_sobOMouse);
		if (_painelDeHabilidades.Visible)
		{
			MontarHabilidades();
		}
	}

	private void AtualizarOrdem()
	{
		foreach (Node filho in _ordem.GetChildren())
		{
			filho.QueueFree();
		}

		_ordem.AddChild(Estilos.Texto($"Rodada {_combate.Rodada}", 22, new Color(0.7f, 0.72f, 0.78f)));
		foreach (Combatente c in _combate.Ordem)
		{
			Color cor = !c.Vivo ? new Color(0.45f, 0.45f, 0.48f)
				: c.Lado == LadoDoCombate.Aliados ? new Color(0.45f, 0.72f, 1.0f) : new Color(1.0f, 0.45f, 0.42f);
			string turnos = c.TurnosPorRodada > 1 ? " x2" : "";
			string texto = c.Vivo ? $"{c.Nome} {c.ValorDeIniciativa}{turnos}" : $"{c.Nome} (morto)";

			var caixa = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
			var estilo = new StyleBoxFlat
			{
				BgColor = c == _combate.DaVez ? new Color(1.0f, 0.82f, 0.25f, 0.9f) : new Color(1, 1, 1, 0.06f),
				CornerRadiusTopLeft = 6,
				CornerRadiusTopRight = 6,
				CornerRadiusBottomLeft = 6,
				CornerRadiusBottomRight = 6,
				ContentMarginLeft = 10,
				ContentMarginRight = 10,
				ContentMarginTop = 4,
				ContentMarginBottom = 4,
			};
			caixa.AddThemeStyleboxOverride("panel", estilo);
			caixa.AddChild(Estilos.Texto(texto, 22, c == _combate.DaVez ? Colors.Black : cor));
			_ordem.AddChild(caixa);
		}
	}

	private void AtualizarVez()
	{
		Combatente? c = _combate.DaVez;
		if (c == null)
		{
			_nomeDaVez.Text = "";
			_numerosDaVez.Text = "";
			_turnoDaVez.Text = "";
			_efeitosDaVez.Text = "";
			return;
		}

		bool aliado = c.Lado == LadoDoCombate.Aliados;
		_nomeDaVez.Text = $"Vez de {c.Nome}" + (aliado ? $" — {c.Ficha.Classe}" : " (inimigo)");
		_nomeDaVez.AddThemeColorOverride("font_color", aliado ? new Color(0.5f, 0.78f, 1.0f) : new Color(1.0f, 0.5f, 0.45f));
		_numerosDaVez.Text = $"Vida {c.Vida}/{c.VidaMaxima}    Foco {c.Foco}/{c.Ficha.Foco}    Sanidade {c.Sanidade}/{c.Ficha.Sanidade}";
		_turnoDaVez.Text = $"Movimento {c.MovimentoRestante}/{c.Ficha.Movimento} casas    Ação: {(c.AcaoDisponivel ? "disponível" : "usada")}";
		string efeitos = c.TextoDosEfeitos();
		_efeitosDaVez.Text = efeitos.Length > 0 ? $"Efeitos: {efeitos}" : "";
	}

	private void AtualizarBotoes()
	{
		bool vez = _combate.VezDoJogadorAgora;
		Combatente? c = _combate.DaVez;
		_atacar.Disabled = !vez || c?.Ficha.AtaqueBasico == null || _combate.MotivoParaNaoUsar(c, c.Ficha.AtaqueBasico) != null;
		_habilidades.Disabled = !vez || c == null || c.Ficha.Habilidades.Count == 0;
		_defender.Disabled = !vez || c == null || !c.AcaoDisponivel;
		_itens.Disabled = !vez;
		_fugir.Disabled = !vez;
		_encerrar.Disabled = !vez;

		if (c?.Ficha.AtaqueBasico is Habilidade basico)
		{
			_atacar.TooltipText = $"{basico.Nome}: dano {basico.DanoMinimo}-{basico.DanoMaximo}, alcance {basico.Alcance} casa(s).\n" +
								  "Atalho: clique direto num inimigo marcado em vermelho.";
		}
		if (!vez)
		{
			FecharHabilidades();
			if (c != null && c.Lado == LadoDoCombate.Inimigos)
			{
				Dica($"Vez de {c.Nome}...");
			}
		}
		else if (_combate.Modo == GerenciadorDeCombate.ModoDaVez.Livre && string.IsNullOrEmpty(_dica.Text))
		{
			Dica("Clique numa casa azul para andar · clique num inimigo em vermelho para atacar · Enter encerra o turno");
		}
	}

	private void AlternarHabilidades()
	{
		_painelDeHabilidades.Visible = !_painelDeHabilidades.Visible;
		if (_painelDeHabilidades.Visible)
		{
			MontarHabilidades();
		}
	}

	private void FecharHabilidades()
	{
		_painelDeHabilidades.Visible = false;
	}

	// Alteração de IA - Revisar
	// O que faz: a lista das habilidades de quem está na vez — custo em Foco, alcance e, se não
	//            puder usar, o motivo.
	private void MontarHabilidades()
	{
		foreach (Node filho in _listaDeHabilidades.GetChildren())
		{
			filho.QueueFree();
		}

		Combatente? c = _combate.DaVez;
		if (c == null)
		{
			return;
		}

		foreach (Habilidade h in c.Ficha.Habilidades)
		{
			string? motivo = _combate.MotivoParaNaoUsar(c, h);
			string alcance = h.Alvo == Habilidade.TipoDeAlvo.ProprioPersonagem ? "em si" : $"alcance {h.Alcance}";
			var botao = new Button
			{
				Text = $"{h.Nome}   ·   {h.CustoDeFoco} Foco   ·   {alcance}" + (motivo != null ? $"   ({motivo})" : ""),
				Disabled = motivo != null,
				Alignment = HorizontalAlignment.Left,
				CustomMinimumSize = new Vector2(760, 50),
				TooltipText = h.Descricao,
				FocusMode = Control.FocusModeEnum.None,
			};
			botao.AddThemeFontSizeOverride("font_size", 21);
			Habilidade escolhida = h;
			botao.Pressed += () =>
			{
				FecharHabilidades();
				_combate.EscolherHabilidade(escolhida);
			};
			_listaDeHabilidades.AddChild(botao);
		}
	}

	// Alteração de IA - Revisar
	// O que faz: o quadro de quem está debaixo do mouse — vida, efeitos e, se for um inimigo, a
	//            chance de acerto do ataque básico de quem está na vez.
	public void MostrarSobOMouse(Combatente? c)
	{
		_sobOMouse = c;
		if (c == null || !c.Vivo)
		{
			_quadroSobOMouse.Visible = false;
			return;
		}

		var linhas = new List<string>
		{
			$"{c.Nome} — {c.Ficha.Classe}",
			$"Vida {c.Vida}/{c.VidaMaxima}   Foco {c.Foco}/{c.Ficha.Foco}",
			$"Precisão {c.Ficha.Precisao}  Reação {c.ReacaoEfetiva}  Robustez {c.RobustezEfetiva}  Iniciativa {c.Ficha.Iniciativa}",
		};
		string efeitos = c.TextoDosEfeitos();
		if (efeitos.Length > 0)
		{
			linhas.Add($"Efeitos: {efeitos}");
		}

		Combatente? vez = _combate.DaVez;
		if (vez != null && vez.Lado == LadoDoCombate.Aliados && c.Lado != vez.Lado)
		{
			linhas.Add($"Chance de acerto de {vez.Nome}: {ExecutorDeHabilidades.ChanceDeAcerto(vez, c):P0}");
		}

		_textoSobOMouse.Text = string.Join("\n", linhas);
		_quadroSobOMouse.Visible = true;
	}

	public void Dica(string texto)
	{
		if (_dica != null)
		{
			_dica.Text = texto;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: guarda as últimas linhas do registro na tela.
	public void Registrar(string texto)
	{
		if (_registro == null)
		{
			return;
		}
		_registro.AddChild(Estilos.Texto(texto, 19, new Color(0.88f, 0.9f, 0.95f), largura: 520));
		while (_registro.GetChildCount() > LinhasDoRegistro)
		{
			Node primeiro = _registro.GetChild(0);
			_registro.RemoveChild(primeiro);
			primeiro.QueueFree();
		}
	}

	public void LimparRegistro()
	{
		if (_registro == null)
		{
			return;
		}
		foreach (Node filho in _registro.GetChildren())
		{
			_registro.RemoveChild(filho);
			filho.QueueFree();
		}
	}

	// Alteração de IA - Revisar
	// O que faz: o aviso grande no meio da tela, que aparece e some.
	public void Aviso(string texto)
	{
		_aviso.Text = texto;
		_aviso.Modulate = Colors.White;
		_animacaoDoAviso?.Kill();
		_animacaoDoAviso = CreateTween();
		_animacaoDoAviso.TweenInterval(1.3f);
		_animacaoDoAviso.TweenProperty(_aviso, "modulate:a", 0.0f, 0.6f);
	}
}

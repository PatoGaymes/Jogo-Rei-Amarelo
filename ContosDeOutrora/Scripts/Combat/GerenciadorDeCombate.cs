using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Alteração de IA - Revisar
// O que faz: comanda o combate inteiro — começa a luta onde ela aconteceu, chama os inimigos que
//            ouviram o grito, posiciona a equipe pela formação, rola a iniciativa, passa a vez,
//            recebe os cliques do jogador, faz os inimigos jogarem e termina a luta (vitória ou fim
//            de jogo).
// Por quê: fica num nó global (autoload "Combate") para valer em qualquer mapa sem precisar ser
//          montado em cada fase: ele escuta sozinho o aviso CombateDeveComecar de todo inimigo que
//          aparece. A única peça que cada mapa precisa ter é a GradeDeCombate.
//
//          As regras seguem as decisões do PO de 06/10/2026 (ver docs/JOGO.md, "Combate"). Os
//          números são simbólicos: o balanceamento ainda está sendo feito.
public partial class GerenciadorDeCombate : Node
{
	public enum ResultadoDoCombate
	{
		Vitoria,
		Derrota,
	}

	// Alteração de IA - Revisar
	// O que faz: em que ponto da vez do jogador estamos — esperando a vez, livre para andar e
	//            atacar, escolhendo o alvo de uma habilidade, ou com uma ação acontecendo.
	public enum ModoDaVez
	{
		Esperando,
		Livre,
		EscolhendoAlvo,
		Executando,
	}

	// Alteração de IA - Revisar
	// O que faz: o tamanho da área do combate, em casas a partir do centro da luta.
	// Por quê: a grade cobre o mapa inteiro, mas só o pedaço em volta da luta aparece e vale.
	public const int RaioDaRegiao = 9;

	// Alteração de IA - Revisar
	// O que faz: até que distância (metros) o jogador pode clicar num inimigo para começar a luta.
	// Por quê: "tanto inimigos quanto o jogador podem começar o combate" (PO, 06/10/2026). A
	//          emboscada e o ataque surpresa vêm depois; por enquanto o clique só começa a luta.
	public const float AlcanceParaComecarPeloClique = 8.0f;

	public static GerenciadorDeCombate? Instancia { get; private set; }

	public bool EmCombate { get; private set; }
	public GradeDeCombate? Grade { get; private set; }
	public HashSet<Casa> Regiao { get; private set; } = new();
	public List<Combatente> Combatentes { get; } = new();
	public List<Combatente> Ordem { get; } = new();
	public Combatente? DaVez { get; private set; }
	public int Rodada { get; private set; }
	public ModoDaVez Modo { get; private set; } = ModoDaVez.Esperando;
	public Habilidade? HabilidadeEscolhida { get; private set; }
	public ResultadoDoCombate? Resultado { get; private set; }

	// Alteração de IA - Revisar
	// O que faz: o sorteio de todo o combate (dados de iniciativa, acerto, dano).
	// Por quê: um só, para os testes poderem fixar a "semente" e repetir uma luta igual.
	public RandomNumberGenerator Sorte { get; } = new();

	public ExecutorDeHabilidades Executor { get; private set; } = null!;
	public IaDeCombate Ia { get; private set; } = null!;

	private InterfaceDeCombate _interface = null!;
	private DesenhoDaGrade? _desenho;
	private CameraIsometrica? _camera;
	private PlayerIsometrico? _jogador;
	private readonly List<InimigoIA> _congelados = new();
	private readonly HashSet<Casa> _casasDeNeutros = new();
	private readonly List<string> _registro = new();
	private Dictionary<Casa, int> _alcancaveis = new();
	private List<Casa> _caminhoPrevisto = new();
	private HashSet<Casa> _alvosValidos = new();
	private HashSet<Casa> _areaPrevista = new();
	private Casa? _casaSobOMouse;
	private TaskCompletionSource<bool>? _fimDaVezDoJogador;
	private int _geracao;
	private bool _mostrandoGradeInteira;

	public override void _EnterTree()
	{
		Instancia = this;

		// Alteração de IA - Revisar
		// O que faz: escuta todo inimigo que entra em cena, em qualquer mapa.
		// Por quê: assim nenhuma fase precisa ligar o aviso do inimigo ao combate à mão — e nada se
		//          perde se o editor do Godot regravar a cena. Liga aqui na entrada, e não quando o nó
		//          fica pronto: no começo do jogo o mapa inteiro entra em cena antes de os nós globais
		//          ficarem prontos, e os inimigos dele passariam sem ser ouvidos (aconteceu no teste com
		//          janela de 06/10/2026).
		GetTree().NodeAdded += AoEntrarNo;
	}

	public override void _Ready()
	{
		Executor = new ExecutorDeHabilidades(this);
		Ia = new IaDeCombate(this);
		// cada partida com dados diferentes (os testes fixam a semente depois, se precisarem)
		Sorte.Randomize();
		_interface = new InterfaceDeCombate { Name = "InterfaceDeCombate" };
		AddChild(_interface);
		_interface.Conectar(this);
		_interface.Visible = false;

		// e os inimigos que já estavam em cena antes daqui (ver _EnterTree)
		foreach (Node no in GetTree().GetNodesInGroup("inimigo"))
		{
			AoEntrarNo(no);
		}
	}

	private readonly HashSet<ulong> _inimigosOuvidos = new();

	private void AoEntrarNo(Node no)
	{
		if (no is InimigoIA inimigo && _inimigosOuvidos.Add(inimigo.GetInstanceId()))
		{
			inimigo.CombateDeveComecar += AoInimigoPedirCombate;
		}
	}

	private void AoInimigoPedirCombate(InimigoIA inimigo)
	{
		if (!EmCombate)
		{
			ComecarComSeguranca(inimigo, null);
		}
	}

	// Alteração de IA - Revisar
	// O que faz: começa a luta e, se algo der errado no meio, mostra o erro e devolve o jogo à
	//            exploração.
	// Por quê: sem isso, um erro dentro do começo da luta sumia em silêncio e o jogo ficava parado.
	public async void ComecarComSeguranca(InimigoIA inimigo, Node3D? quemAtacouPrimeiro)
	{
		try
		{
			await IniciarCombate(inimigo, quemAtacouPrimeiro);
		}
		catch (Exception erro)
		{
			GD.PushError($"Combate: erro durante a luta — {erro}");
			if (EmCombate)
			{
				Encerrar();
			}
		}
	}

	// -------------------------------------------------------------------------
	// INÍCIO
	// -------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: começa a luta com um inimigo. "quemAtacouPrimeiro" é o personagem que deu o
	//            primeiro golpe numa conversa — só ele ganha Vantagem, e só no primeiro ataque.
	// Por quê: o passo a passo segue o combinado com o PO:
	//            1. o inimigo dá o grito de aviso: quem estiver no alcance dele, sem parede no meio,
	//               entra junto (e não grita de novo — sem efeito em cadeia);
	//            2. a exploração congela;
	//            3. cada um ganha uma casa; a equipe entra pela formação, virada para os inimigos;
	//            4. a câmera se aproxima (sem corte de tela) e os hexágonos aparecem;
	//            5. todos rolam a iniciativa (d20 + Iniciativa) e a luta começa.
	public async Task IniciarCombate(InimigoIA iniciador, Node3D? quemAtacouPrimeiro)
	{
		if (EmCombate || !IsInstanceValid(iniciador))
		{
			return;
		}

		Grade = GradeDeCombate.DaFase(GetTree());
		_jogador = GetTree().GetFirstNodeInGroup("player") as PlayerIsometrico;
		_camera = GetTree().GetFirstNodeInGroup("camera_isometrica") as CameraIsometrica;
		if (Grade == null || !Grade.Pronta || _jogador == null || Equipe.Instancia?.Lider == null)
		{
			GD.PushWarning("Combate: não deu para começar — falta a grade da fase, o jogador ou a equipe.");
			return;
		}

		EmCombate = true;
		Resultado = null;
		Rodada = 0;
		int geracao = ++_geracao;
		Combatentes.Clear();
		Ordem.Clear();
		_registro.Clear();
		_casasDeNeutros.Clear();
		_interface.LimparRegistro();

		// 1. o grito de aviso
		List<InimigoIA> inimigos = OuviramOGrito(iniciador);

		// 2. a exploração congela
		CongelarExploracao(inimigos);

		// 3. os lutadores e as casas
		MontarLutadores(inimigos, quemAtacouPrimeiro);
		if (!PosicionarTodos())
		{
			GD.PushWarning("Combate: não achei casas para todos. A luta foi cancelada.");
			Encerrar();
			return;
		}

		// 4. a câmera se aproxima e todos vão para as suas casas
		_interface.Visible = true;
		_interface.Aviso("Combate!");
		Registrar($"Combate! {inimigos.Count} inimigo(s): {string.Join(", ", inimigos.Select(i => i.Name))}.");
		_camera?.DefinirModo(CameraIsometrica.Modo.Transicao);
		DesenharGrade();
		await Task.WhenAll(Combatentes.Select(c => IrParaACasa(c)));
		if (geracao != _geracao)
		{
			return;
		}
		foreach (Combatente c in Combatentes.Where(c => c.Inimigo != null && IsInstanceValid(c.Inimigo)))
		{
			c.Inimigo!.GetNodeOrNull<BalaoAviso>("BalaoAviso")?.Esconder();
		}

		// 5. iniciativa e a luta
		RolarIniciativa();
		await RodarCombate(geracao);
	}

	// Alteração de IA - Revisar
	// O que faz: quem ouviu o grito — inimigos vivos, fora de combate, até o alcance do grito do
	//            inimigo que começou, e sem parede no meio.
	// Por quê: "funciona bem parecido com o range de detecção, não atravessa paredes" (PO). Só o
	//          primeiro inimigo grita: quem foi chamado não chama mais ninguém.
	public List<InimigoIA> OuviramOGrito(InimigoIA iniciador)
	{
		var inimigos = new List<InimigoIA> { iniciador };
		FichaDeCombatente ficha = FichaDoInimigo(iniciador);
		float alcance = ficha.AlcanceDoGrito;
		var espaco = iniciador.GetWorld3D().DirectSpaceState;
		Vector3 boca = iniciador.GlobalPosition + Vector3.Up * 1.5f;

		foreach (Node no in GetTree().GetNodesInGroup("inimigo"))
		{
			if (no is not InimigoIA outro || outro == iniciador || !IsInstanceValid(outro) || outro.EmCombate)
			{
				continue;
			}

			Vector3 d = outro.GlobalPosition - iniciador.GlobalPosition;
			if (new Vector2(d.X, d.Z).Length() > alcance)
			{
				continue;
			}

			var consulta = PhysicsRayQueryParameters3D.Create(boca, outro.GlobalPosition + Vector3.Up * 1.5f, 8);
			if (espaco.IntersectRay(consulta).Count > 0)
			{
				continue;
			}
			inimigos.Add(outro);
		}
		return inimigos;
	}

	public static FichaDeCombatente FichaDoInimigo(InimigoIA inimigo)
	{
		return inimigo.Ficha ?? GD.Load<FichaDeCombatente>("res://Resources/Inimigos/Rasgador.tres");
	}

	// Alteração de IA - Revisar
	// O que faz: para a exploração — o jogador deixa de responder às teclas de andar, os inimigos
	//            param de rondar (os que não estão na luta ficam parados onde estão) e os desenhos de
	//            teste da detecção somem.
	private void CongelarExploracao(List<InimigoIA> participantes)
	{
		_jogador!.ControladoPeloCombate = true;
		_congelados.Clear();
		foreach (Node no in GetTree().GetNodesInGroup("inimigo"))
		{
			if (no is InimigoIA inimigo && IsInstanceValid(inimigo))
			{
				inimigo.EmCombate = true;
				_congelados.Add(inimigo);
				MostrarDesenhosDeTeste(inimigo, false);
				if (participantes.Contains(inimigo))
				{
					inimigo.GetNodeOrNull<BalaoAviso>("BalaoAviso")?.MostrarAlerta();
				}
			}
		}
		MostrarDesenhosDeTeste(_jogador, false);
	}

	private static void MostrarDesenhosDeTeste(Node corpo, bool mostrar)
	{
		if (corpo.GetNodeOrNull<Node3D>("DebugDeteccao") is Node3D desenho)
		{
			desenho.Visible = mostrar;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: cria o lutador de cada um — o líder (o próprio jogador), os outros vivos da equipe
	//            (que aparecem só agora) e os inimigos.
	private void MontarLutadores(List<InimigoIA> inimigos, Node3D? quemAtacouPrimeiro)
	{
		Equipe equipe = Equipe.Instancia!;
		Equipe.Membro lider = equipe.Lider!;

		var doLider = new Combatente(lider.Ficha, _jogador!, LadoDoCombate.Aliados)
		{
			Membro = lider,
			Vida = lider.Vida,
			Sanidade = lider.Sanidade,
		};
		Combatentes.Add(doLider);

		foreach (Equipe.Membro membro in equipe.Vivos.Where(m => m != lider))
		{
			CorpoDeCombate corpo = CorpoDeCombate.Criar(membro.Ficha);
			GetTree().CurrentScene.AddChild(corpo);
			corpo.GlobalPosition = _jogador!.GlobalPosition;
			corpo.Visible = false;
			Combatentes.Add(new Combatente(membro.Ficha, corpo, LadoDoCombate.Aliados)
			{
				Membro = membro,
				Vida = membro.Vida,
				Sanidade = membro.Sanidade,
			});
		}

		foreach (InimigoIA inimigo in inimigos)
		{
			Combatentes.Add(new Combatente(FichaDoInimigo(inimigo), inimigo, LadoDoCombate.Inimigos) { Inimigo = inimigo });
		}

		foreach (Combatente c in Combatentes)
		{
			c.Marcador = MarcadorDeCombate.Criar(c, c.Ficha.AlturaDoDesenho + 0.25f);
			c.Corpo.AddChild(c.Marcador);
			c.Marcador.Atualizar(c);
		}

		if (quemAtacouPrimeiro != null && Combatentes.FirstOrDefault(c => c.Corpo == quemAtacouPrimeiro) is Combatente primeiro)
		{
			primeiro.Aplicar(TipoDeEfeito.Vantagem, 1, -1, primeiro, Sorte);
			primeiro.Efeito(TipoDeEfeito.Vantagem)!.SomeNoPrimeiroAtaque = true;
			Registrar($"{primeiro.Nome} atacou primeiro: Vantagem no primeiro ataque.");
		}
	}

	// Alteração de IA - Revisar
	// O que faz: dá uma casa a cada lutador. O líder e os inimigos ficam na casa livre mais perto de
	//            onde estão; os outros membros seguem a formação, girada para o lado dos inimigos —
	//            e, se a casa prevista tiver obstáculo ou alguém, ficam na livre mais próxima.
	private bool PosicionarTodos()
	{
		GradeDeCombate grade = Grade!;
		var ocupadas = new HashSet<Casa>();
		Combatente lider = Combatentes[0];

		// os inimigos parados fora da luta também ocupam a casa onde estão
		foreach (InimigoIA neutro in _congelados.Where(i => Combatentes.All(c => c.Corpo != i)))
		{
			Casa? casa = grade.CasaEm(neutro.GlobalPosition);
			if (casa != null)
			{
				_casasDeNeutros.Add(casa);
				ocupadas.Add(casa);
			}
		}

		foreach (Combatente c in Combatentes.Where(c => c == lider || c.Lado == LadoDoCombate.Inimigos))
		{
			c.Casa = grade.CasaMaisProxima(c.Corpo.GlobalPosition, casa => !ocupadas.Contains(casa));
			if (c.Casa == null)
			{
				return false;
			}
			ocupadas.Add(c.Casa);
		}

		// a área do combate: em volta do centro dos lutadores, ligada às casas deles
		Vector3 centro = Combatentes.Where(c => c.Casa != null).Aggregate(Vector3.Zero, (soma, c) => soma + c.Casa!.Centro)
			/ Mathf.Max(1, Combatentes.Count(c => c.Casa != null));
		Regiao = grade.Regiao(Combatentes.Where(c => c.Casa != null).Select(c => c.Casa!), grade.HexDoPonto(centro), RaioDaRegiao);

		// a formação, virada para o centro dos inimigos
		Vector3 inimigosNoCentro = Combatentes.Where(c => c.Lado == LadoDoCombate.Inimigos)
			.Aggregate(Vector3.Zero, (soma, c) => soma + c.Casa!.Centro) / Mathf.Max(1, Combatentes.Count(c => c.Lado == LadoDoCombate.Inimigos));
		Vector3 paraOsInimigos = inimigosNoCentro - lider.Casa!.Centro;
		int giro = Hex.DirecaoMaisParecida(new Vector2(paraOsInimigos.X, paraOsInimigos.Z), grade.RaioDaCasa);
		Hex casaDoLiderNaFormacao = lider.Membro!.Formacao;

		foreach (Combatente membro in Combatentes.Where(c => c.Lado == LadoDoCombate.Aliados && c != lider))
		{
			Hex deslocamento = (membro.Membro!.Formacao - casaDoLiderNaFormacao).Girar(giro);
			Hex desejado = lider.Casa.Coordenada + deslocamento;
			Vector2 plano = Hex.CentroNoPlano(desejado, grade.RaioDaCasa);
			Vector3 ponto = new(grade.GlobalPosition.X + plano.X, lider.Casa.Centro.Y, grade.GlobalPosition.Z + plano.Y);

			membro.Casa = grade.CasaMaisProxima(ponto, casa => Regiao.Contains(casa) && !ocupadas.Contains(casa));
			if (membro.Casa == null)
			{
				return false;
			}
			ocupadas.Add(membro.Casa);
			membro.Corpo.GlobalPosition = membro.Casa.Centro;
		}
		return true;
	}

	// Alteração de IA - Revisar
	// O que faz: leva cada lutador ao centro da sua casa. Os membros da equipe aparecem já na casa,
	//            surgindo aos poucos.
	private async Task IrParaACasa(Combatente c)
	{
		if (c.Casa == null)
		{
			return;
		}

		if (c.Corpo is CorpoDeCombate membro)
		{
			membro.Visible = true;
			AnimacaoDeAtaque.Virar(c, Combatentes.Where(o => o.Lado != c.Lado).Select(o => o.Corpo.GlobalPosition).FirstOrDefault());
			if (membro.GetNodeOrNull<SpriteBase3D>("Desenho") is SpriteBase3D desenho)
			{
				desenho.Modulate = new Color(1, 1, 1, 0);
				Tween surgir = CreateTween();
				surgir.TweenProperty(desenho, "modulate:a", 1.0f, 0.45f);
				await ToSignal(surgir, Tween.SignalName.Finished);
			}
			return;
		}

		await AnimacaoDeAtaque.AndarPeloCaminho(this, c, new[] { c.Casa });
		Combatente? inimigoMaisPerto = Combatentes.Where(o => o.Lado != c.Lado).OrderBy(o => o.Corpo.GlobalPosition.DistanceTo(c.Corpo.GlobalPosition)).FirstOrDefault();
		if (inimigoMaisPerto != null)
		{
			AnimacaoDeAtaque.Virar(c, inimigoMaisPerto.Corpo.GlobalPosition);
		}
	}

	// Alteração de IA - Revisar
	// O que faz: a iniciativa — cada lutador rola um dado de 20 lados e soma o atributo Iniciativa;
	//            a ordem vai do maior para o menor, misturando aliados e inimigos. Quem tira 20 no dado
	//            joga 2 turnos seguidos sempre que chega a vez dele.
	// Por quê: decisão do PO em 06/10/2026. O que acontece com quem tira 1 ainda não foi definido
	//          (a frase da decisão ficou incompleta) — por enquanto, nada.
	//          Empate: ganha quem tem mais Iniciativa; persistindo, sorteio.
	public void RolarIniciativa()
	{
		var desempate = new Dictionary<Combatente, float>();
		foreach (Combatente c in Combatentes)
		{
			c.DadoDeIniciativa = Sorte.RandiRange(1, 20);
			c.ValorDeIniciativa = c.DadoDeIniciativa + c.Ficha.Iniciativa;
			c.TurnosPorRodada = c.DadoDeIniciativa == 20 ? 2 : 1;
			desempate[c] = Sorte.Randf();
		}

		Ordem.Clear();
		Ordem.AddRange(Combatentes
			.OrderByDescending(c => c.ValorDeIniciativa)
			.ThenByDescending(c => c.Ficha.Iniciativa)
			.ThenBy(c => desempate[c]));

		Registrar("Iniciativa (d20 + Iniciativa):");
		foreach (Combatente c in Ordem)
		{
			string extra = c.DadoDeIniciativa == 20 ? " — tirou 20: 2 turnos seguidos" : c.DadoDeIniciativa == 1 ? " — tirou 1" : "";
			Registrar($"  {c.Nome}: {c.ValorDeIniciativa} (dado {c.DadoDeIniciativa} + {c.Ficha.Iniciativa}){extra}");
		}
		_interface.AtualizarTudo();
	}

	// -------------------------------------------------------------------------
	// RODADAS E TURNOS
	// -------------------------------------------------------------------------

	private async Task RodarCombate(int geracao)
	{
		while (geracao == _geracao && Resultado == null)
		{
			Rodada++;
			Registrar($"— Rodada {Rodada} —");
			foreach (Combatente c in Ordem.ToList())
			{
				for (int turno = 0; turno < c.TurnosPorRodada; turno++)
				{
					if (geracao != _geracao || Resultado != null || !c.Vivo)
					{
						break;
					}
					if (turno > 0)
					{
						Registrar($"{c.Nome} joga de novo (tirou 20 na iniciativa).");
					}
					await JogarTurno(c, geracao);
				}
				if (geracao != _geracao || Resultado != null)
				{
					break;
				}
			}
		}

		if (geracao == _geracao && Resultado != null)
		{
			await Terminar(Resultado.Value);
		}
	}

	private async Task JogarTurno(Combatente c, int geracao)
	{
		DaVez = c;
		if (_camera != null)
		{
			_camera.Alvo = c.Corpo;
		}
		_interface.AtualizarTudo();

		bool podeAgir = ComecarTurno(c);
		AtualizarMarcadores();
		if (VerificarFim() || !c.Vivo)
		{
			return;
		}

		if (!podeAgir)
		{
			Registrar($"{c.Nome} está atordoado e perde a vez.");
			c.Marcador?.Flutuar("atordoado", new Color(0.8f, 0.8f, 1.0f));
			await Esperar(0.9f);
			return;
		}

		if (c.Lado == LadoDoCombate.Aliados)
		{
			await VezDoJogador(c);
		}
		else
		{
			Modo = ModoDaVez.Esperando;
			_interface.Dica($"Vez de {c.Nome}...");
			DesenharGrade();
			await Ia.JogarTurno(c);
			c.AlvoPrevisto = null;
			c.Marcador?.MostrarIntencao(null);
		}

		if (geracao == _geracao)
		{
			VerificarFim();
			AtualizarMarcadores();
		}
	}

	// Alteração de IA - Revisar
	// O que faz: o começo do turno de um lutador — os efeitos que tiram vida agem (veneno, fogo,
	//            sangramento), os prazos dos efeitos andam, e o turno é montado: 3 casas de movimento
	//            (0 se Enraizado) e a ação. Devolve false se ele perde a vez (Atordoado).
	public bool ComecarTurno(Combatente c)
	{
		bool atordoado = c.Tem(TipoDeEfeito.Atordoado);
		bool enraizado = c.Tem(TipoDeEfeito.Enraizado);

		DanoContinuo(c, TipoDeEfeito.Veneno, acumulos => acumulos * RegrasDeCombate.DanoDoVenenoPorAcumulo);
		DanoContinuo(c, TipoDeEfeito.Sangramento, acumulos =>
			Mathf.RoundToInt(acumulos * RegrasDeCombate.DanoDoSangramentoPorAcumulo * (1.0f + 2.0f * (1.0f - (float)c.Vida / c.VidaMaxima))));
		int fogo = c.Acumulos(TipoDeEfeito.Fogo);
		if (fogo >= RegrasDeCombate.AcumulosDoFogoParaEspalhar)
		{
			EspalharFogo(c);
		}
		DanoContinuo(c, TipoDeEfeito.Fogo, acumulos => acumulos * RegrasDeCombate.DanoDoFogoPorAcumulo);

		if (!c.Vivo)
		{
			return false;
		}

		// os prazos andam
		foreach (EfeitoAtivo efeito in c.Efeitos.ToList())
		{
			if (efeito.RodadasRestantes < 0)
			{
				continue;
			}
			efeito.RodadasRestantes--;
			if (efeito.RodadasRestantes > 0)
			{
				continue;
			}

			c.Efeitos.Remove(efeito);
			if (efeito.Tipo == TipoDeEfeito.Gelo)
			{
				Registrar($"O Gelo de {c.Nome} se parte.");
				Executor.CausarDano(c, RegrasDeCombate.DanoDoGeloAoSair, true, null, false);
			}
		}

		LimparVanguardasSemProtegido();

		c.MovimentoRestante = atordoado || enraizado ? 0 : c.Ficha.Movimento;
		c.AcaoDisponivel = !atordoado;
		if (enraizado && !atordoado)
		{
			Registrar($"{c.Nome} está enraizado: não anda neste turno.");
		}
		return !atordoado && c.Vivo;
	}

	private void DanoContinuo(Combatente c, TipoDeEfeito tipo, Func<int, int> quanto)
	{
		EfeitoAtivo? efeito = c.Efeito(tipo);
		if (efeito == null || !c.Vivo)
		{
			return;
		}

		int dano = Mathf.Max(1, quanto(efeito.Acumulos));
		int tirado = c.PerderVida(dano);
		Registrar($"{RegrasDeCombate.NomeDe(tipo)} tira {tirado} de {c.Nome} ({c.Vida}/{c.VidaMaxima}).");
		c.Marcador?.Flutuar($"-{tirado}", new Color(0.7f, 1.0f, 0.4f));

		efeito.Acumulos--;
		if (efeito.Acumulos <= 0)
		{
			c.Remover(tipo);
		}
		ConferirMorte(c);
	}

	// Alteração de IA - Revisar
	// O que faz: "Fogo espalha para alvos adjacentes com 5+ acúmulos" (Árvore) — 1 acúmulo em cada
	//            vizinho do mesmo lado.
	private void EspalharFogo(Combatente c)
	{
		if (c.Casa == null)
		{
			return;
		}
		foreach (Combatente vizinho in Combatentes.Where(o => o != c && o.Vivo && o.Lado == c.Lado && o.Casa != null
				 && GradeDeCombate.Distancia(o.Casa, c.Casa) == 1))
		{
			Executor.Grudar(vizinho, TipoDeEfeito.Fogo, 1, -1, null);
		}
	}

	private void LimparVanguardasSemProtegido()
	{
		foreach (Combatente c in Combatentes.Where(c => c.Tem(TipoDeEfeito.Vanguarda)))
		{
			bool protegeAlguem = Combatentes.Any(o => o.Vivo && o.Efeito(TipoDeEfeito.Protegido)?.Origem == c);
			if (!protegeAlguem)
			{
				c.Remover(TipoDeEfeito.Vanguarda);
			}
		}
	}

	// Alteração de IA - Revisar
	// O que faz: a vez de um personagem do jogador — espera até ele encerrar o turno (botão, Enter,
	//            ou sozinho quando não sobra movimento nem ação).
	private async Task VezDoJogador(Combatente c)
	{
		_fimDaVezDoJogador = new TaskCompletionSource<bool>();
		Modo = ModoDaVez.Livre;
		HabilidadeEscolhida = null;
		_interface.Dica("");
		AtualizarDestaques();
		_interface.AtualizarTudo();
		await _fimDaVezDoJogador.Task;
		Modo = ModoDaVez.Esperando;
		HabilidadeEscolhida = null;
		_alcancaveis.Clear();
		_caminhoPrevisto.Clear();
		_alvosValidos.Clear();
		_areaPrevista.Clear();
		DesenharGrade();
	}

	// -------------------------------------------------------------------------
	// AÇÕES DO JOGADOR (chamadas pela interface e pelo mouse)
	// -------------------------------------------------------------------------

	public bool VezDoJogadorAgora => EmCombate && DaVez != null && DaVez.Lado == LadoDoCombate.Aliados
									&& (Modo == ModoDaVez.Livre || Modo == ModoDaVez.EscolhendoAlvo);

	// Alteração de IA - Revisar
	// O que faz: escolhe uma habilidade (ou o ataque básico) para usar — as casas dos alvos válidos
	//            acendem. Habilidade em si mesmo é usada na hora.
	public async void EscolherHabilidade(Habilidade habilidade)
	{
		if (!VezDoJogadorAgora || DaVez == null)
		{
			return;
		}

		string? motivo = MotivoParaNaoUsar(DaVez, habilidade);
		if (motivo != null)
		{
			_interface.Dica(motivo);
			return;
		}

		if (habilidade.Alvo == Habilidade.TipoDeAlvo.ProprioPersonagem)
		{
			await UsarHabilidadeDaVez(habilidade, DaVez.Casa!);
			return;
		}

		HabilidadeEscolhida = habilidade;
		Modo = ModoDaVez.EscolhendoAlvo;
		AtualizarDestaques();
		_interface.Dica($"{habilidade.Nome}: clique num alvo marcado. Botão direito ou X cancela.");
		_interface.AtualizarTudo();
	}

	// Alteração de IA - Revisar
	// O que faz: explica por que a habilidade não pode ser usada agora, ou null se pode.
	public string? MotivoParaNaoUsar(Combatente c, Habilidade h)
	{
		if (!c.AcaoDisponivel)
		{
			return "A ação deste turno já foi usada.";
		}
		if (c.Foco < h.CustoDeFoco)
		{
			return $"Foco insuficiente ({c.Foco} de {h.CustoDeFoco}).";
		}
		if (c.HabilidadeBloqueada(h))
		{
			return c.Tem(TipoDeEfeito.Desarmado) && h.EhOfensiva ? "Desarmado: não pode atacar." : "Bloqueada pelo Raio.";
		}
		if (h.Alvo != Habilidade.TipoDeAlvo.ProprioPersonagem && AlvosValidos(c, h).Count == 0)
		{
			return "Nenhum alvo ao alcance.";
		}
		return null;
	}

	public void Cancelar()
	{
		if (Modo == ModoDaVez.EscolhendoAlvo)
		{
			Modo = ModoDaVez.Livre;
			HabilidadeEscolhida = null;
			AtualizarDestaques();
			_interface.Dica("");
			_interface.AtualizarTudo();
		}
	}

	// Alteração de IA - Revisar
	// O que faz: a ação Defender — mais esquiva e mais resistência até o começo do próximo turno.
	// Por quê: o GDD diz que Reação e Robustez "são afetadas pela ação de defender". Valores simbólicos.
	public void Defender()
	{
		if (!VezDoJogadorAgora || DaVez == null || !DaVez.AcaoDisponivel)
		{
			return;
		}
		DaVez.AcaoDisponivel = false;
		Executor.Grudar(DaVez, TipoDeEfeito.Defendendo, 1, 1, DaVez);
		Registrar($"{DaVez.Nome} se defende.");
		DepoisDeUmaAcao();
	}

	// Alteração de IA - Revisar
	// O que faz: Itens e Fugir ainda não funcionam — só avisam.
	// Por quê: o inventário não existe ainda, e a fuga está em desenvolvimento (PO, 06/10/2026:
	//          "apenas deixe a opção lá sem fazer nada").
	public void UsarItem() => _interface.Dica("Itens: o inventário ainda não existe.");

	public void Fugir() => _interface.Dica("Fugir: em desenvolvimento — por enquanto não faz nada.");

	public void EncerrarTurno()
	{
		if (DaVez != null && DaVez.Lado == LadoDoCombate.Aliados && Modo != ModoDaVez.Executando)
		{
			_fimDaVezDoJogador?.TrySetResult(true);
		}
	}

	public async Task<bool> MoverDaVezPara(Casa destino)
	{
		if (!VezDoJogadorAgora || DaVez == null || !_alcancaveis.ContainsKey(destino) || !PodeParar(DaVez, destino))
		{
			return false;
		}

		Modo = ModoDaVez.Executando;
		_interface.AtualizarTudo();
		await Mover(DaVez, destino);
		DepoisDeUmaAcao();
		return true;
	}

	public async Task<bool> UsarHabilidadeDaVez(Habilidade habilidade, Casa alvo)
	{
		if (!VezDoJogadorAgora || DaVez == null || MotivoParaNaoUsar(DaVez, habilidade) != null)
		{
			return false;
		}
		if (habilidade.Alvo != Habilidade.TipoDeAlvo.ProprioPersonagem && !AlvosValidos(DaVez, habilidade).Contains(alvo))
		{
			return false;
		}

		Modo = ModoDaVez.Executando;
		HabilidadeEscolhida = null;
		_alcancaveis.Clear();
		_alvosValidos.Clear();
		_areaPrevista.Clear();
		DesenharGrade();
		_interface.Dica("");
		_interface.AtualizarTudo();
		await Executor.Usar(DaVez, habilidade, alvo);
		DepoisDeUmaAcao();
		return true;
	}

	// Alteração de IA - Revisar
	// O que faz: depois de andar ou agir, volta a esperar o jogador — ou encerra o turno sozinho
	//            quando não sobra nada para fazer, ou a luta quando ela acabou.
	private async void DepoisDeUmaAcao()
	{
		if (!EmCombate)
		{
			return;
		}

		AtualizarMarcadores();
		if (VerificarFim())
		{
			_fimDaVezDoJogador?.TrySetResult(true);
			return;
		}

		Combatente? c = DaVez;
		if (c == null || !c.Vivo)
		{
			_fimDaVezDoJogador?.TrySetResult(true);
			return;
		}

		Modo = ModoDaVez.Livre;
		AtualizarDestaques();
		_interface.AtualizarTudo();

		if (c.MovimentoRestante <= 0 && !c.AcaoDisponivel)
		{
			await Esperar(0.35f);
			if (DaVez == c)
			{
				_fimDaVezDoJogador?.TrySetResult(true);
			}
		}
	}

	// -------------------------------------------------------------------------
	// MOVIMENTO
	// -------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: por onde um lutador pode passar e onde pode parar.
	// Por quê: dá para passar pela casa de um aliado, mas não parar nela; casa de inimigo (e de
	//          inimigo parado fora da luta) bloqueia.
	public bool PodePassar(Combatente quem, Casa casa)
	{
		if (_casasDeNeutros.Contains(casa))
		{
			return false;
		}
		Combatente? ocupante = CombatenteNaCasa(casa);
		return ocupante == null || ocupante == quem || ocupante.Lado == quem.Lado;
	}

	public bool PodeParar(Combatente quem, Casa casa)
	{
		if (_casasDeNeutros.Contains(casa) || !Regiao.Contains(casa))
		{
			return false;
		}
		Combatente? ocupante = CombatenteNaCasa(casa);
		return ocupante == null || ocupante == quem;
	}

	public Combatente? CombatenteNaCasa(Casa casa) => Combatentes.FirstOrDefault(c => c.Vivo && c.Casa == casa);

	public Dictionary<Casa, int> CasasAlcancaveis(Combatente quem)
	{
		if (Grade == null || quem.Casa == null)
		{
			return new Dictionary<Casa, int>();
		}
		return Grade.Alcancaveis(quem.Casa, quem.MovimentoRestante, casa => PodePassar(quem, casa), Regiao);
	}

	// Alteração de IA - Revisar
	// O que faz: anda até uma casa, gastando o movimento do turno (1 por casa).
	public async Task Mover(Combatente quem, Casa destino)
	{
		if (Grade == null || quem.Casa == null)
		{
			return;
		}

		List<Casa>? caminho = Grade.Caminho(quem.Casa, destino, casa => PodePassar(quem, casa), Regiao);
		if (caminho == null || caminho.Count == 0)
		{
			return;
		}
		if (caminho.Count > quem.MovimentoRestante)
		{
			caminho = caminho.Take(quem.MovimentoRestante).ToList();
		}
		// não termina em cima de alguém: recua até a última casa livre do caminho
		while (caminho.Count > 0 && !PodeParar(quem, caminho[^1]))
		{
			caminho.RemoveAt(caminho.Count - 1);
		}
		if (caminho.Count == 0)
		{
			return;
		}

		quem.MovimentoRestante -= caminho.Count;
		quem.Casa = caminho[^1];
		await AnimacaoDeAtaque.AndarPeloCaminho(this, quem, caminho);
		Registrar($"{quem.Nome} anda {caminho.Count} casa(s).");
	}

	// Alteração de IA - Revisar
	// O que faz: quantos passos levam de uma casa até a casa de um alvo (passando por onde dá).
	public int PassosAte(Combatente quem, Casa alvo) => quem.Casa == null ? int.MaxValue : PassosEntre(quem.Casa, alvo, quem);

	public int PassosEntre(Casa de, Casa para, Combatente quem)
	{
		if (Grade == null)
		{
			return int.MaxValue;
		}
		List<Casa>? caminho = Grade.Caminho(de, para, casa => PodePassar(quem, casa), Regiao);
		return caminho?.Count ?? 1000 + GradeDeCombate.Distancia(de, para);
	}

	// Alteração de IA - Revisar
	// O que faz: avança até uma casa colada no alvo (Avanço tático) — sem gastar o movimento do turno.
	public async Task AvancarAte(Combatente quem, Combatente alvo)
	{
		Casa? encaixe = CasaColadaNoAlvo(quem, alvo, int.MaxValue);
		if (encaixe == null || encaixe == quem.Casa || Grade == null || quem.Casa == null)
		{
			return;
		}
		List<Casa>? caminho = Grade.Caminho(quem.Casa, encaixe, casa => PodePassar(quem, casa), Regiao);
		if (caminho == null)
		{
			return;
		}
		quem.Casa = encaixe;
		await AnimacaoDeAtaque.AndarPeloCaminho(this, quem, caminho);
		Registrar($"{quem.Nome} avança até {alvo.Nome}.");
	}

	// Alteração de IA - Revisar
	// O que faz: a casa livre, vizinha ao alvo, mais perto de quem quer chegar nele (até um limite de
	//            passos). Se ele já está colado, é a própria casa dele.
	public Casa? CasaColadaNoAlvo(Combatente quem, Combatente alvo, int limiteDePassos)
	{
		if (quem.Casa == null || alvo.Casa == null || Grade == null)
		{
			return null;
		}
		if (GradeDeCombate.Distancia(quem.Casa, alvo.Casa) == 1 && quem.Casa.Vizinhas.Contains(alvo.Casa))
		{
			return quem.Casa;
		}

		Casa? melhor = null;
		int menor = int.MaxValue;
		foreach (Casa vizinha in alvo.Casa.Vizinhas)
		{
			if (!PodeParar(quem, vizinha))
			{
				continue;
			}
			List<Casa>? caminho = Grade.Caminho(quem.Casa, vizinha, casa => PodePassar(quem, casa), Regiao);
			if (caminho != null && caminho.Count <= limiteDePassos && caminho.Count < menor)
			{
				menor = caminho.Count;
				melhor = vizinha;
			}
		}
		return melhor;
	}

	// Alteração de IA - Revisar
	// O que faz: recua até N casas, para longe do inimigo mais perto (Técnica secreta, "avança para trás").
	public async Task Recuar(Combatente quem, int casas)
	{
		if (Grade == null || quem.Casa == null)
		{
			return;
		}

		var inimigos = Combatentes.Where(o => o.Vivo && o.Lado != quem.Lado && o.Casa != null).ToList();
		if (inimigos.Count == 0)
		{
			return;
		}

		Dictionary<Casa, int> alcance = Grade.Alcancaveis(quem.Casa, casas, casa => PodePassar(quem, casa), Regiao);
		Casa? melhor = null;
		int maiorDistancia = inimigos.Min(o => GradeDeCombate.Distancia(o.Casa!, quem.Casa));
		foreach (var par in alcance.Where(p => PodeParar(quem, p.Key)))
		{
			int distancia = inimigos.Min(o => GradeDeCombate.Distancia(o.Casa!, par.Key));
			if (distancia > maiorDistancia)
			{
				maiorDistancia = distancia;
				melhor = par.Key;
			}
		}
		if (melhor == null)
		{
			Registrar($"  {quem.Nome} não tem para onde recuar.");
			return;
		}

		List<Casa>? caminho = Grade.Caminho(quem.Casa, melhor, casa => PodePassar(quem, casa), Regiao);
		quem.Casa = melhor;
		if (caminho != null)
		{
			await AnimacaoDeAtaque.AndarPeloCaminho(this, quem, caminho);
		}
		Registrar($"  {quem.Nome} recua.");
	}

	// Alteração de IA - Revisar
	// O que faz: empurra o alvo N casas para longe de quem empurrou. Se não houver casa livre atrás
	//            dele (parede, obstáculo, outro personagem, beirada), ele não sai do lugar.
	public void Empurrar(Combatente quem, Combatente alvo, int casas)
	{
		if (quem.Casa == null || alvo.Casa == null)
		{
			return;
		}

		Vector3 direcao = alvo.Casa.Centro - quem.Casa.Centro;
		direcao.Y = 0.0f;
		direcao = direcao.Normalized();
		int andou = 0;

		for (int i = 0; i < casas; i++)
		{
			Casa atual = alvo.Casa!;
			int distanciaAtual = GradeDeCombate.Distancia(quem.Casa, atual);
			Casa? proxima = atual.Vizinhas
				.Where(v => PodeParar(alvo, v) && GradeDeCombate.Distancia(quem.Casa, v) > distanciaAtual)
				.OrderByDescending(v => (v.Centro - atual.Centro).Normalized().Dot(direcao))
				.FirstOrDefault();
			if (proxima == null)
			{
				break;
			}
			alvo.Casa = proxima;
			andou++;
		}

		if (andou == 0)
		{
			Registrar($"  {alvo.Nome} não sai do lugar.");
			return;
		}

		Registrar($"  {alvo.Nome} é empurrado {andou} casa(s).");
		Tween deslizar = CreateTween();
		deslizar.TweenProperty(alvo.Corpo, "global_position", alvo.Casa!.Centro, 0.18f * andou);
	}

	// Alteração de IA - Revisar
	// O que faz: Alma imaculada — cada inimigo decide agora quem vai atacar e isso aparece sobre a
	//            cabeça dele até a vez dele passar.
	public void RevelarIntencoes()
	{
		foreach (Combatente inimigo in Combatentes.Where(c => c.Vivo && c.Lado == LadoDoCombate.Inimigos))
		{
			inimigo.AlvoPrevisto = Ia.EscolherAlvo(inimigo);
			string texto = inimigo.AlvoPrevisto != null ? $"vai atacar: {inimigo.AlvoPrevisto.Nome}" : "sem alvo";
			inimigo.Marcador?.MostrarIntencao(texto);
			Registrar($"  {inimigo.Nome} {texto}.");
		}
	}

	// -------------------------------------------------------------------------
	// ALCANCE E ALVOS
	// -------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: responde se, estando numa casa, a habilidade alcança a casa do alvo.
	// Por quê: o alcance é contado em casas. Corpo a corpo não alcança outro andar (degrau de mais de
	//          1,2 m); nada atravessa parede.
	public bool PodeAtingir(Combatente quem, Habilidade h, Casa de, Casa alvo)
	{
		if (h.Alvo == Habilidade.TipoDeAlvo.ProprioPersonagem)
		{
			return true;
		}
		if (Grade == null || GradeDeCombate.Distancia(de, alvo) > h.Alcance)
		{
			return false;
		}
		if (h.Forma == Habilidade.FormaDoAtaque.CorpoACorpo && Mathf.Abs(de.Centro.Y - alvo.Centro.Y) > 1.2f)
		{
			return false;
		}
		return !h.PrecisaDeLinhaDeVisao || de == alvo || Grade.TemLinhaDeVisao(de, alvo);
	}

	// Alteração de IA - Revisar
	// O que faz: as casas onde estão os alvos que a habilidade pode atingir agora.
	// Por quê: Furtivo "não pode ser alvo de ataques por meios padrões". Habilidades que avançam até
	//          o alvo (Avanço tático) precisam de uma casa livre colada nele dentro do alcance.
	public List<Casa> AlvosValidos(Combatente quem, Habilidade h)
	{
		var casas = new List<Casa>();
		if (quem.Casa == null)
		{
			return casas;
		}
		if (h.Alvo == Habilidade.TipoDeAlvo.ProprioPersonagem)
		{
			casas.Add(quem.Casa);
			return casas;
		}

		bool avanca = h.Efeitos.Any(e => e.Tipo == EfeitoDeHabilidade.Acao.AvancarAteOAlvo);
		bool protege = h.Efeitos.Any(e => e.Efeito == TipoDeEfeito.Protegido && e.Tipo == EfeitoDeHabilidade.Acao.AplicarNoAlvo);

		foreach (Combatente c in Combatentes)
		{
			if (!c.Vivo || c.Casa == null)
			{
				continue;
			}
			bool inimigo = c.Lado != quem.Lado;
			if (h.Alvo == Habilidade.TipoDeAlvo.Inimigo && (!inimigo || c.Tem(TipoDeEfeito.Furtivo)))
			{
				continue;
			}
			if (h.Alvo == Habilidade.TipoDeAlvo.Aliado && (inimigo || (protege && c == quem)))
			{
				continue;
			}

			bool alcanca = avanca
				? GradeDeCombate.Distancia(quem.Casa, c.Casa) <= h.Alcance && CasaColadaNoAlvo(quem, c, h.Alcance) != null
				: PodeAtingir(quem, h, quem.Casa, c.Casa);
			if (alcanca)
			{
				casas.Add(c.Casa);
			}
		}
		return casas;
	}

	// -------------------------------------------------------------------------
	// MORTE E FIM
	// -------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: se o lutador chegou a 0 de vida, ele morre — sai da casa, some do mapa e, se era a
	//            Vanguarda de alguém, a proteção acaba.
	// Por quê: "caso a vida de algum personagem ou inimigo chegue a 0, ele morre" (PO, 06/10/2026).
	//          Inimigo morto não volta (não há respawn na demo). Membro da equipe morto fica marcado
	//          como morto na equipe.
	public void ConferirMorte(Combatente c)
	{
		if (c.Vivo || c.Caiu)
		{
			return;
		}

		c.Caiu = true;
		c.Casa = null;
		Registrar($"{c.Nome} morre.");
		c.Marcador?.Atualizar(c);
		foreach (Combatente o in Combatentes)
		{
			if (o.Efeito(TipoDeEfeito.Protegido)?.Origem == c)
			{
				o.Remover(TipoDeEfeito.Protegido);
			}
		}

		// o desenho "cai" (achata) e depois some; inimigo morto sai do mapa de vez
		Node3D corpo = c.Corpo;
		bool inimigo = c.Lado == LadoDoCombate.Inimigos;
		SpriteBase3D? desenho = corpo.GetNodeOrNull<SpriteBase3D>("Desenho") ?? corpo.GetNodeOrNull<SpriteBase3D>("Sprite3D");
		if (desenho != null)
		{
			Tween cair = CreateTween();
			cair.SetParallel();
			cair.TweenProperty(desenho, "scale:y", 0.15f, 0.45f);
			cair.TweenProperty(desenho, "position:y", 0.12f, 0.45f);
			cair.Chain().TweenCallback(Callable.From(() => Sumir(corpo, inimigo)));
		}
		else
		{
			Sumir(corpo, inimigo);
		}

		VerificarFim();
	}

	private void Sumir(Node3D corpo, bool inimigo)
	{
		if (!IsInstanceValid(corpo))
		{
			return;
		}
		if (inimigo)
		{
			// a câmera não pode ficar presa num corpo que vai sair do mapa
			if (_camera != null && _camera.Alvo == corpo && _jogador != null)
			{
				_camera.Alvo = _jogador;
			}
			corpo.QueueFree();
		}
		else
		{
			corpo.Visible = false;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: a luta acaba quando um dos lados não tem mais ninguém vivo.
	public bool VerificarFim()
	{
		if (Resultado != null)
		{
			return true;
		}
		if (!Combatentes.Any(c => c.Vivo && c.Lado == LadoDoCombate.Inimigos))
		{
			Resultado = ResultadoDoCombate.Vitoria;
		}
		else if (!Combatentes.Any(c => c.Vivo && c.Lado == LadoDoCombate.Aliados))
		{
			Resultado = ResultadoDoCombate.Derrota;
		}

		if (Resultado != null)
		{
			_fimDaVezDoJogador?.TrySetResult(true);
		}
		return Resultado != null;
	}

	// Alteração de IA - Revisar
	// O que faz: termina a luta. Vitória: a equipe guarda vida e sanidade, os outros membros saem de
	//            cena, a câmera volta e a exploração continua (se o líder morreu, o próximo vivo
	//            assume). Derrota: a tela de fim de jogo.
	private async Task Terminar(ResultadoDoCombate resultado)
	{
		DaVez = null;
		Modo = ModoDaVez.Esperando;
		AtualizarMarcadores();

		if (resultado == ResultadoDoCombate.Derrota)
		{
			Registrar("O grupo inteiro caiu.");
			_interface.Aviso("Fim de jogo");
			await Esperar(1.2f);
			TelaDeFimDeJogo.Mostrar(this);
			return;
		}

		Registrar("Vitória!");
		_interface.Aviso("Vitória!");
		await Esperar(1.6f);

		Equipe equipe = Equipe.Instancia!;
		foreach (Combatente c in Combatentes.Where(c => c.Membro != null))
		{
			c.Membro!.Vida = c.Vida;
			c.Membro.Sanidade = c.Sanidade;
			c.Membro.Morto = !c.Vivo;
		}

		Combatente lider = Combatentes[0];
		if (!lider.Vivo)
		{
			Combatente? novo = Combatentes.FirstOrDefault(c => c.Lado == LadoDoCombate.Aliados && c.Vivo && c.Membro != null);
			if (novo != null && _jogador != null)
			{
				equipe.DefinirLider(novo.Membro!);
				_jogador.GlobalPosition = novo.Corpo.GlobalPosition;
				_jogador.TrocarDesenho(novo.Ficha);
				_jogador.Visible = true;
				Registrar($"{novo.Nome} assume a liderança.");
			}
		}

		Encerrar();
	}

	// Alteração de IA - Revisar
	// O que faz: desmonta a luta e devolve o jogo à exploração.
	public void Encerrar()
	{
		_geracao++;
		foreach (Combatente c in Combatentes)
		{
			if (c.Marcador != null && IsInstanceValid(c.Marcador))
			{
				c.Marcador.QueueFree();
			}
			if (c.Corpo is CorpoDeCombate membro && IsInstanceValid(membro))
			{
				membro.QueueFree();
			}
		}

		if (_jogador != null && IsInstanceValid(_jogador))
		{
			_jogador.ControladoPeloCombate = false;
			MostrarDesenhosDeTeste(_jogador, true);
			if (_camera != null)
			{
				_camera.Alvo = _jogador;
			}
		}
		foreach (InimigoIA inimigo in _congelados.Where(IsInstanceValid))
		{
			inimigo.EmCombate = false;
			MostrarDesenhosDeTeste(inimigo, true);
		}
		_congelados.Clear();
		_casasDeNeutros.Clear();

		_camera?.DefinirModo(CameraIsometrica.Modo.Exploracao);
		_desenho?.Limpar();
		_interface.Visible = false;
		Combatentes.Clear();
		Ordem.Clear();
		Regiao = new HashSet<Casa>();
		DaVez = null;
		EmCombate = false;
		Modo = ModoDaVez.Esperando;
	}

	// Alteração de IA - Revisar
	// O que faz: zera tudo — usado pela tela de fim de jogo antes de recarregar o mapa.
	public void Reiniciar()
	{
		_geracao++;
		_fimDaVezDoJogador?.TrySetResult(true);
		Combatentes.Clear();
		Ordem.Clear();
		_congelados.Clear();
		_casasDeNeutros.Clear();
		Regiao = new HashSet<Casa>();
		DaVez = null;
		EmCombate = false;
		Resultado = null;
		Modo = ModoDaVez.Esperando;
		_desenho = null;
		_interface.Visible = false;
	}

	// -------------------------------------------------------------------------
	// MOUSE E TECLADO
	// -------------------------------------------------------------------------

	public override void _UnhandledInput(InputEvent evento)
	{
		if (evento.IsActionPressed("debug_grade"))
		{
			AlternarGradeInteira();
			return;
		}

		if (!EmCombate)
		{
			if (evento is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } clique)
			{
				TentarComecarPeloClique(clique.Position);
			}
			return;
		}

		if (!VezDoJogadorAgora)
		{
			return;
		}

		if (evento is InputEventMouseMotion movimento)
		{
			AtualizarMouse(movimento.Position);
		}
		else if (evento is InputEventMouseButton { Pressed: true } botao)
		{
			if (botao.ButtonIndex == MouseButton.Left)
			{
				AtualizarMouse(botao.Position);
				_ = Clicar(botao.Position);
				GetViewport().SetInputAsHandled();
			}
			else if (botao.ButtonIndex == MouseButton.Right)
			{
				Cancelar();
			}
		}
		else if (evento.IsActionPressed("combat_cancel"))
		{
			Cancelar();
		}
		else if (evento.IsActionPressed("combate_encerrar_turno"))
		{
			EncerrarTurno();
		}
	}

	// Alteração de IA - Revisar
	// O que faz: o clique na vez do jogador. Livre: clicar num inimigo ao alcance ataca com o ataque
	//            básico; clicar numa casa azul anda até ela. Escolhendo alvo: clicar num alvo marcado
	//            usa a habilidade.
	private async Task Clicar(Vector2 posicao)
	{
		if (DaVez == null)
		{
			return;
		}

		(Combatente? lutador, Casa? casa) = OQueEstaSobOMouse(posicao);

		if (Modo == ModoDaVez.EscolhendoAlvo && HabilidadeEscolhida != null)
		{
			if (casa != null && _alvosValidos.Contains(casa))
			{
				await UsarHabilidadeDaVez(HabilidadeEscolhida, casa);
			}
			return;
		}

		Habilidade? basico = DaVez.Ficha.AtaqueBasico;
		if (lutador != null && lutador.Lado != DaVez.Lado && basico != null && DaVez.AcaoDisponivel
			&& AlvosValidos(DaVez, basico).Contains(lutador.Casa!))
		{
			await UsarHabilidadeDaVez(basico, lutador.Casa!);
			return;
		}

		// Alteração de IA - Revisar
		// O que faz: clicar num inimigo fora do alcance anda até a casa mais perto de onde o ataque
		//            básico alcança — se der para chegar com o movimento que sobra — e ataca.
		// Por quê: é como no Pit People: o jogador diz "ataque aquele", e o personagem se aproxima
		//          sozinho. Sem isso, era preciso andar e depois clicar de novo.
		if (lutador != null && lutador.Lado != DaVez.Lado && basico != null && DaVez.AcaoDisponivel
			&& MotivoParaNaoUsar(DaVez, basico) is null or "Nenhum alvo ao alcance.")
		{
			Casa? deOnde = _alcancaveis
				.Where(par => PodeParar(DaVez, par.Key) && PodeAtingir(DaVez, basico, par.Key, lutador.Casa!))
				.OrderBy(par => par.Value)
				.Select(par => par.Key)
				.FirstOrDefault();
			if (deOnde != null && await MoverDaVezPara(deOnde) && DaVez != null && AlvosValidos(DaVez, basico).Contains(lutador.Casa!))
			{
				await UsarHabilidadeDaVez(basico, lutador.Casa!);
			}
			return;
		}

		if (casa != null && casa != DaVez.Casa && _alcancaveis.ContainsKey(casa))
		{
			await MoverDaVezPara(casa);
		}
	}

	private void AtualizarMouse(Vector2 posicao)
	{
		(Combatente? lutador, Casa? casa) = OQueEstaSobOMouse(posicao);
		if (casa == _casaSobOMouse)
		{
			return;
		}
		_casaSobOMouse = casa;
		AtualizarPrevisao();
		DesenharGrade();
		_interface.MostrarSobOMouse(lutador);
	}

	// Alteração de IA - Revisar
	// O que faz: decide o que está debaixo do mouse — primeiro a casa do chão: se tem alguém nela, é
	//            ele; o desenho de um personagem só conta quando a casa do chão está vazia (o mouse na
	//            cabeça de alguém, por cima da casa de trás).
	// Por quê: no teste com janela, dois desenhos vizinhos se sobrepunham na tela e o clique na casa
	//          do inimigo acabava indo para o aliado ao lado. A casa é o que o jogador vê marcado.
	private (Combatente? lutador, Casa? casa) OQueEstaSobOMouse(Vector2 posicao)
	{
		Casa? doChao = CasaSobOMouse(posicao);
		Combatente? nela = doChao != null ? CombatenteNaCasa(doChao) : null;
		if (nela != null)
		{
			return (nela, doChao);
		}
		Combatente? peloDesenho = LutadorSobOMouse(posicao);
		return peloDesenho != null ? (peloDesenho, peloDesenho.Casa) : (null, doChao);
	}

	// Alteração de IA - Revisar
	// O que faz: a casa do chão debaixo do mouse (dentro da área da luta).
	public Casa? CasaSobOMouse(Vector2 posicao)
	{
		if (_camera == null || Grade == null)
		{
			return null;
		}

		Vector3 origem = _camera.ProjectRayOrigin(posicao);
		Vector3 fim = origem + _camera.ProjectRayNormal(posicao) * 300.0f;
		var resultado = _camera.GetWorld3D().DirectSpaceState.IntersectRay(
			PhysicsRayQueryParameters3D.Create(origem, fim, Grade.CamadaDoChao));
		if (resultado.Count == 0)
		{
			return null;
		}

		Casa? casa = Grade.CasaEm((Vector3)resultado["position"], 1.0f);
		return casa != null && Regiao.Contains(casa) ? casa : null;
	}

	// Alteração de IA - Revisar
	// O que faz: o lutador cujo desenho está debaixo do mouse.
	// Por quê: o desenho fica em pé; clicar na cabeça de um inimigo acertaria o chão atrás dele. Por
	//          isso o clique é conferido contra o retângulo que o desenho ocupa na tela.
	public Combatente? LutadorSobOMouse(Vector2 posicao)
	{
		if (_camera == null)
		{
			return null;
		}

		Combatente? melhor = null;
		float maisPerto = float.MaxValue;
		foreach (Combatente c in Combatentes.Where(c => c.Vivo && IsInstanceValid(c.Corpo)))
		{
			if (RetanguloNaTela(c.Corpo.GlobalPosition, c.Ficha.AlturaDoDesenho, ProporcaoDoDesenho(c.Corpo)).HasPoint(posicao))
			{
				float distancia = _camera.GlobalPosition.DistanceTo(c.Corpo.GlobalPosition);
				if (distancia < maisPerto)
				{
					maisPerto = distancia;
					melhor = c;
				}
			}
		}
		return melhor;
	}

	private Rect2 RetanguloNaTela(Vector3 pes, float altura, float proporcao)
	{
		if (_camera == null || _camera.IsPositionBehind(pes))
		{
			return new Rect2();
		}
		Vector2 embaixo = _camera.UnprojectPosition(pes);
		Vector2 emCima = _camera.UnprojectPosition(pes + Vector3.Up * altura);
		float alturaNaTela = Mathf.Max(10.0f, embaixo.Y - emCima.Y);
		float largura = alturaNaTela * proporcao;
		return new Rect2(embaixo.X - largura * 0.5f, emCima.Y, largura, alturaNaTela);
	}

	// Alteração de IA - Revisar
	// O que faz: a largura do desenho de um corpo em relação à altura (o Rasgador é largo, a Caçadora
	//            é estreita), para o clique acertar o desenho inteiro.
	private static float ProporcaoDoDesenho(Node3D corpo)
	{
		Texture2D? imagem = null;
		foreach (Node filho in corpo.GetChildren())
		{
			if (filho is Sprite3D parado)
			{
				imagem = parado.Texture;
				break;
			}
			if (filho is AnimatedSprite3D animado && animado.SpriteFrames != null)
			{
				imagem = animado.SpriteFrames.GetFrameTexture(animado.Animation, animado.Frame);
				break;
			}
		}
		if (imagem == null || imagem.GetHeight() <= 0)
		{
			return 0.5f;
		}
		return Mathf.Clamp(0.85f * imagem.GetWidth() / imagem.GetHeight(), 0.35f, 1.1f);
	}

	// Alteração de IA - Revisar
	// O que faz: fora do combate, clicar num inimigo perto e à vista começa a luta com ele.
	private void TentarComecarPeloClique(Vector2 posicao)
	{
		_camera ??= GetTree().GetFirstNodeInGroup("camera_isometrica") as CameraIsometrica;
		_jogador ??= GetTree().GetFirstNodeInGroup("player") as PlayerIsometrico;
		if (_camera == null || _jogador == null || Equipe.Instancia?.MenuAberto == true)
		{
			return;
		}

		foreach (Node no in GetTree().GetNodesInGroup("inimigo"))
		{
			if (no is not InimigoIA inimigo || !IsInstanceValid(inimigo))
			{
				continue;
			}
			float altura = FichaDoInimigo(inimigo).AlturaDoDesenho;
			if (!RetanguloNaTela(inimigo.GlobalPosition, altura, ProporcaoDoDesenho(inimigo)).HasPoint(posicao))
			{
				continue;
			}
			if (inimigo.GlobalPosition.DistanceTo(_jogador.GlobalPosition) > AlcanceParaComecarPeloClique)
			{
				continue;
			}

			var consulta = PhysicsRayQueryParameters3D.Create(
				_jogador.GlobalPosition + Vector3.Up * 1.5f, inimigo.GlobalPosition + Vector3.Up * 1.5f, 8);
			if (_jogador.GetWorld3D().DirectSpaceState.IntersectRay(consulta).Count > 0)
			{
				continue;
			}

			GetViewport().SetInputAsHandled();
			ComecarComSeguranca(inimigo, null);
			return;
		}
	}

	// -------------------------------------------------------------------------
	// DESENHO DA GRADE E DESTAQUES
	// -------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: recalcula o que fica aceso na vez do jogador — casas para andar, alvos válidos.
	private void AtualizarDestaques()
	{
		_alcancaveis.Clear();
		_alvosValidos.Clear();
		if (DaVez != null && DaVez.Lado == LadoDoCombate.Aliados)
		{
			if (Modo == ModoDaVez.Livre)
			{
				_alcancaveis = CasasAlcancaveis(DaVez);
				if (DaVez.Ficha.AtaqueBasico != null && DaVez.AcaoDisponivel && !DaVez.HabilidadeBloqueada(DaVez.Ficha.AtaqueBasico))
				{
					_alvosValidos = AlvosValidos(DaVez, DaVez.Ficha.AtaqueBasico).ToHashSet();
				}
			}
			else if (Modo == ModoDaVez.EscolhendoAlvo && HabilidadeEscolhida != null)
			{
				_alvosValidos = AlvosValidos(DaVez, HabilidadeEscolhida).ToHashSet();
			}
		}
		AtualizarPrevisao();
		DesenharGrade();
	}

	// Alteração de IA - Revisar
	// O que faz: o caminho até a casa debaixo do mouse, ou a área que a habilidade vai atingir.
	private void AtualizarPrevisao()
	{
		_caminhoPrevisto.Clear();
		_areaPrevista.Clear();
		if (DaVez?.Casa == null || _casaSobOMouse == null || Grade == null)
		{
			return;
		}

		if (Modo == ModoDaVez.Livre && _alcancaveis.ContainsKey(_casaSobOMouse) && PodeParar(DaVez, _casaSobOMouse))
		{
			_caminhoPrevisto = Grade.Caminho(DaVez.Casa, _casaSobOMouse, casa => PodePassar(DaVez, casa), Regiao) ?? new List<Casa>();
		}
		else if (Modo == ModoDaVez.EscolhendoAlvo && HabilidadeEscolhida is { RaioDaArea: > 0 } area && _alvosValidos.Contains(_casaSobOMouse))
		{
			foreach (Casa casa in Regiao)
			{
				if (GradeDeCombate.Distancia(casa, _casaSobOMouse) <= area.RaioDaArea)
				{
					_areaPrevista.Add(casa);
				}
			}
		}
	}

	// Alteração de IA - Revisar
	// O que faz: redesenha os hexágonos da área da luta com as cores de agora.
	// Por quê: as cores seguem o Pit People — azul aliado, vermelho inimigo, amarelo quem está na
	//          vez; azul claro para onde dá para andar, laranja para a área de uma habilidade.
	public void DesenharGrade()
	{
		if (Grade == null || !EmCombate)
		{
			return;
		}
		if (_desenho == null || !IsInstanceValid(_desenho))
		{
			_desenho = new DesenhoDaGrade { Name = "DesenhoDaGrade" };
			Grade.AddChild(_desenho);
		}
		_desenho.Desenhar(Grade, Regiao, CorDaCasa);
	}

	private Color? CorDaCasa(Casa casa)
	{
		Combatente? ocupante = CombatenteNaCasa(casa);
		if (_areaPrevista.Contains(casa))
		{
			return new Color(1.0f, 0.6f, 0.2f, 0.45f);
		}
		if (ocupante != null && ocupante == DaVez)
		{
			return new Color(1.0f, 0.85f, 0.2f, 0.6f);
		}
		if (ocupante != null && _alvosValidos.Contains(casa))
		{
			bool sobOMouse = casa == _casaSobOMouse;
			return ocupante.Lado == LadoDoCombate.Inimigos
				? new Color(1.0f, 0.15f, 0.15f, sobOMouse ? 0.85f : 0.65f)
				: new Color(0.3f, 1.0f, 0.55f, sobOMouse ? 0.75f : 0.55f);
		}
		if (ocupante != null)
		{
			return ocupante.Lado == LadoDoCombate.Aliados
				? new Color(0.25f, 0.55f, 1.0f, 0.5f)
				: new Color(0.9f, 0.25f, 0.25f, 0.45f);
		}
		if (_caminhoPrevisto.Contains(casa))
		{
			return new Color(0.55f, 0.9f, 1.0f, 0.55f);
		}
		if (_alcancaveis.ContainsKey(casa))
		{
			return new Color(0.4f, 0.78f, 1.0f, 0.22f);
		}
		if (casa == _casaSobOMouse)
		{
			return new Color(1.0f, 1.0f, 1.0f, 0.12f);
		}
		return null;
	}

	// Alteração de IA - Revisar
	// O que faz: a tecla F6 mostra a grade do mapa inteiro fora do combate, com cada andar numa cor.
	// Por quê: ferramenta para quem monta a fase conferir onde caem os hexágonos — se uma porta ou um
	//          corredor ficou sem casa, dá para ver antes de testar uma luta ali.
	private void AlternarGradeInteira()
	{
		if (EmCombate)
		{
			return;
		}
		Grade = GradeDeCombate.DaFase(GetTree());
		if (Grade == null || !Grade.Pronta)
		{
			return;
		}

		_mostrandoGradeInteira = !_mostrandoGradeInteira;
		if (_desenho == null || !IsInstanceValid(_desenho))
		{
			_desenho = new DesenhoDaGrade { Name = "DesenhoDaGrade" };
			Grade.AddChild(_desenho);
		}

		if (!_mostrandoGradeInteira)
		{
			_desenho.Limpar();
			return;
		}

		Color[] corDoAndar = { new(1, 1, 1, 0.10f), new(0.3f, 0.9f, 1.0f, 0.35f), new(1.0f, 0.5f, 0.9f, 0.35f) };
		_desenho.Desenhar(Grade, Grade.Casas, casa => casa.Vizinhas.Count == 0
			? new Color(1.0f, 0.3f, 0.2f, 0.4f)
			: corDoAndar[Mathf.Min(casa.Andar, corDoAndar.Length - 1)]);
		Registrar($"Grade do mapa: {Grade.Casas.Count} casas, {Grade.TotalDeLigacoes} ligações ({Grade.TempoDeMontagemMs:0} ms).");
	}

	// -------------------------------------------------------------------------
	// AJUDANTES
	// -------------------------------------------------------------------------

	public void AtualizarMarcadores()
	{
		foreach (Combatente c in Combatentes)
		{
			if (c.Marcador != null && IsInstanceValid(c.Marcador))
			{
				c.Marcador.Atualizar(c);
			}
		}
		_interface.AtualizarTudo();
	}

	public void Registrar(string texto)
	{
		_registro.Add(texto);
		GD.Print($"[Combate] {texto}");
		_interface?.Registrar(texto);
	}

	public IReadOnlyList<string> Registro => _registro;

	public async Task Esperar(float segundos)
	{
		await ToSignal(GetTree().CreateTimer(segundos), SceneTreeTimer.SignalName.Timeout);
	}

	public IReadOnlyDictionary<Casa, int> CasasAlcancaveisAgora => _alcancaveis;
}

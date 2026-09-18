using Godot;

// Alteração de IA - Revisar
// O que faz: o "cérebro" do inimigo. Decide o que ele faz a cada instante — rondar, procurar,
//            perseguir ou ficar parado numa atividade.
// Por quê: separa a decisão do movimento. Este script escolhe **para onde ir e por quê**;
//          quem move o corpo é o sistema de navegação do Godot. Assim dá para mudar o
//          comportamento de um inimigo sem mexer em como ele anda.
public partial class InimigoIA : CharacterBody3D
{
	// Alteração de IA - Revisar
	// O que faz: os quatro estados em que o inimigo pode estar.
	// Por quê: um inimigo só faz uma coisa por vez, e cada situação pede um comportamento
	//          diferente. Nomear os estados deixa claro no código o que ele está fazendo.
	public enum Modo
	{
		Ronda,        // segue o caminho combinado, sem suspeitar de nada
		Alerta,       // ouviu algo: para, estranha e olha em volta antes de decidir
		Busca,        // desconfiou de verdade: vai até onde percebeu algo e procura por lá
		Perseguicao,  // viu de fato: vai atrás para começar o combate
		Idle          // ocupado numa atividade, prestando pouca atenção
	}

	// Alteração de IA - Revisar
	// O que faz: as etapas da reação de quem ouve um barulho estranho.
	// Por quê: o inimigo não pode simplesmente girar na direção do som e enxergar o jogador —
	//          isso não dá chance nenhuma de se esconder e mata a furtividade.
	//
	//          Uma pessoa que ouve algo estranho para, estranha ("hum?"), olha para um lado,
	//          olha para o outro, e **só então** se vira para onde achou que veio o som. Cada
	//          etapa leva um tempo, e é justamente esse tempo que dá ao jogador a janela para
	//          sair da linha de visão. É o mesmo recurso de Metal Gear e Assassin's Creed.
	private enum EtapaDoAlerta
	{
		Estranhou,     // parou, apareceu o "?" — ainda não virou para lado nenhum
		OlhouUmLado,
		OlhouOutroLado,
		VirouParaOSom, // só agora encara a direção de onde veio o barulho
		Decidiu
	}

	[Export]
	public float Velocidade { get; set; } = 3.0f;

	[Export]
	public float VelocidadePerseguicao { get; set; } = 4.5f;

	[Export]
	public float Gravidade { get; set; } = 24.0f;

	// Alteração de IA - Revisar
	// O que faz: o quanto o alvo precisa ter se deslocado para o inimigo refazer o trajeto.
	// Por quê: **é o que impede o inimigo de vibrar parado no lugar.** Avisar o sistema de
	//          navegação de um destino novo faz ele recalcular o caminho inteiro do zero. Ao
	//          perseguir alguém que anda, o destino mudava a cada quadro, o caminho era refeito
	//          a cada quadro, e o "próximo ponto do trajeto" acabava caindo atrás do inimigo —
	//          ele dava um passo, passava do ponto, voltava, e assim para sempre. Avisando só
	//          quando o alvo andou meio metro, o caminho tem tempo de valer e ele anda de verdade.
	[Export]
	public float DistanciaParaRefazerOTrajeto { get; set; } = 0.5f;

	// Alteração de IA - Revisar
	// O que faz: quanto tempo dura cada etapa da reação ao barulho (estranhar, olhar de um
	//            lado, olhar do outro, virar para o som).
	// Por quê: **é este número que define se o jogo tem furtividade ou não.** Ele é a janela
	//          que o jogador tem para sair da linha de visão depois de fazer barulho.
	//          Muito curto, e o inimigo praticamente teleporta o olhar para cima do jogador;
	//          muito longo, e o inimigo parece lerdo. São quatro etapas, então o tempo total
	//          da reação é cerca de quatro vezes este valor.
	[Export]
	public float TempoDeCadaEtapaDoAlerta { get; set; } = 0.9f;

	// Alteração de IA - Revisar
	// O que faz: quantos graus ele vira a cabeça para cada lado ao procurar a origem do som.
	// Por quê: olhar para os lados antes de virar para o som é o que faz parecer que ele está
	//          procurando, e não que já sabe onde o jogador está.
	[Export]
	public float AberturaDoOlharEmVolta { get; set; } = 65.0f;

	// Alteração de IA - Revisar
	// O que faz: a velocidade com que ele gira a cabeça.
	// Por quê: girar instantaneamente entrega a posição do jogador no mesmo quadro em que o
	//          som acontece. Girando aos poucos, o cone de visão varre o ambiente de forma
	//          visível — o jogador enxerga o perigo chegando e consegue reagir.
	[Export]
	public float VelocidadeDeVirar { get; set; } = 4.0f;

	// Alteração de IA - Revisar
	// O que faz: quantos segundos o inimigo procura ao chegar no lugar onde percebeu algo.
	// Por quê: sem essa espera, ele chegaria e desistiria no mesmo instante, o que pareceria
	//          burro. Parar e olhar em volta é o que dá a impressão de que ele está mesmo
	//          procurando.
	[Export]
	public float TempoProcurando { get; set; } = 3.0f;

	// Alteração de IA - Revisar
	// O que faz: quantos segundos o inimigo insiste na perseguição depois de perder o
	//            jogador de vista.
	// Por quê: desistir na hora deixaria fácil demais escapar — bastaria virar uma esquina.
	//          O valor cresce com a **Reação** do inimigo: um bicho mais atento demora mais
	//          para desistir.
	[Export]
	public float TempoAteDesistir { get; set; } = 4.0f;

	// Alteração de IA - Revisar
	// O que faz: a chance, a cada segundo, de o inimigo largar a ronda e ir fazer uma
	//            atividade (sentar, dormir, trabalhar).
	// Por quê: deixa a ronda menos previsível. Em 0, ele nunca para — útil para sentinelas.
	//          Inimigos que **só** ficam parados numa atividade devem começar em Idle e ter
	//          este valor em 0.
	[Export]
	public float ChanceDeIdlePorSegundo { get; set; } = 0.0f;

	[Export]
	public float TempoEmIdle { get; set; } = 6.0f;

	// Alteração de IA - Revisar
	// O que faz: o quanto os raios encolhem enquanto ele está na atividade.
	// Por quê: alguém sentado de costas ou dormindo percebe muito menos. É o que dá sentido
	//          ao modo — e o que permite ao jogador passar por trás de um inimigo distraído.
	[Export]
	public float ReducaoDosRaiosEmIdle { get; set; } = 0.35f;

	public Modo ModoAtual { get; private set; } = Modo.Ronda;
	public float DirecaoOlhando { get; private set; } = 270.0f;

	// Alteração de IA - Revisar
	// O que faz: informa em que ponto da reação ao barulho o inimigo está.
	// Por quê: o desenho de teste mostra isso na tela, para dar para conferir se a sequência
	//          está acontecendo na ordem certa e com o tempo certo.
	public string EtapaDoAlertaTexto => _etapaDoAlerta switch
	{
		EtapaDoAlerta.Estranhou => "estranhou",
		EtapaDoAlerta.OlhouUmLado => "olhando um lado",
		EtapaDoAlerta.OlhouOutroLado => "olhando o outro",
		EtapaDoAlerta.VirouParaOSom => "virou para o som",
		_ => "decidindo"
	};

	// Alteração de IA - Revisar
	// O que faz: o quanto ele desconfia do jogador agora, de 0 a 100.
	// Por quê: substituiu o cronômetro que contava o tempo dentro do cone de visão. Aquele
	//          cronômetro só servia para a visão; a barra vale para **todas** as formas de
	//          perceber e é o que impede o inimigo de ficar travado no susto (ver
	//          MedidorDeSuspeita).
	public float Suspeita => _medidor?.Porcentagem ?? 0.0f;

	// de onde vem a suspeita agora ("ouvindo", "perto", "vendo") — usado pelo desenho de teste
	public string FonteDaSuspeita => _medidor?.FonteAtualTexto ?? "";

	public Vector3 UltimaPosicaoPercebida { get; private set; }

	private SensorDeteccao _sensor = null!;
	private NavigationAgent3D _navegacao = null!;
	private MedidorDeSuspeita _medidor = null!;
	private RotaRonda? _rota;
	private SensorDeteccao? _sensorJogador;
	private BalaoAviso? _balao;

	private float _cronometro;
	private float _raio1Original;
	private float _alcanceVisaoOriginal;
	private Vector3 _localDaAtividade;

	// Alteração de IA - Revisar
	// O que faz: para onde ele **quer** olhar, e o estado da reação ao barulho.
	// Por quê: "para onde olha" e "para onde quer olhar" são coisas diferentes enquanto a
	//          cabeça está girando. Guardar as duas é o que permite o giro acontecer aos
	//          poucos em vez de num salto.
	private float _direcaoDesejada = 270.0f;
	private EtapaDoAlerta _etapaDoAlerta;
	private float _direcaoAoOuvir;

	// avisa se o sinal de combate deste encontro já foi emitido, para não repetir a cada quadro
	private bool _jaAvisouCombate;

	// para onde ele está indo, e se está indo a algum lugar
	private Vector3 _destinoAtual;
	private bool _temDestino;

	private bool _alturaDaNavegacaoAjustada;

	public override void _Ready()
	{
		_sensor = GetNode<SensorDeteccao>("SensorDeteccao");
		_navegacao = GetNode<NavigationAgent3D>("NavigationAgent3D");
		_rota = GetNodeOrNull<RotaRonda>("RotaRonda");
		_balao = GetNodeOrNull<BalaoAviso>("BalaoAviso");

		// Alteração de IA - Revisar
		// O que faz: pega o medidor de suspeita da cena; se não houver, cria um com os valores
		//            de fábrica.
		// Por quê: nenhum inimigo pode ficar sem medidor — sem ele o sistema de furtividade não
		//          funciona. Criar na hora evita que um inimigo montado antes desta mudança, ou
		//          esquecido pelo caminho, quebre o jogo. Quem quiser ajustar os números para um
		//          inimigo específico acrescenta o nó na cena dele.
		_medidor = GetNodeOrNull<MedidorDeSuspeita>("MedidorDeSuspeita");
		if (_medidor == null)
		{
			_medidor = new MedidorDeSuspeita { Name = "MedidorDeSuspeita" };
			AddChild(_medidor);
		}

		_sensor.AplicarProporcao();
		_raio1Original = _sensor.RaioDeteccao1;
		_alcanceVisaoOriginal = _sensor.AlcanceVisao;

		// Alteração de IA - Revisar
		// O que faz: acha o jogador pelo grupo "player".
		// Por quê: o inimigo precisa comparar os raios dele com os do jogador. Procurar pelo
		//          grupo, e não pelo caminho na cena, mantém isso funcionando mesmo que
		//          alguém reorganize os nós do mapa.
		var achados = GetTree().GetNodesInGroup("player");
		if (achados.Count > 0)
		{
			_sensorJogador = (achados[0] as Node)?.GetNodeOrNull<SensorDeteccao>("SensorDeteccao");
		}

		if (_sensorJogador == null)
		{
			GD.PushWarning("InimigoIA: não achei o sensor do jogador. " +
						   "Confira se o jogador está no grupo 'player' e tem um nó SensorDeteccao.");
		}

		if (ModoAtual == Modo.Idle)
		{
			_localDaAtividade = GlobalPosition;
			AplicarReducaoDeIdle(true);
		}
		else
		{
			IrParaProximoPontoDaRonda();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		AlinharAlturaDaNavegacao();
		AvaliarOQuePercebe(dt);

		switch (ModoAtual)
		{
			case Modo.Ronda: ComportamentoRonda(dt); break;
			case Modo.Alerta: ComportamentoAlerta(dt); break;
			case Modo.Busca: ComportamentoBusca(dt); break;
			case Modo.Perseguicao: ComportamentoPerseguicao(dt); break;
			case Modo.Idle: ComportamentoIdle(dt); break;
		}

		MoverSeguindoNavegacao(dt);
		GirarCabecaAosPoucos(dt);
		AtualizarBalao();
	}

	// -------------------------------------------------------------------------
	// PERCEPÇÃO
	// -------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: a cada instante, informa ao medidor o que o inimigo está percebendo e reage ao
	//            que a barra de suspeita disser.
	// Por quê: é o coração do sistema de furtividade. São três formas de perceber, da mais vaga
	//          para a mais certeira:
	//
	//            1. os círculos externos se tocam  -> ouviu algo (barra sobe devagar, até 50%)
	//            2. alcança o círculo interno      -> está quase esbarrando (sobe rápido, até 100%)
	//            3. o jogador está no cone de visão -> está vendo (sobe rápido, até 100%)
	//
	//          **Nenhuma delas decide nada sozinha.** Todas alimentam a mesma barra, e é a barra
	//          que decide: aos 10% ele estranha, aos 100% ele reconhece. Antes cada uma era um
	//          interruptor próprio, e era isso que permitia travar o inimigo entrando e saindo
	//          do alcance.
	//
	//          Nenhuma delas vale se houver parede no meio: antes de qualquer coisa, o sistema
	//          confere se existe linha de visão livre.
	private void AvaliarOQuePercebe(float dt)
	{
		if (_sensorJogador == null)
		{
			return;
		}

		bool semParedeNoMeio = _sensor.TemLinhaDeVisao(_sensorJogador);

		bool ouviu = semParedeNoMeio && _sensor.CirculosExternosSeTocam(_sensorJogador);
		bool estaPerto = semParedeNoMeio && _sensor.AlcancaCirculoInterno(_sensorJogador);
		bool vendo = semParedeNoMeio && _sensor.DentroDoConeDeVisao(_sensorJogador, DirecaoOlhando);

		_medidor.Atualizar(ouviu, estaPerto, vendo, _sensorJogador.TempoParaSerVistoEfetivo, dt);

		// --- barra cheia: não é mais suspeita, ele reconheceu o jogador ---
		if (_medidor.AcabouDeDetectar && ModoAtual != Modo.Perseguicao)
		{
			EntrarEmPerseguicao();
			return;
		}

		// --- cruzou os 10%: o "hum?" ---
		//
		// Alteração de IA - Revisar
		// O que faz: a reação de parar e olhar em volta só acontece quando ele **ouviu** — não
		//            quando já está vendo o jogador.
		// Por quê: olhar para os lados é o que alguém faz quando ouviu um barulho e não sabe de
		//          onde veio. Se o jogador já está no campo de visão dele, virar a cabeça para
		//          os lados seria justamente desviar o olhar de quem ele está olhando — ficaria
		//          estranho e ainda ajudaria o jogador sem querer. Nesse caso ele só mostra o
		//          "?" e continua o que estava fazendo enquanto a barra sobe.
		if (_medidor.AcabouDeEstranhar &&
			_medidor.FonteAtual == MedidorDeSuspeita.Fonte.Ouvindo &&
			(ModoAtual == Modo.Ronda || ModoAtual == Modo.Idle))
		{
			EntrarEmAlerta(_sensorJogador.GlobalPosition);
			return;
		}

		// continua atualizando o rastro enquanto ainda sente a presença.
		// durante o Alerta o rastro NÃO é atualizado de propósito: ele vai investigar onde
		// ouviu, não onde o jogador está agora.
		if (ouviu && ModoAtual == Modo.Busca)
		{
			UltimaPosicaoPercebida = _sensorJogador.GlobalPosition;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: mantém o balão da cabeça de acordo com a barra de suspeita.
	// Por quê: antes o "?" e o "!" eram ligados e desligados em vários pontos do código, e era
	//          fácil esquecer um e deixar o balão preso na tela. Agora há um lugar só: o balão
	//          é o retrato da barra. Acima de 10% ele mostra "?", em perseguição mostra "!", e
	//          abaixo disso some sozinho quando o inimigo se acalma.
	private void AtualizarBalao()
	{
		if (_balao == null)
		{
			return;
		}

		if (ModoAtual == Modo.Perseguicao)
		{
			_balao.MostrarAlerta();
		}
		else if (_medidor.Porcentagem >= _medidor.LimiteParaEstranhar)
		{
			_balao.MostrarDuvida();
		}
		else
		{
			_balao.Esconder();
		}
	}

	// -------------------------------------------------------------------------
	// COMPORTAMENTOS
	// -------------------------------------------------------------------------

	private void ComportamentoRonda(float dt)
	{
		if (!_temDestino || _navegacao.IsNavigationFinished())
		{
			IrParaProximoPontoDaRonda();
		}

		// Alteração de IA - Revisar
		// O que faz: sorteia se ele larga a ronda para ir fazer uma atividade.
		// Por quê: a chance é por segundo, então multiplicar pelo tempo do quadro mantém o
		//          comportamento igual independente da taxa de quadros da máquina.
		if (ChanceDeIdlePorSegundo > 0.0f && GD.Randf() < ChanceDeIdlePorSegundo * dt)
		{
			EntrarEmIdle();
		}
	}

	// Alteração de IA - Revisar
	// O que faz: conduz a reação ao barulho, uma etapa por vez, com uma pausa entre cada.
	// Por quê: é o que transforma "ouviu" em algo jogável. A sequência imita quem ouve um
	//          ruído estranho: para, estranha, olha para um lado, olha para o outro e só
	//          então se vira para onde achou que veio o som.
	//
	//          Ele fica **parado** durante toda a sequência — a navegação é interrompida.
	//          Andar enquanto olha em volta estragaria a leitura da cena para o jogador.
	private void ComportamentoAlerta(float dt)
	{
		PararDeAndar();   // fica onde está durante toda a sequência
		_cronometro -= dt;

		if (_cronometro > 0.0f)
		{
			return;
		}

		_cronometro = TempoDeCadaEtapaDoAlerta;

		switch (_etapaDoAlerta)
		{
			case EtapaDoAlerta.Estranhou:
				// primeiro olhada: para um lado da direção em que já estava
				_direcaoDesejada = Mathf.PosMod(_direcaoAoOuvir + AberturaDoOlharEmVolta, 360.0f);
				_etapaDoAlerta = EtapaDoAlerta.OlhouUmLado;
				break;

			case EtapaDoAlerta.OlhouUmLado:
				_direcaoDesejada = Mathf.PosMod(_direcaoAoOuvir - AberturaDoOlharEmVolta, 360.0f);
				_etapaDoAlerta = EtapaDoAlerta.OlhouOutroLado;
				break;

			case EtapaDoAlerta.OlhouOutroLado:
				// só agora encara a direção de onde veio o som
				_direcaoDesejada = AnguloAte(UltimaPosicaoPercebida);
				_etapaDoAlerta = EtapaDoAlerta.VirouParaOSom;
				break;

			case EtapaDoAlerta.VirouParaOSom:
				_etapaDoAlerta = EtapaDoAlerta.Decidiu;
				break;

			case EtapaDoAlerta.Decidiu:
				// Alteração de IA - Revisar
				// O que faz: decide se vai investigar ou volta para a ronda.
				// Por quê: se o jogador saiu de perto durante a sequência, o inimigo dá de
				//          ombros e volta ao normal — foi a chance de escapar funcionando.
				//          Se ainda sente algo, aí sim vai até o lugar conferir.
				bool aindaSenteAlgo = _sensorJogador != null &&
									  _sensor.TemLinhaDeVisao(_sensorJogador) &&
									  _sensor.CirculosExternosSeTocam(_sensorJogador);

				if (aindaSenteAlgo)
				{
					EntrarEmBusca(UltimaPosicaoPercebida);
				}
				else
				{
					// Alteração de IA - Revisar
					// O que faz: volta para a ronda **sem zerar a barra de suspeita**.
					// Por quê: ele não achou nada, mas também não esqueceu. A barra continua de
					//          onde estava e só desce com o tempo — é o que faz um segundo
					//          barulho ser percebido mais rápido que o primeiro, e o que impede
					//          o jogador de reiniciar a reação de espanto à vontade.
					ModoAtual = Modo.Ronda;
					IrParaProximoPontoDaRonda();
				}
				break;
		}
	}

	private void ComportamentoBusca(float dt)
	{
		if (_temDestino && !_navegacao.IsNavigationFinished())
		{
			return;
		}

		// chegou onde tinha percebido algo: para e olha em volta
		_cronometro -= dt;
		OlharEmVolta(dt);

		if (_cronometro <= 0.0f)
		{
			// não achou nada: volta para a ronda de onde parou, ainda desconfiado
			ModoAtual = Modo.Ronda;
			IrParaProximoPontoDaRonda();
		}
	}

	private void ComportamentoPerseguicao(float dt)
	{
		if (_sensorJogador == null)
		{
			ModoAtual = Modo.Ronda;
			return;
		}

		bool aindaPercebe = _sensor.TemLinhaDeVisao(_sensorJogador) &&
							_sensor.CirculosExternosSeTocam(_sensorJogador);

		if (aindaPercebe)
		{
			UltimaPosicaoPercebida = _sensorJogador.GlobalPosition;
			_cronometro = TempoAteDesistirEfetivo();

			// Alteração de IA - Revisar
			// O que faz: ao chegar na distância da arma, ele **para e encara** o jogador, em vez
			//            de continuar andando até ficar em cima dele. E avisa que o combate deve
			//            começar — uma vez só por encontro.
			// Por quê: dois defeitos foram corrigidos aqui. O primeiro: ele perseguia até a
			//          distância zero, atravessando o jogador e parando por cima dele, o que
			//          parecia um travamento. A distância de encontro existe justamente para
			//          representar o alcance da arma — é onde ele deve parar. O segundo: o aviso
			//          de combate era emitido a cada quadro, 60 vezes por segundo, o que faria o
			//          combate ser iniciado sem parar quando alguém passar a escutá-lo.
			//
			//          Enquanto o combate por turnos não existir, ele fica aqui parado encarando
			//          o jogador, de arma em punho. Não é travamento: é o ponto de entrega para
			//          o sistema de combate.
			if (_sensor.DentroDoRaioDeEncontro(_sensorJogador))
			{
				PararDeAndar();
				_direcaoDesejada = AnguloAte(UltimaPosicaoPercebida);

				if (!_jaAvisouCombate)
				{
					_jaAvisouCombate = true;
					EmitSignal(SignalName.CombateDeveComecar, this);
				}
			}
			else
			{
				// o jogador escapou do alcance da arma: volta a perseguir e o aviso rearma
				_jaAvisouCombate = false;
				SeguirAteAosPoucos(UltimaPosicaoPercebida);
			}
		}
		else
		{
			// perdeu de vista: insiste por um tempo indo até o último lugar conhecido
			_cronometro -= dt;
			IrPara(UltimaPosicaoPercebida);

			if (_cronometro <= 0.0f)
			{
				EntrarEmBusca(UltimaPosicaoPercebida);
			}
		}
	}

	private void ComportamentoIdle(float dt)
	{
		SeguirAteAosPoucos(_localDaAtividade);

		if (TempoEmIdle <= 0.0f)
		{
			return;   // fica na atividade para sempre (sentinela parado, alguém dormindo)
		}

		_cronometro -= dt;
		if (_cronometro <= 0.0f)
		{
			AplicarReducaoDeIdle(false);
			ModoAtual = Modo.Ronda;
			IrParaProximoPontoDaRonda();
		}
	}

	// -------------------------------------------------------------------------
	// TROCAS DE MODO
	// -------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: começa a reação ao barulho — para tudo, mostra o "?" e guarda para onde
	//            estava olhando.
	// Por quê: guardar a direção em que ele **já estava** é o que faz os dois olhares
	//          seguintes saírem dali, e não da posição do jogador. Se saíssem da direção do
	//          som, ele estaria praticamente encarando o jogador de cara — o problema que
	//          esta mudança veio resolver.
	private void EntrarEmAlerta(Vector3 ondeOuviu)
	{
		if (ModoAtual == Modo.Idle)
		{
			AplicarReducaoDeIdle(false);
		}

		ModoAtual = Modo.Alerta;
		UltimaPosicaoPercebida = ondeOuviu;
		_etapaDoAlerta = EtapaDoAlerta.Estranhou;
		_direcaoAoOuvir = DirecaoOlhando;
		_direcaoDesejada = DirecaoOlhando;   // na primeira etapa ele nem vira: só estranha
		_cronometro = TempoDeCadaEtapaDoAlerta;
	}

	private void EntrarEmBusca(Vector3 onde)
	{
		if (ModoAtual == Modo.Idle)
		{
			AplicarReducaoDeIdle(false);
		}
		ModoAtual = Modo.Busca;
		UltimaPosicaoPercebida = onde;
		IrPara(onde);
		_cronometro = TempoProcurando;
	}

	// Alteração de IA - Revisar
	// O que faz: calcula para que lado fica um ponto, em graus, na mesma roda usada pela
	//            câmera (0 à direita, 90 acima, 180 à esquerda, 270 abaixo).
	// Por quê: é o que traduz "onde ouvi o barulho" em "para que lado virar a cabeça".
	private float AnguloAte(Vector3 ponto)
	{
		Vector3 ate = ponto - GlobalPosition;
		ate.Y = 0.0f;
		if (ate.LengthSquared() < 0.001f)
		{
			return DirecaoOlhando;
		}
		return Mathf.PosMod(Mathf.RadToDeg(Mathf.Atan2(-ate.Z, ate.X)), 360.0f);
	}

	// Alteração de IA - Revisar
	// O que faz: aproxima aos poucos a direção do olhar da direção desejada.
	// Por quê: é o que faz o inimigo **virar a cabeça** em vez de teletransportar o olhar.
	//          Usa o caminho mais curto, então virar de 350 para 10 graus passa pelo zero em
	//          vez de dar a volta inteira.
	private void GirarCabecaAosPoucos(float dt)
	{
		float atual = Mathf.DegToRad(DirecaoOlhando);
		float alvo = Mathf.DegToRad(_direcaoDesejada);
		float novo = Mathf.LerpAngle(atual, alvo, dt * VelocidadeDeVirar);
		DirecaoOlhando = Mathf.PosMod(Mathf.RadToDeg(novo), 360.0f);
	}

	private void EntrarEmPerseguicao()
	{
		if (ModoAtual == Modo.Idle)
		{
			AplicarReducaoDeIdle(false);
		}
		if (_sensorJogador != null)
		{
			UltimaPosicaoPercebida = _sensorJogador.GlobalPosition;
			IrPara(UltimaPosicaoPercebida);
		}
		// o balão vira "!" sozinho: quem cuida disso é AtualizarBalao, olhando para o modo
		ModoAtual = Modo.Perseguicao;
		_cronometro = TempoAteDesistirEfetivo();
	}

	private void EntrarEmIdle()
	{
		ModoAtual = Modo.Idle;
		_localDaAtividade = GlobalPosition;
		_cronometro = TempoEmIdle;
		AplicarReducaoDeIdle(true);
	}

	// Alteração de IA - Revisar
	// O que faz: encolhe (ou devolve ao normal) os raios do inimigo ao entrar e sair da
	//            atividade.
	// Por quê: é isso que traduz "está distraído" em algo que o jogador sente no jogo.
	private void AplicarReducaoDeIdle(bool entrando)
	{
		float fator = entrando ? (1.0f - ReducaoDosRaiosEmIdle) : 1.0f;
		_sensor.RaioDeteccao1 = _raio1Original * fator;
		_sensor.AlcanceVisao = _alcanceVisaoOriginal * fator;
		_sensor.AplicarProporcao();
	}

	// Alteração de IA - Revisar
	// O que faz: calcula quanto tempo este inimigo insiste antes de desistir.
	// Por quê: a **Reação** entra aqui. Um inimigo atento persegue por mais tempo — é o
	//          mesmo atributo que aumenta os raios dele, agora pesando na teimosia.
	private float TempoAteDesistirEfetivo()
	{
		float bonus = Mathf.Min(_sensor.Reacao * _sensor.EfeitoPorPonto, _sensor.EfeitoMaximo);
		return TempoAteDesistir * (1.0f + bonus);
	}

	// -------------------------------------------------------------------------
	// MOVIMENTO
	// -------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: descobre a que altura o "chão de navegação" fica em relação aos pés do inimigo
	//            e avisa essa diferença ao sistema de navegação.
	// Por quê: **sem isto o inimigo não anda.** O mapa de navegação não é gerado exatamente em
	//          cima do chão: ele fica meio metro acima. O sistema decide que o inimigo chegou a
	//          um ponto do trajeto medindo a distância **em três dimensões** — e essa distância
	//          nunca ficava abaixo do meio metro exigido, porque a diferença de altura sozinha
	//          já valia meio metro. Resultado: o trajeto nunca avançava para o ponto seguinte e
	//          o inimigo ficava plantado, mesmo com o caminho todo calculado.
	//
	//          Avisando a diferença de altura, a conta passa a ser só horizontal, que é o que
	//          importa para alguém que anda no chão.
	//
	//          A diferença é **medida no próprio mapa**, e não escrita à mão, porque ela depende
	//          de como cada fase foi gerada. Se a fase mudar, isto continua certo sozinho.
	//          (Fases com andares em alturas diferentes vão precisar medir de novo a cada andar.)
	private void AlinharAlturaDaNavegacao()
	{
		if (_alturaDaNavegacaoAjustada)
		{
			return;
		}

		// Alteração de IA - Revisar
		// O que faz: espera o mapa de navegação ficar pronto, e espera o inimigo assentar no chão.
		// Por quê: o mapa é montado em segundo plano e demora alguns instantes depois de a fase
		//          abrir; perguntar antes disso enche o console de erro e devolve lixo. E medir
		//          antes de ele pousar no chão daria uma altura errada, que ficaria valendo para
		//          o resto da partida, já que a medição é feita uma vez só.
		Rid mapa = _navegacao.GetNavigationMap();
		if (!mapa.IsValid || NavigationServer3D.MapGetIterationId(mapa) == 0 || !IsOnFloor())
		{
			return;
		}

		Vector3 noChaoDeNavegacao = NavigationServer3D.MapGetClosestPoint(mapa, GlobalPosition);
		var distanciaHorizontal = new Vector2(noChaoDeNavegacao.X - GlobalPosition.X,
											  noChaoDeNavegacao.Z - GlobalPosition.Z);

		// ainda não terminou de gerar: o ponto devolvido não tem nada a ver com onde ele está
		if (distanciaHorizontal.Length() > 2.0f)
		{
			return;
		}

		_navegacao.PathHeightOffset = noChaoDeNavegacao.Y - GlobalPosition.Y;
		_alturaDaNavegacaoAjustada = true;
	}

	// Alteração de IA - Revisar
	// O que faz: manda o inimigo para um lugar certo e definido — o próximo ponto da ronda, o
	//            lugar onde ouviu algo.
	// Por quê: são destinos que não mudam depois de escolhidos, então avisar o sistema de
	//          navegação uma vez só é o certo e o caminho vale até ele chegar lá.
	private void IrPara(Vector3 destino)
	{
		_destinoAtual = destino;
		_temDestino = true;
		_navegacao.TargetPosition = destino;
	}

	// Alteração de IA - Revisar
	// O que faz: segue alguém que está andando, refazendo o trajeto só quando essa pessoa já se
	//            afastou o bastante do destino anterior.
	// Por quê: perseguir é diferente de ir a um lugar fixo: o alvo se move o tempo todo. Se o
	//          trajeto fosse refeito a cada quadro, o inimigo travava vibrando no lugar (ver a
	//          explicação em DistanciaParaRefazerOTrajeto). Meio metro de folga é curto o
	//          bastante para a perseguição continuar convincente e longo o bastante para o
	//          caminho durar alguns quadros.
	private void SeguirAteAosPoucos(Vector3 destino)
	{
		if (_temDestino && _destinoAtual.DistanceTo(destino) < DistanciaParaRefazerOTrajeto)
		{
			return;
		}
		IrPara(destino);
	}

	// Alteração de IA - Revisar
	// O que faz: cancela o destino — ele para onde está.
	// Por quê: antes, "ficar parado" era feito mandando o inimigo ir até a **própria posição**.
	//          Como a posição dele muda um pouquinho a cada quadro, isso era um destino novo a
	//          cada quadro, e era outra porta de entrada para a vibração. Não ter destino é
	//          diferente de ter um destino em cima de si mesmo.
	private void PararDeAndar()
	{
		_temDestino = false;
	}

	private void IrParaProximoPontoDaRonda()
	{
		if (_rota == null || _rota.QuantidadeDePontos == 0)
		{
			PararDeAndar();   // sem rota: fica onde está
			return;
		}
		IrPara(_rota.ProximoPonto());
	}

	private void MoverSeguindoNavegacao(float dt)
	{
		Vector3 velocidade = Velocity;

		if (!IsOnFloor())
		{
			velocidade.Y -= Gravidade * dt;
		}
		else if (velocidade.Y < 0.0f)
		{
			velocidade.Y = 0.0f;
		}

		// Alteração de IA - Revisar
		// O que faz: só anda se tiver um destino de verdade e ainda não tiver chegado.
		// Por quê: antes bastava perguntar ao sistema de navegação se a viagem tinha acabado.
		//          Mas quando o inimigo não tem para onde ir, ele não tem viagem nenhuma — e a
		//          pergunta dava respostas confusas. Guardar por conta própria se existe destino
		//          deixa "parado" e "chegou" serem duas coisas diferentes e claras.
		bool andando = _temDestino && !_navegacao.IsNavigationFinished();
		float passo = 0.0f;
		Vector3 direcao = Vector3.Zero;

		if (andando)
		{
			Vector3 destino = _navegacao.GetNextPathPosition();
			Vector3 ate = destino - GlobalPosition;
			ate.Y = 0.0f;

			float distancia = ate.Length();
			float vel = ModoAtual == Modo.Perseguicao ? VelocidadePerseguicao : Velocidade;

			// Alteração de IA - Revisar
			// O que faz: se o próximo ponto do trajeto está mais perto do que ele andaria neste
			//            quadro, ele anda só o necessário para chegar — nunca mais que isso.
			// Por quê: **é a segunda causa da vibração.** Andando sempre a velocidade cheia, ele
			//          passava do ponto, e no quadro seguinte o ponto ficava atrás dele: dava
			//          meia-volta, passava de novo, e ficava tremendo no lugar sem sair. Limitar
			//          o passo à distância que falta acaba com isso de vez.
			passo = Mathf.Min(vel, distancia / Mathf.Max(dt, 0.0001f));

			if (distancia > 0.001f)
			{
				direcao = ate / distancia;
			}
			else
			{
				andando = false;
			}
		}

		if (andando)
		{
			velocidade.X = direcao.X * passo;
			velocidade.Z = direcao.Z * passo;

			// Alteração de IA - Revisar
			// O que faz: vira o inimigo para onde ele está andando.
			// Por quê: o cone de visão sai da direção em que ele olha. Sem virar o corpo, ele
			//          andaria para um lado enquanto enxergaria para outro.
			//
			//          Só vira quando está andando com alguma decisão: com passos muito curtos,
			//          a direção oscila de um quadro para o outro e a cabeça ficaria tremendo.
			if (passo > Velocidade * 0.25f)
			{
				_direcaoDesejada = Mathf.PosMod(Mathf.RadToDeg(Mathf.Atan2(-direcao.Z, direcao.X)), 360.0f);
			}
		}
		else
		{
			velocidade.X = Mathf.MoveToward(velocidade.X, 0.0f, Velocidade * 4.0f * dt);
			velocidade.Z = Mathf.MoveToward(velocidade.Z, 0.0f, Velocidade * 4.0f * dt);
		}

		Velocity = velocidade;
		MoveAndSlide();
	}

	// Alteração de IA - Revisar
	// O que faz: faz o inimigo girar no lugar enquanto procura.
	// Por quê: parado e imóvel, ele pareceria travado. Girando, fica claro para o jogador
	//          que ele está vasculhando os arredores — e o cone de visão varre a área junto.
	private void OlharEmVolta(float dt)
	{
		_direcaoDesejada = Mathf.PosMod(_direcaoDesejada + 90.0f * dt, 360.0f);
	}

	[Signal]
	public delegate void CombateDeveComecarEventHandler(InimigoIA inimigo);
}

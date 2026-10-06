using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// Alteração de IA - Revisar
// O que faz: uma casa da grade de combate que existe de verdade no mapa — o hexágono, em que andar
//            ele fica (térreo, segundo andar...), o ponto do chão no centro e as casas vizinhas para
//            onde dá para andar.
// Por quê: o mesmo hexágono do mapa pode ter mais de uma casa empilhada — o térreo da casa e o
//          andar de cima, por exemplo. Cada uma é uma casa diferente, com vizinhas diferentes.
public sealed class Casa
{
	public Casa(Hex coordenada, int andar, Vector3 centro)
	{
		Coordenada = coordenada;
		Andar = andar;
		Centro = centro;
	}

	public Hex Coordenada { get; }
	public int Andar { get; }
	public Vector3 Centro { get; }

	// altura do chão em cada uma das 6 pontas — o desenho do hexágono acompanha rampa e degrau
	public float[] AlturaDasPontas { get; } = new float[6];

	public List<Casa> Vizinhas { get; } = new();

	public override string ToString() => $"{Coordenada} andar {Andar} ({Centro.X:0.0}; {Centro.Y:0.00}; {Centro.Z:0.0})";
}

// Alteração de IA - Revisar
// O que faz: a grade de hexágonos do combate de um mapa. Ela é montada **uma vez, quando a fase
//            carrega**, cobrindo o mapa inteiro: para cada hexágono, acha os chãos que existem ali
//            (térreo, andar de cima, rampa), descarta os que têm parede, árvore ou teto baixo no
//            meio, e liga cada casa às vizinhas para onde dá para andar.
// Por quê: o combate acontece **onde começou**, em qualquer ponto do mapa, como no Baldur's Gate 3
//          (decisão do PO em 06/10/2026). Se a grade fosse montada na hora, em volta de onde a
//          luta começou, cada luta teria hexágonos em lugares diferentes e poderiam cair cortados
//          numa porta ou num muro. Montada uma vez por mapa, fixa, ela é sempre a mesma — o
//          designer da fase pode conferir (tecla F6) e ajustar o tamanho das casas se uma porta
//          ficar ruim. Ela só aparece na tela durante o combate.
public partial class GradeDeCombate : Node3D
{
	// Alteração de IA - Revisar
	// O que faz: o tamanho de cada casa — a distância do centro do hexágono até uma ponta.
	// Por quê: com 1 m, de uma casa para a vizinha são 1,73 m. Um personagem cabe com folga e três
	//          casas de movimento dão uns 5 metros, parecido com o Pit People.
	[Export]
	public float RaioDaCasa { get; set; } = 1.0f;

	// Alteração de IA - Revisar
	// O que faz: a área do mapa coberta pela grade, em metros, centrada neste nó.
	[Export]
	public Vector2 Tamanho { get; set; } = new(70.0f, 70.0f);

	// Alteração de IA - Revisar
	// O que faz: a maior diferença de altura entre duas casas vizinhas para dar para passar — num
	//            degrau (chão que muda de altura de uma vez) e numa rampa (chão que sobe aos poucos).
	// Por quê: a rampa do sandbox tem 20 graus: cada casa dela sobe 63 cm. Um degrau de 63 cm já é
	//          alto (meia perna); numa rampa, não. Por isso a rampa aceita mais, desde que o chão no
	//          meio do caminho esteja mesmo na altura do meio. Nenhum dos dois deixa pular de um andar
	//          para o outro nem subir numa caixa.
	[Export]
	public float AlturaMaximaDoDegrau { get; set; } = 0.6f;

	[Export]
	public float AlturaMaximaNaRampa { get; set; } = 1.2f;

	// Alteração de IA - Revisar
	// O que faz: o espaço livre mínimo acima do chão para a casa existir, e a folga em volta do
	//            centro que precisa estar sem parede nem obstáculo.
	[Export]
	public float AlturaLivreMinima { get; set; } = 1.6f;

	// Alteração de IA - Revisar
	// O que faz: a folga em volta do centro da casa que precisa estar livre de parede, e a que
	//            precisa estar livre de obstáculo (árvore).
	// Por quê: parede é uma divisa fina — uma casa encostada num muro continua valendo. Já uma árvore
	//          **ocupa** a casa em que está: com 0,8 m de folga, qualquer tronco dentro do hexágono
	//          tira a casa da grade (o hexágono tem 1 m do centro à ponta).
	[Export]
	public float FolgaDaParede { get; set; } = 0.4f;

	[Export]
	public float FolgaDoObstaculo { get; set; } = 0.8f;

	// Alteração de IA - Revisar
	// O que faz: as camadas de física que contam como chão, como parede, como obstáculo e como
	//            bloqueio de visão (só parede).
	// Por quê: as mesmas camadas que a exploração já usa: Wall (8) e Obstacle (32, as árvores).
	[Export(PropertyHint.Layers3DPhysics)]
	public uint CamadaDoChao { get; set; } = 8;

	[Export(PropertyHint.Layers3DPhysics)]
	public uint CamadaDeParede { get; set; } = 8;

	[Export(PropertyHint.Layers3DPhysics)]
	public uint CamadaDeObstaculo { get; set; } = 32;

	public uint CamadaQueBloqueiaPassagem => CamadaDeParede | CamadaDeObstaculo;

	[Export(PropertyHint.Layers3DPhysics)]
	public uint CamadaQueBloqueiaVisao { get; set; } = 8;

	// Alteração de IA - Revisar
	// O que faz: de que altura descem as linhas que procuram o chão, e até onde vão.
	[Export]
	public float AlturaDeBusca { get; set; } = 30.0f;

	[Export]
	public float FundoDaBusca { get; set; } = -10.0f;

	public bool Pronta { get; private set; }
	public double TempoDeMontagemMs { get; private set; }
	public int TotalDeLigacoes { get; private set; }
	public IReadOnlyList<Casa> Casas => _casas;

	[Signal]
	public delegate void GradeProntaEventHandler();

	private readonly Dictionary<Hex, List<Casa>> _colunas = new();
	private readonly List<Casa> _casas = new();
	private static readonly List<Casa> Vazio = new();
	private int _quadrosEsperados;

	private CylinderShape3D? _folgaDaParede;
	private CylinderShape3D? _folgaDoObstaculo;
	private BoxShape3D? _formaDaPassagem;

	// Alteração de IA - Revisar
	// O que faz: acha a grade da fase atual.
	// Por quê: procurada pelo grupo, e não pelo caminho dos nós, para não depender de como a fase foi
	//          montada.
	public static GradeDeCombate? DaFase(SceneTree arvore)
	{
		foreach (Node no in arvore.GetNodesInGroup("grade_de_combate"))
		{
			if (no is GradeDeCombate grade && IsInstanceValid(grade))
			{
				return grade;
			}
		}
		return null;
	}

	public override void _EnterTree()
	{
		AddToGroup("grade_de_combate");
	}

	// Alteração de IA - Revisar
	// O que faz: monta a grade no segundo passo da física depois de a fase carregar.
	// Por quê: as perguntas ao mundo ("tem chão aqui?") só são seguras durante a física, e no
	//          primeiro passo as paredes podem ainda não ter sido registradas.
	public override void _PhysicsProcess(double delta)
	{
		if (Pronta)
		{
			SetPhysicsProcess(false);
			return;
		}

		_quadrosEsperados++;
		if (_quadrosEsperados < 2)
		{
			return;
		}

		Montar();
		SetPhysicsProcess(false);
		EmitSignal(SignalName.GradePronta);
	}

	// Alteração de IA - Revisar
	// O que faz: percorre a área inteira, hexágono por hexágono, e monta as casas e as ligações.
	// Por quê: ver o comentário da classe. Medido no sandbox: o tempo fica em TempoDeMontagemMs e
	//          aparece no registro do jogo.
	public void Montar()
	{
		var relogio = Stopwatch.StartNew();
		_colunas.Clear();
		_casas.Clear();
		TotalDeLigacoes = 0;

		var espaco = GetWorld3D().DirectSpaceState;
		_folgaDaParede = new CylinderShape3D { Radius = FolgaDaParede, Height = 1.2f };
		_folgaDoObstaculo = new CylinderShape3D { Radius = FolgaDoObstaculo, Height = 1.2f };
		_formaDaPassagem = new BoxShape3D { Size = new Vector3(0.5f, 1.0f, RaioDaCasa * Mathf.Sqrt(3.0f)) };

		float meiaLargura = Tamanho.X * 0.5f;
		float meiaProfundidade = Tamanho.Y * 0.5f;
		float passoQ = RaioDaCasa * 1.5f;
		float passoR = RaioDaCasa * Mathf.Sqrt(3.0f);
		Vector3 origem = GlobalPosition;

		int qMinimo = Mathf.FloorToInt(-meiaLargura / passoQ) - 1;
		int qMaximo = Mathf.CeilToInt(meiaLargura / passoQ) + 1;

		for (int q = qMinimo; q <= qMaximo; q++)
		{
			int rMinimo = Mathf.FloorToInt(-meiaProfundidade / passoR - q * 0.5f) - 1;
			int rMaximo = Mathf.CeilToInt(meiaProfundidade / passoR - q * 0.5f) + 1;

			for (int r = rMinimo; r <= rMaximo; r++)
			{
				var hex = new Hex(q, r);
				Vector2 plano = Hex.CentroNoPlano(hex, RaioDaCasa);
				if (Mathf.Abs(plano.X) > meiaLargura || Mathf.Abs(plano.Y) > meiaProfundidade)
				{
					continue;
				}

				float x = origem.X + plano.X;
				float z = origem.Z + plano.Y;
				List<float> chaos = AcharChaos(espaco, x, z);
				chaos.Sort();

				var coluna = new List<Casa>();
				foreach (float y in chaos)
				{
					if (!CasaValida(espaco, x, y, z))
					{
						continue;
					}

					var casa = new Casa(hex, coluna.Count, new Vector3(x, y, z));
					MedirPontas(espaco, casa);
					coluna.Add(casa);
					_casas.Add(casa);
				}

				if (coluna.Count > 0)
				{
					_colunas[hex] = coluna;
				}
			}
		}

		LigarVizinhas(espaco);

		relogio.Stop();
		TempoDeMontagemMs = relogio.Elapsed.TotalMilliseconds;
		Pronta = true;
		GD.Print($"[Grade] {_casas.Count} casas, {TotalDeLigacoes} ligações, montada em {TempoDeMontagemMs:0} ms");
	}

	// Alteração de IA - Revisar
	// O que faz: desce uma linha do alto até o fundo e anota cada chão que ela encontra no caminho.
	// Por quê: um mesmo ponto pode ter vários chãos empilhados (o telhado de um muro, a laje do andar
	//          de cima, o térreo). A linha ignora cada coisa já encontrada e desce de novo, até não
	//          achar mais nada. Só conta superfície virada para cima — a lateral de uma caixa não é chão.
	private List<float> AcharChaos(PhysicsDirectSpaceState3D espaco, float x, float z)
	{
		var chaos = new List<float>();
		var ignorar = new Godot.Collections.Array<Rid>();
		Vector3 topo = new(x, GlobalPosition.Y + AlturaDeBusca, z);
		Vector3 fundo = new(x, GlobalPosition.Y + FundoDaBusca, z);

		for (int i = 0; i < 6; i++)
		{
			var consulta = PhysicsRayQueryParameters3D.Create(topo, fundo, CamadaDoChao, ignorar);
			var resultado = espaco.IntersectRay(consulta);
			if (resultado.Count == 0)
			{
				break;
			}

			ignorar.Add((Rid)resultado["rid"]);
			Vector3 normal = (Vector3)resultado["normal"];
			if (normal.Y >= 0.7f)
			{
				chaos.Add(((Vector3)resultado["position"]).Y);
			}
		}
		return chaos;
	}

	// Alteração de IA - Revisar
	// O que faz: decide se dá para ficar em pé nesse chão: precisa de espaço livre acima (sem teto
	//            baixo) e de folga em volta do centro sem parede nem obstáculo.
	private bool CasaValida(PhysicsDirectSpaceState3D espaco, float x, float y, float z)
	{
		var teto = PhysicsRayQueryParameters3D.Create(
			new Vector3(x, y + 0.05f, z), new Vector3(x, y + AlturaLivreMinima, z),
			CamadaDoChao | CamadaQueBloqueiaPassagem);
		if (espaco.IntersectRay(teto).Count > 0)
		{
			return false;
		}

		var noCentro = new Transform3D(Basis.Identity, new Vector3(x, y + 0.75f, z));
		var parede = new PhysicsShapeQueryParameters3D { Shape = _folgaDaParede, Transform = noCentro, CollisionMask = CamadaDeParede };
		if (espaco.IntersectShape(parede, 1).Count > 0)
		{
			return false;
		}
		var obstaculo = new PhysicsShapeQueryParameters3D { Shape = _folgaDoObstaculo, Transform = noCentro, CollisionMask = CamadaDeObstaculo };
		return espaco.IntersectShape(obstaculo, 1).Count == 0;
	}

	// Alteração de IA - Revisar
	// O que faz: mede a altura do chão nas 6 pontas do hexágono.
	// Por quê: numa rampa, um hexágono reto ficaria meio enterrado e meio flutuando. Com as pontas
	//          medidas, o desenho se deita sobre o chão.
	private void MedirPontas(PhysicsDirectSpaceState3D espaco, Casa casa)
	{
		for (int i = 0; i < 6; i++)
		{
			Vector2 ponta = PontaNoPlano(i, 0.92f);
			Vector3 de = new(casa.Centro.X + ponta.X, casa.Centro.Y + 0.6f, casa.Centro.Z + ponta.Y);
			Vector3 ate = new(de.X, casa.Centro.Y - 0.6f, de.Z);
			var resultado = espaco.IntersectRay(PhysicsRayQueryParameters3D.Create(de, ate, CamadaDoChao));
			casa.AlturaDasPontas[i] = resultado.Count > 0 ? ((Vector3)resultado["position"]).Y : casa.Centro.Y;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: a posição de uma ponta do hexágono em relação ao centro (só X e Z).
	public Vector2 PontaNoPlano(int indice, float fracaoDoRaio)
	{
		float angulo = Mathf.DegToRad(60.0f * indice);
		return new Vector2(Mathf.Cos(angulo), Mathf.Sin(angulo)) * RaioDaCasa * fracaoDoRaio;
	}

	// Alteração de IA - Revisar
	// O que faz: liga cada casa às vizinhas para onde dá para andar.
	// Por quê: duas casas lado a lado nem sempre se ligam — pode haver um muro fino entre elas, um
	//          degrau alto demais (o andar de cima e o térreo) ou um buraco no meio.
	private void LigarVizinhas(PhysicsDirectSpaceState3D espaco)
	{
		foreach (Casa casa in _casas)
		{
			// só metade das direções: a outra metade é a mesma ligação vista do outro lado
			for (int direcao = 0; direcao < 3; direcao++)
			{
				foreach (Casa outra in Coluna(casa.Coordenada.Vizinho(direcao)))
				{
					float desnivel = Mathf.Abs(outra.Centro.Y - casa.Centro.Y);
					if (desnivel > AlturaMaximaNaRampa)
					{
						continue;
					}
					if (!PassagemLivre(espaco, casa, outra, exigirRampa: desnivel > AlturaMaximaDoDegrau))
					{
						continue;
					}

					casa.Vizinhas.Add(outra);
					outra.Vizinhas.Add(casa);
					TotalDeLigacoes++;
				}
			}
		}
	}

	// Alteração de IA - Revisar
	// O que faz: confere o caminho entre o centro de duas casas: nenhuma parede ou obstáculo na
	//            altura do corpo, e chão no meio do caminho. Quando o desnível passa do de um degrau,
	//            o chão do meio tem que estar na altura do meio — é rampa, e não um muro baixo.
	private bool PassagemLivre(PhysicsDirectSpaceState3D espaco, Casa a, Casa b, bool exigirRampa)
	{
		float chaoMaisAlto = Mathf.Max(a.Centro.Y, b.Centro.Y);
		float chaoMaisBaixo = Mathf.Min(a.Centro.Y, b.Centro.Y);
		Vector3 meio = (a.Centro + b.Centro) * 0.5f;
		Vector3 direcao = b.Centro - a.Centro;
		direcao.Y = 0.0f;

		// uma caixa do tamanho do caminho, começando 35 cm acima do chão mais alto
		var basePassagem = Basis.LookingAt(direcao.Normalized(), Vector3.Up);
		var passagem = new PhysicsShapeQueryParameters3D
		{
			Shape = _formaDaPassagem,
			Transform = new Transform3D(basePassagem, new Vector3(meio.X, chaoMaisAlto + 0.85f, meio.Z)),
			CollisionMask = CamadaQueBloqueiaPassagem,
		};
		if (espaco.IntersectShape(passagem, 1).Count > 0)
		{
			return false;
		}

		var chaoNoMeio = PhysicsRayQueryParameters3D.Create(
			new Vector3(meio.X, chaoMaisAlto + 0.5f, meio.Z),
			new Vector3(meio.X, chaoMaisBaixo - 0.4f, meio.Z),
			CamadaDoChao);
		var resultado = espaco.IntersectRay(chaoNoMeio);
		if (resultado.Count == 0)
		{
			return false;
		}
		if (!exigirRampa)
		{
			return true;
		}
		float alturaNoMeio = ((Vector3)resultado["position"]).Y;
		return Mathf.Abs(alturaNoMeio - (chaoMaisAlto + chaoMaisBaixo) * 0.5f) <= 0.2f;
	}

	// -------------------------------------------------------------------------
	// PERGUNTAS À GRADE
	// -------------------------------------------------------------------------

	public IReadOnlyList<Casa> Coluna(Hex hex) => _colunas.TryGetValue(hex, out var lista) ? lista : Vazio;

	public Hex HexDoPonto(Vector3 ponto) =>
		Hex.DoPonto(new Vector2(ponto.X - GlobalPosition.X, ponto.Z - GlobalPosition.Z), RaioDaCasa);

	// Alteração de IA - Revisar
	// O que faz: a casa em que um ponto do mapa está — o hexágono, e nele o andar mais perto da
	//            altura do ponto.
	public Casa? CasaEm(Vector3 ponto, float tolerancia = 1.2f)
	{
		Casa? melhor = null;
		float menor = float.MaxValue;
		foreach (Casa casa in Coluna(HexDoPonto(ponto)))
		{
			float diferenca = Mathf.Abs(casa.Centro.Y - ponto.Y);
			if (diferenca < menor)
			{
				menor = diferenca;
				melhor = casa;
			}
		}
		return menor <= tolerancia ? melhor : null;
	}

	// Alteração de IA - Revisar
	// O que faz: a casa mais perto de um ponto que satisfaça uma condição (estar livre, estar na
	//            área do combate...), procurando em volta até um raio.
	// Por quê: é como cada lutador ganha sua casa quando a luta começa — e como a formação contorna
	//          um obstáculo: se a casa prevista tem parede ou árvore, ele fica na mais próxima livre.
	public Casa? CasaMaisProxima(Vector3 ponto, Func<Casa, bool>? aceitar = null, int raioMaximo = 6)
	{
		Hex centro = HexDoPonto(ponto);
		Casa? melhor = null;
		float menor = float.MaxValue;

		for (int dq = -raioMaximo; dq <= raioMaximo; dq++)
		{
			for (int dr = Mathf.Max(-raioMaximo, -dq - raioMaximo); dr <= Mathf.Min(raioMaximo, -dq + raioMaximo); dr++)
			{
				foreach (Casa casa in Coluna(centro + new Hex(dq, dr)))
				{
					if (aceitar != null && !aceitar(casa))
					{
						continue;
					}

					Vector3 d = casa.Centro - ponto;
					// a altura pesa o dobro: preferir o mesmo andar a uma casa logo acima
					float distancia = new Vector3(d.X, d.Y * 2.0f, d.Z).Length();
					if (distancia < menor)
					{
						menor = distancia;
						melhor = casa;
					}
				}
			}
		}
		return melhor;
	}

	public static int Distancia(Casa a, Casa b) => Hex.Distancia(a.Coordenada, b.Coordenada);

	// Alteração de IA - Revisar
	// O que faz: a área do combate — as casas ligadas às casas de partida, até um raio do centro.
	// Por quê: a grade cobre o mapa inteiro, mas só o pedaço em volta da luta aparece e vale. Andar
	//          pelas ligações (e não só pela distância) deixa de fora o telhado de uma caixa, ou o
	//          outro lado de um muro sem passagem.
	public HashSet<Casa> Regiao(IEnumerable<Casa> partidas, Hex centro, int raio)
	{
		var regiao = new HashSet<Casa>();
		var fila = new Queue<Casa>();
		foreach (Casa partida in partidas)
		{
			if (regiao.Add(partida))
			{
				fila.Enqueue(partida);
			}
		}

		while (fila.Count > 0)
		{
			Casa atual = fila.Dequeue();
			foreach (Casa vizinha in atual.Vizinhas)
			{
				if (Hex.Distancia(vizinha.Coordenada, centro) > raio || !regiao.Add(vizinha))
				{
					continue;
				}
				fila.Enqueue(vizinha);
			}
		}
		return regiao;
	}

	// Alteração de IA - Revisar
	// O que faz: todas as casas que dá para alcançar andando até N casas, com quantos passos cada.
	// Por quê: é o que acende as casas azuis do movimento.
	public Dictionary<Casa, int> Alcancaveis(Casa origem, int passos, Func<Casa, bool> podePassar,
											 HashSet<Casa>? limite)
	{
		var distancias = new Dictionary<Casa, int> { [origem] = 0 };
		var fila = new Queue<Casa>();
		fila.Enqueue(origem);

		while (fila.Count > 0)
		{
			Casa atual = fila.Dequeue();
			int d = distancias[atual];
			if (d >= passos)
			{
				continue;
			}

			foreach (Casa vizinha in atual.Vizinhas)
			{
				if (distancias.ContainsKey(vizinha) || (limite != null && !limite.Contains(vizinha)) || !podePassar(vizinha))
				{
					continue;
				}
				distancias[vizinha] = d + 1;
				fila.Enqueue(vizinha);
			}
		}
		return distancias;
	}

	// Alteração de IA - Revisar
	// O que faz: o caminho mais curto entre duas casas, casa por casa (sem contar a de partida).
	//            Devolve null se não houver caminho.
	public List<Casa>? Caminho(Casa de, Casa para, Func<Casa, bool> podePassar, HashSet<Casa>? limite)
	{
		if (de == para)
		{
			return new List<Casa>();
		}

		var veioDe = new Dictionary<Casa, Casa>();
		var custo = new Dictionary<Casa, int> { [de] = 0 };
		var abertas = new PriorityQueue<Casa, int>();
		abertas.Enqueue(de, Distancia(de, para));

		while (abertas.Count > 0)
		{
			Casa atual = abertas.Dequeue();
			if (atual == para)
			{
				var caminho = new List<Casa>();
				for (Casa c = para; c != de; c = veioDe[c])
				{
					caminho.Add(c);
				}
				caminho.Reverse();
				return caminho;
			}

			foreach (Casa vizinha in atual.Vizinhas)
			{
				if (limite != null && !limite.Contains(vizinha))
				{
					continue;
				}
				if (vizinha != para && !podePassar(vizinha))
				{
					continue;
				}

				int novo = custo[atual] + 1;
				if (custo.TryGetValue(vizinha, out int velho) && velho <= novo)
				{
					continue;
				}
				custo[vizinha] = novo;
				veioDe[vizinha] = atual;
				abertas.Enqueue(vizinha, novo + Distancia(vizinha, para));
			}
		}
		return null;
	}

	// Alteração de IA - Revisar
	// O que faz: responde se há parede entre os olhos de quem está em duas casas.
	// Por quê: ataque à distância não atravessa parede. Árvore não bloqueia a visão (mesma regra da
	//          exploração: a camada Obstacle bloqueia passagem, não visão).
	public bool TemLinhaDeVisao(Casa a, Casa b, float alturaDosOlhos = 1.5f)
	{
		var espaco = GetWorld3D().DirectSpaceState;
		var consulta = PhysicsRayQueryParameters3D.Create(
			a.Centro + Vector3.Up * alturaDosOlhos, b.Centro + Vector3.Up * alturaDosOlhos, CamadaQueBloqueiaVisao);
		return espaco.IntersectRay(consulta).Count == 0;
	}
}

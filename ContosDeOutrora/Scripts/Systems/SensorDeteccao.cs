using Godot;

// Alteração de IA - Revisar
// O que faz: guarda os "raios" de percepção de um personagem — as áreas invisíveis em volta
//            dele que definem quem percebe quem, e a que distância.
// Por quê: tanto o jogador quanto os inimigos precisam disso, e cada um usa de um jeito.
//          Ter um componente só, usado pelos dois, evita escrever a mesma conta duas vezes
//          e garante que os dois lados enxerguem o mundo pelas mesmas regras.
//
//          Como funciona a percepção: os raios são círculos, e o que importa é se dois
//          círculos **se tocam** — não se um está dentro do outro. Por isso a distância
//          comparada é sempre a soma dos dois raios. Isso faz a Furtividade do jogador
//          valer de verdade: encolher o círculo dele reduz o alcance de quem o procura.
public partial class SensorDeteccao : Node3D
{
	// Alteração de IA - Revisar
	// O que faz: o raio externo, o maior deles.
	// Por quê: é o primeiro contato. Quando o círculo externo do inimigo toca o círculo
	//          externo do jogador, o inimigo *desconfia* que tem alguém por perto — mas
	//          ainda não sabe onde. É o que dispara o modo de busca.
	[Export]
	public float RaioDeteccao1 { get; set; } = 6.0f;

	// Alteração de IA - Revisar
	// O que faz: o raio interno. Só o jogador usa.
	// Por quê: é a "zona de certeza". Se o círculo externo do inimigo alcança este círculo
	//          menor do jogador, o inimigo não desconfia — ele **viu**, e parte direto para
	//          a perseguição. Os inimigos não têm este raio, por decisão de design.
	//          Ele é sempre metade do externo (ver AplicarProporcao abaixo).
	[Export]
	public float RaioDeteccao2 { get; set; } = 3.0f;

	// Alteração de IA - Revisar
	// O que faz: a distância em que o combate começa. Só os inimigos usam.
	// Por quê: representa o alcance da arma. Um inimigo de arco começa o combate de longe;
	//          um de espada precisa chegar perto. Por isso o valor muda de inimigo para
	//          inimigo, e não é uma constante do jogo.
	[Export]
	public float RaioEncontro { get; set; } = 2.0f;

	// Alteração de IA - Revisar
	// O que faz: até onde a visão alcança, e o quanto ela é aberta (em graus).
	// Por quê: a visão é um cone na direção em que o personagem está olhando, não um
	//          círculo. Alcança mais longe que o raio externo justamente por ser focada
	//          numa direção só — de frente se enxerga longe, de lado não se enxerga nada.
	[Export]
	public float AlcanceVisao { get; set; } = 12.0f;

	[Export]
	public float AberturaVisao { get; set; } = 80.0f;

	// Alteração de IA - Revisar
	// O que faz: divide o cone de visão em três faixas de qualidade, da mais perto para a mais
	//            longe. O `AlcanceVisao` acima passa a ser o alcance **total** (a faixa 3).
	// Por quê: enxergar não é sim ou não. De perto se vê com clareza; mais longe se distingue
	//          menos; no limite do alcance só se percebe que **tem alguma coisa ali**, sem saber
	//          o quê. As frações dizem onde cada faixa termina, em proporção do alcance total —
	//          assim, mudar o alcance total reorganiza as três juntas, sem contas à mão.
	//
	//          **Nível 1 é o mais perto e o mais claro**, seguindo a numeração que a equipe
	//          combinou para o cone. Repare que é o contrário dos raios de detecção, onde o
	//          nível 1 é o de fora — lá o número cresce para fora, aqui cresce para longe.
	[Export]
	public float FracaoDaVisao1 { get; set; } = 0.38f;

	[Export]
	public float FracaoDaVisao2 { get; set; } = 0.70f;

	// Alteração de IA - Revisar
	// O que faz: o que o personagem enxerga em volta de si **sem precisar olhar**, em duas faixas.
	// Por quê: ninguém precisa virar a cabeça para saber o que está encostado nele. A faixa 1 é
	//          clara e a 2 é levemente embaçada — é o que impede a névoa de virar uma coleira,
	//          deixando o jogador cego para os próprios pés.
	//
	//          **São números próprios, separados dos raios de detecção**, por dois motivos: ver
	//          e ser visto são coisas diferentes, e os raios de detecção encolhem ao agachar —
	//          se fossem os mesmos, agachar cegaria o jogador, o contrário do que foi combinado.
	[Export]
	public float VisaoPassiva1 { get; set; } = 3.5f;

	[Export]
	public float VisaoPassiva2 { get; set; } = 6.0f;

	// Alteração de IA - Revisar
	// O que faz: altura dos olhos, a partir do chão.
	// Por quê: a checagem de parede é feita por um "raio de luz" entre os dois personagens.
	//          Se saísse dos pés, qualquer degrau bloquearia a visão. Na altura dos olhos,
	//          o resultado corresponde ao que faz sentido visualmente.
	[Export]
	public float AlturaDosOlhos { get; set; } = 1.5f;

	// Alteração de IA - Revisar
	// O que faz: quais camadas de colisão bloqueiam a visão. O valor 8 é a camada "Parede".
	// Por quê: fica configurável porque nem tudo que é sólido deve bloquear a visão — uma
	//          grade ou uma mureta baixa podem deixar ver através. Basta tirar a camada dela
	//          desta conta.
	//          É um número inteiro comum (e não um tipo especial) porque só assim os testes
	//          automatizados conseguem chamar este método de fora do C#.
	[Export]
	public int CamadaQueBloqueiaVisao { get; set; } = 8;

	// Alteração de IA - Revisar
	// O que faz: Furtividade encolhe os próprios raios; Reação aumenta.
	// Por quê: é o que liga os atributos da ficha ao jogo de verdade.
	//          **Furtividade** é do jogador: quanto maior, menores ficam os círculos dele,
	//          então o inimigo precisa chegar mais perto para perceber qualquer coisa.
	//          **Reação** é do inimigo: quanto maior, maior o círculo dele, então percebe
	//          de mais longe.
	//          Cada ponto muda 5% (valor de partida, a equipe ajusta testando). O limite
	//          de 70% existe para um personagem muito furtivo não virar invisível.
	[Export]
	public int Furtividade { get; set; } = 0;

	[Export]
	public int Reacao { get; set; } = 0;

	[Export]
	public float EfeitoPorPonto { get; set; } = 0.05f;

	[Export]
	public float EfeitoMaximo { get; set; } = 0.70f;

	// Alteração de IA - Revisar
	// O que faz: quanto tempo o personagem precisa ficar dentro do cone de visão até ser
	//            notado, em segundos.
	// Por quê: ser visto não é instantâneo — passar correndo na frente de alguém distraído
	//          é diferente de ficar parado no campo de visão dele. A Furtividade **aumenta**
	//          esse tempo. A Reação do inimigo, por decisão de design, **não diminui**.
	[Export]
	public float TempoParaSerVisto { get; set; } = 1.2f;

	// Alteração de IA - Revisar
	// O que faz: diz se o personagem está agachado neste momento.
	// Por quê: agachar é a forma ativa de ser furtivo — a Furtividade da ficha é passiva e vale
	//          sempre, enquanto isto o jogador liga e desliga na hora. Fica no sensor, e não no
	//          script do jogador, porque é aqui que estão todas as contas de quem percebe quem.
	//          Assim um inimigo furtivo também pode se agachar sem nenhum código novo.
	[Export]
	public bool Agachado { get; set; }

	// Alteração de IA - Revisar
	// O que faz: o quanto os círculos de detecção encolhem enquanto ele está agachado.
	// Por quê: é o primeiro dos dois efeitos de agachar. Em 0,55 os círculos ficam pouco mais
	//          da metade do tamanho, então o inimigo precisa chegar bem mais perto para começar
	//          a desconfiar. Some com a Furtividade da ficha: um personagem furtivo agachado
	//          encolhe duas vezes.
	[Export]
	public float FatorDosRaiosAgachado { get; set; } = 0.55f;

	// Alteração de IA - Revisar
	// O que faz: o quanto aumenta o tempo necessário para ser reconhecido, estando agachado.
	// Por quê: é o segundo efeito, e vale **mesmo dentro do campo de visão do inimigo**. Estar
	//          agachado não torna ninguém invisível: se o inimigo olhar direto, ele acaba vendo.
	//          Mas demora quase o dobro, e é essa demora que dá tempo de sair da linha de visão.
	[Export]
	public float AumentoDoTempoAgachado { get; set; } = 1.8f;

	// Alteração de IA - Revisar
	// O que faz: a altura dos olhos enquanto agachado.
	// Por quê: agachado, a linha entre os olhos dos dois passa mais baixo. Hoje não muda nada,
	//          porque as paredes da sala de teste vão do chão ao teto — mas é o que vai permitir
	//          se esconder atrás de mureta, caixa ou parapeito quando o cenário tiver disso.
	[Export]
	public float AlturaDosOlhosAgachado { get; set; } = 0.8f;

	// ---------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: devolve o raio externo já com o efeito dos atributos aplicado.
	// Por quê: é este valor que as contas usam, nunca o valor bruto. Furtividade encolhe,
	//          Reação aumenta — e um personagem pode ter os dois (um inimigo furtivo, por
	//          exemplo), então os dois efeitos entram na mesma conta.
	public float Raio1Efetivo => RaioDeteccao1 * FatorDeAlcance() * FatorDeAgachar();

	public float Raio2Efetivo => RaioDeteccao2 * FatorDeAlcance() * FatorDeAgachar();

	// Alteração de IA - Revisar
	// O que faz: o alcance da própria visão **não** encolhe ao agachar.
	// Por quê: agachar esconde o personagem, não cega ele. Quem se abaixa continua enxergando
	//          o mesmo tanto à frente — só fica mais difícil de ser visto. Por isso este é o
	//          único dos três que não leva o fator de agachar.
	public float AlcanceVisaoEfetivo => AlcanceVisao * FatorDeAlcance();

	// Alteração de IA - Revisar
	// O que faz: devolve o quanto os círculos encolhem agora, por causa de agachar.
	// Por quê: separado do fator da ficha para os dois poderem se somar sem se confundir —
	//          a Furtividade vale sempre, agachar vale só enquanto estiver agachado.
	private float FatorDeAgachar() => Agachado ? Mathf.Max(0.05f, FatorDosRaiosAgachado) : 1.0f;

	// Alteração de IA - Revisar
	// O que faz: o tempo até ser notado, aumentado pela Furtividade.
	// Por quê: separado do fator de alcance porque aqui a Reação **não** entra — foi
	//          decisão de design que a Reação do inimigo não acelera o reconhecimento
	//          visual, só aumenta o alcance dos círculos.
	public float TempoParaSerVistoEfetivo
	{
		get
		{
			float bonus = Mathf.Min(Furtividade * EfeitoPorPonto, EfeitoMaximo);
			float agachado = Agachado ? Mathf.Max(1.0f, AumentoDoTempoAgachado) : 1.0f;
			return TempoParaSerVisto * (1.0f + bonus) * agachado;
		}
	}

	private float FatorDeAlcance()
	{
		float reducao = Mathf.Min(Furtividade * EfeitoPorPonto, EfeitoMaximo);
		float aumento = Mathf.Min(Reacao * EfeitoPorPonto, EfeitoMaximo);
		return Mathf.Max(0.1f, 1.0f - reducao + aumento);
	}

	// Alteração de IA - Revisar
	// O que faz: mantém o raio interno sempre valendo metade do externo.
	// Por quê: foi assim que a equipe definiu. Deixar os dois soltos abriria espaço para
	//          alguém configurar um interno maior que o externo, o que não faria sentido.
	public void AplicarProporcao()
	{
		RaioDeteccao2 = RaioDeteccao1 * 0.5f;
	}

	public Vector3 PosicaoDosOlhos =>
		GlobalPosition + new Vector3(0.0f, Agachado ? AlturaDosOlhosAgachado : AlturaDosOlhos, 0.0f);

	// Alteração de IA - Revisar
	// O que faz: responde se os círculos externos dos dois personagens estão se tocando.
	// Por quê: é a conta do "primeiro contato". Somar os dois raios é o que faz a
	//          Furtividade do jogador e a Reação do inimigo pesarem na mesma balança.
	public bool CirculosExternosSeTocam(SensorDeteccao outro)
	{
		float distancia = GlobalPosition.DistanceTo(outro.GlobalPosition);
		return distancia <= Raio1Efetivo + outro.Raio1Efetivo;
	}

	// Alteração de IA - Revisar
	// O que faz: responde se o círculo externo deste personagem alcança o círculo **interno**
	//            do outro.
	// Por quê: é a conta da "certeza". Chegar nessa distância significa que não há mais
	//          dúvida — o inimigo sabe exatamente onde o jogador está.
	public bool AlcancaCirculoInterno(SensorDeteccao outro)
	{
		float distancia = GlobalPosition.DistanceTo(outro.GlobalPosition);
		return distancia <= Raio1Efetivo + outro.Raio2Efetivo;
	}

	public bool DentroDoRaioDeEncontro(SensorDeteccao outro)
	{
		float distancia = GlobalPosition.DistanceTo(outro.GlobalPosition);
		return distancia <= RaioEncontro + outro.Raio2Efetivo;
	}

	// Alteração de IA - Revisar
	// O que faz: responde se o outro personagem está dentro do cone de visão — ou seja,
	//            na frente, dentro da abertura, e perto o bastante.
	// Por quê: estar dentro do cone ainda não é ser visto. Só conta junto com o tempo
	//          (ver TempoParaSerVistoEfetivo) e com a checagem de parede.
	public bool DentroDoConeDeVisao(SensorDeteccao outro, float direcaoOlhandoGraus)
	{
		Vector3 ateOutro = outro.GlobalPosition - GlobalPosition;
		ateOutro.Y = 0.0f;

		float distancia = ateOutro.Length();
		if (distancia > AlcanceVisaoEfetivo + outro.Raio2Efetivo)
		{
			return false;
		}
		if (distancia < 0.01f)
		{
			return true;
		}

		float rad = Mathf.DegToRad(direcaoOlhandoGraus);
		Vector3 frente = new Vector3(Mathf.Cos(rad), 0.0f, -Mathf.Sin(rad));

		float anguloAteOutro = Mathf.RadToDeg(frente.AngleTo(ateOutro.Normalized()));
		return anguloAteOutro <= AberturaVisao * 0.5f;
	}

	// Alteração de IA - Revisar
	// O que faz: responde se existe parede entre este personagem e o outro.
	// Por quê: **é o que impede o inimigo de enxergar através de um muro.** Sem isso, ele
	//          perseguiria o jogador do outro lado da parede, o que quebraria o jogo de
	//          furtividade inteiro.
	//
	//          Funciona como um "raio de luz" entre os olhos dos dois: se ele esbarra em
	//          algo da camada Parede antes de chegar, a visão está bloqueada.
	// Alteração de IA - Revisar
	// O que faz: os alcances de cada faixa do cone, já em metros.
	// Por quê: o desenho de teste e a névoa precisam desses três números prontos; calcular a
	//          fração em cada um deles seria repetir a mesma conta em lugares diferentes, com
	//          risco de um sair do passo do outro.
	public float AlcanceVisao1 => AlcanceVisaoEfetivo * Mathf.Clamp(FracaoDaVisao1, 0.0f, 1.0f);
	public float AlcanceVisao2 => AlcanceVisaoEfetivo * Mathf.Clamp(FracaoDaVisao2, 0.0f, 1.0f);
	public float AlcanceVisao3 => AlcanceVisaoEfetivo;

	// Alteração de IA - Revisar
	// O que faz: responde o quanto este personagem enxerga um ponto qualquer do mapa, numa escala
	//            de 0 a 3 — 0 é névoa (não vê nada), 1 é claro, 2 é levemente embaçado, 3 é só
	//            um vulto.
	// Por quê: é a mesma regra usada pela névoa na tela e pelos inimigos para decidirem como
	//          aparecer. Tendo os dois a mesma fonte, não dá para o desenho mostrar uma coisa e
	//          a mecânica valer outra.
	//
	//          Vale o melhor entre o que ele enxerga em volta sem olhar (passiva) e o que o cone
	//          alcança na direção em que está olhando.
	public int NivelDeVisaoDe(Vector3 ponto, float direcaoOlhandoGraus)
	{
		Vector3 ate = ponto - GlobalPosition;
		ate.Y = 0.0f;
		float distancia = ate.Length();

		int porPerto = 0;
		if (distancia <= VisaoPassiva1)
		{
			porPerto = 1;
		}
		else if (distancia <= VisaoPassiva2)
		{
			porPerto = 2;
		}

		int porOlhar = 0;
		if (distancia <= AlcanceVisao3 && distancia > 0.001f)
		{
			float rad = Mathf.DegToRad(direcaoOlhandoGraus);
			var frente = new Vector3(Mathf.Cos(rad), 0.0f, -Mathf.Sin(rad));
			float angulo = Mathf.RadToDeg(frente.AngleTo(ate.Normalized()));

			if (angulo <= AberturaVisao * 0.5f)
			{
				if (distancia <= AlcanceVisao1)
				{
					porOlhar = 1;
				}
				else if (distancia <= AlcanceVisao2)
				{
					porOlhar = 2;
				}
				else
				{
					porOlhar = 3;
				}
			}
		}

		// 0 significa "não vê"; entre os que veem, vale o menor número, que é o mais nítido
		if (porPerto == 0)
		{
			return porOlhar;
		}
		if (porOlhar == 0)
		{
			return porPerto;
		}
		return Mathf.Min(porPerto, porOlhar);
	}

	// Alteração de IA - Revisar
	// O que faz: responde se este personagem enxerga um ponto qualquer **de verdade** — dentro
	//            do alcance, na direção certa e sem parede no meio.
	// Por quê: o `NivelDeVisaoDe` acima só olha distância e ângulo. Esta versão fecha a conta
	//          com a parede, e é a pergunta que a memória precisa fazer: "estou olhando para
	//          onde lembro de ter visto o inimigo, e não tem ninguém lá?".
	public bool EnxergaOPonto(Vector3 ponto, float direcaoOlhandoGraus)
	{
		if (NivelDeVisaoDe(ponto, direcaoOlhandoGraus) == 0)
		{
			return false;
		}

		var espaco = GetWorld3D().DirectSpaceState;
		var consulta = PhysicsRayQueryParameters3D.Create(PosicaoDosOlhos,
														 ponto + new Vector3(0.0f, 0.9f, 0.0f));
		consulta.CollisionMask = (uint)CamadaQueBloqueiaVisao;
		consulta.HitFromInside = false;

		return espaco.IntersectRay(consulta).Count == 0;
	}

	public bool TemLinhaDeVisao(SensorDeteccao outro)
	{
		var espaco = GetWorld3D().DirectSpaceState;
		var consulta = PhysicsRayQueryParameters3D.Create(PosicaoDosOlhos, outro.PosicaoDosOlhos);
		consulta.CollisionMask = (uint)CamadaQueBloqueiaVisao;
		consulta.HitFromInside = false;

		var resultado = espaco.IntersectRay(consulta);
		return resultado.Count == 0;
	}
}

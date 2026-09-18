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

	// ---------------------------------------------------------------------------

	// Alteração de IA - Revisar
	// O que faz: devolve o raio externo já com o efeito dos atributos aplicado.
	// Por quê: é este valor que as contas usam, nunca o valor bruto. Furtividade encolhe,
	//          Reação aumenta — e um personagem pode ter os dois (um inimigo furtivo, por
	//          exemplo), então os dois efeitos entram na mesma conta.
	public float Raio1Efetivo => RaioDeteccao1 * FatorDeAlcance();

	public float Raio2Efetivo => RaioDeteccao2 * FatorDeAlcance();

	public float AlcanceVisaoEfetivo => AlcanceVisao * FatorDeAlcance();

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
			return TempoParaSerVisto * (1.0f + bonus);
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

	public Vector3 PosicaoDosOlhos => GlobalPosition + new Vector3(0.0f, AlturaDosOlhos, 0.0f);

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

using Godot;

// Alteração de IA - Revisar
// O que faz: guarda o quanto este inimigo já desconfia do jogador, numa escala de 0% a 100%.
// Por quê: antes a percepção era um interruptor — no instante em que os círculos se tocavam o
//          inimigo já parava e olhava em volta. Isso abria uma brecha ruim: bastava o jogador
//          entrar e sair do alcance de propósito para o inimigo ficar preso, parando e olhando
//          para sempre, sem nunca voltar à ronda. Um bicho travado assim não é um desafio, é um
//          defeito que o jogador aprende a explorar.
//
//          Com uma barra que sobe e desce, o entra-e-sai deixa de travar: a suspeita se acumula
//          no mesmo lugar em vez de reiniciar a reação. E ela serve para uma segunda coisa,
//          igualmente importante: **quanto mais desconfiado ele já está, mais rápido reconhece
//          o jogador quando enfim o vê.** Quem já ouviu barulho não precisa de um olhar demorado
//          para confirmar; quem estava tranquilo precisa.
//
//          A regra é uma só: a barra caminha sempre em direção ao teto daquilo que ele está
//          percebendo **agora** — sobe rápido, desce devagar. Cada forma de perceber tem o seu
//          teto, e é isso que separa "ouvi alguma coisa" de "estou vendo alguém".
public partial class MedidorDeSuspeita : Node
{
	// Alteração de IA - Revisar
	// O que faz: de onde está vindo a suspeita neste instante.
	// Por quê: não basta saber **quanto** ele desconfia, é preciso saber **por quê** — porque a
	//          reação é outra. Quem só ouviu não sabe onde o jogador está e precisa procurar;
	//          quem está vendo já sabe, e virar a cabeça para os lados seria bobagem.
	public enum Fonte
	{
		Nada,
		Ouvindo,   // os círculos externos se tocam: ouviu algo, não sabe o que é
		Perto,     // está quase esbarrando: não há dúvida de que tem alguém
		Vendo      // o jogador está dentro do cone de visão
	}

	// Alteração de IA - Revisar
	// O que faz: a porcentagem em que aparece o "?" e o inimigo para para olhar em volta.
	// Por quê: um valor baixo, mas não zero. Zero faria a reação disparar no primeiro roçar dos
	//          círculos — que é exatamente o problema que esta barra veio resolver. Em 10% o
	//          jogador precisa ficar perto por um instante para ser notado, e passar correndo de
	//          raspão não desperta ninguém.
	[Export]
	public float LimiteParaEstranhar { get; set; } = 10.0f;

	// Alteração de IA - Revisar
	// O que faz: o quanto a barra sobe por segundo enquanto ele só **ouve** o jogador (os
	//            círculos externos dos dois se tocando), e até onde ela pode chegar por essa via.
	// Por quê: som não identifica ninguém. Por mais tempo que o jogador fique roçando o alcance
	//          auditivo, o inimigo nunca passa de desconfiado — **por ouvir, a barra para na
	//          metade.** Para completar, ele precisa chegar perto ou enxergar de fato. É o que
	//          garante que dá para atravessar uma sala sem ser pego, desde que não se entre no
	//          campo de visão dele.
	[Export]
	public float VelocidadeAoOuvir { get; set; } = 20.0f;

	[Export]
	public float TetoAoOuvir { get; set; } = 50.0f;

	// Alteração de IA - Revisar
	// O que faz: o quanto a barra sobe por segundo quando o jogador está perto o bastante para
	//            não haver dúvida (o alcance do inimigo chega ao círculo interno do jogador).
	// Por quê: a essa distância não é mais questão de ouvir — ele praticamente esbarra na pessoa.
	//          Sobe rápido e vai até 100%, mas ainda **não é instantâneo**: sobra uma fração de
	//          segundo para sair dali.
	[Export]
	public float VelocidadeAoChegarPerto { get; set; } = 70.0f;

	// Alteração de IA - Revisar
	// O que faz: quanto tempo ele leva sem perceber nada antes de a barra começar a baixar, e o
	//            quanto ela baixa por segundo.
	// Por quê: a barra desce bem mais devagar do que sobe, de propósito. Desconfiar é rápido,
	//          esquecer é lento — é assim com gente, e é o que impede o jogador de zerar a
	//          suspeita saindo do alcance por um segundo. A espera existe para a barra não
	//          começar a cair a cada piscada da linha de visão.
	[Export]
	public float EsperaParaEsquecer { get; set; } = 1.5f;

	[Export]
	public float VelocidadeDeEsquecer { get; set; } = 8.0f;

	// Alteração de IA - Revisar
	// O que faz: o quanto a suspeita acumulada acelera a própria subida.
	// Por quê: **é o "ele já estava desconfiado" virando jogo.** Em 1.0, um inimigo com a barra
	//          na metade reconhece o jogador em uma vez e meia a velocidade normal. Em 0, a barra
	//          sobe sempre no mesmo ritmo e o inimigo não aproveita nada do que ouviu antes.
	[Export]
	public float AceleracaoPelaSuspeita { get; set; } = 1.0f;

	// ---------------------------------------------------------------------------

	public float Porcentagem { get; private set; }

	// Alteração de IA - Revisar
	// O que faz: avisam, no exato quadro em que acontece, que a barra cruzou o limite do "?" ou
	//            chegou aos 100%.
	// Por quê: são **avisos de uma vez só**. Enquanto a barra continuar alta, o aviso não se
	//          repete — ele só volta a valer depois que ela cai de novo. Sem isso, o inimigo
	//          recomeçaria a reação de espanto a cada quadro em que estivesse acima de 10%, que
	//          é justamente a trava que estamos tentando evitar.
	public bool AcabouDeEstranhar { get; private set; }
	public bool AcabouDeDetectar { get; private set; }

	public Fonte FonteAtual { get; private set; } = Fonte.Nada;

	// Alteração de IA - Revisar
	// O que faz: a fonte da suspeita escrita em palavras.
	// Por quê: só para o desenho de teste. Ver "45% ouvindo" e "45% vendo" na tela é o que
	//          permite conferir se cada forma de perceber está pesando o que deveria.
	public string FonteAtualTexto => FonteAtual switch
	{
		Fonte.Ouvindo => "ouvindo",
		Fonte.Perto => "perto",
		Fonte.Vendo => "vendo",
		_ => ""
	};

	private bool _jaAvisouQueEstranhou;
	private bool _jaAvisouQueDetectou;
	private float _esperaRestante;

	// Alteração de IA - Revisar
	// O que faz: recebe o que o inimigo está percebendo neste instante e move a barra.
	// Por quê: quem decide *o que* é percebido é o sensor; o medidor só cuida do *quanto*.
	//          Separar as duas coisas deixa a regra de acumulação num lugar só, em vez de
	//          espalhada pelo cérebro do inimigo.
	//
	//          O tempo para ser visto vem do jogador e já traz a Furtividade dele embutida: é
	//          quantos segundos de cone de visão cheio levariam a barra de 0 a 100. Por isso a
	//          Furtividade continua valendo aqui, e não só no tamanho dos círculos.
	public void Atualizar(bool ouviu, bool estaPerto, bool vendo, float tempoParaSerVisto, float dt)
	{
		AcabouDeEstranhar = false;
		AcabouDeDetectar = false;

		// Alteração de IA - Revisar
		// O que faz: escolhe a forma de perceber mais forte entre as que valem agora.
		// Por quê: várias podem valer ao mesmo tempo — estar perto **e** dentro do cone, por
		//          exemplo. Vale a de maior teto; havendo empate no teto, a mais rápida. Compara
		//          pelo teto primeiro justamente para que ninguém, ao ajustar os números, consiga
		//          fazer "ouvir" atropelar "ver" e prender a barra na metade com o jogador parado
		//          na frente do inimigo.
		float teto = 0.0f;
		float velocidade = 0.0f;
		FonteAtual = Fonte.Nada;

		if (ouviu)
		{
			Considerar(TetoAoOuvir, VelocidadeAoOuvir, Fonte.Ouvindo, ref teto, ref velocidade);
		}
		if (estaPerto)
		{
			Considerar(100.0f, VelocidadeAoChegarPerto, Fonte.Perto, ref teto, ref velocidade);
		}
		if (vendo)
		{
			float porSegundo = 100.0f / Mathf.Max(0.05f, tempoParaSerVisto);
			Considerar(100.0f, porSegundo, Fonte.Vendo, ref teto, ref velocidade);
		}

		if (Porcentagem < teto)
		{
			float impulso = 1.0f + (Porcentagem / 100.0f) * AceleracaoPelaSuspeita;
			Porcentagem = Mathf.Min(teto, Porcentagem + velocidade * impulso * dt);
			_esperaRestante = EsperaParaEsquecer;
		}
		else if (Porcentagem > teto)
		{
			// Alteração de IA - Revisar
			// O que faz: a barra cai em direção ao teto do que ele percebe agora — não direto
			//            para zero.
			// Por quê: é o que dá o "ele ainda sabe que tem alguém aí". Um inimigo que viu o
			//          jogador (barra alta) e o perdeu de vista, mas continua ouvindo, desce só
			//          até os 50% do ouvido e para ali. Para a barra zerar mesmo, o jogador
			//          precisa sair inteiramente do alcance dele.
			_esperaRestante -= dt;
			if (_esperaRestante <= 0.0f)
			{
				Porcentagem = Mathf.Max(teto, Porcentagem - VelocidadeDeEsquecer * dt);
			}
		}
		else
		{
			_esperaRestante = EsperaParaEsquecer;
		}

		AtualizarAvisos();
	}

	private void Considerar(float tetoDaFonte, float velocidadeDaFonte, Fonte fonte,
							ref float teto, ref float velocidade)
	{
		bool ganhou = tetoDaFonte > teto ||
					  (Mathf.IsEqualApprox(tetoDaFonte, teto) && velocidadeDaFonte > velocidade);
		if (!ganhou)
		{
			return;
		}

		teto = tetoDaFonte;
		velocidade = velocidadeDaFonte;
		FonteAtual = fonte;
	}

	// Alteração de IA - Revisar
	// O que faz: dispara os avisos ao cruzar os limites para cima, e os rearma quando a barra cai.
	// Por quê: **é aqui que mora a proteção contra travar o inimigo.** O aviso do "?" só volta a
	//          existir depois que a barra cai abaixo dos 10% de novo — ou seja, depois que ele
	//          realmente se acalmou. Entrar e sair do alcance mantém a barra lá em cima, então a
	//          reação de espanto não se repete: ele segue a ronda desconfiado, em vez de ficar
	//          parado olhando em volta sem parar.
	private void AtualizarAvisos()
	{
		if (Porcentagem >= LimiteParaEstranhar)
		{
			if (!_jaAvisouQueEstranhou)
			{
				_jaAvisouQueEstranhou = true;
				AcabouDeEstranhar = true;
			}
		}
		else
		{
			_jaAvisouQueEstranhou = false;
		}

		if (Porcentagem >= 99.999f)
		{
			if (!_jaAvisouQueDetectou)
			{
				_jaAvisouQueDetectou = true;
				AcabouDeDetectar = true;
			}
		}
		else
		{
			_jaAvisouQueDetectou = false;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: zera a barra à força.
	// Por quê: serve para recomeçar uma situação de teste do zero e para quando o combate
	//          terminar e o inimigo precisar voltar ao estado tranquilo.
	public void Zerar()
	{
		Porcentagem = 0.0f;
		_esperaRestante = EsperaParaEsquecer;
		_jaAvisouQueEstranhou = false;
		_jaAvisouQueDetectou = false;
		FonteAtual = Fonte.Nada;
	}
}

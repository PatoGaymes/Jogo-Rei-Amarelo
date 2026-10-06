using Godot;
using System.Collections.Generic;
using System.Linq;

// Alteração de IA - Revisar
// O que faz: os dois lados da luta.
public enum LadoDoCombate
{
	Aliados,
	Inimigos,
}

// Alteração de IA - Revisar
// O que faz: um efeito grudado num lutador — qual é, quantos acúmulos, quanto tempo falta e quem
//            aplicou.
// Por quê: "quem aplicou" importa para a Vanguarda: o Protegido precisa saber para quem mandar o
//          dano. As habilidades bloqueadas guardam o que o Raio travou.
public sealed class EfeitoAtivo
{
	public TipoDeEfeito Tipo { get; init; }
	public int Acumulos { get; set; } = 1;

	// rodadas que faltam, contadas no começo do turno de quem tem o efeito; -1 = sem prazo
	public int RodadasRestantes { get; set; } = -1;

	public Combatente? Origem { get; set; }

	public List<Habilidade> Bloqueadas { get; } = new();

	// some no primeiro ataque (a vantagem de quem atacou primeiro num diálogo)
	public bool SomeNoPrimeiroAtaque { get; set; }
}

// Alteração de IA - Revisar
// O que faz: o estado de um lutador durante um combate — vida, Foco, sanidade, casa onde está,
//            efeitos, iniciativa, quanto ainda pode andar e se ainda tem a ação do turno.
// Por quê: a ficha (.tres) diz como o personagem é; isto diz como ele está **agora**. Separar os
//          dois impede que um combate estrague os dados do game designer — o veneno de hoje não
//          pode ir parar no arquivo da ficha.
public sealed class Combatente
{
	public Combatente(FichaDeCombatente ficha, Node3D corpo, LadoDoCombate lado)
	{
		Ficha = ficha;
		Corpo = corpo;
		Lado = lado;
		Vida = ficha.Vida;
		Foco = ficha.Foco;
		Sanidade = ficha.Sanidade;
	}

	public FichaDeCombatente Ficha { get; }
	public Node3D Corpo { get; }
	public LadoDoCombate Lado { get; }

	public string Nome => string.IsNullOrEmpty(Ficha.Nome) ? Corpo.Name.ToString() : Ficha.Nome;

	public int Vida { get; set; }
	public int Foco { get; set; }
	public int Sanidade { get; set; }
	public Casa? Casa { get; set; }
	public List<EfeitoAtivo> Efeitos { get; } = new();

	// Alteração de IA - Revisar
	// O que faz: a iniciativa tirada no começo do combate — o dado de 20 lados e o total com o
	//            atributo — e quantos turnos seguidos o lutador tem em cada rodada.
	// Por quê: quem tira 20 no dado joga 2 turnos seguidos sempre que chega a vez dele
	//          (decisão do PO em 06/10/2026).
	public int DadoDeIniciativa { get; set; }
	public int ValorDeIniciativa { get; set; }
	public int TurnosPorRodada { get; set; } = 1;

	// Alteração de IA - Revisar
	// O que faz: o que sobra do turno — casas que ainda pode andar e se ainda tem a ação.
	// Por quê: andar não gasta a ação (decisão de 06/10/2026). Dá para andar, atacar e andar o resto.
	public int MovimentoRestante { get; set; }
	public bool AcaoDisponivel { get; set; }

	public Equipe.Membro? Membro { get; init; }
	public InimigoIA? Inimigo { get; init; }
	public MarcadorDeCombate? Marcador { get; set; }

	// Alteração de IA - Revisar
	// O que faz: para inimigos, quem ele pretende atacar — mostrado quando o Desgarrado usa
	//            Alma imaculada ("mostra os próximos movimentos de todos os inimigos").
	// Por quê: quando a intenção foi revelada, ele cumpre: a previsão não pode mentir.
	public Combatente? AlvoPrevisto { get; set; }

	public bool Vivo => Vida > 0;

	// já foi tratado como morto (saiu da casa, o desenho caiu) — para não tratar duas vezes
	public bool Caiu { get; set; }

	public bool Tem(TipoDeEfeito tipo) => Efeitos.Any(e => e.Tipo == tipo);

	public EfeitoAtivo? Efeito(TipoDeEfeito tipo) => Efeitos.FirstOrDefault(e => e.Tipo == tipo);

	public int Acumulos(TipoDeEfeito tipo) => Efeito(tipo)?.Acumulos ?? 0;

	// Alteração de IA - Revisar
	// O que faz: os atributos já com os efeitos somados.
	// Por quê: Escuridão baixa a vida máxima, Corrosão baixa a resistência, Defender e as posturas
	//          sobem a esquiva. Quem calcula acerto e dano pergunta sempre por estes números.
	public int VidaMaxima =>
		Mathf.Max(1, Mathf.RoundToInt(Ficha.Vida *
			(1.0f - RegrasDeCombate.VidaMaximaPerdidaPorEscuridao * Acumulos(TipoDeEfeito.Escuridao))));

	public int ReacaoEfetiva =>
		Ficha.Reacao
		+ (Tem(TipoDeEfeito.Defendendo) ? RegrasDeCombate.ReacaoAoDefender : 0)
		+ (Tem(TipoDeEfeito.EsquivaAumentada) ? RegrasDeCombate.ReacaoDaEsquivaAumentada : 0)
		+ (Tem(TipoDeEfeito.PosturaOculta) ? RegrasDeCombate.ReacaoNaPosturaOculta : 0);

	public int RobustezEfetiva =>
		Mathf.Max(0, Ficha.Robustez
			- RegrasDeCombate.RobustezPerdidaPorCorrosao * Acumulos(TipoDeEfeito.Corrosao)
			+ (Tem(TipoDeEfeito.Defendendo) ? RegrasDeCombate.RobustezAoDefender : 0)
			+ (Tem(TipoDeEfeito.PosturaOculta) ? RegrasDeCombate.RobustezNaPosturaOculta : 0));

	// Alteração de IA - Revisar
	// O que faz: gruda um efeito, respeitando o máximo de acúmulos e as regras especiais.
	// Por quê: Vulnerável não recebe efeitos bons; Gelo também atordoa; Raio trava habilidades ao
	//          acaso; Escuridão encolhe a vida máxima na hora. Devolve o texto para o registro, ou
	//          null quando o efeito foi recusado.
	public string? Aplicar(TipoDeEfeito tipo, int acumulos, int duracao, Combatente? origem,
						   RandomNumberGenerator sorte)
	{
		if (RegrasDeCombate.EhBom(tipo) && Tem(TipoDeEfeito.Vulneravel))
		{
			return null;
		}

		// reaplicar soma acúmulos (até o limite) e renova o prazo — nunca encurta
		EfeitoAtivo? existente = Efeito(tipo);
		if (existente == null)
		{
			existente = new EfeitoAtivo { Tipo = tipo, Acumulos = 0, RodadasRestantes = duracao };
			Efeitos.Add(existente);
		}
		else if (existente.RodadasRestantes >= 0)
		{
			existente.RodadasRestantes = duracao < 0 ? -1 : Mathf.Max(existente.RodadasRestantes, duracao);
		}

		existente.Acumulos = Mathf.Min(RegrasDeCombate.LimiteDe(tipo), existente.Acumulos + Mathf.Max(1, acumulos));
		existente.Origem = origem ?? existente.Origem;

		if (tipo == TipoDeEfeito.Gelo)
		{
			Aplicar(TipoDeEfeito.Atordoado, 1, 1, origem, sorte);
		}
		else if (tipo == TipoDeEfeito.Raio)
		{
			TravarHabilidades(existente, sorte);
		}
		else if (tipo == TipoDeEfeito.Escuridao)
		{
			Vida = Mathf.Min(Vida, VidaMaxima);
		}

		string acumulo = existente.Acumulos > 1 ? $" {existente.Acumulos}" : "";
		return $"{RegrasDeCombate.NomeDe(tipo)}{acumulo}";
	}

	// Alteração de IA - Revisar
	// O que faz: o Raio "bloqueia uma habilidade aleatória" por acúmulo.
	private void TravarHabilidades(EfeitoAtivo raio, RandomNumberGenerator sorte)
	{
		var livres = Ficha.Habilidades.Where(h => !raio.Bloqueadas.Contains(h)).ToList();
		while (raio.Bloqueadas.Count < raio.Acumulos && livres.Count > 0)
		{
			int i = sorte.RandiRange(0, livres.Count - 1);
			raio.Bloqueadas.Add(livres[i]);
			livres.RemoveAt(i);
		}
	}

	public void Remover(TipoDeEfeito tipo) => Efeitos.RemoveAll(e => e.Tipo == tipo);

	// Alteração de IA - Revisar
	// O que faz: tira todos os efeitos ruins. Devolve quantos saíram.
	public int RemoverNegativos()
	{
		return Efeitos.RemoveAll(e => !RegrasDeCombate.EhBom(e.Tipo));
	}

	// Alteração de IA - Revisar
	// O que faz: responde se a habilidade está travada agora.
	// Por quê: Raio trava habilidades ao acaso; Desarmado "impede de usar habilidades de ataque" —
	//          o ataque básico incluído.
	public bool HabilidadeBloqueada(Habilidade h)
	{
		if (h.EhOfensiva && Tem(TipoDeEfeito.Desarmado))
		{
			return true;
		}
		return Efeito(TipoDeEfeito.Raio)?.Bloqueadas.Contains(h) ?? false;
	}

	// Alteração de IA - Revisar
	// O que faz: tira vida (sem contar defesa — quem calcula a defesa é o executor) e devolve
	//            quanto saiu de verdade.
	public int PerderVida(int quanto)
	{
		int antes = Vida;
		Vida = Mathf.Max(0, Vida - Mathf.Max(0, quanto));
		return antes - Vida;
	}

	// Alteração de IA - Revisar
	// O que faz: devolve vida, sem passar da vida máxima. Luz diminui a cura recebida.
	public int Curar(int quanto)
	{
		float fator = 1.0f - RegrasDeCombate.CuraPerdidaPorLuz * Acumulos(TipoDeEfeito.Luz);
		int cura = Mathf.Max(0, Mathf.RoundToInt(quanto * Mathf.Max(0.0f, fator)));
		int antes = Vida;
		Vida = Mathf.Min(VidaMaxima, Vida + cura);
		return Vida - antes;
	}

	// Alteração de IA - Revisar
	// O que faz: o texto curto dos efeitos, para mostrar sobre a cabeça e no painel.
	public string TextoDosEfeitos()
	{
		return string.Join(" · ", Efeitos.Select(e =>
			RegrasDeCombate.NomeDe(e.Tipo) + (e.Acumulos > 1 ? $" {e.Acumulos}" : "")));
	}
}

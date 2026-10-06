using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Alteração de IA - Revisar
// O que faz: executa uma habilidade (ou o ataque básico) do começo ao fim — gasta o Foco, avança
//            até o alvo se for o caso, toca a animação, e no momento do impacto rola o acerto, tira
//            a vida, aplica os efeitos, empurra, cura; depois aplica o que a habilidade faz em quem
//            a usou.
// Por quê: aliados e inimigos usam exatamente o mesmo caminho — a regra é uma só. Quem decide QUAL
//          habilidade e em QUEM é o jogador (pela interface) ou a IA do inimigo; quem faz acontecer
//          é este executor.
public sealed class ExecutorDeHabilidades
{
	private readonly GerenciadorDeCombate _combate;

	public ExecutorDeHabilidades(GerenciadorDeCombate combate)
	{
		_combate = combate;
	}

	private RandomNumberGenerator Sorte => _combate.Sorte;

	// Alteração de IA - Revisar
	// O que faz: usa a habilidade na casa escolhida.
	public async Task Usar(Combatente usuario, Habilidade habilidade, Casa casaAlvo)
	{
		usuario.Foco = Mathf.Max(0, usuario.Foco - habilidade.CustoDeFoco);
		usuario.AcaoDisponivel = false;

		Combatente? alvoPrincipal = habilidade.Alvo == Habilidade.TipoDeAlvo.ProprioPersonagem
			? usuario
			: _combate.CombatenteNaCasa(casaAlvo);

		string emQuem = alvoPrincipal != null && alvoPrincipal != usuario ? $" em {alvoPrincipal.Nome}" : "";
		_combate.Registrar($"{usuario.Nome} usa {habilidade.Nome}{emQuem}.");

		// 1. avançar até o alvo antes do golpe (Avanço tático)
		if (alvoPrincipal != null && habilidade.Efeitos.Any(e => e.Tipo == EfeitoDeHabilidade.Acao.AvancarAteOAlvo))
		{
			await _combate.AvancarAte(usuario, alvoPrincipal);
		}

		// 2. quem é atingido: o alvo, ou todos da área
		List<Combatente> atingidos = QuemEAtingido(usuario, habilidade, alvoPrincipal, casaAlvo);

		// 3. a animação, e no impacto o resultado
		Vector3 pontoDoAlvo = alvoPrincipal?.Corpo.GlobalPosition ?? casaAlvo.Centro;
		var plano = PlanoDoGolpe.Calcular(usuario, alvoPrincipal == usuario ? null : alvoPrincipal, pontoDoAlvo, habilidade);
		var contraAtaques = new List<Combatente>();
		await AnimacaoDeAtaque.Executar(_combate, plano, habilidade, () =>
		{
			foreach (Combatente atingido in atingidos)
			{
				if (Resolver(usuario, habilidade, atingido, habilidade.RaioDaArea > 0))
				{
					contraAtaques.Add(atingido);
				}
			}
		});

		// 4. a Postura oculta devolve o golpe corpo a corpo
		foreach (Combatente defensor in contraAtaques)
		{
			ContraAtacar(defensor, usuario);
		}

		// 5. o que a habilidade faz em quem a usou
		foreach (EfeitoDeHabilidade efeito in habilidade.Efeitos)
		{
			await AplicarEmSi(usuario, efeito);
		}

		// a vantagem de quem atacou primeiro (diálogo) vale só para o primeiro ataque
		if (habilidade.EhOfensiva && usuario.Efeito(TipoDeEfeito.Vantagem) is { SomeNoPrimeiroAtaque: true })
		{
			usuario.Remover(TipoDeEfeito.Vantagem);
		}

		_combate.AtualizarMarcadores();
	}

	private List<Combatente> QuemEAtingido(Combatente usuario, Habilidade habilidade, Combatente? alvo, Casa casaAlvo)
	{
		var atingidos = new List<Combatente>();
		if (habilidade.Alvo == Habilidade.TipoDeAlvo.ProprioPersonagem)
		{
			return atingidos;
		}

		if (habilidade.RaioDaArea > 0)
		{
			foreach (Combatente c in _combate.Combatentes)
			{
				if (!c.Vivo || c.Casa == null || GradeDeCombate.Distancia(c.Casa, casaAlvo) > habilidade.RaioDaArea)
				{
					continue;
				}
				bool doOutroLado = c.Lado != usuario.Lado;
				if (habilidade.EhOfensiva == doOutroLado)
				{
					atingidos.Add(c);
				}
			}
		}
		else if (alvo != null && alvo.Vivo)
		{
			atingidos.Add(alvo);
		}
		return atingidos;
	}

	// Alteração de IA - Revisar
	// O que faz: o resultado em um atingido. Devolve true se ele deve contra-atacar (Postura oculta).
	private bool Resolver(Combatente usuario, Habilidade habilidade, Combatente alvo, bool emArea)
	{
		if (!alvo.Vivo)
		{
			return false;
		}

		if (habilidade.EhOfensiva)
		{
			float chance = ChanceDeAcerto(usuario, alvo);
			bool acertou = habilidade.Garantido || Sorte.Randf() < chance;
			if (!acertou)
			{
				_combate.Registrar($"  {usuario.Nome} erra {alvo.Nome} ({chance:P0} de chance).");
				alvo.Marcador?.Flutuar("errou", new Color(0.8f, 0.8f, 0.85f));
				return false;
			}

			if (habilidade.DanoMaximo > 0)
			{
				(int dano, bool ignoraDefesa) = RolarDano(usuario, habilidade);
				CausarDano(alvo, dano, ignoraDefesa, usuario, emArea);
			}
		}

		foreach (EfeitoDeHabilidade efeito in habilidade.Efeitos)
		{
			if (alvo.Vivo && Sorte.Randf() <= efeito.Chance)
			{
				AplicarNoAtingido(usuario, alvo, efeito);
			}
		}

		return habilidade.EhOfensiva && habilidade.Forma == Habilidade.FormaDoAtaque.CorpoACorpo
			&& alvo.Vivo && alvo.Tem(TipoDeEfeito.PosturaOculta);
	}

	// Alteração de IA - Revisar
	// O que faz: a chance de acertar — base, mais Precisão contra Reação, mais Vantagem, menos
	//            Desvantagem e Cego.
	// Por quê: Reação é "capacidade de evitar golpes" (GDD). Os números são simbólicos (ver
	//          RegrasDeCombate).
	public static float ChanceDeAcerto(Combatente atacante, Combatente alvo)
	{
		float chance = RegrasDeCombate.ChanceBaseDeAcerto
			+ RegrasDeCombate.AcertoPorPonto * (atacante.Ficha.Precisao - alvo.ReacaoEfetiva);
		if (atacante.Tem(TipoDeEfeito.Vantagem))
		{
			chance += RegrasDeCombate.AcertoDaVantagem;
		}
		if (atacante.Tem(TipoDeEfeito.Desvantagem))
		{
			chance -= RegrasDeCombate.AcertoDaVantagem;
		}
		if (atacante.Tem(TipoDeEfeito.Cego))
		{
			chance -= RegrasDeCombate.AcertoDoCego;
		}
		return Mathf.Clamp(chance, RegrasDeCombate.AcertoMinimo, RegrasDeCombate.AcertoMaximo);
	}

	// Alteração de IA - Revisar
	// O que faz: rola o dano entre o mínimo e o máximo da habilidade, com Vantagem, Desvantagem e a
	//            Postura oculta. Avisa se tirou o máximo numa habilidade Crítica (ignora a defesa).
	private (int dano, bool ignoraDefesa) RolarDano(Combatente usuario, Habilidade habilidade)
	{
		int minimo = Mathf.Min(habilidade.DanoMinimo, habilidade.DanoMaximo);
		int bruto = Sorte.RandiRange(minimo, habilidade.DanoMaximo);
		bool maximo = bruto == habilidade.DanoMaximo;

		float fator = 1.0f;
		if (usuario.Tem(TipoDeEfeito.Vantagem))
		{
			fator += RegrasDeCombate.DanoDaVantagem;
		}
		if (usuario.Tem(TipoDeEfeito.Desvantagem))
		{
			fator -= RegrasDeCombate.DanoDaVantagem;
		}
		if (usuario.Tem(TipoDeEfeito.PosturaOculta))
		{
			fator *= RegrasDeCombate.DanoNaPosturaOculta;
		}
		return (Mathf.Max(1, Mathf.RoundToInt(bruto * fator)), habilidade.Critico && maximo);
	}

	// Alteração de IA - Revisar
	// O que faz: entrega o dano — passando pela Vanguarda: o dano de quem é Protegido vai todo para
	//            quem o protege; em dano de área, metade fica com o protegido e metade vai para a
	//            Vanguarda (decisão do PO em 06/10/2026).
	public void CausarDano(Combatente alvo, int dano, bool ignoraDefesa, Combatente? atacante, bool emArea)
	{
		Combatente? vanguarda = alvo.Efeito(TipoDeEfeito.Protegido)?.Origem;
		if (vanguarda != null && vanguarda.Vivo && vanguarda != alvo)
		{
			if (emArea)
			{
				int metade = dano / 2;
				_combate.Registrar($"  {vanguarda.Nome} divide o golpe com {alvo.Nome} (Vanguarda).");
				Receber(alvo, dano - metade, ignoraDefesa, atacante);
				Receber(vanguarda, metade, ignoraDefesa, atacante);
			}
			else
			{
				_combate.Registrar($"  {vanguarda.Nome} toma o golpe no lugar de {alvo.Nome} (Vanguarda).");
				Receber(vanguarda, dano, ignoraDefesa, atacante);
			}
			return;
		}
		Receber(alvo, dano, ignoraDefesa, atacante);
	}

	// Alteração de IA - Revisar
	// O que faz: um lutador recebe um golpe — Couraça bloqueia o golpe inteiro, Robustez tira um
	//            pouco, Espinhos devolve parte ao atacante.
	private void Receber(Combatente quem, int dano, bool ignoraDefesa, Combatente? atacante)
	{
		if (dano <= 0 || !quem.Vivo)
		{
			return;
		}

		EfeitoAtivo? couraca = quem.Efeito(TipoDeEfeito.Couraca);
		if (couraca != null)
		{
			couraca.Acumulos--;
			if (couraca.Acumulos <= 0)
			{
				quem.Remover(TipoDeEfeito.Couraca);
			}
			_combate.Registrar($"  A Couraça de {quem.Nome} bloqueia o golpe.");
			quem.Marcador?.Flutuar("bloqueou", new Color(0.7f, 0.85f, 1.0f));
			return;
		}

		int final = ignoraDefesa ? dano : Mathf.Max(1, dano - quem.RobustezEfetiva);
		int tirado = quem.PerderVida(final);
		_combate.Registrar($"  {quem.Nome} perde {tirado} de vida ({quem.Vida}/{quem.VidaMaxima}).");
		quem.Marcador?.Flutuar($"-{tirado}", new Color(1.0f, 0.35f, 0.3f));

		if (atacante != null && atacante != quem && atacante.Vivo && quem.Tem(TipoDeEfeito.Espinhos))
		{
			int devolvido = Mathf.Max(1, tirado / 2);
			atacante.PerderVida(devolvido);
			_combate.Registrar($"  Espinhos devolvem {devolvido} a {atacante.Nome}.");
			atacante.Marcador?.Flutuar($"-{devolvido}", new Color(1.0f, 0.55f, 0.3f));
			_combate.ConferirMorte(atacante);
		}

		_combate.ConferirMorte(quem);
	}

	// Alteração de IA - Revisar
	// O que faz: o que a habilidade faz em quem foi atingido, além do dano.
	private void AplicarNoAtingido(Combatente usuario, Combatente alvo, EfeitoDeHabilidade efeito)
	{
		switch (efeito.Tipo)
		{
			case EfeitoDeHabilidade.Acao.AplicarNoAlvo:
				if (efeito.Efeito == TipoDeEfeito.Protegido)
				{
					Proteger(usuario, alvo, efeito.Duracao);
				}
				else
				{
					Grudar(alvo, efeito.Efeito, efeito.Acumulos, efeito.Duracao, usuario);
				}
				break;

			case EfeitoDeHabilidade.Acao.GeloOuFogoNoAlvo:
				// a Árvore diz "aplica gelo ou fogo": por enquanto é sorteado (a definir pela equipe)
				// Gelo dura o prazo da habilidade; Fogo vai por acúmulos (cada turno queima um)
				TipoDeEfeito sorteado = Sorte.Randf() < 0.5f ? TipoDeEfeito.Gelo : TipoDeEfeito.Fogo;
				bool fogo = sorteado == TipoDeEfeito.Fogo;
				Grudar(alvo, sorteado, fogo ? efeito.Acumulos : 1, fogo ? -1 : efeito.Duracao, usuario);
				break;

			case EfeitoDeHabilidade.Acao.Empurrar:
				_combate.Empurrar(usuario, alvo, efeito.Quantidade);
				break;

			case EfeitoDeHabilidade.Acao.Curar:
				int curado = alvo.Curar(efeito.Quantidade);
				_combate.Registrar($"  {alvo.Nome} recupera {curado} de vida ({alvo.Vida}/{alvo.VidaMaxima}).");
				alvo.Marcador?.Flutuar($"+{curado}", new Color(0.45f, 1.0f, 0.5f));
				break;

			case EfeitoDeHabilidade.Acao.RemoverEfeitosNegativos:
				int removidos = alvo.RemoverNegativos();
				if (removidos > 0)
				{
					_combate.Registrar($"  {removidos} efeito(s) ruim(ns) saem de {alvo.Nome}.");
				}
				break;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: gruda o efeito e conta no registro.
	public void Grudar(Combatente alvo, TipoDeEfeito tipo, int acumulos, int duracao, Combatente? origem)
	{
		string? texto = alvo.Aplicar(tipo, acumulos, duracao, origem, Sorte);
		if (texto == null)
		{
			_combate.Registrar($"  {alvo.Nome} está Vulnerável e não recebe {RegrasDeCombate.NomeDe(tipo)}.");
			return;
		}
		_combate.Registrar($"  {alvo.Nome}: {texto}.");
		alvo.Marcador?.Flutuar(RegrasDeCombate.NomeDe(tipo), new Color(1.0f, 0.85f, 0.45f));
	}

	// Alteração de IA - Revisar
	// O que faz: a Vanguarda — quem usa vira a Vanguarda, o alvo vira o Protegido dela.
	// Por quê: dá para proteger vários aliados, um por uso (decisão do PO em 06/10/2026). O prazo
	//          vem da habilidade (Protetora: 3 rodadas, provisório).
	private void Proteger(Combatente vanguarda, Combatente protegido, int duracao)
	{
		if (vanguarda == protegido)
		{
			return;
		}

		protegido.Remover(TipoDeEfeito.Protegido);
		string? texto = protegido.Aplicar(TipoDeEfeito.Protegido, 1, duracao, vanguarda, Sorte);
		if (texto == null)
		{
			_combate.Registrar($"  {protegido.Nome} está Vulnerável e não pode ser protegido.");
			return;
		}
		vanguarda.Aplicar(TipoDeEfeito.Vanguarda, 1, -1, vanguarda, Sorte);
		_combate.Registrar($"  {vanguarda.Nome} é a Vanguarda de {protegido.Nome}: o dano dele vai para ela.");
		protegido.Marcador?.Flutuar("Protegido", new Color(0.6f, 0.85f, 1.0f));
	}

	// Alteração de IA - Revisar
	// O que faz: o que a habilidade faz em quem a usou (postura, couraça, recuar, Foco, revelar).
	private async Task AplicarEmSi(Combatente usuario, EfeitoDeHabilidade efeito)
	{
		if (!usuario.Vivo)
		{
			return;
		}

		switch (efeito.Tipo)
		{
			case EfeitoDeHabilidade.Acao.AplicarEmSi:
				Grudar(usuario, efeito.Efeito, efeito.Acumulos, efeito.Duracao, usuario);
				break;

			case EfeitoDeHabilidade.Acao.RemoverDeSi:
				if (usuario.Tem(efeito.Efeito))
				{
					usuario.Remover(efeito.Efeito);
					_combate.Registrar($"  {usuario.Nome} sai de {RegrasDeCombate.NomeDe(efeito.Efeito)}.");
				}
				break;

			case EfeitoDeHabilidade.Acao.RecuperarFoco:
				int antes = usuario.Foco;
				usuario.Foco = Mathf.Min(usuario.Ficha.Foco, usuario.Foco + efeito.Quantidade);
				_combate.Registrar($"  {usuario.Nome} recupera {usuario.Foco - antes} de Foco.");
				break;

			case EfeitoDeHabilidade.Acao.Recuar:
				await _combate.Recuar(usuario, efeito.Quantidade);
				break;

			case EfeitoDeHabilidade.Acao.RevelarIntencoes:
				_combate.RevelarIntencoes();
				break;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: o contra-ataque da Postura oculta — metade do dano do ataque básico, e aplica a
	//            "maldição" (que ainda não tem efeito definido).
	private void ContraAtacar(Combatente defensor, Combatente atacante)
	{
		if (!defensor.Vivo || !atacante.Vivo || defensor.Ficha.AtaqueBasico == null)
		{
			return;
		}

		(int dano, bool ignora) = RolarDano(defensor, defensor.Ficha.AtaqueBasico);
		int contra = Mathf.Max(1, Mathf.RoundToInt(dano * RegrasDeCombate.DanoDoContraAtaque));
		_combate.Registrar($"  {defensor.Nome} contra-ataca da Postura oculta.");
		CausarDano(atacante, contra, ignora, defensor, false);
		if (atacante.Vivo)
		{
			Grudar(atacante, TipoDeEfeito.Maldicao, 1, 2, defensor);
		}
	}
}

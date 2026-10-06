using System.Collections.Generic;

// Alteração de IA - Revisar
// O que faz: todos os números das regras do combate num lugar só — chance de acerto, quanto o
//            veneno tira por acúmulo, quanto o Defender protege, etc.
// Por quê: **são valores simbólicos, só para testar a mecânica** (decisão do PO em 06/10/2026: o
//          balanceamento ainda está sendo feito). Juntar tudo aqui deixa claro o que é provisório e
//          facilita trocar quando a equipe definir. Os números das fichas e das habilidades ficam
//          nos arquivos .tres; aqui ficam só as regras gerais. A lista completa está em docs/JOGO.md.
public static class RegrasDeCombate
{
	// acerto: 75% de base, 5% por ponto de Precisão do atacante contra Reação do alvo
	public const float ChanceBaseDeAcerto = 0.75f;
	public const float AcertoPorPonto = 0.05f;
	public const float AcertoMinimo = 0.10f;
	public const float AcertoMaximo = 0.95f;

	// Vantagem e Desvantagem mexem no acerto e no dano; Cego tira acerto
	public const float AcertoDaVantagem = 0.15f;
	public const float DanoDaVantagem = 0.25f;
	public const float AcertoDoCego = 0.30f;

	// a ação Defender, até o começo do próximo turno
	public const int ReacaoAoDefender = 3;
	public const int RobustezAoDefender = 3;

	// efeitos que tiram vida no começo do turno de quem tem o efeito
	public const int DanoDoVenenoPorAcumulo = 2;
	public const int DanoDoFogoPorAcumulo = 2;
	public const int DanoDoSangramentoPorAcumulo = 1;
	public const int AcumulosDoFogoParaEspalhar = 5;
	public const int DanoDoGeloAoSair = 3;

	// efeitos que enfraquecem
	public const float VidaMaximaPerdidaPorEscuridao = 0.10f;
	public const float CuraPerdidaPorLuz = 0.25f;
	public const int RobustezPerdidaPorCorrosao = 1;

	// estados das habilidades padrão (sem definição oficial)
	public const float DanoNaPosturaOculta = 0.70f;
	public const int ReacaoNaPosturaOculta = 2;
	public const int RobustezNaPosturaOculta = 2;
	public const float DanoDoContraAtaque = 0.50f;
	public const int ReacaoDaEsquivaAumentada = 3;

	// Alteração de IA - Revisar
	// O que faz: o máximo de acúmulos de cada efeito.
	// Por quê: os limites de Corrosão (5), Raio (2), Escuridão (2) e Luz (3) são os da Árvore de
	//          Habilidades. Os outros a Árvore não diz — ficam altos e provisórios.
	public static readonly Dictionary<TipoDeEfeito, int> MaximoDeAcumulos = new()
	{
		{ TipoDeEfeito.Corrosao, 5 },
		{ TipoDeEfeito.Raio, 2 },
		{ TipoDeEfeito.Escuridao, 2 },
		{ TipoDeEfeito.Luz, 3 },
		{ TipoDeEfeito.Veneno, 10 },
		{ TipoDeEfeito.Fogo, 10 },
		{ TipoDeEfeito.Sangramento, 10 },
		{ TipoDeEfeito.Couraca, 3 },
		{ TipoDeEfeito.Vantagem, 3 },
	};

	public static int LimiteDe(TipoDeEfeito tipo) =>
		MaximoDeAcumulos.TryGetValue(tipo, out int limite) ? limite : 1;

	// Alteração de IA - Revisar
	// O que faz: diz se um efeito é bom para quem o recebe.
	// Por quê: duas regras dependem disso — Vulnerável "não pode receber buffs", e Fogo acolhedor
	//          "remove debuffs" (só os ruins).
	public static bool EhBom(TipoDeEfeito tipo) => tipo switch
	{
		TipoDeEfeito.Vantagem or TipoDeEfeito.Couraca or TipoDeEfeito.Vanguarda or
		TipoDeEfeito.Protegido or TipoDeEfeito.Furtivo or TipoDeEfeito.Espinhos or
		TipoDeEfeito.PosturaOculta or TipoDeEfeito.EsquivaAumentada or TipoDeEfeito.Defendendo => true,
		_ => false,
	};

	// Alteração de IA - Revisar
	// O que faz: o nome de cada efeito como aparece na tela.
	public static string NomeDe(TipoDeEfeito tipo) => tipo switch
	{
		TipoDeEfeito.Corrosao => "Corrosão",
		TipoDeEfeito.Escuridao => "Escuridão",
		TipoDeEfeito.Vulneravel => "Vulnerável",
		TipoDeEfeito.Couraca => "Couraça",
		TipoDeEfeito.PosturaOculta => "Postura oculta",
		TipoDeEfeito.Maldicao => "Maldição",
		TipoDeEfeito.EsquivaAumentada => "Esquiva",
		_ => tipo.ToString(),
	};
}

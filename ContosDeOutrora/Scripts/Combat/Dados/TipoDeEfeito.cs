// Alteração de IA - Revisar
// O que faz: a lista de tudo o que pode ficar "grudado" num personagem durante o combate —
//            efeitos de status, condições e alguns estados usados pelas habilidades padrão.
// Por quê: um nome só para todos facilita aplicar, mostrar na tela, contar acúmulos e remover.
//          A divisão abaixo segue a Árvore de Habilidades. Os do último grupo aparecem dentro de
//          habilidades mas não têm definição oficial — os números deles são provisórios e estão
//          listados em docs/JOGO.md para a equipe definir.
public enum TipoDeEfeito
{
	// efeitos de status (Árvore de Habilidades)
	Gelo,
	Veneno,
	Sangramento,
	Fogo,
	Corrosao,
	Raio,
	Escuridao,
	Luz,

	// condições (Árvore de Habilidades)
	Desarmado,
	Desvantagem,
	Vantagem,
	Atordoado,
	Enraizado,
	Vulneravel,
	Couraca,
	Vanguarda,     // quem protege (decisão do PO em 06/10/2026)
	Protegido,     // quem é protegido: o dano dele vai para a Vanguarda
	Furtivo,
	Cego,
	Espinhos,

	// estados usados pelas habilidades padrão, sem definição oficial
	PosturaOculta,     // Desgarrado — "Postura oculta"
	Maldicao,          // Desgarrado — aplicada pela Postura oculta; sem efeito definido
	EsquivaAumentada,  // Caçadora — "Técnica secreta"
	Defendendo,        // a ação Defender
}

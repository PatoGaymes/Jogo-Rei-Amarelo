using Godot;

// Alteração de IA - Revisar
// O que faz: uma das coisas que uma habilidade faz além do dano — aplicar veneno, empurrar,
//            curar, avançar até o alvo, etc. Uma habilidade pode ter várias, na ordem da lista.
// Por quê: cada habilidade da Árvore é uma combinação dessas peças ("causa dano e enraíza",
//          "avança, causa dano e empurra"). Montar a habilidade com peças, num arquivo de dados,
//          deixa o game designer criar e ajustar habilidades sem programar.
[GlobalClass]
public partial class EfeitoDeHabilidade : Resource
{
	public enum Acao
	{
		AplicarNoAlvo,            // grudar um efeito (veneno, enraizado...) em quem foi atingido
		AplicarEmSi,              // grudar um efeito em quem usou a habilidade
		RemoverDeSi,              // tirar um efeito de quem usou (ex.: sair da Postura oculta)
		Empurrar,                 // afasta o alvo N casas de quem usou
		AvancarAteOAlvo,          // quem usa anda até ficar colado no alvo ANTES de bater
		Recuar,                   // quem usa se afasta N casas do inimigo mais perto
		Curar,                    // devolve vida a quem foi atingido
		RecuperarFoco,            // devolve Foco a quem usou
		RemoverEfeitosNegativos,  // limpa os efeitos ruins de quem foi atingido
		RevelarIntencoes,         // mostra o próximo movimento dos inimigos (Alma imaculada)
		GeloOuFogoNoAlvo,         // sorteia entre Gelo e Fogo (Transfiguração térmica)
	}

	// Alteração de IA - Revisar
	// O que faz: o que esta peça faz, e qual efeito ela gruda ou tira (quando for o caso).
	[Export]
	public Acao Tipo { get; set; } = Acao.AplicarNoAlvo;

	[Export]
	public TipoDeEfeito Efeito { get; set; } = TipoDeEfeito.Veneno;

	// Alteração de IA - Revisar
	// O que faz: quantos acúmulos aplica, por quantas rodadas dura (-1 = até ser removido) e o
	//            número da ação (casas para empurrar/recuar, vida curada, Foco recuperado).
	// Por quê: todos com valores simbólicos por enquanto — o balanceamento ainda está sendo feito.
	[Export]
	public int Acumulos { get; set; } = 1;

	[Export]
	public int Duracao { get; set; } = 2;

	[Export]
	public int Quantidade { get; set; } = 1;

	// Alteração de IA - Revisar
	// O que faz: a chance de a peça acontecer, de 0 a 1.
	// Por quê: algumas habilidades da Árvore falam em "chance de". Hoje todas as padrão usam 1.
	[Export(PropertyHint.Range, "0,1,0.05")]
	public float Chance { get; set; } = 1.0f;
}

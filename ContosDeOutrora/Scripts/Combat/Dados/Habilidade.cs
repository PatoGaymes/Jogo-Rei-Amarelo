using Godot;

// Alteração de IA - Revisar
// O que faz: uma habilidade (ou o ataque básico) — quanto custa, até onde alcança, em quem pode
//            ser usada, quanto dano dá e o que mais ela faz.
// Por quê: fica num arquivo de dados (.tres em Resources/Habilidades/) para o game designer
//          balancear sem programar. Os números de hoje são simbólicos: a equipe ainda está
//          fazendo o balanceamento.
[GlobalClass]
public partial class Habilidade : Resource
{
	public enum TipoDeAlvo
	{
		Inimigo,
		Aliado,
		ProprioPersonagem,
	}

	// Alteração de IA - Revisar
	// O que faz: como o ataque chega ao alvo — o personagem vai até ele, ou dispara de longe.
	// Por quê: a animação de ataque vai **encostar de verdade** no alvo. No corpo a corpo, quem
	//          ataca anda até a distância de encaixe da arma e só então golpeia; à distância, fica
	//          parado e o disparo viaja até o alvo (ver AnimacaoDeAtaque).
	public enum FormaDoAtaque
	{
		CorpoACorpo,
		ADistancia,
		SemAtaque,   // cura, proteção, postura: não há golpe
	}

	[Export]
	public string Nome { get; set; } = "";

	[Export(PropertyHint.MultilineText)]
	public string Descricao { get; set; } = "";

	// Alteração de IA - Revisar
	// O que faz: quanto Foco a habilidade gasta. O ataque básico custa 0.
	// Por quê: o recurso das habilidades é o Foco (decisão do PO em 06/10/2026).
	[Export]
	public int CustoDeFoco { get; set; } = 0;

	[Export]
	public TipoDeAlvo Alvo { get; set; } = TipoDeAlvo.Inimigo;

	// Alteração de IA - Revisar
	// O que faz: até quantas casas de distância a habilidade alcança.
	// Por quê: o alcance é contado em casas da grade, como o movimento (decisão de 06/10/2026).
	//          1 = só a casa vizinha. Usada em si mesmo, o alcance não importa.
	[Export]
	public int Alcance { get; set; } = 1;

	// Alteração de IA - Revisar
	// O que faz: o tamanho da área atingida em volta do alvo, em casas. 0 = só o alvo.
	[Export]
	public int RaioDaArea { get; set; } = 0;

	[Export]
	public int DanoMinimo { get; set; } = 0;

	[Export]
	public int DanoMaximo { get; set; } = 0;

	// Alteração de IA - Revisar
	// O que faz: as especificações da Árvore. Garantido ignora a esquiva; Crítico ignora a defesa
	//            quando tira o dano máximo.
	[Export]
	public bool Garantido { get; set; }

	[Export]
	public bool Critico { get; set; }

	[Export]
	public FormaDoAtaque Forma { get; set; } = FormaDoAtaque.CorpoACorpo;

	// Alteração de IA - Revisar
	// O que faz: no corpo a corpo, a que distância do alvo (em metros) quem ataca para para golpear.
	// Por quê: é o "encaixe" da animação. Uma espada encosta (perto de 0,9 m); uma lança golpeia de
	//          mais longe. A animação anda a diferença entre onde o personagem está e esse ponto —
	//          então o golpe sempre chega no alvo, seja ele vizinho ou a duas casas.
	[Export]
	public float DistanciaDeEncaixe { get; set; } = 0.9f;

	[Export]
	public bool PrecisaDeLinhaDeVisao { get; set; } = true;

	[Export]
	public Godot.Collections.Array<EfeitoDeHabilidade> Efeitos { get; set; } = new();

	// Alteração de IA - Revisar
	// O que faz: responde se a habilidade machuca quem é atingido.
	// Por quê: decide se há rolagem de acerto — curar um aliado não pode "errar".
	public bool EhOfensiva => Alvo == TipoDeAlvo.Inimigo;
}

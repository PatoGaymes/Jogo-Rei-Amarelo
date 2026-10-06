using Godot;

// Alteração de IA - Revisar
// O que faz: a ficha de quem luta — herói ou inimigo: nome, desenho, atributos, movimento, ataque
//            básico e habilidades.
// Por quê: cada personagem e cada tipo de inimigo é um arquivo de dados (.tres em Resources/),
//          e não números escritos no código, para o game designer balancear sem programar.
//          Os atributos seguem o GDD: Corpo, Mente e Essência. Os valores de hoje são simbólicos.
[GlobalClass]
public partial class FichaDeCombatente : Resource
{
	[Export]
	public string Nome { get; set; } = "";

	[Export]
	public string Classe { get; set; } = "";

	// Alteração de IA - Revisar
	// O que faz: o desenho do personagem. A animação, quando existir; senão, a imagem provisória.
	// Por quê: só o Desgarrado tem animação hoje. Os outros entram em combate com a imagem parada
	//          da pasta Provisorio, que é trocada sozinha quando a animação chegar.
	[Export]
	public SpriteFrames? Animacoes { get; set; }

	[Export]
	public Texture2D? Imagem { get; set; }

	[Export]
	public float AlturaDoDesenho { get; set; } = 1.85f;

	[ExportGroup("Corpo")]
	[Export]
	public int Vida { get; set; } = 20;

	[Export]
	public int Precisao { get; set; } = 1;

	[Export]
	public int Furtividade { get; set; } = 0;

	// Alteração de IA - Revisar
	// O que faz: o atributo Iniciativa, somado ao dado de 20 lados no começo do combate.
	// Por quê: entrou no GDD do Drive em 06/10/2026 ("atributo focado exclusivamente para
	//          iniciativa em combates"). É a exceção à regra de que a documentação local vale mais.
	[Export]
	public int Iniciativa { get; set; } = 0;

	[Export]
	public int Reacao { get; set; } = 0;

	[Export]
	public int Robustez { get; set; } = 0;

	[ExportGroup("Mente")]
	[Export]
	public int Sanidade { get; set; } = 20;

	[Export]
	public int Vontade { get; set; } = 0;

	[ExportGroup("Essência")]
	[Export]
	public int Foco { get; set; } = 10;

	[Export]
	public int Energia { get; set; } = 0;

	[Export]
	public int Aura { get; set; } = 0;

	[ExportGroup("Combate")]
	// Alteração de IA - Revisar
	// O que faz: quantas casas o personagem anda por turno.
	// Por quê: 3 para todos por enquanto (decisão do PO em 06/10/2026). Andar não gasta a ação.
	[Export]
	public int Movimento { get; set; } = 3;

	[Export]
	public Habilidade? AtaqueBasico { get; set; }

	[Export]
	public Godot.Collections.Array<Habilidade> Habilidades { get; set; } = new();

	[ExportGroup("Inimigo")]
	// Alteração de IA - Revisar
	// O que faz: o alcance, em metros, do "grito de aviso" deste inimigo.
	// Por quê: quando a luta começa com ele, todo inimigo dentro desse alcance, e sem parede no
	//          meio, entra junto. Quem entrou pelo grito não grita de novo — não há efeito em cadeia
	//          (decisão do PO em 06/10/2026). Só vale para inimigos.
	[Export]
	public float AlcanceDoGrito { get; set; } = 10.0f;
}

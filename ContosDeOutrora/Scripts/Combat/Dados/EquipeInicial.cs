using Godot;

// Alteração de IA - Revisar
// O que faz: quem forma a equipe no começo do jogo, em que casa da formação cada um começa e quem
//            é o líder (o personagem que anda pelo mapa).
// Por quê: arquivo de dados (Resources/Equipe/EquipeInicial.tres) para trocar a equipe de teste sem
//          programar. A formação usa o endereço de hexágono (Q, R) da mini-região do menu de equipe,
//          com a frente para cima (R negativo).
[GlobalClass]
public partial class EquipeInicial : Resource
{
	[Export]
	public Godot.Collections.Array<FichaDeCombatente> Membros { get; set; } = new();

	[Export]
	public Godot.Collections.Array<Vector2I> Formacao { get; set; } = new();

	[Export]
	public int Lider { get; set; } = 0;
}

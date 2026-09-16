using Godot;

// Alteração de IA - Revisar
// O que faz: a câmera do corredor. Ela acompanha o personagem para os lados, mas fica
//            sempre na mesma altura, nunca subindo nem descendo.
// Por quê: é o enquadramento do Darkest Dungeon, nossa referência. A cena é montada como
//          um quadro de teatro: a "plateia" não muda de altura, só desliza para o lado
//          acompanhando quem anda. Se a câmera subisse e descesse junto com o personagem,
//          o cenário balançaria e a arte perderia o efeito de quadro parado.
public partial class CameraCorredor : Camera2D
{
	// Alteração de IA - Revisar
	// O que faz: quem a câmera deve acompanhar. Normalmente o personagem do jogador.
	//            O "?" avisa que pode estar vazio.
	// Por quê: fica com [Export] para permitir apontar outro alvo em cenas específicas,
	//          como uma cutscene que acompanha outro personagem. Se ficar vazio, a câmera
	//          procura sozinha (ver _Ready abaixo) — por isso ele pode começar vazio.
	[Export]
	public Node2D? Alvo { get; set; }

	// Alteração de IA - Revisar
	// O que faz: a altura em que a câmera fica travada.
	// Por quê: é o que define o enquadramento do corredor — o quanto aparece de chão e de
	//          teto. Ajustar este número sobe ou desce o "quadro" inteiro. Precisa ser
	//          conferido a olho em cada cenário, junto com a arte.
	[Export]
	public float AlturaFixa { get; set; } = 560.0f;

	public override void _Ready()
	{
		// Alteração de IA - Revisar
		// O que faz: se ninguém apontou um alvo, procura o personagem do jogador sozinho.
		// Por quê: o personagem está no grupo "player". Buscar pelo grupo, em vez de pelo
		//          caminho da cena, faz a câmera continuar funcionando mesmo que alguém
		//          mova ou renomeie os nós dentro do corredor.
		if (Alvo == null)
		{
			var encontrados = GetTree().GetNodesInGroup("player");
			if (encontrados.Count > 0)
			{
				Alvo = encontrados[0] as Node2D;
			}
		}

		if (Alvo == null)
		{
			// Alteração de IA - Revisar
			// O que faz: escreve um aviso no painel de saída do Godot.
			// Por quê: sem alvo, a câmera fica parada e parece que o jogo travou. O aviso
			//          diz exatamente qual é o problema, em vez de deixar alguém procurando.
			GD.PushWarning("CameraCorredor: nenhum alvo encontrado. " +
						   "Confira se o personagem está no grupo 'player'.");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Alvo == null)
		{
			return;
		}

		// Alteração de IA - Revisar
		// O que faz: copia apenas a posição horizontal do alvo, mantendo a altura travada.
		// Por quê: é o que faz a câmera deslizar para o lado sem subir nem descer.
		//          O movimento suave não é feito aqui: ele vem da opção "Position Smoothing"
		//          configurada na própria câmera, dentro da cena.
		GlobalPosition = new Vector2(Alvo.GlobalPosition.X, AlturaFixa);
	}
}

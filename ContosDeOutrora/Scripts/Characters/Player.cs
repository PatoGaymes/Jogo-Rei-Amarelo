using Godot;

// Alteração de IA - Revisar
// O que faz: script de teste ligado ao personagem, com um movimento lateral simples.
// Por quê: serve para confirmar que o C# está compilando e rodando dentro do Godot.
//          O movimento definitivo do jogo ainda será definido junto com a equipe.
public partial class Player : CharacterBody2D
{
	// Alteração de IA - Revisar
	// O que faz: velocidade de caminhada do personagem, em pixels por segundo.
	// Por quê: está marcado com [Export] para que a equipe ajuste o valor direto
	//          pelo painel do Godot, sem precisar mexer no código.
	[Export]
	public float Speed { get; set; } = 300.0f;

	public override void _Ready()
	{
		// Alteração de IA - Revisar
		// O que faz: escreve uma mensagem no painel de saída do Godot ao iniciar a cena.
		// Por quê: é a confirmação visual de que o C# foi compilado e executado.
		//          Pode ser apagado assim que o ambiente estiver validado.
		GD.Print("C# funcionando: script Player carregado.");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Alteração de IA - Revisar
		// O que faz: lê as setas (ou A/D) do teclado e move o personagem na horizontal.
		// Por quê: teste mínimo de entrada do jogador. Usa as ações "ui_left" e
		//          "ui_right", que já vêm prontas no Godot. Quando criarmos as nossas
		//          próprias ações no Input Map, esta linha deve ser trocada.
		float direction = Input.GetAxis("ui_left", "ui_right");

		Velocity = new Vector2(direction * Speed, 0);
		MoveAndSlide();
	}
}

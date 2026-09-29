using Godot;

// Alteração de IA - Revisar
// O que faz: diz se esta fase é limpa, coberta de névoa ou mergulhada na escuridão — e guarda os
//            ajustes de cada um.
// Por quê: **nem todo mapa tem névoa ou escuridão.** Um mapa sem este nó é limpo: tudo aparece
//          normalmente, sem nada ofuscando. Colocando o nó, o mapa passa a esconder o que o
//          personagem não enxerga. Fica como um nó da fase, e não como ajuste da câmera, porque
//          é uma característica do lugar — a mesma câmera serve para as minas escuras e para a
//          superfície ao ar livre.
//
//          Os dois tipos escondem **de verdade** o que está fora da visão, não só escurecem:
//
//            Escuridão: preto total onde não há luz. É o Don't Starve Together — sem fogo, nem o
//                       próprio personagem aparece. Com luz, só se vê o que está iluminado E
//                       perto do personagem ou na direção em que ele olha.
//
//            Névoa: não é preta, e **não é uniforme**. Ela se move em massas, como neblina de
//                   verdade: num mesmo lugar a névoa pode estar encobrindo 100%, depois 70%,
//                   depois 100% de novo. É o Silent Hill antigo — dá para ver o que está perto,
//                   e às vezes uma forma surge e some no meio da névoa.
public partial class AmbienteDaFase : Node
{
	public enum TipoDeAmbiente
	{
		Limpo,      // tudo aparece normalmente
		Nevoa,      // neblina clara que se move e varia de densidade
		Escuridao   // preto total onde não há luz
	}

	[Export]
	public TipoDeAmbiente Tipo { get; set; } = TipoDeAmbiente.Nevoa;

	// ---------------------------------------------------------------- névoa

	[Export]
	public Color CorDaNevoa { get; set; } = new(0.60f, 0.62f, 0.64f);

	// Alteração de IA - Revisar
	// O que faz: o quanto a névoa encobre no ponto mais ralo e no mais denso, fora da visão.
	// Por quê: são os "70% a 100%" pedidos. No ponto mais ralo dá para adivinhar uma forma; no
	//          mais denso some tudo. **Longe do personagem ela sempre chega ao máximo** — a
	//          variação só aparece perto de onde ele enxerga, senão daria para espiar o mapa
	//          inteiro pelas brechas.
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float DensidadeMinima { get; set; } = 0.70f;

	[Export(PropertyHint.Range, "0,1,0.01")]
	public float DensidadeMaxima { get; set; } = 1.0f;

	// Alteração de IA - Revisar
	// O que faz: o tamanho aproximado de cada "massa" de névoa, em metros, e para onde o vento a
	//            leva, em metros por segundo.
	// Por quê: massas pequenas dão névoa picotada, que parece defeito; grandes demais e a
	//          variação quase não aparece. O vento é o que faz a névoa **passar**, em vez de
	//          ficar pulsando no lugar.
	[Export]
	public float TamanhoDasMassas { get; set; } = 16.0f;

	[Export]
	public Vector2 Vento { get; set; } = new(0.55f, 0.20f);

	// ---------------------------------------------------------------- escuridão

	[Export]
	public Color CorDaEscuridao { get; set; } = new(0.0f, 0.0f, 0.0f);

	// Alteração de IA - Revisar
	// O que faz: um pouco de luz que existe em todo lugar, mesmo sem fogueira.
	// Por quê: em 0 é breu total — o Don't Starve. Um valor pequeno faz um lugar "muito escuro"
	//          em vez de "sem luz nenhuma", onde ainda se adivinham vultos por perto.
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float LuzAmbiente { get; set; } = 0.0f;

	// Alteração de IA - Revisar
	// O que faz: apaga todas as fontes de luz da fase, sem precisar mexer em cada uma.
	// Por quê: é o que permite testar o breu total (a segunda imagem de referência do Don't
	//          Starve). Usado pela tecla F4 no ciclo de testes.
	public bool ApagarTodasAsLuzes { get; set; }

	// Alteração de IA - Revisar
	// O que faz: devolve o ambiente da fase que está rodando, ou nada se a fase for limpa.
	// Por quê: quem precisa saber (a névoa da tela, os inimigos) pergunta aqui, em vez de cada
	//          um procurar o nó do seu jeito.
	public static AmbienteDaFase? DaFase(SceneTree arvore)
	{
		var achados = arvore.GetNodesInGroup("ambiente_da_fase");
		return achados.Count > 0 ? achados[0] as AmbienteDaFase : null;
	}

	public bool EscondeOQueNaoSeVe => Tipo != TipoDeAmbiente.Limpo;

	public override void _EnterTree()
	{
		AddToGroup("ambiente_da_fase");
	}

	// Alteração de IA - Revisar
	// O que faz: a tecla F4 percorre os ambientes: limpo → névoa → escuridão → breu total → limpo.
	// Por quê: **ferramenta de teste.** Permite ver o mesmo mapa nos três ambientes sem montar
	//          três mapas. O "breu total" é a escuridão com todas as luzes apagadas.
	public override void _Process(double delta)
	{
		if (!Input.IsActionJustPressed("debug_nevoa"))
		{
			return;
		}

		if (Tipo == TipoDeAmbiente.Limpo)
		{
			Tipo = TipoDeAmbiente.Nevoa;
		}
		else if (Tipo == TipoDeAmbiente.Nevoa)
		{
			Tipo = TipoDeAmbiente.Escuridao;
			ApagarTodasAsLuzes = false;
		}
		else if (!ApagarTodasAsLuzes)
		{
			ApagarTodasAsLuzes = true;
		}
		else
		{
			Tipo = TipoDeAmbiente.Limpo;
			ApagarTodasAsLuzes = false;
		}

		GD.Print($"Ambiente: {NomeDoAmbiente}");
	}

	public string NomeDoAmbiente => Tipo switch
	{
		TipoDeAmbiente.Limpo => "limpo",
		TipoDeAmbiente.Nevoa => "névoa",
		_ => ApagarTodasAsLuzes ? "breu total" : "escuridão"
	};
}

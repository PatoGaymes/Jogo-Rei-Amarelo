using Godot;

// Alteração de IA - Revisar
// O que faz: a tela de fim de jogo, quando o grupo inteiro cai — pergunta se o jogador quer voltar
//            ao último save ou ao menu.
// Por quê: pedido do PO em 06/10/2026, só com o básico: o save e o menu principal ainda não existem.
//          Por enquanto **os dois botões recomeçam o mapa com a equipe inicial**, e a tela avisa isso.
//          Quando o save e o menu existirem, é só trocar o que cada botão faz.
public partial class TelaDeFimDeJogo : CanvasLayer
{
	public static void Mostrar(Node dono)
	{
		var tela = new TelaDeFimDeJogo { Name = "TelaDeFimDeJogo", Layer = 30 };
		dono.AddChild(tela);
	}

	public override void _Ready()
	{
		var fundo = new ColorRect { Color = new Color(0.0f, 0.0f, 0.0f, 0.82f) };
		fundo.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		AddChild(fundo);

		var centro = new CenterContainer();
		centro.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		fundo.AddChild(centro);

		var coluna = new VBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
		coluna.AddThemeConstantOverride("separation", 22);
		centro.AddChild(coluna);

		var titulo = Estilos.Texto("Fim de jogo", 84, new Color(0.9f, 0.25f, 0.22f));
		titulo.HorizontalAlignment = HorizontalAlignment.Center;
		coluna.AddChild(titulo);

		var subtitulo = Estilos.Texto("O grupo inteiro caiu.", 30, Colors.White);
		subtitulo.HorizontalAlignment = HorizontalAlignment.Center;
		coluna.AddChild(subtitulo);

		coluna.AddChild(Botao("Voltar ao último save"));
		coluna.AddChild(Botao("Voltar ao menu"));

		var aviso = Estilos.Texto("Provisório: o save e o menu ainda não existem — os dois botões recomeçam o mapa com a equipe inicial.",
			20, new Color(0.7f, 0.7f, 0.75f), largura: 760);
		aviso.HorizontalAlignment = HorizontalAlignment.Center;
		coluna.AddChild(aviso);
	}

	private Button Botao(string texto)
	{
		var botao = new Button { Text = texto, CustomMinimumSize = new Vector2(460, 70) };
		botao.AddThemeFontSizeOverride("font_size", 28);
		botao.Pressed += Recomecar;
		return botao;
	}

	// Alteração de IA - Revisar
	// O que faz: recomeça — zera o combate e a equipe e recarrega o mapa.
	private void Recomecar()
	{
		GerenciadorDeCombate.Instancia?.Reiniciar();
		Equipe.Instancia?.Carregar();
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
		QueueFree();
	}
}

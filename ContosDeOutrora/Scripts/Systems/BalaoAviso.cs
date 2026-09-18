using Godot;

// Alteração de IA - Revisar
// O que faz: o balãozinho que aparece sobre a cabeça do inimigo mostrando o que ele está
//            sentindo — "?" quando estranha um barulho, "!" quando percebe o jogador.
// Por quê: sem esse aviso, o jogador não tem como saber que foi ouvido, e a furtividade vira
//          adivinhação. É o mesmo recurso usado em Metal Gear e Assassin's Creed: o balão
//          avisa que dá tempo de se esconder, e é isso que torna a situação jogável em vez
//          de injusta.
//
//          **Isto não é ferramenta de teste** — faz parte do jogo e continua ligado na versão
//          publicada, diferente do desenho dos raios.
public partial class BalaoAviso : Label3D
{
	// Alteração de IA - Revisar
	// O que faz: quanto tempo o balão leva para aparecer e para sumir.
	// Por quê: surgir e desaparecer de repente fica seco e chama menos atenção do que um
	//          aparecimento rápido. Some mais devagar do que aparece, para o jogador ter
	//          tempo de notar mesmo se estiver olhando para outro canto da tela.
	[Export]
	public float TempoParaAparecer { get; set; } = 0.12f;

	[Export]
	public float TempoParaSumir { get; set; } = 0.45f;

	private string _simbolo = "";
	private float _opacidade;
	private float _alvoDeOpacidade;

	public override void _Ready()
	{
		Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		NoDepthTest = true;
		FontSize = 96;
		OutlineSize = 24;
		Text = "";
		Modulate = new Color(1, 1, 1, 0);
	}

	// Alteração de IA - Revisar
	// O que faz: mostra o "?" — o inimigo ouviu algo mas não sabe o que é.
	// Por quê: é o aviso de que a janela para se esconder começou.
	public void MostrarDuvida()
	{
		_simbolo = "?";
		Modulate = new Color(1.0f, 0.9f, 0.35f, Modulate.A);
		OutlineModulate = new Color(0, 0, 0, Modulate.A);
		_alvoDeOpacidade = 1.0f;
	}

	// Alteração de IA - Revisar
	// O que faz: mostra o "!" — o inimigo percebeu o jogador.
	// Por quê: marca o fim da chance de passar despercebido. A cor vermelha separa à
	//          primeira vista de uma simples desconfiança.
	public void MostrarAlerta()
	{
		_simbolo = "!";
		Modulate = new Color(1.0f, 0.3f, 0.25f, Modulate.A);
		OutlineModulate = new Color(0, 0, 0, Modulate.A);
		_alvoDeOpacidade = 1.0f;
	}

	public void Esconder()
	{
		_alvoDeOpacidade = 0.0f;
	}

	public override void _Process(double delta)
	{
		float velocidade = _alvoDeOpacidade > _opacidade
			? 1.0f / Mathf.Max(0.01f, TempoParaAparecer)
			: 1.0f / Mathf.Max(0.01f, TempoParaSumir);

		_opacidade = Mathf.MoveToward(_opacidade, _alvoDeOpacidade, (float)delta * velocidade);

		Text = _opacidade > 0.01f ? _simbolo : "";
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, _opacidade);
		OutlineModulate = new Color(0, 0, 0, _opacidade);
	}
}

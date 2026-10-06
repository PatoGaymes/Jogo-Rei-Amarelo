using Godot;
using System.Collections.Generic;
using System.Linq;

// Alteração de IA - Revisar
// O que faz: a equipe do jogador — os 5 personagens, a formação com que entram na luta, quem é o
//            líder, quem morreu e quanto de vida e sanidade cada um tem entre um combate e outro.
//            Também abre e fecha o menu de equipe (tecla T).
// Por quê: a equipe existe fora de qualquer mapa, por isso fica num nó global (autoload "Equipe").
//          A vida e a sanidade continuam de uma luta para a outra — morrer é permanente e "volta ao
//          save" (decisão do PO). O Foco começa cheio em cada luta: é provisório, a regra de
//          recuperação de Foco ainda não foi definida.
public partial class Equipe : Node
{
	// Alteração de IA - Revisar
	// O que faz: um lugar da equipe — a ficha do personagem, a casa dele na formação e o estado.
	public sealed class Membro
	{
		public Membro(FichaDeCombatente ficha, Hex formacao)
		{
			Ficha = ficha;
			Formacao = formacao;
			Vida = ficha.Vida;
			Sanidade = ficha.Sanidade;
		}

		public FichaDeCombatente Ficha { get; }
		public Hex Formacao { get; set; }
		public bool Morto { get; set; }
		public int Vida { get; set; }
		public int Sanidade { get; set; }
	}

	public const string CaminhoDaEquipeInicial = "res://Resources/Equipe/EquipeInicial.tres";

	// Alteração de IA - Revisar
	// O que faz: o tamanho da mini-região do menu de equipe — 2 casas em volta do centro, 19 casas.
	public const int RaioDaFormacao = 2;

	public static Equipe? Instancia { get; private set; }

	public List<Membro> Membros { get; } = new();
	public Membro? Lider { get; private set; }

	public IEnumerable<Membro> Vivos => Membros.Where(m => !m.Morto);

	private MenuDeEquipe? _menu;

	public override void _EnterTree()
	{
		Instancia = this;
		// continua recebendo a tecla T com o jogo pausado pelo próprio menu
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _Ready()
	{
		Carregar();
	}

	// Alteração de IA - Revisar
	// O que faz: monta a equipe a partir do arquivo de dados.
	// Por quê: também é o "voltar ao último save" provisório da tela de fim de jogo — o sistema de
	//          save ainda não existe, então voltar é recomeçar com a equipe inicial.
	public void Carregar()
	{
		Membros.Clear();
		Lider = null;

		var inicial = GD.Load<EquipeInicial>(CaminhoDaEquipeInicial);
		if (inicial == null)
		{
			GD.PushWarning($"Equipe: não achei {CaminhoDaEquipeInicial}. A equipe começa vazia.");
			return;
		}

		for (int i = 0; i < inicial.Membros.Count; i++)
		{
			Vector2I posicao = i < inicial.Formacao.Count ? inicial.Formacao[i] : new Vector2I(0, 0);
			Membros.Add(new Membro(inicial.Membros[i], new Hex(posicao.X, posicao.Y)));
		}

		if (Membros.Count > 0)
		{
			Lider = Membros[Mathf.Clamp(inicial.Lider, 0, Membros.Count - 1)];
		}
	}

	// Alteração de IA - Revisar
	// O que faz: troca o líder — usado quando o líder morre e o grupo sobrevive.
	public void DefinirLider(Membro membro)
	{
		Lider = membro;
	}

	// Alteração de IA - Revisar
	// O que faz: põe um membro numa casa da formação. Se já houver alguém lá, os dois trocam.
	// Por quê: é o "arrastar" do menu de equipe. Trocar, em vez de recusar, deixa reorganizar a
	//          formação sem precisar de uma casa vazia de passagem.
	public bool MoverNaFormacao(Membro membro, Hex destino)
	{
		if (Hex.Distancia(destino, new Hex(0, 0)) > RaioDaFormacao)
		{
			return false;
		}

		Membro? ocupante = Membros.FirstOrDefault(m => m != membro && m.Formacao == destino);
		if (ocupante != null)
		{
			ocupante.Formacao = membro.Formacao;
		}
		membro.Formacao = destino;
		return true;
	}

	// Alteração de IA - Revisar
	// O que faz: a tecla T abre e fecha o menu de equipe — fora de combate.
	// Por quê: no meio de uma luta a formação já foi usada; mudar ali não teria efeito.
	public override void _UnhandledInput(InputEvent evento)
	{
		if (!evento.IsActionPressed("menu_equipe"))
		{
			return;
		}

		if (_menu != null && IsInstanceValid(_menu))
		{
			FecharMenu();
			GetViewport().SetInputAsHandled();
			return;
		}

		if (GerenciadorDeCombate.Instancia?.EmCombate == true)
		{
			return;
		}

		AbrirMenu();
		GetViewport().SetInputAsHandled();
	}

	public void AbrirMenu()
	{
		if (_menu != null && IsInstanceValid(_menu))
		{
			return;
		}

		_menu = new MenuDeEquipe { Name = "MenuDeEquipe" };
		AddChild(_menu);
		_menu.Fechou += FecharMenu;
		GetTree().Paused = true;
	}

	public void FecharMenu()
	{
		if (_menu != null && IsInstanceValid(_menu))
		{
			_menu.QueueFree();
		}
		_menu = null;
		GetTree().Paused = false;
	}

	public bool MenuAberto => _menu != null && IsInstanceValid(_menu);
}

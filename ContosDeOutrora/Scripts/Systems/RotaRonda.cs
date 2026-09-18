using Godot;
using System.Collections.Generic;

// Alteração de IA - Revisar
// O que faz: guarda o caminho que o inimigo percorre na ronda e entrega o próximo ponto
//            sempre que ele chega no anterior.
// Por quê: a rota é desenhada na cena, arrastando nós vazios para onde o inimigo deve
//          passar. Assim o level designer monta a patrulha sem escrever código.
public partial class RotaRonda : Node3D
{
	// Alteração de IA - Revisar
	// O que faz: escolhe como o inimigo percorre os pontos.
	// Por quê: "Circuito" serve para uma volta fechada (ele dá voltas no mesmo caminho).
	//          "VaiEVolta" serve para corredores sem saída — ele vai até a ponta e refaz o
	//          caminho de trás para frente, em vez de teletransportar para o começo.
	public enum Estilo
	{
		Circuito,
		VaiEVolta
	}

	[Export]
	public Estilo Percurso { get; set; } = Estilo.Circuito;

	// Alteração de IA - Revisar
	// O que faz: aponta para um nó da fase cujos filhos são os pontos da ronda. Deixando vazio,
	//            valem os filhos deste próprio nó.
	// Por quê: a rota costuma pertencer ao **mapa**, não ao inimigo. Pendurando os pontos no
	//          inimigo, a patrulha anda junto com ele — mover o inimigo um metro na cena move a
	//          ronda inteira, e dois inimigos não podem dividir o mesmo trajeto. Apontando para
	//          um nó da fase, os pontos ficam onde o level designer os colocou e qualquer
	//          inimigo pode usar o mesmo caminho.
	//
	//          É um **caminho em texto**, e não uma ligação direta ao nó, porque o inimigo é uma
	//          cena montada à parte: uma ligação direta feita de dentro da fase não chega a ser
	//          resolvida e sempre chegava vazia aqui. O caminho em texto é resolvido na hora em
	//          que o inimigo entra na fase, quando os dois já existem.
	[Export]
	public NodePath CaminhoDaFase { get; set; } = new NodePath();

	// Alteração de IA - Revisar
	// O que faz: o nome do nó da fase que guarda os pontos da ronda, procurado automaticamente
	//            quando nada foi indicado à mão.
	// Por quê: é uma **combinação de nome**: chame de "MarcasDaRonda" o nó com os pontos e o
	//          inimigo acha sozinho, sem precisar ligar nada na cena. Isso existe porque ligar
	//          pela cena se mostrou frágil — o editor do Godot descarta propriedades que ele
	//          ainda não conhece (por exemplo, quando o código foi alterado mas o editor está
	//          com a versão antiga carregada) e regrava o arquivo sem elas, sem avisar.
	//
	//          Com o nome, a ligação mora no código e não pode ser perdida ao salvar a cena.
	//          Fases com **mais de uma rota** continuam usando o CaminhoDaFase acima, que tem
	//          prioridade sobre esta procura.
	[Export]
	public string NomeDoCaminhoNaFase { get; set; } = "MarcasDaRonda";

	private readonly List<Vector3> _pontos = new();
	private int _atual = -1;
	private int _passo = 1;

	public int QuantidadeDePontos => _pontos.Count;

	public override void _Ready()
	{
		// Alteração de IA - Revisar
		// O que faz: lê os nós filhos e usa a posição de cada um como ponto da rota.
		// Por quê: qualquer nó serve de marcador. Basta arrastá-lo na cena para mudar o
		//          trajeto, sem mexer em número nenhum no código.
		Node3D dono = AcharDonoDosPontos();

		foreach (Node filho in dono.GetChildren())
		{
			if (filho is Node3D marcador)
			{
				_pontos.Add(marcador.GlobalPosition);
			}
		}

		if (_pontos.Count == 0)
		{
			GD.PushWarning("RotaRonda: nenhum ponto encontrado. " +
						   $"Crie um nó chamado '{NomeDoCaminhoNaFase}' na fase com os pontos " +
						   "da ronda como filhos, aponte 'CaminhoDaFase' para outro nó, ou " +
						   "adicione nós filhos (Marker3D) neste nó.");
		}
	}

	// Alteração de IA - Revisar
	// O que faz: decide de onde vêm os pontos da ronda, em ordem de prioridade.
	// Por quê: são três formas de montar uma ronda, da mais explícita para a mais automática.
	//          Ter uma ordem clara evita surpresa: o que o level designer indicou à mão sempre
	//          ganha; a procura pelo nome é só a rede de segurança para o caso comum.
	//
	//            1. o nó indicado à mão em 'CaminhoDaFase'
	//            2. os próprios nós filhos deste nó, se houver
	//            3. um nó da fase com o nome combinado (padrão "MarcasDaRonda")
	private Node3D AcharDonoDosPontos()
	{
		if (!CaminhoDaFase.IsEmpty)
		{
			Node3D? indicado = GetNodeOrNull<Node3D>(CaminhoDaFase);
			if (indicado != null)
			{
				return indicado;
			}
			GD.PushWarning($"RotaRonda: não achei o nó '{CaminhoDaFase}' indicado em " +
						   "'CaminhoDaFase'. Confira o caminho na cena da fase.");
		}

		foreach (Node filho in GetChildren())
		{
			if (filho is Node3D)
			{
				return this;
			}
		}

		if (!string.IsNullOrEmpty(NomeDoCaminhoNaFase))
		{
			Node? naFase = GetTree().Root.FindChild(NomeDoCaminhoNaFase, true, false);
			if (naFase is Node3D achado)
			{
				return achado;
			}
		}

		return this;
	}

	// Alteração de IA - Revisar
	// O que faz: devolve o próximo ponto do trajeto.
	// Por quê: no circuito, ao passar do último volta para o primeiro. No vai e volta, ao
	//          chegar na ponta o sentido é invertido — é o que evita o inimigo "pular" de
	//          uma ponta à outra do corredor.
	public Vector3 ProximoPonto()
	{
		if (_pontos.Count == 0)
		{
			return GlobalPosition;
		}

		if (Percurso == Estilo.Circuito)
		{
			_atual = (_atual + 1) % _pontos.Count;
			return _pontos[_atual];
		}

		if (_atual < 0)
		{
			_atual = 0;
			return _pontos[0];
		}

		_atual += _passo;
		if (_atual >= _pontos.Count)
		{
			_atual = Mathf.Max(0, _pontos.Count - 2);
			_passo = -1;
		}
		else if (_atual < 0)
		{
			_atual = Mathf.Min(1, _pontos.Count - 1);
			_passo = 1;
		}
		return _pontos[_atual];
	}

	public Vector3 PontoEm(int indice) => _pontos[indice];
}

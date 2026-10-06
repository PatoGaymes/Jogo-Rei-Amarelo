using Godot;
using System;

// Alteração de IA - Revisar
// O que faz: o endereço de uma casa (hexágono) da grade de combate — duas coordenadas, Q e R.
// Por quê: a grade é de hexágonos, como no Pit People. Hexágono tem 6 vizinhas, todas à mesma
//          distância, e por isso "andar 3 casas" vale o mesmo em qualquer direção — o que não
//          acontece num quadriculado, onde a diagonal engana.
//
//          Os hexágonos ficam **de lado chato para cima** no mapa (topo reto). O endereço segue o
//          sistema "axial", o mais usado em jogos de hexágono: Q anda para leste, R para o sul.
public readonly struct Hex : IEquatable<Hex>
{
	public readonly int Q;
	public readonly int R;

	public Hex(int q, int r)
	{
		Q = q;
		R = r;
	}

	// Alteração de IA - Revisar
	// O que faz: as 6 direções vizinhas, começando pelo norte e girando no sentido horário.
	// Por quê: a ordem importa para girar a formação da equipe — passar de uma direção para a
	//          seguinte é girar 60 graus.
	public static readonly Hex[] Direcoes =
	{
		new(0, -1),  // norte
		new(1, -1),  // nordeste
		new(1, 0),   // sudeste
		new(0, 1),   // sul
		new(-1, 1),  // sudoeste
		new(-1, 0),  // noroeste
	};

	public static Hex operator +(Hex a, Hex b) => new(a.Q + b.Q, a.R + b.R);
	public static Hex operator -(Hex a, Hex b) => new(a.Q - b.Q, a.R - b.R);
	public static bool operator ==(Hex a, Hex b) => a.Q == b.Q && a.R == b.R;
	public static bool operator !=(Hex a, Hex b) => !(a == b);

	public bool Equals(Hex outro) => this == outro;
	public override bool Equals(object? obj) => obj is Hex h && this == h;
	public override int GetHashCode() => HashCode.Combine(Q, R);
	public override string ToString() => $"({Q}, {R})";

	public Hex Vizinho(int direcao) => this + Direcoes[((direcao % 6) + 6) % 6];

	// Alteração de IA - Revisar
	// O que faz: quantas casas separam duas casas, andando de vizinha em vizinha.
	// Por quê: é a régua do combate — movimento e alcance de ataque são contados nela.
	public static int Distancia(Hex a, Hex b)
	{
		int dq = a.Q - b.Q;
		int dr = a.R - b.R;
		return (Math.Abs(dq) + Math.Abs(dq + dr) + Math.Abs(dr)) / 2;
	}

	// Alteração de IA - Revisar
	// O que faz: gira o endereço 60 graus no sentido horário em volta da origem (0, 0).
	// Por quê: a formação é montada no menu de equipe com a frente "para cima". No mapa, a frente
	//          tem que apontar para os inimigos — e isso é girar a formação inteira de 60 em 60 graus.
	public Hex GirarHorario() => new(-R, Q + R);

	public Hex Girar(int passosHorarios)
	{
		Hex h = this;
		int passos = ((passosHorarios % 6) + 6) % 6;
		for (int i = 0; i < passos; i++)
		{
			h = h.GirarHorario();
		}
		return h;
	}

	// Alteração de IA - Revisar
	// O que faz: converte o endereço no ponto do mapa (só X e Z) onde fica o centro da casa.
	// Por quê: as contas do hexágono de topo reto. "raio" é a distância do centro a uma ponta.
	public static Vector2 CentroNoPlano(Hex h, float raio)
	{
		float x = raio * 1.5f * h.Q;
		float z = raio * Mathf.Sqrt(3.0f) * (h.R + h.Q * 0.5f);
		return new Vector2(x, z);
	}

	// Alteração de IA - Revisar
	// O que faz: descobre em que casa cai um ponto qualquer do mapa (só X e Z).
	// Por quê: é como o clique do mouse e a posição de um personagem viram uma casa da grade.
	//          O arredondamento é feito em três eixos ("cubo") porque arredondar Q e R soltos
	//          erra nas bordas entre hexágonos.
	public static Hex DoPonto(Vector2 ponto, float raio)
	{
		float qf = (2.0f / 3.0f * ponto.X) / raio;
		float rf = (-1.0f / 3.0f * ponto.X + Mathf.Sqrt(3.0f) / 3.0f * ponto.Y) / raio;
		return Arredondar(qf, rf);
	}

	private static Hex Arredondar(float qf, float rf)
	{
		float sf = -qf - rf;
		int q = Mathf.RoundToInt(qf);
		int r = Mathf.RoundToInt(rf);
		int s = Mathf.RoundToInt(sf);

		float dq = Mathf.Abs(q - qf);
		float dr = Mathf.Abs(r - rf);
		float ds = Mathf.Abs(s - sf);

		if (dq > dr && dq > ds)
		{
			q = -r - s;
		}
		else if (dr > ds)
		{
			r = -q - s;
		}
		return new Hex(q, r);
	}

	// Alteração de IA - Revisar
	// O que faz: a direção (0 a 5) que mais se parece com uma direção qualquer do mapa.
	// Por quê: serve para saber "para que lado ficam os inimigos" em termos de hexágono, e assim
	//          virar a formação da equipe para eles.
	public static int DirecaoMaisParecida(Vector2 direcaoNoPlano, float raio)
	{
		int melhor = 0;
		float melhorProduto = float.NegativeInfinity;
		for (int i = 0; i < 6; i++)
		{
			Vector2 d = CentroNoPlano(Direcoes[i], raio).Normalized();
			float produto = d.Dot(direcaoNoPlano.Normalized());
			if (produto > melhorProduto)
			{
				melhorProduto = produto;
				melhor = i;
			}
		}
		return melhor;
	}
}

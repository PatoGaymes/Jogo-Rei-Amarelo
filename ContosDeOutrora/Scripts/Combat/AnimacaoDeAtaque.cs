using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Alteração de IA - Revisar
// O que faz: o plano de um golpe — quem ataca, quem é atingido, a distância entre os dois, a que
//            distância a arma "encaixa" e, com isso, quanto o atacante precisa andar antes de golpear.
// Por quê: pedido do PO em 06/10/2026: a animação de ataque vai **atingir de verdade** o outro
//          personagem, e não ser uma animação fixa que nem encosta nele. Para isso ela precisa saber
//          a distância até o alvo e qual é o alvo. Calcular o plano separado da animação deixa o
//          desenho do golpe trocável: quando os quadros de ataque chegarem, eles usam este mesmo plano.
public readonly struct PlanoDoGolpe
{
	public Combatente Atacante { get; init; }
	public Combatente? Alvo { get; init; }
	public Vector3 Inicio { get; init; }
	public Vector3 PontoDoAlvo { get; init; }
	public float DistanciaAteOAlvo { get; init; }
	public float DistanciaDeEncaixe { get; init; }
	public float QuantoAndar { get; init; }
	public Vector3 PontoDoGolpe { get; init; }
	public Vector3 Direcao { get; init; }

	// Alteração de IA - Revisar
	// O que faz: monta o plano. "Encaixe" vem da habilidade (espada perto, lança mais longe).
	// Por quê: andar = distância até o alvo − encaixe. Um alvo vizinho a 1,73 m com espada (0,9 m)
	//          pede 0,83 m de passo; o mesmo alvo com lança (1,6 m) pede quase nada.
	public static PlanoDoGolpe Calcular(Combatente atacante, Combatente? alvo, Vector3 pontoDoAlvo, Habilidade habilidade)
	{
		Vector3 inicio = atacante.Corpo.GlobalPosition;
		Vector3 delta = pontoDoAlvo - inicio;
		Vector3 plano = new(delta.X, 0.0f, delta.Z);
		float distancia = plano.Length();
		Vector3 direcao = distancia > 0.001f ? plano / distancia : Vector3.Forward;
		float encaixe = Mathf.Clamp(habilidade.DistanciaDeEncaixe, 0.4f, Mathf.Max(0.4f, distancia));
		float andar = habilidade.Forma == Habilidade.FormaDoAtaque.CorpoACorpo ? Mathf.Max(0.0f, distancia - encaixe) : 0.0f;

		// a altura acompanha o caminho até o alvo (degrau, rampa)
		float fracao = distancia > 0.001f ? andar / distancia : 0.0f;
		Vector3 pontoDoGolpe = inicio + direcao * andar + Vector3.Up * (delta.Y * fracao);

		return new PlanoDoGolpe
		{
			Atacante = atacante,
			Alvo = alvo,
			Inicio = inicio,
			PontoDoAlvo = pontoDoAlvo,
			DistanciaAteOAlvo = distancia,
			DistanciaDeEncaixe = encaixe,
			QuantoAndar = andar,
			PontoDoGolpe = pontoDoGolpe,
			Direcao = direcao,
		};
	}
}

// Alteração de IA - Revisar
// O que faz: as animações do combate com a arte provisória — andar de casa em casa, o golpe corpo a
//            corpo (anda até encaixar, golpeia, volta), o disparo à distância e o gesto de usar uma
//            habilidade sem golpe.
// Por quê: as animações de ataque de verdade ainda estão sendo feitas pela equipe (o PO vai
//          detalhar depois). Isto é o "esqueleto": o tempo de cada fase e, principalmente, o
//          **momento do impacto** — o dano só aparece quando o golpe chega no alvo.
public static class AnimacaoDeAtaque
{
	// velocidades provisórias, em metros por segundo
	public const float VelocidadeDeAndar = 3.6f;
	public const float VelocidadeDoPassoDeAtaque = 4.5f;
	public const float VelocidadeDoDisparo = 16.0f;

	// Alteração de IA - Revisar
	// O que faz: toca o golpe e chama "noImpacto" no instante em que ele acerta.
	public static async Task Executar(Node dono, PlanoDoGolpe plano, Habilidade habilidade, Action noImpacto)
	{
		Combatente atacante = plano.Atacante;
		Virar(atacante, plano.PontoDoAlvo);

		switch (habilidade.Forma)
		{
			case Habilidade.FormaDoAtaque.CorpoACorpo:
				if (plano.QuantoAndar > 0.05f)
				{
					await Deslocar(dono, atacante, plano.Inicio, plano.PontoDoGolpe,
						plano.QuantoAndar / VelocidadeDoPassoDeAtaque, animarPassos: true);
				}

				// o golpe: um tranco para a frente e de volta
				Vector3 tranco = plano.PontoDoGolpe + plano.Direcao * 0.28f;
				await Deslocar(dono, atacante, plano.PontoDoGolpe, tranco, 0.09f, animarPassos: false);
				noImpacto();
				await Deslocar(dono, atacante, tranco, plano.PontoDoGolpe, 0.12f, animarPassos: false);

				if (plano.QuantoAndar > 0.05f)
				{
					// volta deslizando, ainda de frente para o alvo — andar de costas com a animação de
					// andar daria o "moonwalk"
					await Deslocar(dono, atacante, plano.PontoDoGolpe, plano.Inicio,
						plano.QuantoAndar / (VelocidadeDoPassoDeAtaque * 1.4f), animarPassos: false);
				}
				break;

			case Habilidade.FormaDoAtaque.ADistancia:
				await Disparar(dono, atacante.Corpo.GlobalPosition + Vector3.Up * 1.2f,
					plano.PontoDoAlvo + Vector3.Up * 1.1f, new Color(1.0f, 0.85f, 0.45f));
				noImpacto();
				break;

			default:
				await Pulinho(dono, atacante);
				noImpacto();
				break;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: anda pelas casas de um caminho, uma por uma, virando para cada uma.
	public static async Task AndarPeloCaminho(Node dono, Combatente quem, IReadOnlyList<Casa> caminho)
	{
		Vector3 atual = quem.Corpo.GlobalPosition;
		foreach (Casa casa in caminho)
		{
			Vector3 proximo = casa.Centro;
			Virar(quem, proximo);
			float distancia = (proximo - atual).Length();
			await Deslocar(dono, quem, atual, proximo, distancia / VelocidadeDeAndar, animarPassos: true);
			atual = proximo;
		}
		ParaCorpo(quem);
	}

	// Alteração de IA - Revisar
	// O que faz: leva o corpo de um ponto a outro em linha reta, no tempo pedido.
	// Por quê: "animarPassos" informa a velocidade ao corpo — é por ela que a AnimacaoDirecional sabe
	//          que deve tocar a animação de andar. Sem ela, o corpo desliza parado (usado no tranco do
	//          golpe e na volta).
	public static async Task Deslocar(Node dono, Combatente quem, Vector3 de, Vector3 para, float tempo, bool animarPassos)
	{
		if (!GodotObject.IsInstanceValid(quem.Corpo))
		{
			return;
		}

		tempo = Mathf.Max(0.01f, tempo);
		if (animarPassos && quem.Corpo is CharacterBody3D corpo)
		{
			corpo.Velocity = (para - de) / tempo;
		}

		quem.Corpo.GlobalPosition = de;
		Tween animacao = dono.CreateTween();
		animacao.TweenProperty(quem.Corpo, "global_position", para, tempo);
		await dono.ToSignal(animacao, Tween.SignalName.Finished);
		ParaCorpo(quem);
	}

	public static void ParaCorpo(Combatente quem)
	{
		if (GodotObject.IsInstanceValid(quem.Corpo) && quem.Corpo is CharacterBody3D corpo)
		{
			corpo.Velocity = Vector3.Zero;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: vira o personagem para um ponto — a direção que a AnimacaoDirecional usa para
	//            escolher frente, costas ou lado.
	public static void Virar(Combatente quem, Vector3 ponto)
	{
		if (!GodotObject.IsInstanceValid(quem.Corpo))
		{
			return;
		}

		Vector3 d = ponto - quem.Corpo.GlobalPosition;
		if (new Vector2(d.X, d.Z).LengthSquared() < 0.0001f)
		{
			return;
		}
		float graus = Mathf.PosMod(Mathf.RadToDeg(Mathf.Atan2(-d.Z, d.X)), 360.0f);

		switch (quem.Corpo)
		{
			case PlayerIsometrico jogador:
				jogador.OlharPara(graus);
				break;
			case CorpoDeCombate membro:
				membro.DirecaoOlhando = graus;
				break;
			case InimigoIA inimigo:
				inimigo.OlharPara(ponto);
				break;
		}
	}

	// Alteração de IA - Revisar
	// O que faz: uma bolinha brilhante que voa até o alvo — o disparo provisório (flecha, magia).
	private static async Task Disparar(Node dono, Vector3 de, Vector3 para, Color cor)
	{
		var bola = new MeshInstance3D
		{
			Mesh = new SphereMesh { Radius = 0.11f, Height = 0.22f },
			CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
			MaterialOverride = new StandardMaterial3D
			{
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
				AlbedoColor = cor,
				NoDepthTest = true,
				RenderPriority = 115,
			},
		};
		dono.GetTree().CurrentScene.AddChild(bola);
		bola.GlobalPosition = de;

		float tempo = Mathf.Max(0.08f, de.DistanceTo(para) / VelocidadeDoDisparo);
		Tween voo = dono.CreateTween();
		voo.TweenProperty(bola, "global_position", para, tempo);
		await dono.ToSignal(voo, Tween.SignalName.Finished);
		bola.QueueFree();
	}

	// Alteração de IA - Revisar
	// O que faz: um pulinho no lugar — o gesto provisório de usar uma habilidade sem golpe.
	private static async Task Pulinho(Node dono, Combatente quem)
	{
		Vector3 chao = quem.Corpo.GlobalPosition;
		await Deslocar(dono, quem, chao, chao + Vector3.Up * 0.3f, 0.12f, animarPassos: false);
		await Deslocar(dono, quem, chao + Vector3.Up * 0.3f, chao, 0.14f, animarPassos: false);
	}
}

using Godot;
using System;
using System.Collections.Generic;

// Alteração de IA - Revisar
// O que faz: desenha os hexágonos da grade no chão — o contorno de cada casa e, quando for o caso,
//            o preenchimento colorido (casa de aliado, de inimigo, para onde dá para andar...).
// Por quê: os hexágonos só aparecem durante o combate (decisão do PO em 06/10/2026). Fora dele,
//          este desenho fica vazio. As cores seguem a referência do Pit People: azul para os
//          aliados, vermelho para os inimigos, amarelo para quem está na vez.
//
//          O desenho é feito **por cima da névoa e da escuridão**, para a grade ficar legível. Como
//          o ambiente vai afetar o combate ainda está em aberto (WIP), isso é provisório.
public partial class DesenhoDaGrade : MeshInstance3D
{
	// Alteração de IA - Revisar
	// O que faz: a cor do contorno das casas e a altura do desenho acima do chão.
	[Export]
	public Color CorDoContorno { get; set; } = new(1.0f, 1.0f, 1.0f, 0.28f);

	[Export]
	public float AlturaSobreOChao { get; set; } = 0.04f;

	private readonly ArrayMesh _malha = new();

	public override void _Ready()
	{
		// desenhado em coordenadas do mundo, sem herdar a posição de quem o contém
		TopLevel = true;
		GlobalTransform = Transform3D.Identity;
		Mesh = _malha;
		CastShadow = ShadowCastingSetting.Off;
		MaterialOverride = new StandardMaterial3D
		{
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
			VertexColorUseAsAlbedo = true,
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
			CullMode = BaseMaterial3D.CullModeEnum.Disabled,
			// depois da névoa (prioridade 100): a grade continua legível no escuro
			RenderPriority = 110,
		};
	}

	// Alteração de IA - Revisar
	// O que faz: apaga o desenho.
	public void Limpar()
	{
		_malha.ClearSurfaces();
	}

	// Alteração de IA - Revisar
	// O que faz: desenha as casas pedidas. "preenchimento" devolve a cor de dentro de cada casa,
	//            ou null para só o contorno.
	// Por quê: redesenhar tudo a cada mudança é barato (algumas centenas de casas), e evita
	//          guardar um estado de cor por casa que poderia ficar velho.
	public void Desenhar(GradeDeCombate grade, IEnumerable<Casa> casas, Func<Casa, Color?> preenchimento)
	{
		_malha.ClearSurfaces();

		var desenho = new SurfaceTool();
		desenho.Begin(Mesh.PrimitiveType.Triangles);
		int triangulos = 0;

		foreach (Casa casa in casas)
		{
			Vector3[] fora = Pontas(grade, casa, 0.97f);
			Vector3[] dentro = Pontas(grade, casa, 0.88f);

			// contorno: uma faixa entre o hexágono de fora e o de dentro
			for (int i = 0; i < 6; i++)
			{
				int j = (i + 1) % 6;
				Triangulo(desenho, fora[i], fora[j], dentro[j], CorDoContorno);
				Triangulo(desenho, fora[i], dentro[j], dentro[i], CorDoContorno);
				triangulos += 2;
			}

			Color? cor = preenchimento(casa);
			if (cor.HasValue)
			{
				Vector3 centro = casa.Centro + Vector3.Up * AlturaSobreOChao;
				for (int i = 0; i < 6; i++)
				{
					Triangulo(desenho, centro, dentro[i], dentro[(i + 1) % 6], cor.Value);
					triangulos++;
				}
			}
		}

		if (triangulos > 0)
		{
			desenho.Commit(_malha);
		}
	}

	private Vector3[] Pontas(GradeDeCombate grade, Casa casa, float fracao)
	{
		var pontas = new Vector3[6];
		for (int i = 0; i < 6; i++)
		{
			Vector2 p = grade.PontaNoPlano(i, fracao);
			// a altura da ponta acompanha o chão medido (rampa), proporcional à distância do centro
			float altura = Mathf.Lerp(casa.Centro.Y, casa.AlturaDasPontas[i], fracao / 0.92f);
			pontas[i] = new Vector3(casa.Centro.X + p.X, altura + AlturaSobreOChao, casa.Centro.Z + p.Y);
		}
		return pontas;
	}

	private static void Triangulo(SurfaceTool desenho, Vector3 a, Vector3 b, Vector3 c, Color cor)
	{
		desenho.SetColor(cor);
		desenho.AddVertex(a);
		desenho.SetColor(cor);
		desenho.AddVertex(b);
		desenho.SetColor(cor);
		desenho.AddVertex(c);
	}
}

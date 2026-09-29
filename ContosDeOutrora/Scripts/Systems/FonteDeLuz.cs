using Godot;

// Alteração de IA - Revisar
// O que faz: um ponto que ilumina o que está em volta — fogueira, lamparina, tocha na mão.
// Por quê: nos mapas de escuridão, **sem luz não se vê nada**, nem o próprio personagem. É o
//          funcionamento do Don't Starve Together: a luz é o que desenha o mundo. Cada fonte de
//          luz abre um círculo onde o jogador consegue enxergar — e ainda assim só enxerga o que
//          estiver perto dele ou na direção em que olha.
//
//          A luz **não atravessa parede**: uma fogueira do outro lado do muro não clareia o lado
//          de cá. Isso é calculado uma vez para luzes paradas e só é refeito quando a luz anda
//          (uma tocha na mão, por exemplo), para não pesar.
//
//          Em mapas sem escuridão ela não faz nada — pode ficar montada no mapa sem custo.
public partial class FonteDeLuz : Node3D
{
	// Alteração de IA - Revisar
	// O que faz: até onde a luz alcança, em metros.
	// Por quê: é o tamanho do círculo iluminado. A borda é suave, não um corte seco: a luz
	//          começa a enfraquecer um pouco antes da metade do raio.
	[Export]
	public float Raio { get; set; } = 6.0f;

	// Alteração de IA - Revisar
	// O que faz: o quão forte é a luz, de 0 a 1.
	// Por quê: uma vela ilumina menos que uma fogueira do mesmo tamanho. Abaixo de 1 o lugar
	//          iluminado continua na penumbra.
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float Intensidade { get; set; } = 1.0f;

	// Alteração de IA - Revisar
	// O que faz: a cor que a luz dá ao que ilumina.
	// Por quê: fogo é alaranjado, lua é azulada. A cor tinge levemente o que está sob a luz, e é
	//          boa parte do clima da cena.
	[Export]
	public Color CorDaLuz { get; set; } = new(1.0f, 0.72f, 0.42f);

	[Export]
	public bool Acesa { get; set; } = true;

	// Alteração de IA - Revisar
	// O que faz: faz a luz oscilar de leve, como chama.
	// Por quê: fogo parado parece lâmpada. O tremor pequeno no tamanho do círculo é o que faz a
	//          tocha parecer viva.
	[Export]
	public bool Tremula { get; set; } = true;

	// Alteração de IA - Revisar
	// O que faz: desenha uma pequena marca brilhante no lugar da luz.
	// Por quê: enquanto não houver arte de fogueira e lamparina, é o que mostra onde a luz está.
	//          A tocha na mão do jogador não precisa dela.
	[Export]
	public bool MostrarMarca { get; set; } = true;

	// o raio de verdade neste instante, já com o tremor
	public float RaioAgora { get; private set; }

	private float _fase;

	public override void _EnterTree()
	{
		AddToGroup("fonte_de_luz");
	}

	public override void _Ready()
	{
		RaioAgora = Raio;
		_fase = GD.Randf() * 100.0f;   // cada chama treme no seu próprio ritmo

		if (MostrarMarca)
		{
			var marca = new MeshInstance3D
			{
				Name = "Marca",
				Mesh = new SphereMesh { Radius = 0.18f, Height = 0.36f },
				CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
				MaterialOverride = new StandardMaterial3D
				{
					ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
					AlbedoColor = CorDaLuz.Lightened(0.3f)
				}
			};
			AddChild(marca);
		}
	}

	public override void _Process(double delta)
	{
		if (!Tremula)
		{
			RaioAgora = Raio;
			return;
		}

		// duas ondas de ritmos diferentes: o tremor não se repete de forma perceptível
		_fase += (float)delta;
		float tremor = Mathf.Sin(_fase * 11.0f) * 0.6f + Mathf.Sin(_fase * 17.3f) * 0.4f;
		RaioAgora = Raio * (1.0f + tremor * 0.035f);
	}
}

using Godot;

// Alteração de IA - Revisar
// O que faz: o corpo de um membro da equipe que aparece só no combate — o desenho em pé, que vira
//            para onde anda, com a mesma escolha de frente/costas/lado do jogador.
// Por quê: na exploração só o líder anda pelo mapa. Quando a luta começa, os outros quatro entram
//          nas casas da formação montada no menu de equipe. Quando ela acaba, saem.
public partial class CorpoDeCombate : CharacterBody3D, IPersonagemQueOlha
{
	public float DirecaoOlhando { get; set; } = 270.0f;

	// Alteração de IA - Revisar
	// O que faz: monta o corpo de um personagem a partir da ficha dele.
	// Por quê: sem colisão — no combate quem decide onde cada um pode ficar é a grade, e corpos
	//          esbarrando uns nos outros só atrapalhariam as animações.
	public static CorpoDeCombate Criar(FichaDeCombatente ficha)
	{
		var corpo = new CorpoDeCombate
		{
			Name = $"Combate_{ficha.Nome.Replace(" ", "")}",
			CollisionLayer = 0,
			CollisionMask = 0,
		};

		var desenho = new AnimatedSprite3D { Name = "Desenho" };
		AplicarDesenho(desenho, ficha);
		corpo.AddChild(desenho);
		corpo.AddChild(new AnimacaoDirecional { Name = "AnimacaoDirecional" });
		return corpo;
	}

	// Alteração de IA - Revisar
	// O que faz: põe o desenho da ficha num sprite — a animação, se houver, ou a imagem provisória
	//            recortada no tamanho certo, com os pés no chão.
	// Por quê: as imagens provisórias vêm com sobra transparente em volta, de tamanhos diferentes.
	//          Recortar a sobra e escalar pela altura da ficha deixa todos do mesmo tamanho e com os
	//          pés no lugar certo, sem precisar mexer nos arquivos de arte.
	//          Devolve a altura do centro do desenho, para quem precisar (o jogador, ao agachar).
	public static float AplicarDesenho(AnimatedSprite3D desenho, FichaDeCombatente ficha)
	{
		desenho.Billboard = BaseMaterial3D.BillboardModeEnum.FixedY;
		desenho.Shaded = false;
		desenho.AlphaCut = SpriteBase3D.AlphaCutMode.Discard;
		desenho.TextureFilter = BaseMaterial3D.TextureFilterEnum.Linear;

		float alturaEmPixels;
		if (ficha.Animacoes != null)
		{
			desenho.SpriteFrames = ficha.Animacoes;
			string primeira = ficha.Animacoes.HasAnimation("andar_frente") ? "andar_frente" : ficha.Animacoes.GetAnimationNames()[0];
			desenho.Animation = primeira;
			Texture2D? quadro = ficha.Animacoes.GetFrameTexture(primeira, 0);
			alturaEmPixels = quadro?.GetHeight() ?? 400;
		}
		else
		{
			desenho.SpriteFrames = QuadrosDaImagem(ficha.Imagem, out alturaEmPixels);
			desenho.Animation = "andar_frente";
		}

		desenho.PixelSize = ficha.AlturaDoDesenho / Mathf.Max(1.0f, alturaEmPixels);
		float centro = ficha.AlturaDoDesenho * 0.5f;
		desenho.Position = new Vector3(0.0f, centro, 0.0f);
		return centro;
	}

	// Alteração de IA - Revisar
	// O que faz: transforma uma imagem parada nas três "animações" de um quadro só (frente, costas
	//            e lado), recortada sem a sobra transparente.
	// Por quê: assim o personagem provisório usa o mesmo caminho do animado (AnimacaoDirecional),
	//          e trocar pela arte de verdade é só preencher o campo Animacoes da ficha.
	private static SpriteFrames QuadrosDaImagem(Texture2D? imagem, out float alturaEmPixels)
	{
		var quadros = new SpriteFrames();
		alturaEmPixels = 400;
		if (imagem == null)
		{
			return quadros;
		}

		Texture2D textura = imagem;
		Image? pixels = imagem.GetImage();
		if (pixels != null)
		{
			if (pixels.IsCompressed())
			{
				pixels.Decompress();
			}
			Rect2I usado = pixels.GetUsedRect();
			if (usado.Size.X > 0 && usado.Size.Y > 0)
			{
				textura = new AtlasTexture { Atlas = imagem, Region = new Rect2(usado.Position, usado.Size) };
				alturaEmPixels = usado.Size.Y;
			}
			else
			{
				alturaEmPixels = imagem.GetHeight();
			}
		}

		foreach (string nome in new[] { "andar_frente", "andar_costas", "andar_lado" })
		{
			quadros.AddAnimation(nome);
			quadros.AddFrame(nome, textura);
		}
		return quadros;
	}
}

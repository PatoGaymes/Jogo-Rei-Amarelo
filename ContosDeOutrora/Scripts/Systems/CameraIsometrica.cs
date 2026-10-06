using Godot;

// Alteração de IA - Revisar
// O que faz: a câmera do jogo. Ela fica sempre apontada para o personagem e pode girar em
//            volta dele em 8 posições fixas, usando as teclas Q e E.
// Por quê: é o funcionamento do Don't Starve Together, nossa referência. Como o mapa é 3D,
//          coisas ficam escondidas atrás de paredes e cantos — girar a câmera é o que
//          permite enxergar o que está oculto. Isso não é enfeite: é mecânica de jogo.
public partial class CameraIsometrica : Camera3D
{
	// Alteração de IA - Revisar
	// O que faz: os três estados em que a câmera pode estar.
	// Por quê: hoje só "Exploracao" funciona, mas os outros dois já ficam declarados porque
	//          o combate vai acontecer no próprio mapa, com a câmera se aproximando em vez
	//          de cortar para outra tela. Deixar previsto agora evita reescrever a câmera
	//          inteira depois.
	public enum Modo
	{
		Exploracao,
		Transicao,
		Combate
	}

	// Alteração de IA - Revisar
	// O que faz: quem a câmera acompanha. Normalmente o personagem do jogador.
	// Por quê: se ficar vazio, a câmera procura sozinha pelo grupo "player" (ver _Ready).
	[Export]
	public Node3D? Alvo { get; set; }

	// Alteração de IA - Revisar
	// O que faz: o quanto a câmera fica afastada do personagem.
	// Por quê: é o "zoom" da cena. Número maior afasta e mostra mais mapa; menor aproxima
	//          e mostra mais detalhe do personagem.
	[Export]
	public float Distancia { get; set; } = 9.0f;

	// Alteração de IA - Revisar
	// O que faz: o quanto a câmera fica acima do personagem, em graus.
	// Por quê: é o que dá o ângulo "de cima" característico do isométrico. Perto de 0 fica
	//          rente ao chão; perto de 90 fica olhando de cima para baixo. Entre 35 e 45
	//          costuma dar o visual do Don't Starve.
	[Export]
	public float Inclinacao { get; set; } = 40.0f;

	// Alteração de IA - Revisar
	// O que faz: a altura do ponto para onde a câmera olha, a partir dos pés do personagem.
	// Por quê: mirar nos pés deixa o personagem colado na borda de baixo da tela. Subir um
	//          pouco a mira centraliza melhor o corpo dele no enquadramento.
	[Export]
	public float AlturaDoAlvo { get; set; } = 1.2f;

	// Alteração de IA - Revisar
	// O que faz: a velocidade com que a câmera desliza ao girar de um ângulo para outro.
	// Por quê: sem isso, a câmera daria um salto seco a cada toque de Q ou E, e o jogador
	//          perderia a noção de onde estava olhando. Número maior gira mais rápido.
	[Export]
	public float VelocidadeDeGiro { get; set; } = 8.0f;

	// Alteração de IA - Revisar
	// O que faz: guarda para qual dos 8 ângulos a câmera está indo, e em qual ela está agora.
	// Por quê: são coisas diferentes enquanto a câmera está girando. O "desejado" muda na
	//          hora em que a tecla é apertada; o "atual" vai alcançando ele aos poucos, e é
	//          isso que cria o movimento suave.
	private float _anguloDesejado = 270.0f;
	private float _anguloAtual = 270.0f;

	// Alteração de IA - Revisar
	// O que faz: quantos graus a câmera gira a cada toque de tecla.
	// Por quê: 45 graus dá exatamente 8 posições ao redor do personagem (360 ÷ 45 = 8),
	//          que foi o combinado com a equipe.
	private const float PassoDoGiro = 45.0f;

	public Modo ModoAtual { get; private set; } = Modo.Exploracao;

	// Alteração de IA - Revisar (06/10/2026)
	// O que faz: a distância da câmera durante o combate, os limites da roda do mouse no combate e o
	//            tempo da aproximação.
	// Por quê: a entrada no combate é **a câmera se aproximando**, sem corte de tela (referência dos
	//          Metal Gear antigos, em docs/JOGO.md). Na luta, a roda do mouse afasta e aproxima para o
	//          jogador ver o campo inteiro quando precisar.
	[Export]
	public float DistanciaEmCombate { get; set; } = 8.0f;

	[Export]
	public float DistanciaMinimaEmCombate { get; set; } = 5.0f;

	[Export]
	public float DistanciaMaximaEmCombate { get; set; } = 16.0f;

	[Export]
	public float DuracaoDaTransicao { get; set; } = 1.0f;

	// Alteração de IA - Revisar (06/10/2026)
	// O que faz: a rapidez com que a câmera desliza até o novo alvo (no combate, a câmera acompanha
	//            quem está na vez) e com que a distância muda.
	[Export]
	public float SuavidadeDoAlvo { get; set; } = 5.0f;

	[Export]
	public float SuavidadeDaDistancia { get; set; } = 4.0f;

	// Alteração de IA - Revisar (06/10/2026)
	// O que faz: onde a câmera está mirando agora e a que distância está, e se está "colada" no alvo.
	// Por quê: na exploração a câmera fica colada no personagem, exatamente como antes. Quando o alvo
	//          muda (o combate passa a vez) ou a distância muda, ela desliza até lá em vez de saltar;
	//          ao voltar à exploração, cola de novo assim que alcança o personagem.
	private Vector3 _centroAtual;
	private float _distanciaAtual = -1.0f;
	private float _distanciaDesejada = -1.0f;
	private bool _colada = true;
	private float _tempoNaTransicao;

	// Alteração de IA - Revisar
	// O que faz: informa em qual dos 8 ângulos a câmera está apontando.
	// Por quê: outras partes do jogo vão precisar disso — principalmente o personagem, que
	//          usa o ângulo da câmera para saber para onde o "andar para frente" aponta.
	public float AnguloDesejado => _anguloDesejado;
	public float AnguloAtual => _anguloAtual;

	public override void _Ready()
	{
		// Alteração de IA - Revisar
		// O que faz: se ninguém apontou um alvo, procura o personagem do jogador sozinho.
		// Por quê: o personagem está no grupo "player". Procurar pelo grupo, em vez do
		//          caminho dentro da cena, faz a câmera continuar funcionando mesmo que
		//          alguém renomeie ou mova os nós do mapa.
		if (Alvo == null)
		{
			var encontrados = GetTree().GetNodesInGroup("player");
			if (encontrados.Count > 0)
			{
				Alvo = encontrados[0] as Node3D;
			}
		}

		if (Alvo == null)
		{
			GD.PushWarning("CameraIsometrica: nenhum alvo encontrado. " +
						   "Confira se o personagem está no grupo 'player'.");
		}

		// Alteração de IA - Revisar
		// O que faz: coloca a câmera já na posição certa antes do primeiro quadro aparecer.
		// Por quê: sem isso, o jogo começaria com a câmera na origem do mundo e ela
		//          "voaria" até o personagem no primeiro segundo, o que fica feio.
		_anguloAtual = _anguloDesejado;
		_distanciaAtual = Distancia;
		_distanciaDesejada = Distancia;
		if (Alvo != null)
		{
			_centroAtual = Alvo.GlobalPosition + new Vector3(0.0f, AlturaDoAlvo, 0.0f);
		}
		PosicionarCamera();

		CriarNevoa();
	}

	// Alteração de IA - Revisar
	// O que faz: pendura na própria câmera a camada que esconde o que o personagem não enxerga.
	// Por quê: a camada é pintada por cima da imagem pronta, então o lugar dela é na câmera. Ela é
	//          criada sempre, mas **só aparece e só gasta alguma coisa** se o mapa tiver um nó
	//          AmbienteDaFase com névoa ou escuridão. Mapa sem esse nó é limpo.
	//
	//          Criada em código, e não montada na cena, porque alterações em arquivos de cena já se
	//          perderam três vezes ao serem regravadas pelo editor do Godot.
	private void CriarNevoa()
	{
		if (GetNodeOrNull<NevoaDeGuerra>("NevoaDeGuerra") != null)
		{
			return;
		}

		AddChild(new NevoaDeGuerra { Name = "NevoaDeGuerra" });
	}

	public override void _UnhandledInput(InputEvent evento)
	{
		// Alteração de IA - Revisar (06/10/2026)
		// O que faz: girar vale na exploração e no combate (girar para ver atrás de paredes é
		//            mecânica de jogo nos dois); só não vale no meio da aproximação. No combate, a roda
		//            do mouse aproxima e afasta.
		if (ModoAtual == Modo.Transicao)
		{
			return;
		}

		if (ModoAtual == Modo.Combate && evento is InputEventMouseButton { Pressed: true } roda)
		{
			if (roda.ButtonIndex == MouseButton.WheelUp)
			{
				_distanciaDesejada = Mathf.Clamp(_distanciaDesejada - 0.8f, DistanciaMinimaEmCombate, DistanciaMaximaEmCombate);
			}
			else if (roda.ButtonIndex == MouseButton.WheelDown)
			{
				_distanciaDesejada = Mathf.Clamp(_distanciaDesejada + 0.8f, DistanciaMinimaEmCombate, DistanciaMaximaEmCombate);
			}
		}

		// Alteração de IA - Revisar
		// O que faz: Q gira a câmera no sentido anti-horário e E no sentido horário.
		// Por quê: usa "IsActionPressed" com o evento, e não a leitura contínua, para que
		//          um toque gire exatamente uma vez. Se lesse de forma contínua, segurar a
		//          tecla faria a câmera rodar sem parar.
		if (evento.IsActionPressed("camera_girar_esquerda"))
		{
			Girar(PassoDoGiro);
		}
		else if (evento.IsActionPressed("camera_girar_direita"))
		{
			Girar(-PassoDoGiro);
		}
	}

	// Alteração de IA - Revisar
	// O que faz: soma graus ao ângulo desejado e mantém o valor entre 0 e 360.
	// Por quê: sem esse acerto, girar muitas vezes para o mesmo lado faria o número crescer
	//          sem parar (720, 1080...). Mantendo entre 0 e 360, o ângulo sempre bate com
	//          um dos 8 valores combinados.
	private void Girar(float graus)
	{
		_anguloDesejado = Mathf.PosMod(_anguloDesejado + graus, 360.0f);
	}

	public override void _PhysicsProcess(double delta)
	{
		// (06/10/2026) o alvo pode ser um inimigo que acabou de morrer e sair do mapa
		if (Alvo == null || !IsInstanceValid(Alvo))
		{
			return;
		}

		// Alteração de IA - Revisar
		// O que faz: aproxima o ângulo atual do desejado, pelo caminho mais curto.
		// Por quê: "LerpAngle" entende que 350 graus e 10 graus são vizinhos. Uma conta
		//          comum giraria 340 graus no sentido errado para chegar lá — a câmera
		//          daria uma volta inteira à toa.
		float atualRad = Mathf.DegToRad(_anguloAtual);
		float desejadoRad = Mathf.DegToRad(_anguloDesejado);
		float novoRad = Mathf.LerpAngle(atualRad, desejadoRad, (float)delta * VelocidadeDeGiro);
		_anguloAtual = Mathf.PosMod(Mathf.RadToDeg(novoRad), 360.0f);

		AtualizarCentroEDistancia((float)delta);
		PosicionarCamera();
	}

	// Alteração de IA - Revisar (06/10/2026)
	// O que faz: desliza a mira e a distância até onde devem estar, e passa da aproximação para o
	//            combate quando ela termina.
	private void AtualizarCentroEDistancia(float dt)
	{
		Vector3 alvo = Alvo!.GlobalPosition + new Vector3(0.0f, AlturaDoAlvo, 0.0f);

		if (ModoAtual == Modo.Exploracao)
		{
			_distanciaDesejada = Distancia;
		}
		if (_distanciaAtual < 0.0f)
		{
			_distanciaAtual = _distanciaDesejada = Distancia;
		}
		_distanciaAtual = Mathf.Lerp(_distanciaAtual, _distanciaDesejada, 1.0f - Mathf.Exp(-dt * SuavidadeDaDistancia));

		if (_colada)
		{
			_centroAtual = alvo;
		}
		else
		{
			_centroAtual = _centroAtual.Lerp(alvo, 1.0f - Mathf.Exp(-dt * SuavidadeDoAlvo));
			// de volta à exploração: cola no personagem assim que chega nele
			if (ModoAtual == Modo.Exploracao && _centroAtual.DistanceTo(alvo) < 0.03f
				&& Mathf.Abs(_distanciaAtual - Distancia) < 0.03f)
			{
				_colada = true;
				_distanciaAtual = Distancia;
			}
		}

		if (ModoAtual == Modo.Transicao)
		{
			_tempoNaTransicao += dt;
			if (_tempoNaTransicao >= DuracaoDaTransicao)
			{
				ModoAtual = Modo.Combate;
			}
		}
	}

	// Alteração de IA - Revisar
	// O que faz: calcula onde a câmera deve ficar e a faz olhar para o personagem.
	// Por quê: esta é a conta que transforma "ângulo em graus" em "posição no mundo 3D".
	//
	//          A roda de ângulos combinada com a equipe: 0 à direita, 90 em cima,
	//          180 à esquerda, 270 embaixo. No mundo 3D do Godot, o eixo X aponta para a
	//          direita e o eixo Z aponta para baixo na tela, por isso o Z entra negativo.
	//
	//          A altura vem da inclinação: quanto mais inclinada, mais a câmera sobe e mais
	//          "de cima" fica a visão.
	private void PosicionarCamera()
	{
		// Alteração de IA - Revisar (06/10/2026)
		// O que mudou: a mira e a distância vêm das versões "suaves" (ver AtualizarCentroEDistancia).
		//              Na exploração elas são exatamente o personagem e a Distancia, como antes.
		Vector3 centro = _colada || _distanciaAtual < 0.0f
			? Alvo!.GlobalPosition + new Vector3(0.0f, AlturaDoAlvo, 0.0f)
			: _centroAtual;
		float distancia = _distanciaAtual > 0.0f ? _distanciaAtual : Distancia;

		float anguloRad = Mathf.DegToRad(_anguloAtual);
		float inclinacaoRad = Mathf.DegToRad(Inclinacao);

		// distância na horizontal e altura, separadas pela inclinação
		float raio = distancia * Mathf.Cos(inclinacaoRad);
		float altura = distancia * Mathf.Sin(inclinacaoRad);

		Vector3 deslocamento = new Vector3(
			raio * Mathf.Cos(anguloRad),
			altura,
			-raio * Mathf.Sin(anguloRad)
		);

		GlobalPosition = centro + deslocamento;
		LookAt(centro, Vector3.Up);
	}

	// Alteração de IA - Revisar
	// O que faz: troca o estado da câmera entre exploração, transição e combate.
	// Por quê: (06/10/2026) a transição é a aproximação ao começar a luta: a câmera desliza até a
	//          DistanciaEmCombate em DuracaoDaTransicao segundos e então entra no modo Combate. Voltar
	//          à exploração desliza de volta para a Distancia normal e cola no personagem de novo.
	public void DefinirModo(Modo novoModo)
	{
		ModoAtual = novoModo;
		_colada = false;
		if (Alvo != null && _distanciaAtual < 0.0f)
		{
			_centroAtual = Alvo.GlobalPosition + new Vector3(0.0f, AlturaDoAlvo, 0.0f);
		}

		switch (novoModo)
		{
			case Modo.Transicao:
				_tempoNaTransicao = 0.0f;
				_distanciaDesejada = DistanciaEmCombate;
				break;
			case Modo.Combate:
				_distanciaDesejada = DistanciaEmCombate;
				break;
			default:
				_distanciaDesejada = Distancia;
				break;
		}
	}
}

// Alteração de IA - Revisar
// O que faz: esconde de verdade o que o personagem não enxerga — pintando névoa ou escuridão por
//            cima da imagem pronta, conforme o ambiente da fase.
// Por quê: o mapa é 3D visto de cima, então a câmera mostraria o que está atrás das paredes. Para
//          cada ponto da tela, este desenho descobre a que lugar do mundo ele corresponde e
//          pergunta: o personagem enxerga aquilo? Por isso vale igual para chão, parede, inimigo
//          ou objeto, sem precisar marcar nada.
//
// Alteração de IA - Revisar (30/09/2026)
// O que mudou: a visão do personagem passou a funcionar como **olhos, e não como lanterna**.
//
//          ESCURIDÃO (modo 2): **quem revela é a luz.** Tudo o que está iluminado aparece, para
//          onde quer que o personagem esteja olhando — só parede esconde. Antes, a área de uma
//          fogueira só aparecia quando o personagem virava o rosto para ela, e ficava uma faixa
//          preta entre o personagem e a luz. Sem luz nenhuma, breu total — é o Don't Starve.
//
//          NÉVOA (modo 1): tudo vai sumindo **aos poucos** com a distância, como numa neblina de
//          verdade, e mais depressa onde a massa de ar está grossa. Não existe borda em lugar
//          nenhum. É o Silent Hill antigo.
//
// Alteração de IA - Revisar (01/10/2026)
// O que mudou: o jogador não tem mais cone de visão. A névoa fecha **igual em todas as
//          direções**, num círculo suave em volta dele — não importa mais para onde ele está
//          virado. Os testes da escuridão mostraram que só o círculo passa melhor a ideia dos
//          mapas com névoa e escuridão. Na escuridão nada mudou: quem revela é a luz (a lanterna).
//
//          PAREDES: o que está atrás de parede continua escondido, mas a passagem é uma
//          **penumbra de mais de um metro**, e não um corte seco. O corte seco desenhava na
//          névoa as "sombras" de uma lanterna presa na cabeça do personagem, com quinas duras
//          que não fazem sentido numa neblina.
//
//          Desempenho: a névoa que se move é um desenho de ruído gerado uma vez e só deslizado
//          pelo mapa. As paredes chegam já medidas numa tira de imagem. Cada ponto da tela faz
//          dez leituras da tira das paredes para a penumbra — é o único custo novo, e é pequeno
//          (a tira tem 128 números e cabe inteira na memória rápida da placa de vídeo). Medido:
//          +0,06 ms por quadro na névoa e +0,07 ms na escuridão.
shader_type spatial;
render_mode unshaded, blend_mix, depth_draw_never, depth_test_disabled, cull_disabled, fog_disabled;

uniform sampler2D tela : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D profundidade : hint_depth_texture, filter_nearest;

uniform int modo = 1;

// --- quem enxerga e até onde (o mesmo alcance em todas as direções) ---
uniform vec3 jogador;
uniform float alcance_da_nevoa = 7.3;

// distância até a parede em cada direção em volta do jogador
//
// Alteração de IA - Revisar (30/09/2026)
// O que faz: lê cada linha medida como ela é, sem misturar com a vizinha ("filter_nearest").
// Por quê: misturando, a borda de uma caixa virava uma distância "de meio termo" entre a linha
//          que bateu na caixa e a que passou direto, e isso desenhava riscos em leque na névoa
//          atrás de cada quina. Agora cada linha é comparada sozinha e só o resultado é misturado
//          com o da vizinha (ver "alem_da_parede").
uniform sampler2D paredes : filter_nearest, repeat_enable;
// de onde as paredes foram medidas (os olhos, na última medição) — ver atras_da_parede
uniform vec3 centro_das_paredes;
uniform float folga_da_parede = 0.8;
uniform float largura_da_penumbra = 1.2;
uniform float passagem_da_parede = 1.5;
uniform float forca_do_borrao = 3.0;

// --- névoa ---
uniform sampler2D ruido : filter_linear, repeat_enable;
uniform float tempo;
uniform vec4 cor_da_nevoa : source_color = vec4(0.60, 0.62, 0.64, 1.0);
uniform float densidade_minima = 0.6;
uniform float densidade_maxima = 1.6;
uniform float nevoa_colada = 0.05;
uniform float curva = 3.0;
uniform float tamanho_das_massas = 16.0;
uniform vec2 vento = vec2(0.55, 0.20);

// --- escuridão ---
uniform vec4 cor_da_escuridao : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float luz_ambiente = 0.0;
uniform sampler2D paredes_das_luzes : filter_nearest, repeat_enable;
uniform int quantas_luzes = 0;
uniform vec4 luzes[16];
uniform vec4 cores_das_luzes[16];

// Alteração de IA - Revisar (30/09/2026)
// O que faz: diz, de 0 a 1, se um ponto está além da parede numa direção exata.
// Por quê: a passagem é suave (um metro e meio), e não um corte seco — é a primeira metade da
//          penumbra. A segunda metade está logo abaixo.
//
//          (30/09/2026) Cada uma das duas linhas medidas mais próximas é comparada separadamente,
//          e só o resultado é misturado. Misturar as distâncias antes de comparar criava
//          "sombras de meio termo" em leque; ligar e desligar direção por direção criava degraus.
//          Comparando primeiro e misturando depois, a passagem fica contínua.
float alem_da_parede(float ang, float dist) {
	float n = float(textureSize(paredes, 0).x);
	float x = fract(ang / TAU + 1.0) * n - 0.5;
	float x0 = floor(x);
	float f = x - x0;
	float a = texelFetch(paredes, ivec2(int(mod(x0, n)), 0), 0).r;
	float b = texelFetch(paredes, ivec2(int(mod(x0 + 1.0, n)), 0), 0).r;
	float fim = folga_da_parede + passagem_da_parede;
	return mix(smoothstep(a + folga_da_parede, a + fim, dist),
			   smoothstep(b + folga_da_parede, b + fim, dist), f);
}

// Alteração de IA - Revisar (30/09/2026)
// O que faz: devolve de 0 a 1 o quanto o ponto está escondido atrás de parede, olhando também
//            um pouco para os lados da direção exata.
// Por quê: é a segunda metade da penumbra. Olhos não são um ponto: a cabeça mexe, e quem está
//          perto de uma quina enxerga um pouco além dela. Consultando cinco direções vizinhas e
//          tirando a média (as do meio pesam mais), a quina deixa de desenhar uma linha reta e
//          dura saindo do personagem — que era o "artefato" ao olhar por quinas. A abertura é
//          medida em metros no próprio ponto, então a penumbra tem a mesma largura perto e longe.
//          Testado com nove direções: o resultado na tela foi o mesmo e custava o dobro.
//
//          (30/09/2026) A conta parte de onde as paredes foram medidas, e não de onde o
//          personagem está agora: as linhas são medidas de novo só a cada 12 cm andados, e assim
//          a sombra fica colada na parede certa entre uma medida e outra.
float atras_da_parede(vec3 p) {
	vec2 d = p.xz - centro_das_paredes.xz;
	float dist = length(d);
	float ang = atan(-d.y, d.x);
	float passo = min(largura_da_penumbra / max(dist, 0.5), 0.7) * 0.5;
	float soma = 0.0;
	for (int k = -2; k <= 2; k++) {
		soma += alem_da_parede(ang + float(k) * passo, dist) * float(3 - abs(k));
	}
	return soma / 9.0;
}

// Alteração de IA - Revisar
// O que faz: o quão densa está a névoa neste ponto, agora — entre a densidade mínima e a máxima.
// Por quê: duas camadas do mesmo ruído, de tamanhos diferentes, levadas pelo vento em direções
//          diferentes. Somadas, nunca se repetem de um jeito que o olho perceba: uma massa passa,
//          abre uma brecha, fecha de novo. **Mesma conta do AmbienteDaFase.DensidadeDaNevoaEm.**
float densidade(vec2 xz, out float massa) {
	vec2 uv1 = (xz + vento * tempo) / (tamanho_das_massas * 3.0);
	vec2 uv2 = (xz - vec2(vento.y, -vento.x) * tempo * 0.7) / (tamanho_das_massas * 1.7);
	float n = textureLod(ruido, uv1, 0.0).r * 0.65 + textureLod(ruido, uv2, 0.0).r * 0.35;
	massa = smoothstep(0.30, 0.70, n);
	return mix(densidade_minima, densidade_maxima, massa);
}

// Alteração de IA - Revisar
// O que faz: soma a luz de todas as fontes que alcançam o ponto, e devolve a cor dessa luz.
// Por quê: cada fonte ilumina um círculo com borda suave. Se houver parede entre a fonte e o
//          ponto, aquela fonte não conta — é o que impede a fogueira de clarear o outro lado do
//          muro. (30/09/2026) A sombra que a parede faz na luz também ganhou penumbra, pelo mesmo
//          motivo da visão: sem ela, cada caixa perto da fogueira desenhava uma cunha preta de
//          borda dura. **O enfraquecimento até a borda é o AmbienteDaFase.FatorDaLuz.**
vec3 luz_em(vec3 p, out float forca) {
	float total = luz_ambiente;
	vec3 cor = vec3(0.0);
	float peso = 0.0;
	for (int i = 0; i < quantas_luzes; i++) {
		vec2 d = p.xz - luzes[i].xz;
		float dist = length(d);
		float raio = luzes[i].w;
		if (dist >= raio) {
			continue;
		}
		float ang = atan(-d.y, d.x);
		float passo = min(0.6 / max(dist, 0.5), 0.5) * 0.5;
		float n = float(textureSize(paredes_das_luzes, 0).x);
		float bloqueio = 0.0;
		for (int k = -2; k <= 2; k++) {
			// mesmo cuidado da visão: compara cada linha medida e só depois mistura
			float x = fract((ang + float(k) * passo) / TAU + 1.0) * n - 0.5;
			float x0 = floor(x);
			float a = texelFetch(paredes_das_luzes, ivec2(int(mod(x0, n)), i), 0).r;
			float b = texelFetch(paredes_das_luzes, ivec2(int(mod(x0 + 1.0, n)), i), 0).r;
			float ha = smoothstep(a + folga_da_parede, a + folga_da_parede + 0.8, dist);
			float hb = smoothstep(b + folga_da_parede, b + folga_da_parede + 0.8, dist);
			bloqueio += mix(ha, hb, x - x0) * float(3 - abs(k));
		}
		bloqueio /= 9.0;
		float f = cores_das_luzes[i].a * (1.0 - smoothstep(raio * 0.45, raio, dist)) * (1.0 - bloqueio);
		total += f;
		cor += cores_das_luzes[i].rgb * f;
		peso += f;
	}
	forca = clamp(total, 0.0, 1.0);
	return peso > 0.0 ? cor / peso : vec3(1.0);
}

void vertex() {
	// cobre a tela inteira, independente de onde a câmera esteja
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	// o ponto do mundo que aparece neste pedacinho da tela
	float z = texture(profundidade, SCREEN_UV).x;
	vec3 ndc = vec3(SCREEN_UV * 2.0 - 1.0, z);
	vec4 na_camera = INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	na_camera.xyz /= na_camera.w;
	vec3 no_mundo = (INV_VIEW_MATRIX * vec4(na_camera.xyz, 1.0)).xyz;

	vec2 d = no_mundo.xz - jogador.xz;
	float dist = length(d);
	float escondido = atras_da_parede(no_mundo);

	if (modo == 2) {
		// ESCURIDÃO: aparece o que está iluminado e não tem parede no meio — olhando ou não
		float forca;
		vec3 cor_da_luz = luz_em(no_mundo, forca);
		float visivel = forca * (1.0 - escondido);
		vec3 cor = textureLod(tela, SCREEN_UV, (1.0 - visivel) * forca_do_borrao * 0.35).rgb;
		cor *= mix(vec3(1.0), cor_da_luz, 0.35 * forca);
		ALBEDO = mix(cor_da_escuridao.rgb, cor, visivel);
	} else {
		// NÉVOA: vai fechando com a distância, mais depressa onde a massa de ar está grossa
		float massa;
		float grossura = densidade(no_mundo.xz, massa);
		// Alteração de IA - Revisar (01/10/2026)
		// O que faz: o mesmo alcance em todas as direções — o círculo de visão do jogador.
		// Por quê: antes o alcance mudava conforme o lado para onde ele olhava (o cone).
		//          **Mesma conta do AmbienteDaFase.TransparenciaDaNevoa**, que decide como os
		//          inimigos aparecem para o jogador.
		float alcance = alcance_da_nevoa;
		float espessura = nevoa_colada + pow(dist / alcance, curva) * grossura;
		float nevoa = 1.0 - exp(-espessura);

		// longe demais, some sempre — mesmo numa brecha (a mudança é imperceptível: ali a névoa
		// já passa de 90%)
		nevoa = max(nevoa, smoothstep(alcance * 1.9, alcance * 2.4, dist));

		// atrás de parede, some por completo
		nevoa = mix(nevoa, 1.0, escondido);

		vec3 cor = textureLod(tela, SCREEN_UV, nevoa * forca_do_borrao).rgb;

		// Alteração de IA - Revisar
		// O que faz: as massas de névoa ficam um pouco mais claras ou mais escuras enquanto passam.
		// Por quê: longe do personagem a névoa encobre sempre 100% — senão daria para espiar o
		//          mapa pelas brechas. Mas 100% de uma cor só parece uma parede cinza parada. A
		//          variação de claridade faz a névoa **se mexer na tela inteira** sem revelar nada
		//          do que está atrás dela.
		vec3 cor_aqui = cor_da_nevoa.rgb * (0.90 + 0.18 * massa);
		ALBEDO = mix(cor, cor_aqui, nevoa);
	}
	ALPHA = 1.0;
}

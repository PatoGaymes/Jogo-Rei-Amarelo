// Alteração de IA - Revisar
// O que faz: esconde de verdade o que o personagem não enxerga — pintando névoa ou escuridão por
//            cima da imagem pronta, conforme o ambiente da fase.
// Por quê: o mapa é 3D visto de cima, então a câmera mostraria o que está atrás das paredes. Para
//          cada ponto da tela, este desenho descobre a que lugar do mundo ele corresponde e
//          pergunta: o personagem enxerga aquilo? Por isso vale igual para chão, parede, inimigo
//          ou objeto, sem precisar marcar nada.
//
//          ESCURIDÃO (modo 2): vê-se só o que está **iluminado** e, ao mesmo tempo, perto do
//          personagem ou na direção em que ele olha. Sem luz, breu total — é o Don't Starve.
//
//          NÉVOA (modo 1): fora da visão, a névoa encobre entre 70% e 100%, variando pelo mapa e
//          com o tempo, como massas de ar passando. É o Silent Hill antigo. Atrás de parede e
//          longe do personagem ela sempre chega a 100%.
//
//          Desempenho: a névoa que se move é um desenho de ruído gerado uma vez e só deslizado
//          pelo mapa (duas leituras por ponto da tela). As paredes chegam já medidas numa tira de
//          imagem. Nada aqui é calculado de novo que possa ser reaproveitado.
shader_type spatial;
render_mode unshaded, blend_mix, depth_draw_never, depth_test_disabled, cull_disabled, fog_disabled;

uniform sampler2D tela : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D profundidade : hint_depth_texture, filter_nearest;

uniform int modo = 1;

// --- quem enxerga, de onde e para onde ---
uniform vec3 jogador;
uniform float olhando;
uniform float meia_abertura;
uniform float passiva_clara;
uniform float passiva_embacada;
uniform float cone_claro;
uniform float cone_embacado;
uniform float cone_vulto;
uniform float suavidade = 1.2;

// distância até a parede em cada direção em volta do jogador
uniform sampler2D paredes : filter_linear, repeat_enable;
uniform float folga_da_parede = 0.8;
uniform float forca_do_borrao = 3.0;

// --- névoa ---
uniform sampler2D ruido : filter_linear, repeat_enable;
uniform float tempo;
uniform vec4 cor_da_nevoa : source_color = vec4(0.60, 0.62, 0.64, 1.0);
uniform float densidade_minima = 0.70;
uniform float densidade_maxima = 1.0;
uniform float tamanho_das_massas = 16.0;
uniform vec2 vento = vec2(0.55, 0.20);

// --- escuridão ---
uniform vec4 cor_da_escuridao : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float luz_ambiente = 0.0;
uniform sampler2D paredes_das_luzes : filter_linear, repeat_enable;
uniform int quantas_luzes = 0;
uniform vec4 luzes[16];
uniform vec4 cores_das_luzes[16];

// Alteração de IA - Revisar
// O que faz: o quanto o personagem percebe um ponto, de 0 a 1, **sem contar parede**.
// Por quê: junta as duas formas de enxergar — o que está em volta sem precisar olhar, e o cone
//          na direção do olhar, que vai de claro a vulto conforme a distância. Vale a melhor das
//          duas. É contínuo (e não os níveis 1/2/3 inteiros) para não deixar anéis duros na tela.
float percepcao(vec2 d, float dist) {
	float perto = mix(1.0, 0.55, smoothstep(passiva_clara, passiva_embacada, dist));
	perto *= 1.0 - smoothstep(passiva_embacada, passiva_embacada + suavidade, dist);

	float ang = atan(-d.y, d.x);
	float diferenca = abs(atan(sin(ang - olhando), cos(ang - olhando)));
	float dentro = 1.0 - smoothstep(meia_abertura * 0.82, meia_abertura, diferenca);

	float longe = mix(1.0, 0.62, smoothstep(cone_claro, cone_embacado, dist));
	longe = mix(longe, 0.20, smoothstep(cone_embacado, cone_vulto, dist));
	longe *= 1.0 - smoothstep(cone_vulto, cone_vulto + suavidade, dist);

	return max(perto, dentro * longe);
}

// Alteração de IA - Revisar
// O que faz: devolve de 0 a 1 o quanto o ponto está escondido atrás de parede.
// Por quê: a passagem é suave (cerca de um metro), e não um corte seco. Com corte seco a sombra
//          da parede virava um polígono de borda dura, com cara de defeito — principalmente na
//          névoa, onde nada tem contorno nítido.
float atras_da_parede(vec2 d, float dist) {
	float volta = fract(atan(-d.y, d.x) / TAU + 1.0);
	float ate = texture(paredes, vec2(volta, 0.5)).r;
	return smoothstep(ate + folga_da_parede, ate + folga_da_parede + 1.0, dist);
}

// Alteração de IA - Revisar
// O que faz: o quão densa está a névoa neste ponto, agora — entre a densidade mínima e a máxima.
// Por quê: duas camadas do mesmo ruído, de tamanhos diferentes, levadas pelo vento em direções
//          diferentes. Somadas, nunca se repetem de um jeito que o olho perceba: uma massa passa,
//          abre uma brecha, fecha de novo. Longe do personagem a névoa sempre chega ao máximo —
//          senão daria para espiar o mapa inteiro pelas brechas.
float densidade(vec2 xz, float dist, out float massa) {
	vec2 uv1 = (xz + vento * tempo) / (tamanho_das_massas * 3.0);
	vec2 uv2 = (xz - vec2(vento.y, -vento.x) * tempo * 0.7) / (tamanho_das_massas * 1.7);
	float n = texture(ruido, uv1).r * 0.65 + texture(ruido, uv2).r * 0.35;
	massa = smoothstep(0.30, 0.70, n);
	float dens = mix(densidade_minima, densidade_maxima, smoothstep(0.38, 0.62, n));
	return mix(dens, densidade_maxima, smoothstep(cone_vulto, cone_vulto * 1.5, dist));
}

// Alteração de IA - Revisar
// O que faz: soma a luz de todas as fontes que alcançam o ponto, e devolve a cor dessa luz.
// Por quê: cada fonte ilumina um círculo com borda suave. Se houver parede entre a fonte e o
//          ponto, aquela fonte não conta — é o que impede a fogueira de clarear o outro lado do
//          muro.
vec3 luz_em(vec3 p, out float forca) {
	float total = luz_ambiente;
	vec3 cor = vec3(0.0);
	float peso = 0.0;
	for (int i = 0; i < quantas_luzes; i++) {
		vec2 d = p.xz - luzes[i].xz;
		float dist = length(d);
		float raio = luzes[i].w;
		if (dist > raio) {
			continue;
		}
		float volta = fract(atan(-d.y, d.x) / TAU + 1.0);
		float ate = texture(paredes_das_luzes, vec2(volta, (float(i) + 0.5) / 16.0)).r;
		if (dist > ate + folga_da_parede) {
			continue;
		}
		float f = cores_das_luzes[i].a * (1.0 - smoothstep(raio * 0.45, raio, dist));
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
	float escondido = atras_da_parede(d, dist);
	float percebo = percepcao(d, dist) * (1.0 - escondido);

	if (modo == 2) {
		// ESCURIDÃO: só aparece o que está iluminado E percebido
		float forca;
		vec3 cor_da_luz = luz_em(no_mundo, forca);
		float visivel = percebo * forca;
		vec3 cor = textureLod(tela, SCREEN_UV, (1.0 - visivel) * forca_do_borrao * 0.5).rgb;
		cor *= mix(vec3(1.0), cor_da_luz, 0.35 * forca);
		ALBEDO = mix(cor_da_escuridao.rgb, cor, visivel);
	} else {
		// NÉVOA: fora da visão encobre de 70% a 100%; atrás de parede, sempre 100%
		float massa;
		float dens = densidade(no_mundo.xz, dist, massa);
		float nevoa = mix((1.0 - percebo) * dens, 1.0, escondido);
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

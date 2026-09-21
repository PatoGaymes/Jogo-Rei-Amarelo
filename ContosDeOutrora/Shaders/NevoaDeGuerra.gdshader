// Alteração de IA - Revisar
// O que faz: cobre a tela com névoa em tudo que o personagem não está enxergando, e deixa
//            limpo o que ele enxerga.
// Por quê: o mapa é 3D visto de cima, então a câmera mostra o que está atrás das paredes — até
//          o outro canto da sala. Isso entrega o mapa inteiro de graça e acaba com qualquer
//          tensão. A névoa devolve ao jogador só o que o personagem realmente veria.
//
//          Funciona **em cima da imagem pronta**: para cada ponto da tela, descobre a que lugar
//          do mundo ele corresponde e pergunta se o personagem enxerga aquele lugar. Por isso
//          funciona igual para chão, parede, inimigo ou objeto, sem precisar marcar nada.
//
//          O embaçado usa as versões reduzidas da própria imagem (os "mipmaps"), que o motor já
//          calcula de qualquer jeito — sai de graça em vez de custar dezenas de leituras.
// Desenhada na passada das coisas transparentes, e por último de todas (ver RenderPriority no
// script), para que a imagem que ela lê já tenha o cenário e os personagens prontos.
shader_type spatial;
render_mode unshaded, blend_mix, depth_draw_never, depth_test_disabled, cull_disabled, fog_disabled;

uniform sampler2D tela : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D profundidade : hint_depth_texture, filter_nearest;

// onde o personagem está e para onde está olhando (em radianos, na roda usada pelo jogo)
uniform vec3 jogador;
uniform float olhando;
uniform float meia_abertura;

// o que ele enxerga sem precisar olhar
uniform float passiva_clara;
uniform float passiva_embacada;

// as três faixas do cone de visão
uniform float cone_claro;
uniform float cone_embacado;
uniform float cone_vulto;

// Alteração de IA - Revisar
// O que faz: uma tira fina onde cada pixel guarda a distância até a parede numa direção.
// Por quê: é como as paredes chegam até aqui. O script mede com linhas imaginárias e entrega
//          nesta tira; aqui só se consulta a direção do ponto e compara. É o que impede a
//          névoa de limpar o cômodo do outro lado do muro.
uniform sampler2D paredes : filter_linear, repeat_enable;
uniform float folga_da_parede = 0.8;

// Alteração de IA - Revisar
// O que faz: a grade com tudo o que o personagem já viu alguma vez no mapa.
// Por quê: lugar já visto continua aparecendo, apagado, mesmo de costas — casa não anda, então
//          saber que ela está ali não se perde ao virar o rosto. Sem isso, virar de lado apagava
//          o mundo inteiro e o jogador se perdia no próprio mapa.
uniform sampler2D memoria : filter_linear;
uniform float peso_da_memoria = 0.45;
uniform vec2 canto_do_mapa;
uniform float tamanho_do_mapa = 80.0;
uniform float dessaturacao_da_memoria : hint_range(0.0, 1.0) = 0.7;

uniform vec4 cor_da_nevoa : source_color = vec4(0.13, 0.14, 0.18, 1.0);
uniform float forca_da_nevoa : hint_range(0.0, 1.0) = 0.93;
uniform float forca_do_borrao : hint_range(0.0, 6.0) = 3.2;
uniform float suavidade = 1.2;

// Alteração de IA - Revisar
// O que faz: devolve o quanto o personagem enxerga um ponto, de 0 (névoa) a 1 (claro).
// Por quê: é um valor contínuo, e não os níveis 1/2/3 inteiros, para as faixas se misturarem
//          sem deixar anéis e bordas duras na tela. Os níveis continuam existindo na mecânica;
//          aqui eles viram degraus suaves de clareza.
float clareza(vec3 ponto) {
	vec2 d = ponto.xz - jogador.xz;
	float dist = length(d);

	// --- parede na frente? então acaba aqui ---
	// a direção do ponto vira uma posição na tira de distâncias; se o ponto está mais longe do
	// que a parede naquela direção, ele está escondido atrás dela.
	float volta = fract(atan(-d.y, d.x) / TAU + 1.0);
	float ate_a_parede = texture(paredes, vec2(volta, 0.5)).r;
	if (dist > ate_a_parede + folga_da_parede) {
		return 0.0;
	}

	// --- o que ele percebe em volta sem olhar ---
	float perto = mix(1.0, 0.55, smoothstep(passiva_clara, passiva_embacada, dist));
	perto *= 1.0 - smoothstep(passiva_embacada, passiva_embacada + suavidade, dist);

	// --- o que o cone de visão alcança ---
	float ang = atan(-d.y, d.x);
	float diferenca = abs(atan(sin(ang - olhando), cos(ang - olhando)));
	float dentro_do_cone = 1.0 - smoothstep(meia_abertura * 0.82, meia_abertura, diferenca);

	float por_distancia = mix(1.0, 0.62, smoothstep(cone_claro, cone_embacado, dist));
	por_distancia = mix(por_distancia, 0.20, smoothstep(cone_embacado, cone_vulto, dist));
	por_distancia *= 1.0 - smoothstep(cone_vulto, cone_vulto + suavidade, dist);

	return max(perto, dentro_do_cone * por_distancia);
}

void vertex() {
	// cobre a tela inteira, independente de onde a câmera esteja
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	// Alteração de IA - Revisar
	// O que faz: descobre o ponto do mundo que aparece neste pedacinho da tela.
	// Por quê: é o que permite perguntar "o personagem enxerga ISTO?" sobre qualquer coisa
	//          desenhada — usa a profundidade que o motor já guardou ao desenhar a cena.
	float z = texture(profundidade, SCREEN_UV).x;
	vec3 ndc = vec3(SCREEN_UV * 2.0 - 1.0, z);
	vec4 na_camera = INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	na_camera.xyz /= na_camera.w;
	vec3 no_mundo = (INV_VIEW_MATRIX * vec4(na_camera.xyz, 1.0)).xyz;

	float vejo = clareza(no_mundo);

	// Alteração de IA - Revisar
	// O que faz: soma ao que ele vê agora o que ele **lembra** de já ter visto.
	// Por quê: o lugar lembrado nunca fica tão nítido quanto o que está sendo olhado (o peso da
	//          memória segura isso), mas também não volta a ser névoa. É a diferença entre
	//          "não sei o que tem lá" e "sei o que tem lá, só não estou olhando agora".
	vec2 no_mapa = (no_mundo.xz - canto_do_mapa) / tamanho_do_mapa;
	float lembro = 0.0;
	if (no_mapa.x > 0.0 && no_mapa.x < 1.0 && no_mapa.y > 0.0 && no_mapa.y < 1.0) {
		lembro = texture(memoria, no_mapa).r * peso_da_memoria;
	}

	float visivel = max(vejo, lembro);
	float nevoa = 1.0 - visivel;

	// quanto menos ele enxerga, mais borrada fica a imagem
	vec3 cor = textureLod(tela, SCREEN_UV, nevoa * forca_do_borrao).rgb;

	// Alteração de IA - Revisar
	// O que faz: tira a cor do que é só memória, deixando acinzentado.
	// Por quê: separa à primeira vista o que ele está vendo agora do que ele apenas sabe que
	//          está ali. Sem isso, memória e visão ficariam parecidas demais e o jogador
	//          confiaria numa informação velha achando que é atual.
	float so_lembranca = clamp(lembro - vejo, 0.0, 1.0) / max(peso_da_memoria, 0.001);
	float cinza = dot(cor, vec3(0.299, 0.587, 0.114));
	cor = mix(cor, vec3(cinza), so_lembranca * dessaturacao_da_memoria);

	ALBEDO = mix(cor, cor_da_nevoa.rgb, nevoa * forca_da_nevoa);
	ALPHA = 1.0;
}

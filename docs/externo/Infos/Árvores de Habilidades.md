# Árvores de Habilidades

> **Fonte oficial do sistema de combate.** Sincronizado com a versão do repositório
> (`Árvores de HabilidadesDoRepositorio.docx`) em 18/09/2026.
>
> Em caso de conflito com o GDD, **este documento vale**.

---

## Mecânicas gerais de combate
### Ações
- Os personagens tem 4 ações: atacar/usar habilidades, usar itens, defender, fugir
-cada personagem só pode fazer uma ação por turno, a não ser que ele tenha habilidades que contornam isso
### Efeitos de status
- Gelo:Atordoa o alvo e causa um pouco de dano ao sair do estado
- Veneno: causa ticks de dano
- Sangramento: causa ticks de dano que aumentam conforme menos vida o alvo tem
- Fogo: causa ticks de dano, e espalha pra alvos adjacentes com 5+ acúmulos
- Corrosão: diminui as resistências do alvo, com no máximo até 5 acúmulos
- Raio: Bloqueia uma habilidade aleatória, com no máximo 2 acúmulos
- Escuridão: Diminui a vida máxima do alvo com no máximo 2 acúmulos-luz: Diminui a cura recebida pelo alvo, com no máximo 3 acúmulos
### Condições
-desarmado: impede o alvo de usar habilidades de ataque
-desvantagem: diminui precisão e dano do alvo
-vantagem: aumenta precisão e dano do alvo
- Atodordoado: atordoa o alvo
- Enraizado: impede o alvo de se mover
- Vulnerável: não pode recebr buffs-couraça: bloqueia o próximo dano recebido, mas não o efeito de status
-vanguarda: a unidade com essa condição é protegida de qualquer ataque lançado a ela
- Furtivo: não pode ser alvo de ataques por meios padrões
- Cego: reduz a precisão e suas habilidades passam a ter um alvo aleatório
- Espinhos: o alvo devolve parte do dano que recebe
### Especificações de habilidades
- Crítico: Habilidades com essa especificação ignoram a defesa do alvo quando acertam um ataque com dano máximo
- Garantido: Habilidades com essa especificação ignoram a esquiva do alvo
- Múltiplo: Habilidades com essa especificação causam várias instancias de dano ao invés de apenas uma
- Radial: Habilidades com essa especificação acertam vários alvos de uma vez
## Árvores de Habilidades
- Cavaleira-
Suporte/Tank Flexível, podendo ser tanto ofensivo quanto defensivo, aplicando debuffs e protegendo aliados
Habilidades padrão: Ataque básico, Protetora, Avanço tático, Compasso
Caminhos
Agressora/-/Defensora
Jogo de pés        /         Pesado
Ataque de oportunidade / Dispersar/ Apancada de escudo                /              Flanquear/Inabalável/Guardiã da Alcateia
Intimidar-Golpe desleal/Ultimo suspiro-Tudo ou nada/Dor de cabeça-Dentes afiados            /           Comando-Rompe malha/Astúcia-Chamado da matilha/Temperança uivante-Lobo guardião
## Descrição das habilidades
Protetora: Aplica "Vanguarda" a um aliado
Avanço Tático: Avança, causa dano e empurra
Compasso: Ganha 1 "Couraça"
Jogo de pés: você pode usar suas habilidades a qualquer distância
Pesado: perde 1 de movimento, mas ganha 1 de resistência pra cada 5 pontos de vida
Ataque de oportunidade: quando um aliado adjacente a você é atacado, você ataca o atacante. Essse ataque tem precisão aumentada
Dispersar: Usar intens não gasta mais o seu turno
Pancada de escudo: Causa dano e atordoa
Flanquear: Marca um alvo, e enquanto marcado ele perde efeitos de proteção e toma mais dano
Inabalável: ataques com efeito de "garantido" não tem efeito em você
Guardiã da Alcateia: Enquanto estiver com "vanguarda", recebe hp regen
Intimidar: causar dano massivo no alvo e impede com que ele ataque você em específico
Golpe desleal: quando um aliado causa dano máximo, você realiza um ataque extra em  conjunto nesse mesmo inimigo
Último suspiro: evita o primeiro dano fatal, aumentando imensamente sua resistência por algumas rodadas
Tudo ou Nada: Seus ataques ignoram vanguarda e causam mais danos em inimigos com tal efeito
Dor de cabeça: causa dano, atordoa e empurra o alvo
Dentes afiados: Ganha dano de espinhos por algumas rodadas
Comando: Concede um turno extra pra um aliado
Rompe-Malha: Seus ataques removem um pouco da resistência inimiga, não acumulável. Acertar o mesmo inimigo de novo reseta a duração do efeito
Astúcia: Ao atingir metade da vida, você recebe hp regen por algumas rodadas
Chamado da matilha: buffa aliados adjacentes com sangramento nos ataques
Temperança uivante: Aliados com vanguarda tem o dano aumentado e a margem de dano massivo diminuida
Lobo guardião: "Guardiã da alcateia" agora dá "vanguarda" para 2 aliados ao invés de 1
- Caçadora-
Dps explosivo, com jogabilidade agressiva e nenhuma defesa
Habilidades padrão: Ataque básico, Embuste, Técnica Secreta, Matadora de YokaisCaminhos
Razão/-/Ódio
Táticas internas                /                           ”Você vai morrer”
Um contra todos / Penetração Letal/ Ritmo Fatal                /              Morte lenta/”Matar faz bem”/Carrasco de Facínoras
Predador e Presa-Artilharia Pesada/ Pouco Espaço-Navalhas voadoras/Retribuição -Sentidos Shinobi       /     ”Queime”-”Dance”/”Minha presa”-“Eu quero mais”/”Inútil”-”Não pode resistir”
## Descrição das habilidades
Embuste: Causa dano e enraíza o alvo
Técnica Secreta: Avança para trás e Aumenta a esquiva
Matadora de Yokais: Causa dano e aplica veneno
Táticas Internas: Ganha "Vantagem", e pula para a primeira na iniciativa no próximo turno
"Você vai morrer": Quando atingida enquanto não estiver com a vida cheia, tem chance de ganhar um turno extra
Um contra todos: Ganha mais dano e esquiva para cada espaço de aliados desocupado
Penetração Letal: Após usar técnica secreta, seu próximo ataque se torna garantido e aplica veneno
Ritmo fatal: Quando atingir um inimigo com dano massivo, ganha mais uma ação de ataque
Morte lenta: causa dano, veneno e marca o alvo. Enquanto o alvo estiver marcado, os acúmulos de veneno não descem
"Matar faz bem": Ao finalizar uma criatura, recupera uma quantidade de sanidade
Carrasco de facínoras: Seus ataques Ignoram "Couraça"
Predador e Presa: Ganha vantagem contra inimigos com iniciativa menor que a sua
Artilharia pesada: "Embuste" agora causa uma explosão de dano nos espaços inimigos adjacentes ao alvo, e aplica veneno em todos
Espaço aberto: Seu dano aumenta mas sua precisão diminui pra cada  espaço adjacente ocupado
Navalhas Voadoras: Ganha Mais esquiva e 5 cargas de "Navalha", que aplicam uma segunda instancia de dano aos seus ataques de alvos únicos, aplicando veneno. Ser atacado também remove 1 carga
Retribuição: Causa dano em área e aplica veneno nos inimigos
Sentidos Shinobi: Ataques de alvo único ignoram camuflagem
"Queime": no fim de cada turno, aplica veneno em inimigos marcados por "Morte lenta"
"Dance": Morte lenta agora marca todos os inimigos
"Minha presa": Caso o inimigo esquive se seu. Você lança um segundo ataque
"Eu quero mais": No segundo turno concedido por "Você vai morrer", você tem duas ações de ataque
"Inútil": você ignora a imunidade a status dos inimigos e é imune ao dano de sanidade que
eles causam. Estar com a "máscara de yokai" equipada  anula o efeito de sanidade
Não pode resistir: ao atingir ¼ da sua vida máxima,você ganha de roubo de vida em ataques e 1 "couraça" ao final de cada turno,perdendo o benefício ao recuperar completamente a barra de vida
- Peregrino-
Suporte/Dps artilheiro, com jogabilidade defensiva e uso de efeitos de status
Habilidades padrão: Ataque básico, Energia comprimida, Transfiguração Térmica, Voz Amaldiçoada      Caminhos
Canalha/-/Ocultista
Estigma da Conveniência                                       /                                                   Criadouro de insetos
Mãos do abismo / Renovação/ Misto de coquetéis                /              Mina de espinhos/Dilascerar/Fratura terminal
Vortex Negro-Poço das moléstias/ Propagação-Manipulação de energia/Prismático-Colorismo       /     Sangue Grosso-Sangue frio /Derreter as entranhas-Matriz encantada/Ossos famintos-Foco interior
## Descrição das habilidades
Energia comprimida: causa dano e recupera energia
Transfiguração térmica: aplica status de gelo ou fogo no alvo
Voz amaldiçoada: Causa dano em área e aplica efeito de raio
Estigma da conveniência: diminui o agro inimigo em você e aumenta o agro em aliados
Criadouro de insestos: Ganha hp temporário que enquanto ativo, reflete parte do dano recebido e aplica status de fogo
Mãos do abismo: seu ataque básico agora tem chance de aplicar um efeito de status aleatório
Renovação: recupera uma certa quantidade de energia
Misto de coquetéis: aplica um status extra ao inimigo ao causar dano massivo com ataques
Mina de espinhos: demarca uma área inimiga, e sempre que um inimigo terminar a rodada em cima dela, ele recebe dano e aplica escuridão por algumas rodadas
Dilascerar: Ao usar "Criadouro de insetos" novamente, enquanto ele estiver ativo, se transforma num ataque em área
Fratura terminal: após o inimigo afetado por "transfiguração térmica", ele explode e aplica congelamento aos adjacentes
Vortex negro: Aplica escuridão em área e dano. Quando atinge inimigos com status que não sejam escuridão, os convertem para tal
Poço das moléstias: causa dano em área, e deixa poças que aplicam veneno e corrosão
Propagação: quando aplicar um status a um inimigo já afetado, o status atual passa para um dos inimigos atuais. Caso o inimigo seja de estatura "grande" ou maior, a quantidade de status lançado nele é trplicada
Manipulação de energia: quando um aliado consome mais do que uma certa quantidade de energia, você recupera metade do gasto da energia
Prismático: você marca um alvo por algumas rodadas, deixando-o
desarmado. E aplicando efeitos status aleatórios para cada ataque que receba
Colorismo: Ganha afinidade com relíquias de "arma" e intrísicos, e ganha +1 espaço de relíquia
Sangue grosso: ao perder vida, recupera energia e sanidade, metade do valor perdido para cada
Sangue frio: aplica frio em si mesmo, e ganha dano de espinhos, e aplica gelo
Derreter as entranhas: "dilascerar" agora causa cegueira nos inimigos, e reduz a precisão também
Matriz encantada: quando estiver com a vida cheia, não gasta energia nas habilidades; hp temporário não é considerado
Ossos famintos: causa dano, aplica corrosão e enraiza o inimigo
Foco interior: Ao receber um acerto de dano massivo, aumenta em muito todas suas estatísticas
- Desgarrado-
Bruiser Flexível, com jogabilidade tática, baseado no gerenciamento de habilidades e contra-ataques
Habilidades padrão: Ataque básico, Postura Oculta, Rasga-ossos, Alma imaculada   Caminhos
Guerreio/-/Bruto
Mão firme                                                              /                                                   Mastodonte
Foco total / Esmagador/ Manobra rasante                       /                                 Escamas duras/Reflexos dilatados/Ataque poderoso
Destrinchar-Feitiço Inato/ Pés Arrastados-Cruel/Forte intenção-Matador de colossos                         /            Ignorar a dor-Indestrutível /Procurar e Destruir-Cólera Draconiana/Ataque Destruidor-Ataque volátil
## Descrição das habilidades
Postura oculta: ao entrar nessa postura, seu dano é reduzido, mas aumenta a esquiva e resistência, ganha um incremento de contra ataque, além de aplicar efeito de "maldição"
Rasga ossos: causa dano e sai da postura oculta
Alma imaculada: mostra os próximos movimentos de todos os inimigos
Mão firme: você é imune ao efeito de "desarmado" e devolve o efeito caso atingido por tal
Mastodonte: Seus contra ataques causam o dobro do dano, mas todos os seus ataques que não funcionem como contra ataque causam menos dano
Foco total: recupera energia, entra e sai na postura oculta
Esmagador: causa mais dano caso não esteja com nenhuma relíquia do tipo "arma"
Manobra rasante: Danos massivos aplicam desarmado no alvo
Escamas duras: ganha 1 "couraça" e seu contra ataque causa mais dano. Só pode ser usado na Postura oculta. Fica incapacitado por 1 rodada ou até ser atacado
Reflexos dilatados: Não perde mais os benefícios da postura oculta ao sair dela
Ataque poderoso: Causa dano e entra na postura oculta. Não é afetado por "Mastodonte"
Destrinchar: Seu ataque básico agora gasta energia, mas agora tem a margem de acerto massivo drasticamente diminuida e causa maldição mesmo fora da postura oculta
Feitiço inato: gastar energia recupera um pouco de  vida com base na quantia gasta
Pés arrastados: ganha +1 movimento, +1 ação de ataque, mas não recebe os buffs de relíquias
Cruel: os inimigos ganham uma barra de execução com base no quão baixa está sua sanidade, causar dano ao inimigo caso ele esteja abaixo dessa barra mata ele na hora
Forte intenção: escolhe um alvo pra ignorar suas habilidades pelo próximo turno
Matador de colossos: seus ataques tem mais precisão em alvos maiores, enquanto em alvos menores o dano aumenta e muito
Ignorar a dor: "Couraça" agora também te dá imunidade a dano verdadeiro, e ganha 1 couraça extra
Indestrutível: "couraça" agora se torna "indestrutível", que ao invés de apenas impedir o dano, impede efeitos de status e impede de empurrar e puxar
Cruel: "Rasga ossos"e  ataque básico agora ignoram a armadura inimiga, e você ganha ataque destruidor
Cólera draconiana: seus contra ataques tem um segundo acerto que causa dano verdadeiro
Ataque destruidor: causa dano verdadeiro e sai da postura oculta.
Ataque volátil: seus contra ataques são garantidos e você soma o dano do inimigo ao seu.
- Hemomante-
Bruiser, com jogabilidade Agresiva, com foco no manejo de vida
Habilidades padrão: Ataque básico, Manifestar Cônjuge, Copiar, Talhar MúsculosCaminhos
Mortal/-/Imortal
União selada                                                              /                                                   Controle de Fluxo
Chamas sangrentas / Corte Limpo/ Bomba de sangue                      /                           Fera Vampírica/Genocida/Pontos vitais expostos
Inevitabilidade-Ossos cristalinos/Letal-Rede de arames/Sangue venenoso-Emprestar força            /      Garras atrozes-Endurecer sangue/Golpe Visceral-Manipulação de vida/Martir-Multilador
## Descrição das habilidades
Manifestar Cônjuge: Invoca a "Noiva", que aumenta sua vida máxima, e atual, além de aprimorar seu ataque básico (com sangramento) e certas habilidades. A noiva desaparece após um ataque letal
Copiar: copia a habilidade de um inimigo que você tenha atingido recentemente, ou de um aliado adjacente a você, so pode ser usado enquanto a noiva estiver ativa
Talhar músculos: gasta vida pra causar dano e sangramento. Com a "Noiva" manifestada, a margem pra dano massivo diminui, e o sangramento aumenta
União selada: a noiva não desaparece após um golpe fatal, mas só pode ser invocada se você estiver com vida cheia
Controle de fluxo: ao acertar um ataque, tem chance de ganhar mais um ataque, que não pode ser usado no mesmo alvo
Chamas sangrentas: perde vida pra causar dano, e aplica fogo
Corte limpo: "Talhar músculos" agora ignora resistências e imunidades do alvo em dano massivo
Bomba de sangue: perde vida pra causar dano e sangramento
Besta vampírica: Se torna imune a efeitos de: sangramento, veneno, corrosão e aumenta sua precisão em ataques
Genocida: Ganha mais um turno na primeira rodada. Quando a noiva esta ativa, matar um inimigo reseta sua rodada
Pontos vitais expostos: atacar um inimigo sangrando aumenta o dano causado a ele
Inevitabilidade: "Chamas sangrentas" tem acerto garantido em alvos com sangramento, além de que quando a Noiva está ativa, se torna um ataque em área
Ossos cristalinos: se torna imune a dano massivo, e ao receber sangramento. Ganha uma certa quantidade de resistência
Letal: caso receba um golpe fatal, caso a noiva esteja ativa, ela irá revidar o ataque
Rede de arames: "Talhar músculos" não gasta mais vida, e agora se divide em várias instâncias de dano
Sangue venenoso: ao ser atingido, dispara rajadas de sangue que aplicam veneno  no alvo
Emprestar força: agora você manifesta a noiva completamente, o que a torna uma criatura independente.
Garras atrozes: ataque básico agora também causa sangramento, mas agora também faz perder vida
Endurecer sangue: causa dano e se cura, aumentando o dano causado com base no sangramento do alvo
Golpe visceral: Ao atingir o inimigo com dano massivo no ataque básico, você causa "desarme" e sangramento permanentes ao alvo
Manupulação de vida: você se cura ao matar um inimigo
Martir: aumenta o agro inimigo em você e você perde hp enquanto ativo.
Multilador: enquanto não estiver com a vida cheia, ganha hp regen, que aumenta conforme menos vida você tem. Com a noiva ativa, o hp regen aumenta mais ainda
- Aberração-
Bruiser/suporte, com jogabilidade mista, de alto risco e alta recompensa, transitando entre formas
Habilidades padrão: Ataque básico, Metamorfose, RezarCaminhos
Moribunda/-/Incontrolável
Refluxo                                                              /                                                   Urro da besta
Aliviar peso / Corpo doente/ Fiquem longe                      /                         Brutalidade animal/Faro aguçado/Esfolar
Cheiro de medo-Pânico/Criatura desagradável-Canibalizar/Podridão mórbida-Manifestar besta            /      Avanço-Mandíbula Firme/Robustez Maciça-Predador supremo/Saliva venenosa-Negrosar
## Descrição das habilidades
Metamorfose: Você adquire uma
forma bestial, ganhando
vida máxima e atual, resistência a dano e sua arma muda para “garras”. Enquanto na bestial, você causa dano de sanidade em aliados e  em si. Usar essa habilidade de novo ou tomar dano massivo retorna imediatamente a forma humana
Rezar: recupera sanidade
Refluxo: causa dano e corrosão ao alvo. Pode ser usado na forma monstro
Urro da besta: causa dano e  atordoa inimigos e aliados
Aliviar peso: ao voltar a forma humana, você recupera sanidade dos aliados e própria
Corpo doente: atacar e ser atacada causa corrosão e náusea aos inimigos, mas também causa dano aos aliados adjacentes
Fiquem longe: ataca,  avança pra trás e ganha furtividade
Brutalidade animal: ataca, causa muito dano e ignora a resistência do inimigo quando estiver com metade da vida. Só pode ser usada na forma monstro
Faro aguçado: ignora inimigos no estado furtivo, e aumenta precisão contra inimigos furtivos. Funciona apenas na forma monstro
Esfolar: causa dano e sangramento. Na forma monstro, causa ainda mais dano, e aplica veneno ao inves de sangramento
Cheiro de medo: o agro em você aumenta muito, e na forma monstro, você causa medo nos inimigos que te atacarem
Pânico: sacrifica sanidade para ganhar vida e remover devuffs
Criatura desagradável: o agro inimigo em  você diminui, mas sempre que você é atingido, seus aliados recuperam sanidade
Canibalizar: causa dano a si mesmo para ganhar hp regen. Na forma monstro, se torna um ataque com roubo de vida
Podridão mórbida: ao zerar a vida, você não morre de imediato, e sim, ganha um hp negativo que te mantém vivo
Manifestar besta: ataca e entra na forma monstro
Avanço: avança até a primeira posição, causa dano e empurra o alvo
Mandíbula firme: na forma monstro, é imune a todos os efeitos
Robustez maciça: 1 vez por combate, quando tem seu hp zerado na forma humana, muda imediatamente
pra forma bestial
Predador supremo: na forma monstro, sua margem de acerto garantido é anulada, mas diminui a precisão e perde sanidade ao errar ataques
Saliva venenosa: Quando um inimigo estiver com 2  acúmulos de veneno,
o próximo aplicado por você estoura uma
quantidade massiva de dano igual a quantidade restante de venenox3 e consome
todos os acúmulos
Necrosar: Na forma monstro, você tem corta cura, e causa desarmado. Caso o alvo seja um membro, você o inutiliza
- Xamã-
Suporte invocadora, com jogabilidade flexível, com foco no manejo de invocações e efeitos de status
Habilidades padrão: Ataque básico, Arcano menor, Arcano maior, Cartas de baralhoCaminhos
Mortal/-/Imortal
Espírito sol                                                              /                                                   Espírito lua
Estrela / Diabo/ Imperador                                                            /                           Imperatriz/Julgamento/Sacerdotisa
Eremita-Cavalo/Morte-Mundo/Fortuna-Temperança                                                          /                                            Mago-Hierofante/Enforcado-Louco/Amantes-Torre
## Descrição das habilidades
Arcano menor: recupera vida e sanidade
Arcano maior: aumenta precisão e dano
Roda dos 6 Desejos: rola uma roda com 6 efeitos diferentes, e dependendo da cor um incremento para o próximo ataque seu ou de um aliado.
Azul: atordoa o alvo no acerto
Preto: aplica um status aleatório
Vermelho: O dano causado respinga nos adjacentes em, mesmo em ataques em área
Verde: Recupera energia no acerto
Amarelo: empurra no acerto
Espírito sol: invoca o "espírito sol", que tem muita vida e dano moderado, focado em defensiva
Espírito lua: invoca o "espírito lua", que tem pouca vida, e muito dano, focado na ofensiva
Estrela: causa dano a todos os inimigos e causa cegueira. Com o espírito sol invocado, ele também lança o mesmo ataque
Diabo: o espírito sol drena a sanidade dos aliados pra ganhar dano e resistência
Imperador: quando invocado, o espírito sol aplica "Vanguarda" á um aliado aleatório
Imperatriz: O espírito lua esquiva do primeiro ataque lançado contra ele no turno, e contra ataca
Julgamento: "Arcano menor" e "Arcano maior" agora podem ser usados nos inimigos, porém causando o efeito contrário
Sacerdotisa: o espirito lua ganha esquiva e precisão ao ter aliados adjacentes a ele
Eremita: aplica o máximo de acúmulos de vantagem ao alvo, e da um turno extra. Funciona se usado em si
Cavalo: Causa dano, empurra e atordoa. Para o espírito sol,  ele empurra 2 posições ao invés de 1
Morte: amaldiçoa o alvo com uma quantidade de dano. Se o alvo cair pra menos que a vida amaldiçoada, ele morre automaticamente
Mundo: causa dano verdadeiro a todos os inimigos
Fortuna: "Roda dos 6 desejos" agora aplica 3 benefícios ao invés de 1
Temperança: o espírito sol compartilha suas resistências com os alvos adjacentes
Mago: o espírito lua recupera vida e energia para xamã ao acertar um ataque de dano massivo
Hierofante: Causa dano a todos os inimgos e aplica desvantagem. Caso o espírito lua esteja invocado, ele ganha vantagem após o uso
Enforcado: causa dano e atordoa o inimigo, causando dano enquanto ele estiver atordoado nos turnos seguintes, passando imunidade em dano massivo. Se ele morrer estando atordoado, ele explode e causa dano aos adjacentes
Louco: o espírito lua causa mais dano conforme a sanidade da cartomante, ela pode ativamente perder sanidade usando esta habilidade
Amantes: ao morrer, o espirito lua rouba a vida do inimigo mais fraco presente e se recupera. Caso mate o alvo nesse ataque, volta com a vida cheia e ganha um turno
Torre: causa dano e gera um efeito nos inimigos, que bloqueia ataques a distância por algumas rodadas
- Escurdeiro-
Suporte Defensivo, com foco em curar e dar buff aos aliados
Habilidades padrão: Ataque básico, Lança sagrada, Fogo acolhedor, Sermão divino         Caminhos
Santificado/-/Abençoado
Em nome do pai                                                              /                                                   Excomungar
Em nome do filho / Espírito santo/ Crucifixo                                   /                           Impeto fugaz/Acusação/Puritano
Fé Inabalável-Exaltação/Baluarte da fé-Em ti confio/Armadura de deus-Caminhar dos anjos              /               Sacramentol-Abadia monumental/Empírico-Mente iluminada/Palavra divina-Redenção
## Descrição das habilidades
Lança sagrada: causa dano e aplica luz
Fogo acolhedor: cura o alvo e remove debuffs
Sermão divino: atordoa o alvo e aplica luz
Em nome do pai: Agora, "Fogo acolhedor" cura também a sanidade do alvo.
Exocomungar: causa dano, aplica luz e remove os buffs do inimigo
Em nome do filho: todos os aliados a sua frente recuperam energia
Espírito santo: Suas curas, tanto de sanidade quanto de vida concedem sobre-cura
Crucifixo: você é imune a empurrões e puxões, e pode usar "fogo acolhedor" sem gastar seu turno, uma vez por combate
Impeto fulgaz: "fogo acolhedor" se torna hp regen ao invés de uma cura instantânea, e o alvo não pode ser acometido por efeitos de status enquanto sob efeito da habilidade
Acusação: Marca o alvo e o impede de receber buffs enquanto a marca durar
Puritano: Seus pontos de energia aumentam conforme menos vida máxima você tem
Fé inabalável: ao receber um golpe fatal, você cura seus aliados, e se preserva com 1 de hp
Exaltação: Torna um aliado invencível por algumas rodadas, aumenta seu dano e concede dano de espinhos que aplicam luz
Baluarte da fé: "lança sagrada" se torna um ataque em área, e após o uso, aumenta o dano dos aliados de acordo com quantos alvos foram atingidos
Em ti confio: faz um aliado aplicar "vanguarda" a outro aliado, e recupera sanidade do alvo
Armadura de deus: torna um aliado imune a dano verdadeiro pelo combate
Caminhar dos anjos: ao iniciar um combate, você aumenta os pontos de energia e sanidade máximos e atuais dos seus aliados
Sacramento: Sermão divino e fogo acolhedor não gastam mais energia para serem lançados
Abadia monumental: "fogo acolhedor" agora é lançado em todos os aliados em campo
Empírico: Você é imune ao efeito de fogo, e reflete ele ao ser atingido por tal
Mente iluminada: você não pode mais morrer por sanidade e nem enlouquecer
Palavra divina: Impede que um aliado morra por algumas rodadas
Redenção: uma vez por turno, metade do primeiro dano direcionado a você ou aliados adjacentes é refletido
- Brutamonte-
Bruiser Agressivo, com jogabilidade frenética, baseado em causar dano absurdo, focado em combates longos
Habilidades padrão: Ataque básico, Poder e fúria, Mastigar os Ossos, Alma BestialCaminho
Guerreiro
Agressor
Impacto repentino/ Monstro carmesim/ Sangue de guerreiro
Carnificina-Falange/Deus da guerra-Maculado pelo sangue/Todos de uma vez-Rito de passagem
## Descrição das habilidades
Poder e fúria: entra em fúria, e aprimora seu próximo ataque
Mastigar os ossos: Causa dano, sangramento e cura.
Alma bestial: quanto menos vida, mais dano você causa
Agressor: sempre que causar ou receber dano, seu ataque aumenta, até o máximo de 3 vezes. Perde os acumulos se não atacar nem receber dano.
Impacto repentino: Causa dano e empurra o alvo. Caso haja um inimigo atras do alvo empurrado, eles se chocam, causando mais dano e atordoando
Monstro Carmesim: "Poder e fúria" agora tem efeito permanente no combate
Sangue de guerreiro: desafia um inimigo para um duelo, onde ambos causam mais dano e tem precisão aumentada entre si. Se qualquer outra pessoa fora desse efeito atingir qualquer um dos dois, o efeito se encerra
Carnificina: sua margem de dano massivo diminui muito, e matar um inimigo ou acertar um ataque massivo diminui mais ainda
Falange: lança diversos micro ataques. Em fúria, o dano é verdadeiro
Deus da guerra: carrega um ataque poderoso, que causa dano massivo ao alvo e um pouco a si, e destrói uma arma. Ao destruir a arma, pelo resto do combate, seu dano e vida máxima diminuem um pouco, mas ganha 1 ataque a mais (máximo de 2 vezes)
Maculado pelo sangue: "Agressor" se torna uma habilidade ativa, e passiva ao mesmo tempo
Todos de uma vez: aplica taunt em si, e ganha dano de espinhos caso esteja com fúria ativada;
Rito de passagem: "Poder e fúria" vem seguido de um ataque massivo, além de roubo de vida. O roubo de vida e o dano aumentam conforme menos vida
- Bruxo-
Bruiser/dps artilheiro, com jogabilidade tática, baseado em causar e evitar danos
Habilidades padrão: Ataque básico, Palavras cortantes, Encantamento superior, Verdade sublime  Caminho
Místico
Tríade perfeita
Buraco negro/ Maré de medo/ Destino mutávelx
Esfera negra perfeita-Transcender/Fluxo liberto-Chuva de raios/Fragmentar-Ritual não ortodoxo
## Descrição das habilidades
Palavras cortantes: causa dano e desvantagem
Encantamento superior: medita por 1 rodada, para aumentar todas as suas estátisticas ofensivas. Ser atacado interrompe a conjuração
Verdade sublime: causa dano e recupera sanidade
Tríade perfeita: quando estiver com 3 relíquias do tipo "intrínseco", você se torna imune a todos os efeitos, além de tirar a rodada de cast de "encantamento superior"
Buraco negro: causa dano a todos os inimigos e embaralha suas posições
Maré de medo: causa dano, aplica escuridão e medo aos inimigos
Destino mutável: pula o turno do inimigo escolhido
Esfera negra perfeita: "buraco negro" ganha uma variante de ataque em alvo único, causando dano verdadeiro e dando "desarme" permanente
Transcender: desbloqueia mais uma árvore de habilidade
Fluxo liberto: enquanto não estiver com relíquias do tipo "armadura" você recebe mais dano, mas causa mais dano, e aplica escuridão em todos os ataques. Ataques que causem efeitos tem seu efeito substituido por tal
Chuva de raios: invoca 6 disparos que causam dano e aplicam luz e cegueira
Fragmentar: quando um aliado erra um ataque ou um inimigo acerta um ataque em quem quer que seja, você tem chance de refazer esse ataque, alterando o resultado
Ritual não-ortodoxo: se terminar um combate com buffs acumulados, você os preserva até o próximo combate, com sua duração renovada

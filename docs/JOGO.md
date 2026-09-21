# O jogo — Contos de Outrora: O Rei De Amarelo

Detalhamento de lore, personagens e mecânicas. O [CLAUDE.md](../CLAUDE.md) tem só o essencial e
aponta para cá, para não carregar tudo isso em toda sessão.

**Fontes:** `docs/externo/Infos/` — GDD (lore e mundo), Árvores de Habilidades (**combate, é a
fonte oficial**) e Demo (escopo do que será entregue).

---

## Identidade

| Item | Definição |
|---|---|
| Gênero | RPG tático por turnos |
| **Referência de câmera e mapa** | **Don't Starve Together** (definido em 18/09/2026, substituiu Darkest Dungeon) |
| Mapa | **3D isométrico** |
| Personagens | **2D**, como folhas de papel em pé dentro do mundo 3D |
| Resolução base | 1920x1080 |

### Câmera

Isométrica, **girando em torno do personagem**, que fica sempre no centro.

- **8 ângulos:** 0, 45, 90, 135, 180, 225, 270, 315
- **Padrão: 270°** (câmera ao sul, olhando para o norte)
- **Q** gira anti-horário, **E** gira horário — 45° por toque, com transição suave
- Poder girar a câmera é **mecânica de jogo**, não enfeite: serve para olhar atrás de paredes e
  descobrir o que está escondido

### Combate no lugar onde começou

**Não há tela separada de combate.** O encontro acontece no ponto do mapa onde foi disparado, como
em Baldur's Gate e Divinity. A entrada no combate é a **câmera se aproximando** — referência dos
Metal Gear antigos — e nunca um corte com tela preta.

Isso dá continuidade: o cenário, a posição dos inimigos e os obstáculos do mapa continuam valendo
durante a luta.

---

## Premissa

A ilha de Euduro, rebatizada **Carcosa** pelo rei **Hastur Carcosa**, que se fundiu a um homúnculo
para fugir da morte. O tempo na ilha foi congelado pelo mago **Eldaquias**, e os não nobres foram
escravizados nas profundezas. Os personagens chegam por caminhos diferentes e todos terminam presos
nas masmorras.

### Narrador

O jogo é narrado por **Abanur, o deus da morte e das artes**, que aparece ao jogador trajado como
um **bufão contador de histórias**. Ele não só narra trechos do jogo, como **aparece em cena**.

**Confirmado pelo PO em 18/09/2026:** o NPC antes chamado "Bufão Alegre" **é o próprio Abanur**.
A pasta de assets foi renomeada para `Abanur_Bufao`.

---

## Personagens

| Personagem | Classe | Pasta de assets |
|---|---|---|
| Khalid de Nortumbria | Cavaleira | `Khalid_Cavaleira` |
| Amana A'Bajal | Xamã | `Amana_Xama` |
| Rosaria Percival | Aberração | `Rosaria_Aberracao` |
| Gael Nebraska | Hemomante | `Gael_Hemomante` |
| Tao A'Bajal | Peregrino | `Tao_Peregrino` |
| Uzhan N'Daka | Desgarrado | `Uzhan_Desgarrado` |
| Lancelot Claivar | Escudeiro | `Lancelot_Escudeiro` |
| Jedara, Filho de Tauron | Brutamonte | `Jedara_Brutamonte` |
| Varossa K'Ushim / Homem Misterioso | Bruxo | `Varossa_Bruxo` |
| Emi Matsunaga | Caçadora | `Emi_Cacadora` |
| Criança | (recrutável) | `Crianca` |

**Grafias oficiais:** Xamã, Tao, Jedara. O GDD tem grafias antigas em alguns trechos
("Cartomante", "Thao", "Jedah") — usar sempre as oficiais.

Personagens jogáveis têm **lore em 4 Atos**, com 2 escolhas por Ato. Os recrutáveis só por
gameplay (Amana, Gael, Jedara, Varossa, Rosaria) **não precisam** de lore extensa.

---

## Combate

> A **Árvore de Habilidades é a fonte oficial**, não o GDD. As seções de combate e status do GDD
> estão desatualizadas e já levam aviso.

### As 5 ações do turno

**Atacar · Habilidades · Defender · Itens · Fugir**

**1 ação por turno**, salvo item ou habilidade que contorne isso.

**Conversar foi removida** (não seria viável em boa parte do jogo), junto com Enganar, Ameaçar,
Furtar e Expor. **Não existe ação de "mover"**: a movimentação na formação acontece pelas próprias
habilidades (*Avanço Tático* avança e empurra, etc.).

### Formação

5 posições. A posição importa: há habilidades que só funcionam em certos lugares da formação.

### Efeitos de status

**Gelo · Veneno · Sangramento · Fogo · Corrosão · Raio · Escuridão · Luz**

> "Veneno" era chamado de "Ácido" até 18/09/2026. "Corrompido" e "Purificado" são nomes antigos de
> Escuridão e Luz — **não existem no jogo**.

### Condições

Desarmado, Desvantagem, Vantagem, Atordoado, Enraizado, **Vulnerável** (não pode receber buffs),
Couraça, **Vanguarda** (protege os aliados e toma o dano no lugar deles), Furtivo, Cego e
**Espinhos** (devolve parte do dano recebido).

### Atributos

- **Corpo** — vida, Precisão, **Furtividade**, Reação, Robustez
- **Mente** — sanidade, Vontade
- **Essência** — foco, Energia, Aura

**PF (Pontos de Foco)** é o recurso gasto pelas habilidades.

> **Decisão do PO em 18/09/2026:** **Lábia, Intuição e Análise foram removidas** de vez. Elas só
> serviam à ação Conversar, que saiu do jogo. Não entram na ficha do personagem.

### Furtividade — dois efeitos

A Furtividade **ficou**, e agora faz duas coisas:

1. **Em combate:** reduz o agro inimigo, como já fazia.
2. **Na exploração:** entra na **detecção dos inimigos** — e se soma a **agachar**, que é o efeito
   que o jogador liga na hora (ver "Agachar" mais abaixo). Cada inimigo tem um **raio de agro** —
   a distância em que percebe o personagem. Quanto maior a Furtividade, **menor fica esse raio**,
   e mais perto o personagem consegue chegar sem ser notado.

Isso liga a Furtividade direto à mecânica de **perseguidores** e às **safe zones** da demo: um
personagem furtivo consegue atravessar trajetos vigiados que um barulhento não conseguiria.

---

## Exploração e progressão

- **Estátuas do Rei** — salvar e viajar rápido
- **Morrer volta ao save**
- **Inimigos e loot são fixos e não respawnam.** Confirmado pelo PO em 18/09/2026. Está previsto
  criar, **depois da demo**, um item ou mecânica que faça um inimigo ou uma área inteira voltar a
  aparecer — mas **não entra na demo**
- **Mapa estático**, com continuidade: o que o jogador alterou permanece alterado
- **Safe zones** primárias (mais recursos e opções) e secundárias (limitadas)
- **Perseguidores** que caçam o jogador em certos trajetos
- **Armadilhas fixas** que se camuflam depois de acionadas
- **Minimapa** liberado aos poucos

**Progressão:** "lascas de Euduroh" (de inimigos comuns) desbloqueiam habilidades; "jóias douradas"
(drop garantido de chefe) sobem o nível. Skill tree com 3 caminhos — escolher um **bloqueia os
outros dois**, e só uma jóia sacrificada numa estátua permite refazer.

> Como **não há respawn na demo**, as lascas são **finitas**. Isso torna o balanceamento sensível:
> se o jogador gastar errado, ele trava sem ter como juntar mais. A mecânica de respawn prevista
> para depois da demo resolve isso — mas até lá, a quantidade distribuída pelo mapa **é tudo que
> existe**.

---

## Escopo da Demo

**Mapa:** primeira região — minas subterrâneas, com 10 áreas: Prisão, Ala das abominações,
Escritório, Centro de mineração, Poço das moléstias, Refúgio dos desamparados (safe zone), Antigo
sistema de esgoto (secreta), Portão do sacrifício (secreta/safe zone), Túnel soterrado (secreta) e
Sala de contenção (secreta).

**Chefes e inimigos únicos:** Homem mascarado, Ciclope, Quimera rastejante, Grupo ocultista,
Aparição nefasta, Senhor das moscas, Brutamonte e Centopéia gigante.

**Side-quests (4):** Brutamonte, Xamã, Hemomante e Ocultistas.

> A **quest do Ferreiro saiu da demo** (decisão do PO em 18/09/2026). Era ela que fazia a conta
> não fechar: o documento dizia "4 side-quests" mas listava 5.

**3 finais:** derrotar a quimera e abrir o portão do sacrifício; vencer a Centopéia gigante e subir
à superfície; ou a Centopéia destruir o elevador e o jogador cair nas profundezas.

---

## Detecção e perseguição (implementado em 18/09/2026)

### Os quatro modos do inimigo

| Modo | Quando | O que faz |
|---|---|---|
| **Ronda** | padrão | Segue o caminho combinado, sem suspeitar de nada |
| **Ouviu algo** | a suspeita chegou a 10% **por ouvido** | Para, estranha ("?"), olha para os dois lados e só então vira para o som |
| **Busca** | investigou e ainda sente algo | Vai até onde percebeu algo, olha em volta e, se não achar, volta à ronda |
| **Perseguição** | a suspeita chegou a 100% | Vai atrás até chegar na distância de combate |
| **Parado (idle)** | ocupado | Numa atividade (sentado, dormindo), com os raios encolhidos |

### A barra de suspeita — quem decide tudo

Nenhum raio decide nada sozinho. **Todos alimentam a mesma barra de 0% a 100%**, e é ela que dá as
ordens: aos **10%** o inimigo estranha, aos **100%** ele reconheceu o jogador.

**O problema que isso resolve:** quando o contato dos círculos era um interruptor, bastava entrar e
sair do alcance de propósito para o inimigo ficar preso parando e olhando em volta, sem nunca voltar
à ronda. Era uma trava que o jogador aprendia a explorar. Com uma barra, o entra-e-sai **acumula**
em vez de reiniciar, e a reação de espanto não se repete enquanto ele não se acalmar de verdade.

| O que ele percebe | Quanto sobe por segundo | Até onde pode chegar |
|---|---|---|
| **Ouviu** (os círculos externos se tocam) | 20% | **só 50%** |
| **Está quase esbarrando** (alcança o círculo interno) | 70% | 100% |
| **Está vendo** (cone de visão) | o bastante para encher em 1,2 s | 100% |

**Por ouvir, a barra para na metade.** Por mais tempo que o jogador fique roçando o alcance
auditivo, o inimigo nunca passa de desconfiado — para completar, ele precisa chegar perto ou
enxergar de fato. É isso que permite atravessar uma sala sem ser pego, desde que não se entre no
campo de visão.

**Quanto mais cheia a barra, mais rápido ele reconhece.** Quem já ouviu barulho não precisa de um
olhar demorado para confirmar. Medido: um inimigo tranquilo leva **0,85 s** de cone de visão para
partir para a perseguição; o mesmo inimigo com a barra na metade leva **0,47 s** — quase o dobro
de rápido. E o contrário também vale: a barra é o que impede o inimigo de reconhecer o jogador no
mesmo instante em que o vê.

**Desconfiar é rápido, esquecer é lento.** A barra sobe em segundos e desce a 8% por segundo,
depois de 1,5 s sem perceber nada. Um inimigo que ouviu algo e não achou volta à ronda **ainda
desconfiado** — o segundo barulho é percebido mais rápido que o primeiro.

Os números ficam no nó `MedidorDeSuspeita` de cada inimigo, ajustáveis um a um no editor.

### A visão do personagem segue o mouse

O personagem **vira para onde o cursor aponta**, e a câmera **não gira junto** — ela continua nos
8 ângulos fixos, movida só por Q e E.

Isso separa duas coisas que antes eram a mesma: **para onde ele anda** e **para onde ele olha**.
Dá para atravessar um corredor de costas para a parede vigiando a porta, ou recuar sem tirar os
olhos do inimigo. É o que torna a névoa jogável: como só se enxerga dentro do cone, o jogador
precisa poder escolher para onde olhar sem abrir mão de para onde vai.

Verificado nos 8 ângulos da câmera: com o cursor sobre um ponto fixo do mapa, o personagem encara
esse ponto em todos eles (erro máximo de 0,2°).

Em `ApontarComOMouse = false` o personagem volta a olhar para onde anda, que era o comportamento
anterior.

### Névoa de guerra — o jogador só vê o que o personagem veria

**O problema que isso resolve:** num mapa 3D visto de cima, a câmera mostra o que está atrás das
paredes — inclusive o outro lado da sala. O mapa inteiro se entrega de graça e não sobra tensão
nenhuma.

O mundo fica **sempre coberto de névoa**, e ela abre só onde o personagem enxerga:

| Faixa | O que o jogador vê |
|---|---|
| **Passiva 1** (3,5 m) | Claro, sem precisar olhar |
| **Passiva 2** (6,0 m) | Levemente embaçado, sem precisar olhar |
| **Cone nível 1** (até 6,1 m) | Claro |
| **Cone nível 2** (até 11,2 m) | Levemente embaçado |
| **Cone nível 3** (até 16,0 m) | Só um vulto: dá para ver que tem alguém, não quem é |
| Fora de tudo | Névoa |

> **Atenção à numeração:** no cone, **nível 1 é o mais perto e o mais claro**. É o contrário dos
> raios de detecção, onde o nível 1 é o de fora. Lá o número cresce para fora, aqui cresce para
> longe.

**Os alcances da visão são números próprios**, separados dos raios de detecção. Ver e ser visto são
coisas diferentes — e os raios de detecção encolhem ao agachar, então, se fossem os mesmos, agachar
cegaria o jogador.

**A parede esconde o que está atrás.** Não é só distância e ângulo: o personagem lança linhas
imaginárias em roda e mede onde estão as paredes, e a névoa respeita esse contorno. Um inimigo a
11 m, **dentro do cone de visão**, mas atrás de uma divisória, fica invisível.

**A cor da névoa é o que separa neblina de escuridão.** Cinza claro dá névoa de exterior; quase
preto dá porão sem lamparina. A mesma mecânica serve para iluminação, mudando só a cor.

**A memória visual:** o jogador não esquece o que viu.

| O que | Como a memória se comporta |
|---|---|
| **Cenário** (paredes, casa, caixas) | Fica **para sempre**, apagado e sem cor. Casa não anda: saber onde ela está nunca fica errado |
| **Inimigo** | Fica um **borrão** no último lugar visto, que envelhece e some em **5 s** — e some **na hora** se o jogador olhar para lá e não achar ninguém |

É a diferença entre "não sei o que tem lá" e "sei o que tem lá, só não estou olhando agora". O
lembrado aparece **sem cor**, para o jogador distinguir de relance informação de agora de
informação velha.

O borrão do inimigo é desenhado por cima da névoa e **atravessa parede** — a lembrança está na
cabeça do jogador, não no mundo.

**Ainda por fazer, já previsto:** inimigos que a névoa não afeta (o interruptor `IgnoraANevoa` já
existe em cada inimigo) e itens que ampliam a visão do jogador — basta mexer nos alcances do
sensor.

**F4 liga e desliga a névoa** durante o jogo. Isso é ferramenta de teste, ao contrário da névoa em
si, que é mecânica.

### Agachar — a furtividade que o jogador liga na hora

**Segurar `Ctrl`** (ou `C`) deixa o personagem agachado. A Furtividade da ficha é passiva e vale
sempre; agachar é a versão ativa, que o jogador decide usar no momento em que precisa.

| | Em pé | Agachado |
|---|---|---|
| Raio externo (n1) | 5,00 m | **2,75 m** (−45%) |
| Raio interno (n2) | 2,50 m | **1,38 m** (−45%) |
| Tempo até ser reconhecido | 1,20 s | **2,16 s** (+80%) |
| Altura dos olhos | 1,50 m | 0,80 m |
| Alcance da própria visão | 10,0 m | 10,0 m — **não muda** |
| Velocidade de caminhada | 5,0 m/s | **2,25 m/s** |

**Os dois efeitos são diferentes e se completam.** Encolher os círculos muda **a que distância**
o inimigo começa a desconfiar: a distância em que os círculos se tocam cai de 10,0 m para 7,75 m.
Aumentar o tempo muda **quanto ele demora a confirmar** depois que já está olhando — medido, dentro
do cone de visão a 9 m: **0,85 s em pé contra 1,52 s agachado**.

**Agachado não é invisível.** Se o inimigo olhar direto, ele acaba vendo — só demora quase o dobro,
e é essa demora que dá tempo de sair da linha de visão.

**Agachar não cega:** o alcance da própria visão do personagem continua o mesmo. Ele se esconde,
não deixa de enxergar.

**Segurar ou alternar** é escolha do jogador: `ModoDeAgachar` aceita as duas, e o **módulo de
configurações** que vier depois só precisa escrever nesse campo. O padrão é segurar.

**O preço é a velocidade**, e é o único motivo para levantar. Sem ele, agachar seria sempre melhor
que andar em pé e viraria uma tecla obrigatória em vez de uma escolha. Em
`FatorDeVelocidadeAgachado = 1.0` a penalidade some, se a equipe preferir.

A altura dos olhos mais baixa ainda não muda nada, porque as paredes da sala de teste vão do chão
ao teto — é o que vai permitir se esconder atrás de mureta, caixa ou parapeito quando o cenário
tiver disso.

### A reação ao barulho — o que torna a furtividade possível

**O problema que isso resolve:** antes, no instante em que os círculos se tocavam o inimigo virava
na direção do jogador. O cone de visão caía em cima dele de imediato e a perseguição começava. Não
havia chance nenhuma de se esconder — qualquer plano de furtividade morria ali.

Agora o contato dos círculos significa apenas que **o inimigo ouviu um barulho**. Ele não sabe o
que é nem exatamente de onde veio, e reage como uma pessoa reagiria — a sequência abaixo começa
quando a barra de suspeita cruza os 10%, não no primeiro roçar dos círculos:

| Etapa | O que acontece |
|---|---|
| 1. Estranhou | Para de andar, aparece o **"?"** sobre a cabeça. **Não vira para lado nenhum** |
| 2. Olha um lado | Vira a cabeça ~65° para um lado, devagar |
| 3. Olha o outro | Vira para o outro lado |
| 4. Vira para o som | Só agora encara a direção de onde veio o barulho |
| 5. Decide | Ainda sente algo? Vai investigar. Não sente? Dá de ombros e volta à ronda |

Cada etapa leva um tempo (padrão 0,9 s), e ele fica **parado** durante toda a sequência. Isso dá
ao jogador cerca de **4 segundos** para sair da linha de visão — é essa janela que faz a
furtividade existir.

A cabeça **gira aos poucos**, nunca num salto. Girar instantaneamente entregaria a posição do
jogador no mesmo quadro do barulho; girando devagar, o cone varre o ambiente de forma visível e o
jogador vê o perigo se aproximando.

**A sequência só acontece quando ele ouviu.** Se o jogador já está no campo de visão dele, virar a
cabeça para os lados seria desviar o olhar de quem ele está olhando. Nesse caso ele só mostra o "?"
e continua o que estava fazendo enquanto a barra sobe.

**Os balões são parte do jogo, não ferramenta de teste:** "?" a partir de 10% de suspeita, "!" ao
reconhecer. Continuam ligados na versão publicada — é por eles que o jogador sabe que ainda dá
tempo de correr.

O inimigo pode largar a ronda para ir fazer uma atividade — a chance por segundo é configurável.
Inimigos que **só** ficam parados devem começar em Idle com essa chance em zero.

### Os raios

**A regra central:** os raios são círculos, e o que importa é se dois círculos **se tocam** — não
se um está dentro do outro. Por isso a distância que conta é sempre a **soma dos dois raios**. É
isso que faz a Furtividade valer: encolher o círculo do jogador reduz o alcance de quem o procura.

| Raio | Quem tem | Para que serve |
|---|---|---|
| **Externo (nível 1)** | os dois | Quando os externos se tocam, o inimigo **ouve** — a barra sobe devagar, até no máximo 50% |
| **Interno (nível 2)** | só o jogador | Quando o externo do inimigo alcança este, não há dúvida — a barra sobe rápido, sem teto. É sempre metade do externo |
| **Encontro** | só o inimigo | Distância em que o combate começa. Varia com a arma — arco alcança mais que espada |
| **Visão** | os dois | Cone na direção em que olha. Alcança mais longe que o externo, por ser focado. Enche a barra sem teto |

Nenhum deles vira detecção sozinho: **os três alimentam a barra de suspeita**, e é ela que decide.
A Furtividade **aumenta** o tempo de cone necessário para encher a barra; a Reação do inimigo, por
decisão de design, **não o diminui**.

### Como os atributos entram

| Atributo | De quem | Efeito |
|---|---|---|
| **Furtividade** | jogador | **Encolhe** os próprios círculos e **aumenta** o tempo até ser notado |
| **Reação** | inimigo | **Aumenta** o próprio círculo e o tempo que ele insiste antes de desistir |

Cada ponto vale 5% (valor de partida), com teto de 70% para ninguém virar invisível. Medido:

| | Furtividade 0 | Furtividade 5 | Furtividade 10 |
|---|---|---|---|
| Primeiro contato | 10,0 m | 8,75 m | 7,50 m |
| Tempo até ser notado | 1,20 s | 1,50 s | 1,80 s |

### Paredes bloqueiam

Antes de qualquer decisão, o sistema confere se há **linha de visão livre** entre os dois. Sem
isso, o inimigo perseguiria o jogador através de um muro e o jogo de furtividade não existiria.

Quais paredes bloqueiam é configurável: uma grade ou mureta baixa pode deixar ver através, bastando
tirá-la da camada considerada.

### Testar

A cena `Scenes/Levels/Sandbox.tscn` é o mapa de testes: **70 x 70 m**, com uma casa de dois
andares e rampa (para testar interiores e elevação), caixas e divisórias soltas, e **dois
inimigos** — um em ronda por cinco pontos e um parado em Idle dentro da casa.
**F3 liga e desliga o desenho dos raios.**

> **Ao montar uma sala:** o nó que guarda os pontos da ronda deve se chamar **`MarcasDaRonda`** —
> é por esse nome que o inimigo acha o trajeto sozinho, sem precisar ligar nada na cena. Salas com
> mais de uma rota indicam cada uma pelo campo `CaminhoDaFase` do inimigo.

| Cor | O que é |
|---|---|
| Amarelo | Raio externo (desconfiança) |
| Laranja | Raio interno do jogador (certeza) |
| Vermelho | Raio de encontro do inimigo (combate) |
| Azul | Cone de visão |

Sobre a cabeça do inimigo aparecem o modo em que ele está, a etapa da reação ao barulho e a barra
de suspeita — por exemplo `[====......] 41% ouvindo`. A palavra do fim diz **qual** dos alcances
está enchendo a barra (`ouvindo`, `perto`, `vendo`), que é metade do trabalho na hora de ajustar os
números.

### Ainda não implementado

- **Ouvir** — a estrutura prevê "última posição percebida", mas só a visão alimenta isso hoje
- **Início do combate** — ao chegar na distância de encontro o inimigo **para, encara o jogador e
  emite o aviso `CombateDeveComecar`**, uma vez por encontro. Quem for fazer o combate por turnos
  escuta esse aviso. Até lá ele fica parado ali de arma em punho: não é travamento, é a espera. Se
  o jogador correr, ele volta a perseguir; se sumir, ele desiste e retoma a ronda

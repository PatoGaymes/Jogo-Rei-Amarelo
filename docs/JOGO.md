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
2. **Na exploração:** entra na **detecção dos inimigos**. Cada inimigo tem um **raio de agro** —
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

A cena `Scenes/Levels/SalaTeste.tscn` tem paredes, divisórias e um inimigo em ronda.
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

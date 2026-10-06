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
- **No combate** (06/10/2026): a câmera se aproxima suavemente ao começar a luta (de 9 para 8 m, sem
  corte de tela), acompanha quem está na vez, continua girando com Q/E e a roda do mouse afasta e
  aproxima (de 5 a 16 m). Ao terminar a luta, volta à exploração e cola de novo no personagem.

### Cenário: objetos 2D que encaram a câmera (30/09/2026)

É o jeito do Don't Starve de fazer um mundo 3D com arte 2D:

- **Objeto de uma casa só** — uma árvore, uma pedra: **uma imagem** que fica sempre de frente
  para a **câmera** (e não para o personagem). Girando a câmera, a árvore continua de frente; o que
  muda na tela é a posição dela no mapa, e é isso que dá a impressão de 3D.
- **Estrutura de várias casas** — um muro, uma cerca, uma construção sem teto: **uma imagem por
  casa**, cada peça encarando a câmera. As peças vizinhas se sobrepõem e formam um muro contínuo, e
  a planta da estrutura (o desenho dela no chão) gira junto com a câmera.

No sandbox: **16 pinheiros** e uma **estrutura sem teto de 32 blocos de pedra** (9 × 8 casas,
entrada ao sul e uma parede interna em L), no espírito da do exemplo do Don't Starve.

| Peça | Cena | Bloqueia passagem | Bloqueia visão |
|---|---|---|---|
| Pinheiro | `Scenes/Cenarios/Pinheiro.tscn` | sim (o tronco) | não — um tronco fino não deve riscar a névoa |
| Bloco de muro | `Scenes/Cenarios/MuroDePedra.tscn` | sim | sim |

Os inimigos contornam os dois sozinhos. Para isso foi criada a 6ª camada de física, **Obstacle**:
o que bloqueia a passagem mas não a visão.

**Por que as imagens ficam em pé, e não inclinadas de frente para a câmera como no Don't Starve:**
o nosso mapa tem **paredes 3D de verdade** (a casa). Uma imagem inclinada para trás atravessaria a
parede — um personagem encostado na casa teria a cabeça cortada por ela. Em pé, personagens,
árvores, muros e paredes 3D se escondem uns atrás dos outros na ordem certa. O efeito colateral vem
da perspectiva da câmera: **nas bordas da tela, objetos altos parecem levemente tortos para fora**.
Diminuir o campo de visão da câmera (hoje 50°) e afastá-la reduz isso — fica como sugestão para
testar.

**Sombra:** cada objeto tem uma **sombra redonda e suave no pé**, como no Don't Starve. O sol não
projeta a sombra das imagens: projetaria triângulos pretos que mudam de forma quando a câmera gira.
A sombra redonda aparece no mapa limpo; na névoa e na escuridão, a camada que esconde o mapa pinta
por cima dela.

**Arte:** o pinheiro e o bloco são **provisórios**, desenhados por código
(`Assets/Art/Cenarios/<Objeto>/Provisorio/`). Para a arte definitiva vale a mesma regra dos
personagens: a base do objeto no centro, embaixo da imagem. E imagens em pé, vistas pela câmera a
50°, aparecem com cerca de **64% da altura desenhada** — vale desenhar muros um pouco mais altos do
que se quer ver na tela.

### Combate no lugar onde começou

**Não há tela separada de combate.** O encontro acontece no ponto do mapa onde foi disparado, como
em Baldur's Gate e Divinity. A entrada no combate é a **câmera se aproximando** — referência dos
Metal Gear antigos — e nunca um corte com tela preta.

Isso dá continuidade: o cenário, a posição dos inimigos e os obstáculos do mapa continuam valendo
durante a luta. Desde 06/10/2026 a luta é em **hexágonos**, no estilo do Pit People — ver "Combate".

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

> **Desde 06/10/2026 a documentação do projeto (esta) vale mais que a do Drive** quando as duas
> divergem — com exceções indicadas pela equipe (a primeira: o atributo Iniciativa, que veio do GDD
> do Drive). A Árvore de Habilidades continua sendo a lista oficial de habilidades, efeitos e
> condições; as decisões abaixo, tomadas pelo PO em 06/10/2026, corrigem o que ela dizia sobre
> ações, Vanguarda e o recurso das habilidades. A versão corrigida foi para a pasta **Revisão** do
> Drive.

> **Os números do combate são simbólicos.** O balanceamento ainda está sendo feito; o que existe
> hoje serve para testar a mecânica, as animações e a jogabilidade.

### Como é a luta — hexágonos no próprio mapa (06/10/2026)

Referência de disposição e movimentação: **Pit People**. A luta acontece **no lugar do mapa onde
começou**, como no Baldur's Gate 3: o cenário daquele ponto é o campo de batalha, com terreno mais
alto e mais baixo, obstáculos e construções valendo.

- O chão é dividido em **casas hexagonais** (1 m do centro à ponta; 1,73 m de uma casa à vizinha).
- **Os hexágonos só aparecem durante o combate**, numa área de 9 casas em volta da luta.
- **A grade é fixa por mapa**: é calculada uma vez quando a fase carrega (no sandbox, 1.863 casas em
  ~0,3 s), e não na hora em que a luta começa. Assim os hexágonos caem sempre nos mesmos lugares e
  não ficam cortados de um jeito diferente a cada luta. Cada mapa tem um nó `GradeDeCombate`; quem
  monta a fase confere a grade inteira com **F6** e, se uma porta ou rampa ficar sem passagem, ajusta
  a posição do nó (no sandbox ela foi deslocada 0,5 m para oeste, para a rampa funcionar).
- **Andares e rampas contam.** O mesmo hexágono pode ter uma casa no térreo e outra no andar de cima.
  Dá para subir de uma casa para a vizinha num degrau de até 0,6 m, ou numa rampa de até 1,2 m por
  casa (a rampa do sandbox tem 20°: 0,63 m por casa).
- **Parede** some com a casa só se encostar no centro dela; **árvore e outros obstáculos ocupam a casa
  inteira** em que estão. Topo de caixa e de muro existe, mas não se liga ao chão.

### O turno

As **5 ações**: **Atacar · Habilidades · Defender · Itens · Fugir** — 1 por turno, salvo item ou
habilidade que contorne isso.

**Mover (06/10/2026):** todos, aliados e inimigos, andam **até 3 casas por turno**, e andar **não
gasta a ação**. Dá para andar, atacar e andar o resto. Enraizado impede de andar naquele turno;
Atordoado perde o turno inteiro. Dá para passar pela casa de um aliado, mas não parar nela; casa de
inimigo bloqueia.

**Alcance em casas.** O alcance de ataques e habilidades é contado em casas. Ataque corpo a corpo não
alcança outro andar (mais de 1,2 m de diferença); nada atravessa parede. Árvore não bloqueia a visão
(mesma regra da exploração).

**A animação do golpe encosta no alvo.** O ataque é montado a partir da distância até o alvo e de quem
é o alvo: no corpo a corpo, o atacante anda até a **distância de encaixe** da arma (espada 0,9 m,
lança 1,6 m) e só então golpeia; à distância, o disparo voa até o alvo. **O dano aparece no instante
do impacto.** Hoje isso é feito com a arte provisória (um passo, um tranco, uma bolinha voando); os
quadros de ataque de verdade usam o mesmo plano quando chegarem — o PO vai detalhar essa parte.

| Ação | Hoje |
|---|---|
| Atacar | O ataque básico da ficha. Atalho: clicar direto num inimigo marcado em vermelho; se ele estiver fora do alcance, o personagem anda até uma casa de onde alcança (se o movimento der) e ataca |
| Habilidades | As 3 habilidades padrão de cada personagem, com custo em Foco e alcance |
| Defender | +3 de Reação e +3 de Robustez até o começo do próximo turno (provisório) |
| Itens | Botão presente; o inventário ainda não existe |
| Fugir | Botão presente, **não faz nada** (em desenvolvimento, PO 06/10/2026) |

### Quem entra na luta

**Os dois lados podem começar.** O inimigo começa quando chega no alcance da arma dele (o aviso
`CombateDeveComecar`); o jogador começa **clicando num inimigo** a até 8 m e à vista. Emboscada e
ataque surpresa vêm depois (o PO vai explicar).

**O grito de aviso.** Cada inimigo tem um alcance de grito (campo `AlcanceDoGrito` da ficha, 10 m no
Rasgador, provisório). Quando a luta começa com ele, **todo inimigo dentro desse alcance e sem parede
no meio entra junto** — funciona como a detecção, não atravessa parede. Quem entrou pelo grito
**não grita de novo**: não há efeito em cadeia, então não dá para puxar o mapa inteiro numa luta só.
Futuramente o "início de combate furtivo" vai mexer nesse alcance.

Os inimigos que **não** entraram na luta ficam parados onde estão até ela acabar (provisório — ver
as perguntas no fim).

**A equipe: 5 personagens.** Na exploração só o líder anda pelo mapa; quando a luta começa, os outros
quatro entram nas casas da **formação** montada no menu de equipe.

**Menu de equipe (tecla T, fora do combate).** Por enquanto só a formação: uma mini-região de 19
hexágonos onde dá para **arrastar** cada personagem para outra casa (soltar em cima de alguém troca os
dois). A frente da formação (para cima no menu) é **virada para o lado dos inimigos** quando a luta
começa, de 60 em 60 graus. Se a casa prevista tiver parede, árvore ou alguém no mapa, o personagem
fica na casa livre mais próxima — **os obstáculos contam**.

**Atacar primeiro num diálogo.** Quando o jogador dá o primeiro golpe num NPC, quem atacou ganha
**Vantagem só no primeiro ataque** (só ele). O combate já aceita isso; falta o sistema de diálogo.

### Iniciativa

No começo da luta **cada um rola um dado de 20 lados e soma o atributo Iniciativa**; a ordem vai do
maior para o menor, misturando aliados e inimigos. **Quem tira 20 no dado joga 2 turnos seguidos
sempre que chega a vez dele.** Empate: ganha quem tem mais Iniciativa; persistindo, sorteio.

> O que acontece com quem tira **1** ainda não foi definido — a frase da decisão ficou incompleta.
> Por enquanto, nada (o registro só avisa "tirou 1").

### Vanguarda (06/10/2026)

**Vanguarda é a condição de quem protege**; quem é protegido fica com a condição **Protegido**:

- todo o dano que o protegido receberia vai para a Vanguarda;
- em **dano de área**, metade fica com o protegido e metade vai para a Vanguarda;
- dá para proteger **vários aliados, um por uso** (1 por turno, já que é 1 ação).

Provisório: a proteção da habilidade Protetora dura 3 rodadas, e os efeitos do golpe (veneno etc.)
continuam caindo no protegido — só o dano é desviado.

### Foco

**O recurso das habilidades é o Foco** (PO, 06/10/2026). A Árvore falava em "gastar e recuperar
energia"; a versão corrigida diz Foco. O **atributo** Energia continua sendo o que reduz o custo de
Foco (GDD). Provisório: o Foco começa cheio em cada luta.

### Morte, sanidade e fim de jogo

- **Vida em 0: morre.** Inimigo morto sai do mapa (não há respawn na demo). Membro da equipe morto fica
  morto. Se o líder morre e o grupo vence, o próximo vivo assume e passa a andar pelo mapa.
- A **vida** e a **sanidade** continuam de uma luta para a outra.
- **Grupo inteiro caído: tela de fim de jogo**, perguntando se o jogador quer voltar ao último save ou
  ao menu. São provisórios: como o save e o menu ainda não existem, os dois botões recomeçam o mapa
  com a equipe inicial.

**Sanidade baixa — alucinações (regra definida em 06/10/2026, ainda não implementada):**

| Sanidade | O que acontece |
|---|---|
| 30% ou menos (por enquanto) | Alucinações aparecem no mapa e vagam de forma neutra: não atacam se o jogador não atacar. No combate, entram na iniciativa normalmente, mas ficam paradas ou só andam a esmo |
| 0 | Todas ficam hostis: atacam e perseguem o jogador, com a mesma mecânica dos inimigos comuns |

As alucinações são vultos/borrões pretos, cópias **mais fracas em tudo** dos inimigos que existem no
jogo (dependendo da área).

### O ambiente no combate

Em desenvolvimento (WIP): a névoa e a escuridão vão afetar o combate, além de outras mudanças que a
equipe ainda está desenhando. Hoje elas continuam valendo durante a luta como na exploração, e a grade
é desenhada por cima delas para continuar legível.

### Formação

A antiga "formação de 5 posições" deu lugar à formação em hexágonos do menu de equipe (ver "Quem entra
na luta"). As habilidades que falam de "adjacente", "a sua frente", "primeira posição", "empurrar"
passam a valer na grade: adjacente é a casa vizinha; empurrar afasta o alvo casa a casa (se não houver
casa livre atrás, ele não sai do lugar).

### Efeitos de status

**Gelo · Veneno · Sangramento · Fogo · Corrosão · Raio · Escuridão · Luz**

> "Veneno" era chamado de "Ácido" até 18/09/2026. "Corrompido" e "Purificado" são nomes antigos de
> Escuridão e Luz — **não existem no jogo**.

### Condições

Desarmado, Desvantagem, Vantagem, Atordoado, Enraizado, **Vulnerável** (não pode receber buffs),
Couraça, **Vanguarda** (quem protege — ver acima) e **Protegido**, Furtivo, Cego e **Espinhos**
(devolve parte do dano recebido).

### Atributos

- **Corpo** — vida, Precisão, **Furtividade**, **Iniciativa**, Reação, Robustez
- **Mente** — sanidade, Vontade
- **Essência** — foco, Energia, Aura

**PF (Pontos de Foco)** é o recurso gasto pelas habilidades.

> **Iniciativa** entrou no GDD do Drive em 06/10/2026 ("atributo focado exclusivamente para
> iniciativa em combates"), na lista logo depois de Furtividade. O GDD não diz de qual grupo ela é;
> aqui ficou em Corpo, pela posição na lista.

> **Decisão do PO em 18/09/2026:** **Lábia, Intuição e Análise foram removidas** de vez. Elas só
> serviam à ação Conversar, que saiu do jogo. Não entram na ficha do personagem.

### Furtividade — dois efeitos

A Furtividade **ficou**, e agora faz duas coisas:

1. **Em combate:** reduz o agro inimigo. Implementado na escolha de alvo dos inimigos: cada ponto de
   Furtividade faz o personagem parecer meia casa mais longe.
2. **Na exploração:** entra na **detecção dos inimigos** — e se soma a **agachar**, que é o efeito
   que o jogador liga na hora (ver "Agachar" mais abaixo). Cada inimigo tem um **raio de agro** —
   a distância em que percebe o personagem. Quanto maior a Furtividade, **menor fica esse raio**,
   e mais perto o personagem consegue chegar sem ser notado.

Isso liga a Furtividade direto à mecânica de **perseguidores** e às **safe zones** da demo: um
personagem furtivo consegue atravessar trajetos vigiados que um barulhento não conseguiria.

### O que foi implementado (06/10/2026)

**Equipe de teste:** Uzhan (Desgarrado, líder), Khalid (Cavaleira), Lancelot (Escudeiro), Emi
(Caçadora) e Tao (Peregrino), cada um com ataque básico e as 3 habilidades padrão da Árvore. Inimigo:
o Rasgador (ataque básico). Todos são arquivos de dados em `Resources/`, para o game designer ajustar
sem programar.

| Personagem | Ataque básico | Habilidades padrão (Foco · alcance) |
|---|---|---|
| Desgarrado | 4–7, corpo a corpo | Postura oculta (2 · em si) · Rasga-ossos (3 · 1) · Alma imaculada (2 · em si) |
| Cavaleira | 3–6, corpo a corpo | Protetora (2 · 3) · Avanço tático (3 · 3) · Compasso (2 · em si) |
| Escudeiro | 3–5, lança, alcance 2 | Lança sagrada (2 · 2) · Fogo acolhedor (3 · 3) · Sermão divino (3 · 2) |
| Caçadora | 3–6, arco, alcance 5 | Embuste (2 · 4) · Técnica secreta (2 · em si) · Matadora de Yokais (3 · 5) |
| Peregrino | 2–5, disparo, alcance 3 | Energia comprimida (2 · 3) · Transfiguração térmica (3 · 4) · Voz amaldiçoada (4 · 3, área 1) |
| Rasgador | 3–6, garras | — |

**Números simbólicos** (em `Scripts/Combat/RegrasDeCombate.cs`):

| Regra | Valor provisório |
|---|---|
| Chance de acerto | 75% + 5% por ponto de Precisão do atacante acima da Reação do alvo (entre 10% e 95%) |
| Vantagem / Desvantagem | ±15% de acerto e ±25% de dano |
| Cego | −30% de acerto |
| Dano | sorteado entre o mínimo e o máximo da habilidade, menos a Robustez do alvo (mínimo 1). Crítico + dano máximo ignora a Robustez |
| Veneno / Fogo | 2 de dano por acúmulo no começo do turno; perde 1 acúmulo por turno. Fogo com 5+ acúmulos passa 1 aos vizinhos |
| Sangramento | 1 por acúmulo, até o triplo quanto menos vida o alvo tem |
| Gelo | atordoa 1 turno; ao sair, 3 de dano |
| Corrosão | −1 de Robustez por acúmulo (até 5) |
| Raio | trava 1 habilidade ao acaso por acúmulo (até 2), por 2 rodadas |
| Escuridão | −10% de vida máxima por acúmulo (até 2) |
| Luz | −25% de cura recebida por acúmulo (até 3) |
| Couraça | bloqueia um golpe inteiro (os efeitos do golpe ainda pegam) |
| Espinhos | devolve metade do dano recebido |

**Efeitos que aparecem nas habilidades e não são explicados em lugar nenhum** (pedido do PO em
06/10/2026). Os que as habilidades padrão usam receberam um valor provisório; os outros ainda não
existem no jogo:

| Efeito | Onde aparece | Hoje |
|---|---|---|
| Maldição | Postura oculta, Destrinchar (Desgarrado) | aplicada pelo contra-ataque da Postura oculta, **sem efeito** |
| Postura oculta | Desgarrado | dano −30%, +2 Reação, +2 Robustez, contra-ataca golpes corpo a corpo com metade do dano |
| Esquiva | Técnica secreta, Um contra todos, Navalhas voadoras... | "+3 de Reação até o próximo turno" na Técnica secreta |
| Marca / marcado | Flanquear, Morte lenta, Prismático, Acusação | não implementado |
| Medo | Cheiro de medo, Maré de medo | não implementado |
| Náusea | Corpo doente | não implementado |
| Taunt (provocação) | Todos de uma vez | não implementado |
| Fúria | Poder e fúria, Falange, Todos de uma vez | não implementado |
| Camuflagem | Sentidos Shinobi | não implementado (é o Furtivo?) |
| Frio / congelamento | Sangue frio, Fratura terminal | não implementado (é o Gelo?) |
| Indestrutível | Indestrutível (Desgarrado) | não implementado |
| Dano verdadeiro | várias | não implementado (ignora a defesa?) |
| Dano massivo / margem de dano massivo | várias | não implementado (é o dano máximo?) |
| Vida temporária, regeneração, sobre-cura, roubo de vida, corta cura | várias | não implementado |
| Invencível | Exaltação | não implementado |
| Imunidade a status | "Inútil", Mandíbula firme | não implementado |
| Estatura (grande, membro) | Propagação, Matador de colossos, Necrosar | não implementado |
| Movimento como número ("perde 1 de movimento", "+1 movimento") | Pesado, Pés arrastados | **agora tem sentido**: são as 3 casas por turno |

**Decisões provisórias tomadas para dar para testar** (a confirmar): Transfiguração térmica sorteia
entre Gelo e Fogo; Técnica secreta recua até 2 casas; Alma imaculada mostra quem cada inimigo vai
atacar, e ele cumpre; Protetora dura 3 rodadas.

### Testar o combate

No sandbox (`Scenes/Levels/Sandbox.tscn`), que começa no escuro: **F4 duas vezes** deixa o mapa limpo.
Há um grupo de dois inimigos parados a oeste do ponto de partida, perto o bastante para um ouvir o grito
do outro.

| Tecla / clique | O que faz |
|---|---|
| T | Menu de equipe (fora do combate) |
| Clique num inimigo a até 8 m | Começa a luta |
| Clique numa casa azul | Anda até ela (a casa mais clara mostra o caminho) |
| Clique num inimigo em vermelho | Ataque básico |
| Botões Atacar / Habilidades | Escolhe o golpe; as casas dos alvos válidos acendem; a área aparece em laranja |
| Botão direito ou X | Cancela a escolha |
| Enter ou "Encerrar turno" | Passa a vez |
| Q / E e roda do mouse | Giram e afastam a câmera durante a luta |
| F6 | Mostra a grade do mapa inteiro (fora do combate) |

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

### Névoa e escuridão — o jogador só vê o que o personagem veria

**O problema que isso resolve:** num mapa 3D visto de cima, a câmera mostra o que está atrás das
paredes — inclusive o outro lado da sala. O mapa inteiro se entrega de graça e não sobra tensão.

**Nem todo mapa tem isso** (decidido em 29/09/2026). Cada fase escolhe o seu ambiente num nó
`AmbienteDaFase`:

| Ambiente | Como fica | Referência |
|---|---|---|
| **Limpo** (mapa sem o nó) | Tudo aparece normalmente, nada ofusca | — |
| **Névoa** | Neblina clara que vai **escondendo** tudo com a distância, e que se move em massas | Silent Hill antigo |
| **Escuridão** | **Breu total** onde não há luz — nem o próprio personagem aparece | Don't Starve Together |

Nos dois últimos, o que não se vê **some de verdade**, não fica só mais escuro.

#### Só um círculo em volta do personagem (01/10/2026)

**O jogador não tem cone de visão.** Ele enxerga num **círculo em volta de si**, igual em todas as
direções — não importa para onde está virado. Foi decidido depois dos testes da escuridão: só o
círculo passa melhor a ideia dos mapas com névoa e escuridão, mantendo a lanterna. A **mira pelo
mouse saiu junto**, e com ela o "moonwalk" (mouse para um lado, andando para o outro, o desenho
mostrava o personagem de frente andando de costas). O personagem olha sempre para onde anda.

> O cone do jogador e a mira pelo mouse existiram de 21/09 a 01/10/2026 — estão registrados em
> [IDEIAS-ARQUIVADAS.md](IDEIAS-ARQUIVADAS.md). **Os inimigos continuam com o cone deles.**

- **Na névoa, tudo vai sumindo aos poucos com a distância.** Não existe borda: a passagem de
  "vejo" para "não vejo" ocupa uns 5 metros.
- **Na escuridão, quem revela é a luz.** A lanterna do jogador desenha o círculo em volta dele.
- **Parede esconde o que está atrás**, com uma **penumbra de pouco mais de um metro** em vez de corte
  seco — a cabeça mexe, e quem está numa quina enxerga um pouco além dela.

Medido na tela, em campo aberto, com a névoa na densidade normal (porcentagem encoberta, em qualquer
direção — a diferença entre olhar para o norte ou para o sul ficou em até 4 pontos, que é a própria
névoa se mexendo):

| Distância | Névoa |
|---|---|
| 2 m | 6 a 7% |
| 3,5 m | 13 a 15% |
| 6 m | 40 a 49% |
| 8,5 m | 72 a 84% |

As duas faixas do círculo continuam como referência: `VisaoPassiva1` (3,5 m) limpa e
`VisaoPassiva2` (6 m) levemente embaçada. Saem do `SensorDeteccao` do jogador — **mudar no inspetor
muda a névoa junto**.

| Antes × depois (medido) | 29/09 | 30/09 |
|---|---|---|
| Borda da sombra de uma caixa, na névoa | de 0% a 96% em 25 cm | passagem de mais de 1 m |
| Maior salto de névoa, a cada 1° em volta de uma caixa | 61% | 9% |
| Chão em volta da fogueira, de costas para ela | preto | igual a olhando para ela |

#### Escuridão

**Tudo o que está iluminado aparece**, olhando ou não — só parede esconde. Medido com a lanterna
apagada: o chão em volta da fogueira aparece com o mesmo brilho olhando para ela e de costas (0,243
nos dois casos; antes, de costas, era 0). Uma fogueira no canto da tela aparece.

Sem luz nenhuma, breu total. A **lanterna do jogador (tecla L)** ilumina o caminho, mas **ilumina o
próprio jogador** — com ela acesa, os inimigos o veem de longe (ver abaixo).

As luzes são nós `FonteDeLuz`: fogueira, lamparina, tocha na mão. Cada uma tem alcance,
intensidade e cor — fogo alaranjado tinge de leve o que ilumina. **A luz não atravessa parede**, e a
sombra que a parede faz na luz também tem penumbra. Chamas tremem de leve para não parecerem lâmpada.

`LuzAmbiente` acima de zero transforma o breu num lugar "muito escuro", onde ainda se adivinham
vultos por perto.

#### Névoa

A névoa **muda com o tempo e de lugar para lugar**, como massas de ar passando: numa brecha ela é
40% mais rala, numa massa densa 60% mais grossa (`DensidadeMinima` 0,6 e `DensidadeMaxima` 1,6 —
1 é a névoa normal). Numa distância média isso dá os **70% a 100%** combinados: a 8,5 m vai de
63% a 92% conforme a massa que passa.

Duas regras seguram o que a névoa pode revelar:

- **Atrás de parede, sempre 100%.** A névoa rala não deixa espiar através de muro.
- **Longe demais, sempre 100%** (a partir de uns 14 m), mesmo numa brecha — senão daria para ver o
  mapa inteiro pelas brechas. A cor da névoa continua se mexendo mesmo longe, para ela não parecer
  uma parede cinza parada.

O inimigo que está longe, dentro da névoa, vira **silhueta escura** — escurece aos poucos conforme
a névoa entre ele e o jogador engrossa, e reaparece por um instante quando uma brecha passa.

#### Os inimigos também são afetados (30/09/2026)

**Só a visão do inimigo muda.** Ouvir (os círculos de detecção) e "quase esbarrar" funcionam igual
no escuro e na névoa: barulho não precisa de luz. Por isso, no escuro, chegar perto demais continua
sendo perigoso.

Antes de "ver" o jogador, o inimigo pergunta o quanto o ambiente deixa enxergar até ele (de 0 a
100%). Abaixo de 15% (`VisaoMinimaParaEnxergar`) não vê; acima, vê, mas **demora na mesma
proporção**: com metade da visão, o dobro do tempo. A conta é a mesma que decide o que o jogador vê
na tela — um lugar que parece escuro para o jogador está escuro para o inimigo.

Medido com o inimigo parado olhando para o jogador:

| Situação | Reconhece o jogador em |
|---|---|
| Mapa limpo, 10,5 m | 0,87 s |
| Escuro, lanterna **acesa** | 0,87 s |
| Escuro, lanterna **apagada** | **nunca** (suspeita fica em 0%) |
| Escuro, lanterna apagada, jogador **ao lado da fogueira** | 0,87 s |
| Névoa, 8,5 m | 2,3 s |
| Névoa, 10,5 m | 5,1 s |

**Cada tipo de inimigo pode sofrer de um jeito.** São dois números no `SensorDeteccao` de cada
inimigo, de 0 a 1:

| Campo | 0 (padrão) | 0,5 | 1 |
|---|---|---|---|
| `EnxergaNoEscuro` | precisa de luz, como gente | distingue vultos no breu (reconheceu em 1,7 s) | enxerga no breu como de dia (0,87 s) |
| `EnxergaNaNevoa` | a névoa atrapalha por inteiro | atrapalha pela metade | a névoa não atrapalha (0,87 s a 10,5 m) |

> **A definir pela equipe:** quais inimigos enxergam no escuro ou ignoram a névoa. A documentação
> ainda não diz, então os dois inimigos do sandbox estão com os valores de gente comum (0 e 0).

O campo `IgnoraANevoa`, da aparência do inimigo, é outra coisa: faz o inimigo **aparecer** por cima
da névoa e da escuridão para o jogador (olhos que brilham no escuro, uma aparição).

#### Desempenho

| | Mapa limpo | Névoa | Escuridão |
|---|---|---|---|
| Placa de vídeo, por quadro | 1,04 ms | 1,50 ms | 1,56 ms |
| Processador, parado | — | 0,003 ms | 0,04 ms |
| Processador, andando | — | 0,16 a 0,21 ms | 0,34 a 0,38 ms |

A penumbra custou **0,04 a 0,05 ms** na placa de vídeo. No processador, o custo andando ficou igual ao
de antes mesmo com as linhas indo mais longe (32 m na escuridão, para enxergar luzes distantes;
24 m na névoa, onde além disso tudo já some — e, desde que o jogador ficou só com o círculo, uns
14 m), porque as paredes passaram a ser medidas de novo a cada
**12 cm andados** em vez de a cada dois passos da física.

Em mapa limpo o sistema não aparece e **não calcula nada**. O que tornou o resto barato:

- A névoa que se move é **um desenho de ruído gerado uma vez**, no início, e só deslizado pelo
  mapa — em vez de calcular a névoa a cada quadro.
- As paredes em volta do personagem só são medidas de novo **quando ele anda**. Parede não anda.
- Luz parada mede suas paredes **uma única vez**. Só a tocha na mão é medida de novo, e só ao andar.
- No máximo 16 luzes contam ao mesmo tempo (as mais perto do jogador), então o custo não cresce
  com o tamanho do mapa.

> A janela de teste nesta máquina fica travada em 30 quadros por segundo em qualquer modo, então os
> números acima são o tempo medido pelo próprio Godot, e não quadros por segundo.

#### Testar

**F4** percorre os ambientes: limpo → névoa → escuridão → **breu total** (todas as luzes
apagadas) → limpo. **L** acende e apaga a lanterna. Com **F3** ligado, aparecem sobre o jogador a
luz que chega nele e o estado da lanterna, e sobre o inimigo o quanto ele enxerga o jogador
(`enxerga 35%`, ou `não enxerga`). É ferramenta de teste; o ambiente de verdade de cada fase vem do
nó `AmbienteDaFase`.

#### Ainda por fazer

- Itens que ampliam a visão: basta mexer nos alcances do sensor.
- Definir, por inimigo, `EnxergaNoEscuro` e `EnxergaNaNevoa` (ver acima).

> As ideias de **lembrar do inimigo** e **desbravar o mapa** existiram e foram retiradas em
> 29/09/2026. Estão registradas em [IDEIAS-ARQUIVADAS.md](IDEIAS-ARQUIVADAS.md).

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
| **Visão** | só o inimigo | Cone na direção em que olha. Alcança mais longe que o externo, por ser focado. Enche a barra sem teto (o jogador não tem cone desde 01/10/2026) |

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
andares e rampa (para testar interiores e elevação), caixas e divisórias soltas, **16 pinheiros e
uma estrutura de muro de pedra sem teto** no estilo Don't Starve (ver "Cenário", no começo deste
documento), e **quatro inimigos** — um em ronda por cinco pontos, um parado em Idle dentro da casa e,
desde 06/10/2026, um grupo de dois parados a oeste do ponto de partida, para testar o grito de aviso
do combate.
**F3 liga e desliga o desenho dos raios; F4 troca o ambiente; L acende e apaga a lanterna.**

> **Ao montar uma sala:** o nó que guarda os pontos da ronda deve se chamar **`MarcasDaRonda`** —
> é por esse nome que o inimigo acha o trajeto sozinho, sem precisar ligar nada na cena. Salas com
> mais de uma rota indicam cada uma pelo campo `CaminhoDaFase` do inimigo.

| Cor | O que é |
|---|---|
| Amarelo | Raio externo (desconfiança) |
| Laranja | Raio interno do jogador (certeza) |
| Vermelho | Raio de encontro do inimigo (combate) |
| Azul | Cone de visão (só nos inimigos) |
| Verde | Os dois círculos de visão do jogador |

Sobre a cabeça do inimigo aparecem o modo em que ele está, a etapa da reação ao barulho e a barra
de suspeita — por exemplo `[====......] 41% ouvindo`. A palavra do fim diz **qual** dos alcances
está enchendo a barra (`ouvindo`, `perto`, `vendo`), que é metade do trabalho na hora de ajustar os
números.

### Ainda não implementado

- **Ouvir** — a estrutura prevê "última posição percebida", mas só a visão alimenta isso hoje
- ~~**Início do combate**~~ — **feito em 06/10/2026.** Ao chegar na distância de encontro o inimigo
  emite o aviso `CombateDeveComecar`, uma vez por encontro, e o combate (nó global "Combate") começa
  a luta ali mesmo. Ver a seção "Combate".

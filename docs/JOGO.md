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

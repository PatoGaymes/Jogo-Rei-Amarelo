# Assets — Contos de Outrora: O Rei De Amarelo

<!-- Alteração de IA - Revisar
     O que faz: explica onde guardar cada arquivo de arte e som, e em que formato.
     Por quê: sem uma regra combinada, cada pessoa salva num lugar e num formato
              diferente, e depois ninguém acha nada nem consegue usar. -->

## Formatos — decidido em 12/09/2026

| Tipo de arquivo | Formato | Por quê |
|---|---|---|
| **Imagem** (personagens, cenários, interface) | **`.webp`** sem perda (*lossless*) | Metade do tamanho de um PNG, com **exatamente** os mesmos pixels. Testado nas 37 imagens do projeto: 4,39 MB viraram 2,24 MB |
| **Música** | **`.ogg`** | Comprimido. O `.wav` do Ciclope tinha 38 MB e em `.ogg` ficou 4,7 MB — mesma música, diferença que não se escuta |
| **Efeito sonoro curto** | **`.wav`** | Toca instantaneamente, sem precisar descomprimir. Como são curtos, ocupam pouco |
| **Animação** | **Nunca `.gif`** | Ver a seção de animação abaixo |

### Por que não usar `.gif` — **importante**

1. **O Godot não abre GIF.** Não existe importador para esse formato. Só funciona com uma
   extensão feita por terceiros, que usa um recurso que a própria engine já marcou como
   obsoleto e pode remover.
2. **O GIF estraga a arte.** Ele só guarda **256 cores** e a transparência é "tudo ou nada":
   cada ponto da imagem ou é totalmente visível ou totalmente invisível, sem meio-termo. Numa
   arte desenhada em alta resolução, isso arruína os degradês e deixa as bordas serrilhadas.

Se alguém entregar um GIF, ele precisa ser convertido antes de entrar no projeto. Peça ajuda —
não dá para simplesmente arrastar para dentro do Godot.

### Por que música em `.ogg` e não `.wav`

O `.wav` guarda o som **sem nenhuma compressão** — por isso fica gigante. O problema não é só o
espaço em disco: o Git guarda **uma cópia inteira de cada versão** de um arquivo. Cinco ajustes
numa música de 38 MB viram 190 MB no repositório, para sempre, para todo mundo que clonar.

**Ao exportar uma música nova, já exporte em `.ogg`.** Não exporte `.wav` para depois converter:
converter duas vezes perde qualidade.

---

## Animação — **tudo quadro a quadro** (decidido em 29/09/2026)

> Até 29/09/2026 a regra era animar personagens por ossos e só os efeitos quadro a quadro. A equipe
> mudou: **o jogo inteiro é quadro a quadro**, no combate e na exploração.

Cada animação é uma sequência de desenhos completos, um por quadro, tocados em ordem.

**A única exceção é a Quimera** — ela pode ser *produzida* de outro jeito na ferramenta de arte,
mas **dentro do jogo também chega quadro a quadro**. Para quem programa, não existe exceção: todo
personagem, chefe, inimigo, NPC e efeito é uma sequência de imagens.

### Como entregar personagens

**Uma pasta por direção**, com os quadros em ordem:

```
Map/Andar/Frente/   Map/Andar/Costas/   Map/Andar/Lado/
```

- **Frente, costas e um lado só.** O outro lado é o mesmo desenho espelhado pelo jogo. O lado
  desenhado deve olhar **para a direita** (se vier para a esquerda, avisar — é um ajuste).
- A direção mostrada é escolhida pelo jogo comparando para onde o personagem olha com de onde a
  câmera olha. É a técnica do Doom, e é por isso que cada ação precisa das três vistas.
- **Não precisa se preocupar com o enquadramento.** A arte pode vir numa tela grande com o
  personagem em qualquer canto — na inserção cada direção é recortada com **os pés no centro e na
  base**, que é o que impede o personagem de pular de lugar ao virar (ver `inserir/README.md`).

**No Godot:** os quadros viram um recurso `SpriteFrames` (arquivo `.tres` na pasta `Map/` do
personagem), com uma animação por direção chamada `<ação>_frente`, `<ação>_costas` e
`<ação>_lado` — por exemplo `andar_frente`. O componente `AnimacaoDirecional` escolhe qual tocar.

### Efeitos

Explosões, magias, fogo, sangue, fumaça: quadro a quadro também, em `Art/Effects/`, numa pasta com
o nome do efeito.

### O custo em espaço — **atenção da equipe de devs**

A regra antiga existia por causa do tamanho: a estimativa era de ~6 MB para todas as entidades por
ossos contra **~216 MB quadro a quadro**, e cada revisão de arte soma tudo de novo no histórico do
repositório. A animação de andar do Desgarrado sozinha tem **96 quadros e 4,6 MB**.

O Git guarda para sempre cada versão de cada imagem. Com o jogo inteiro quadro a quadro, vale a
equipe de devs considerar **Git LFS** para as pastas de arte antes que o repositório fique pesado
demais para clonar.

---

## Arte nova: largue em `inserir/`

Existe uma caixa de entrada na raiz do projeto: **[`inserir/`](../../inserir/)**. Largue ali os
arquivos novos como vieram e peça a inserção — eles são convertidos, recortados, renomeados e
movidos para a pasta certa, e a caixa é esvaziada no fim. As regras completas estão no
[README de lá](../../inserir/README.md).

---

## Onde guardar cada coisa

**Cada personagem, chefe, inimigo e NPC tem a sua própria pasta.**

```
Art/
├── Characters/    # personagens jogáveis e recrutáveis
├── Bosses/        # chefes
├── Enemies/       # inimigos comuns
├── Npcs/          # personagens que não entram em combate
├── Effects/       # explosões, magias, partículas (sprite sheets)
├── Cenarios/      # árvores, muros, pedras, salas — uma pasta por objeto
└── UI/            # botões, molduras, ícones, menus
```

Dentro da pasta de cada personagem:

| Pasta | O que guardar |
|---|---|
| `Combat/` | Os quadros das animações de batalha, uma pasta por ação e direção |
| `Dialogue/` | Retrato usado nas conversas |
| `Map/` | Os quadros das animações no mapa (andar, parar...) e o `SpriteFrames` do personagem |
| `Provisorio/` | Arte temporária, só para testar. **Sai quando a definitiva ficar pronta** |

### Cenário — árvores, muros, construções (30/09/2026)

O cenário segue o jeito do Don't Starve: **objetos 2D que ficam sempre de frente para a câmera**.
Cada objeto tem a sua pasta em `Cenarios/` (`Cenarios/Pinheiro/`, `Cenarios/MuroDePedra/`), com
`Provisorio/` dentro para a arte temporária.

| Regra | Por quê |
|---|---|
| **Objeto de uma casa** (árvore, pedra): uma imagem só | A mesma imagem serve para todos os ângulos da câmera |
| **Estrutura de várias casas** (muro, cerca): **uma peça por casa**, desenhada um pouco mais larga que a casa | As peças vizinhas se sobrepõem e formam um muro contínuo em qualquer ângulo |
| **Base do objeto no centro, embaixo** da imagem | A mesma regra dos personagens: é por esse ponto que o objeto fica em pé no chão |
| Desenhar **cerca de 1,5 vez mais alto** do que se quer ver na tela | Em pé e vista pela câmera a 50°, a imagem aparece com ~64% da altura desenhada |
| **Sem sombra desenhada** no chão | O objeto já ganha uma sombra redonda no pé, na cena |

### Como nomear

Pastas de personagem: **`NomePróprio_Classe`** — `Khalid_Cavaleira`, `Emi_Cacadora`. Assim a
pessoa acha procurando pelo nome **ou** pela classe. Chefes, inimigos e NPCs usam só o nome.

**Sem espaço e sem acento** em nome de pasta e de arquivo. Acento e espaço quebram caminhos de
arquivo e atrapalham o controle de versão.

### Áudio

```
Audio/
├── Music/
│   ├── Personagens/   # tema de um personagem
│   ├── Bosses/        # tema de um chefe
│   └── Ambiente/      # música de cenário
└── SFX/               # efeitos sonoros curtos
```

---

## Ao adicionar um personagem novo

1. Criar a pasta no padrão `NomePróprio_Classe`
2. Criar dentro dela `Combat/`, `Dialogue/` e `Map/`, mesmo que fiquem vazias por enquanto
3. Colocar as imagens em `.webp`
4. Rodar `Godot --headless --path ContosDeOutrora --import` para o Godot reconhecer os arquivos

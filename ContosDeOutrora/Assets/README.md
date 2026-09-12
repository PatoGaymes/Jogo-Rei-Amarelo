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

## Animação — decidido em 12/09/2026

O jogo usa **dois métodos diferentes**, cada um onde funciona melhor:

### Personagens, chefes, inimigos e NPCs → **por ossos (recortes)**

O personagem é desenhado **uma vez**, em partes separadas: cabeça, tronco, braço, perna, arma.
Depois essas partes são montadas dentro do Godot como um boneco articulado, e a animação
**move as partes** em vez de redesenhar tudo.

É como funciona o Darkest Dungeon, que é a nossa referência.

**Por que este método:**

- **Espaço:** estimamos ~6 MB para todas as entidades do jogo. No método quadro a quadro seriam
  cerca de 216 MB — e cada revisão da arte somaria tudo de novo no repositório.
- **Retrabalho:** mudar uma pose não exige redesenhar o personagem inteiro, só mover as partes.
- **Qualidade:** a arte continua em alta resolução, sem precisar espremer dezenas de quadros.

**No Godot:** as partes viram nós `Sprite2D` organizados em hierarquia (o braço "pendurado" no
tronco, a mão no braço), e o `AnimationPlayer` grava o movimento delas. Para dobras mais
orgânicas, existe o `Skeleton2D`.

**Como entregar a arte:** cada parte do corpo como uma imagem separada, com fundo transparente,
todas dentro de `Combat/` do personagem.

### Efeitos → **quadro a quadro (sprite sheet)**

Explosões, magias, fogo, sangue, fumaça. Esses **não** funcionam bem com ossos, porque a forma
muda completamente a cada quadro.

**Como entregar:** todos os quadros numa única imagem, lado a lado, em grade regular (todos os
quadros do mesmo tamanho). Isso é um *sprite sheet*. No Godot vira um `AnimatedSprite2D`.

Guardar em `Art/Effects/`, numa pasta com o nome do efeito.

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
├── Cenarios/      # salas, corredores, fundos
└── UI/            # botões, molduras, ícones, menus
```

Dentro da pasta de cada personagem:

| Pasta | O que guardar |
|---|---|
| `Combat/` | As partes do corpo para a animação de batalha |
| `Dialogue/` | Retrato usado nas conversas |
| `Map/` | Sprite usado andando pelo cenário |
| `Provisorio/` | Arte temporária, só para testar. **Sai quando a definitiva ficar pronta** |

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

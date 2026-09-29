# `inserir/` — caixa de entrada de arte e som

Largue aqui qualquer arquivo novo — animação, música, efeito sonoro, retrato, cenário — e peça ao
Claude para inserir. Ele coloca cada arquivo no lugar certo da estrutura do projeto e **esvazia
esta pasta** no fim.

Esta pasta é só de passagem. **Nada aqui é usado pelo jogo.**

## Como largar os arquivos

Pode largar do jeito que veio: pasta baixada do Drive, `.zip` já extraído, arquivos soltos. Não
precisa renomear nem organizar antes — é justamente esse o trabalho que o Claude faz.

O que ajuda é o **nome da pasta dizer de quem é e do que se trata**. Estes dois formatos são
entendidos sozinhos:

```
inserir/Desgarrado/Andar/frente/...
inserir/Emi/Retrato/...
```

Quando o nome não deixar claro de qual personagem é, ou o que aquilo faz no jogo, o Claude
pergunta em vez de chutar.

## O que o Claude faz com os arquivos

| Passo | O que acontece |
|---|---|
| 1 | Descobre de qual personagem, chefe, inimigo ou NPC é o material |
| 2 | Converte para o formato do projeto, se precisar (`.webp` para imagem, `.ogg` para música) |
| 3 | **Recorta o vazio em volta**, com **os pés no centro e na base** — a mesma moldura para todos os quadros de uma mesma direção |
| 4 | Renomeia sem espaço e sem acento, numerando os quadros em ordem |
| 5 | Move para a pasta certa (ver [Assets/README.md](../ContosDeOutrora/Assets/README.md)) |
| 6 | **Se for animação:** monta o `SpriteFrames`, liga ao personagem e **confere no jogo** que ela toca em todas as direções |
| 7 | Registra o que foi feito em [docs/ALTERACOES-IA.md](../docs/ALTERACOES-IA.md) |
| 8 | Apaga o que ficou aqui |

**O passo 6 existe por causa de um erro.** Na primeira entrega do Desgarrado os quadros foram só
movidos para a pasta, e a inserção foi dada como pronta — mas nada no jogo usava aqueles
arquivos, e o personagem continuou com a imagem provisória. Arquivo no lugar certo não é animação
funcionando: a inserção só termina quando ela aparece tocando no jogo.

**Por que recortar, e por que pelos pés:** a arte costuma vir numa tela grande com o personagem em
qualquer canto. Esse vazio quase não pesa em disco, mas **bagunça a posição do personagem no
jogo** — o motor centraliza a imagem inteira, vazio incluído.

E cada direção vem de um jeito: na entrega do Desgarrado, o corpo de frente e de costas estava
**0,9 m à direita** do corpo de lado. Sem corrigir, o personagem pularia quase um metro ao virar. Com
os pés no centro de cada direção, ele gira no lugar — e o lado espelhado cai exatamente em cima do
desenhado. Dentro de uma mesma direção a moldura é a mesma para todos os quadros, senão a animação
treme.

## O que **não** entra aqui

- **Documentos** (GDD, planilhas, textos). Esses vão para `docs/externo/` e seguem a regra de
  nunca alterar a fonte original.
- **Código.**
- **`.gif`** — o Godot não abre. Peça os quadros separados ou um vídeo.

## Antes de pedir a inserção

Confira se o Godot está **fechado**. Ele regrava arquivos de cena com o que tem na memória e
pode desfazer alterações feitas de fora.

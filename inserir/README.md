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
| 3 | **Recorta o vazio em volta** das imagens, usando a mesma moldura em todos os quadros da animação |
| 4 | Renomeia sem espaço e sem acento, numerando os quadros em ordem |
| 5 | Move para a pasta certa (ver [Assets/README.md](../ContosDeOutrora/Assets/README.md)) |
| 6 | Registra o que foi feito em [docs/ALTERACOES-IA.md](../docs/ALTERACOES-IA.md) |
| 7 | Apaga o que ficou aqui |

**Por que recortar:** a arte costuma vir numa tela grande com o personagem pequeno no meio. Esse
vazio não pesa quase nada em disco, mas **bagunça a posição do personagem no jogo** — o motor
centraliza a imagem inteira, vazio incluído. A moldura é a mesma para todos os quadros da mesma
entrega, senão a animação treme.

## O que **não** entra aqui

- **Documentos** (GDD, planilhas, textos). Esses vão para `docs/externo/` e seguem a regra de
  nunca alterar a fonte original.
- **Código.**
- **`.gif`** — o Godot não abre. Peça os quadros separados ou um vídeo.

## Antes de pedir a inserção

Confira se o Godot está **fechado**. Ele regrava arquivos de cena com o que tem na memória e
pode desfazer alterações feitas de fora.

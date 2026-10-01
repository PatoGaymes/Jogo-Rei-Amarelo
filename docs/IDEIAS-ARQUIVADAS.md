# Ideias arquivadas

Mecânicas que chegaram a ser **implementadas e testadas** e depois foram retiradas do jogo. Ficam
registradas aqui para não se perderem: por que existiram, como funcionavam, por que saíram e onde
está o código, caso a equipe queira reaproveitar no futuro.

Nada aqui está no jogo hoje.

---

## Lembrança do inimigo — o "borrão de memória"

**Existiu de:** 21/09/2026 · **Retirada em:** 29/09/2026, por decisão do PO
**Código:** commit `7757945` — `Scripts/Systems/VisibilidadeDoInimigo.cs`

### A ideia

Pensar a visão do jogador como a cabeça de alguém: quem vê um inimigo e vira o rosto não esquece
na hora onde ele estava. Ao sair de vista, o inimigo deixava um **borrão apagado no último lugar
em que foi visto**.

Como um inimigo anda, essa lembrança não podia valer para sempre:

| Situação | O que acontecia com o borrão |
|---|---|
| O jogador vira o rosto | Aparece no último lugar visto, pálido e sem cor |
| O tempo passa | Desbota até sumir em **5 segundos** |
| O jogador olha para o lugar e não tem ninguém | Some **na hora** — a lembrança foi conferida e era falsa |

O borrão era desenhado por cima da névoa e atravessando parede, de propósito: a lembrança estava
na cabeça do jogador, não no mundo.

### Por que saiu

O PO achou que ficou **fora da estética do jogo**.

### Se um dia voltar

- O número que decidia tudo era o tempo de desbotar: curto demais e o borrão não ajuda; longo
  demais e o jogador passa a caçar um inimigo que já saiu dali.
- Foi verificado que a lembrança caía corretamente ao ser conferida (memória 0,84 → 0,00 no
  instante em que o jogador olhou para o lugar vazio).

---

## Desbravar o mapa — a memória do cenário

**Existiu de:** 21/09/2026 · **Retirada em:** 29/09/2026, por decisão do PO
**Código:** commit `7757945` — `Scripts/Systems/NevoaDeGuerra.cs` e `Shaders/NevoaDeGuerra.gdshader`

### A ideia

Casa não anda. Uma vez vista, o jogador **sabe** que ela está ali, mesmo de costas. Tudo o que o
personagem já tinha enxergado ficava registrado numa grade sobre o mapa e continuava aparecendo
**para sempre**, apagado e sem cor, quando ele deixava de olhar. O que nunca tinha sido visto
continuava coberto.

Era o mesmo "mapa que se revela aos poucos" de jogos de estratégia.

### Por que saiu

Decisão do PO junto com a reestruturação da névoa: a névoa e a escuridão passaram a **esconder
por completo** o que está fora da visão (ver `docs/JOGO.md`), e um mapa que se revela e fica
revelado contradiz isso.

### Se um dia voltar

- A grade cobria **80 m** centrados na origem, dividida em 256 × 256 pedaços (cerca de 31 cm cada).
  Mapa maior que a grade não era lembrado nas bordas.
- O lembrado aparecia com 45% da nitidez de estar olhando, e sem cor — sem tirar a cor, memória e
  visão ficavam parecidas demais e o jogador confiava em informação velha.
- A anotação rodava a cada 3 passos da física, percorrendo só o pedaço da grade ao alcance da
  visão.

---

## Cone de visão do jogador e mira pelo mouse

**Existiu de:** 21/09/2026 · **Retirada em:** 01/10/2026, por decisão do PO
**Código:** commit `890fb15` — `Scripts/Characters/PlayerIsometrico.cs` (mira pelo mouse) e
`Scripts/Systems/SensorDeteccao.cs` (as faixas do cone)

### A ideia

Além do círculo em volta, o jogador tinha um **cone de visão** na direção em que olhava, que
enxergava mais longe dentro da névoa, em três faixas: claro até 6,1 m, levemente embaçado até 11,2 m
e só vulto até 16 m, com 80° de abertura. Para escolher para onde olhar sem abrir mão de para onde
ia, o personagem **virava para onde o mouse apontava**, e a câmera continuava nos 8 ângulos fixos.
Dava para atravessar um corredor vigiando a porta, ou recuar sem tirar os olhos do inimigo.

### Como funcionava

| Parte | Como |
|---|---|
| Mira | Uma linha imaginária saía da câmera, passava pelo cursor e ia até o chão na altura dos pés; o ponto onde ela encostava era para onde o personagem olhava. Conferido nos 8 ângulos da câmera: erro máximo de 0,2° |
| Névoa | A névoa fechava mais devagar na direção do olhar: o alcance ia de uns 7 m nos lados a uns 13 m na frente, com uma passagem suave de 14° a 88° |

### Por que saiu

Nos testes da escuridão, o PO viu que **só o círculo em volta** passa melhor a ideia dos mapas com
névoa e escuridão, mantendo a lanterna. E a mira pelo mouse criava o **"moonwalk"**: com o mouse
para um lado e o personagem andando para o outro, o desenho mostrava o personagem de frente andando
de costas.

### Se um dia voltar

- O moonwalk precisa de solução própria: animações de andar de costas e de lado, ou o desenho
  seguir a caminhada e só a visão seguir o mouse.
- A névoa estava calibrada para as três faixas, olhando para a frente: 13% aos 6 m, 46% aos
  11,2 m e 89% aos 16 m.

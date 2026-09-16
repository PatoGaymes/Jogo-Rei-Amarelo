# Contos de Outrora: O Rei De Amarelo — contexto do projeto

RPG tático por turnos desenvolvido pela equipe **PatoGaymes** na engine **Godot 4.6**, com **C#**.

---

## Regras obrigatórias para o Claude

Estas regras foram definidas pela equipe e valem para **todas as sessões**, sem exceção.

### Regra 1 — Nunca commitar

Não executar `git commit`, `git push`, `git merge`, `git rebase` nem qualquer comando que
altere o histórico ou o repositório remoto. **Todo o versionamento é feito pela equipe de
devs.** O trabalho do Claude termina no arquivo salvo em disco.

### Regra 2 — Marcar toda alteração feita por IA

Todo trecho de código, configuração ou arquivo criado/alterado pelo Claude deve levar o
comentário `Alteração de IA - Revisar`, seguido da explicação do **que faz** e do **porquê**.

A explicação precisa ser **clara e objetiva, sem jargão técnico**, mesmo que o jargão esteja
tecnicamente correto. Ela deve fazer sentido para quem está lendo o código ou a documentação
pela primeira vez.

Formato padrão em C#:

```csharp
// Alteração de IA - Revisar
// O que faz: guarda a velocidade de caminhada do personagem.
// Por quê: o valor precisa ser ajustável no editor sem mexer no código.
```

Em arquivos que **não aceitam comentário** (JSON puro, cenas `.tscn`, recursos `.tres` — o
Godot apaga comentários ao salvar), registrar a alteração em
[docs/ALTERACOES-IA.md](docs/ALTERACOES-IA.md) com a mesma explicação.

### Regra 3 — Nunca alterar a documentação externa

O Trello e o Google Drive são a **fonte de verdade** do projeto e não podem ser alterados
pelo Claude. Se algo precisar mudar lá, escrever a sugestão em
[docs/SUGESTOES-PARA-DOCUMENTACAO.md](docs/SUGESTOES-PARA-DOCUMENTACAO.md) para o dev
revisar e subir manualmente.

> OneNote e Canva foram **descartados** pela equipe em 11/09/2026 por estarem desatualizados.

---

## Sobre o jogo

- **Nome oficial:** Contos de Outrora: O Rei De Amarelo
- **Gênero:** RPG tático por turnos
- **Referência:** **Darkest Dungeon** — disposição de tela e mecânicas. Arte e design próprios.
- **Estilo visual:** arte 2D em **alta resolução** (desenhada, **não** pixel art)
- **Resolução base:** 1920x1080
- **Câmera:** fixa por cena/corredor — **não** segue o personagem

**Consequências práticas** (não mudar sem falar com a equipe): filtro de textura **Linear**
(nunca `Nearest`), escala **fracionária** (nunca `integer`), snap de pixel **desligado**,
UI com âncoras bem definidas (o modo de tela é `expand`).

### Premissa

A ilha de Euduro, rebatizada **Carcosa** pelo rei **Hastur Carcosa**, que se fundiu a um
homúnculo para fugir da morte. O tempo na ilha foi congelado pelo mago Eldaquias, e os não
nobres foram escravizados nas profundezas. Os personagens jogáveis chegam ali por caminhos
diferentes e todos terminam presos nas masmorras.

### Mecânicas centrais (do GDD)

- **Combate por turnos**, cada personagem ocupa **1 de 5 posições**. Posição importa: há
  habilidades que só funcionam em certos lugares da formação.
- **Uma ação por turno.** A lógica é de **máquina de estados**, não de física em tempo real.
- **Atributos em 3 categorias:** **Corpo** (vida, Precisão, Furtividade, Reação, Robustez),
  **Mente** (sanidade, Lábia, Intuição, Análise, Vontade) e **Essência** (foco, Energia, Aura).
- **PF (Pontos de Foco)** é o recurso gasto pelas habilidades.
- **Exploração lateral** (só frente e trás), com salas que resetam ao sair, e "Estátuas do Rei"
  para salvar e viajar rápido.
- **Progressão:** "lascas de Euduroh" (de inimigos comuns) desbloqueiam habilidades; "jóias
  douradas" (drop garantido de chefe) sobem o nível. Skill tree com 3 caminhos — escolher um
  **bloqueia os outros dois**, e só uma jóia sacrificada numa estátua permite refazer.

### Regras de combate — qual documento vale

> ⚠️ **A Árvore de Habilidades é a fonte oficial do combate, não o GDD.** Definido pelo PO em
> 12/09/2026: as regras foram alteradas durante a criação das árvores e só aquele documento foi
> atualizado. **O GDD ainda tem as regras antigas escritas — ignorar as seções de status e
> combate dele.**

| Regra | Valor oficial |
|---|---|
| **Efeitos de status** | Gelo, Ácido, Sangramento, Fogo, Corrosão, Raio, Escuridão, Luz |
| **Vanguarda** | Quem tem Vanguarda **protege os aliados e toma o dano no lugar deles** |
| **Vulnerável** | **Não pode receber buffs** |
| **Ações por turno** | **1**, salvo item ou habilidade que contorne isso |
| **Corrompido / Purificado** | Nomes **antigos** de Escuridão e Luz. **Não existem no jogo** |

### As 5 ações do turno (definido pelo PO em 12/09/2026)

**Atacar · Habilidades · Defender · Itens · Fugir**

**Conversar foi removida** do jogo — não seria viável em boa parte da campanha. As sub-opções
dela (Enganar, Ameaçar, Furtar, Expor) saem junto.

**Não existe ação de "mover".** Como o combate tem 5 posições e várias habilidades dependem de
posição, a movimentação na formação acontece **através das próprias habilidades** (ex.: *Avanço
Tático*, que avança e empurra). É como o Darkest Dungeon funciona.

Personagens sem lore in-game (Amana, Gael, Jedara, Varossa, Rosaria) **não são lacuna**: são
recrutáveis só via gameplay e não precisam de lore extensa.

### Personagens jogáveis e recrutáveis

| Personagem | Classe | Pasta de assets |
|---|---|---|
| Khalid de Nortumbria | Cavaleira | `Khalid_Cavaleira` |
| Amana A'Bajal | **Xamã** | `Amana_Xama` |
| Rosaria Percival | Aberração | `Rosaria_Aberracao` |
| Gael Nebraska | Hemomante | `Gael_Hemomante` |
| **Tao** A'Bajal | Peregrino | `Tao_Peregrino` |
| Uzhan N'Daka | Desgarrado | `Uzhan_Desgarrado` |
| Lancelot Claivar | Escudeiro | `Lancelot_Escudeiro` |
| **Jedara**, Filho de Tauron | Brutamonte | `Jedara_Brutamonte` |
| Varossa K'Ushim / Homem Misterioso | Bruxo | `Varossa_Bruxo` |
| Emi Matsunaga | Caçadora | `Emi_Cacadora` |

Grafias oficiais confirmadas pelo PO em 12/09/2026: **Xamã**, **Tao**, **Jedara**. O GDD tem
grafias antigas em alguns trechos ("Cartomante", "Thao", "Jedah") — usar sempre as oficiais.

Cada personagem jogável tem **Lore in-game em 4 Atos**, com 2 escolhas por Ato.

---

## Linguagens

- **C#** é a linguagem principal
- **GDScript** pode ser usado pontualmente em scripts pequenos de cena

**Godot não suporta JavaScript, Lua nem Python.** Se alguém pedir código nessas linguagens
para dentro do jogo, avisar que não é possível e oferecer C# ou GDScript.

---

## Ambiente (validado e funcionando)

| Item | Versão / caminho |
|---|---|
| Godot | 4.6.1-stable **mono** (`Documents/Godot/Godot_v4.6.1-stable_mono_win64.exe`) |
| .NET SDK | 10.0.300 |
| Alvo do projeto C# | `net8.0` — **testado e rodando** com o SDK 10 |
| Godot.NET.Sdk | 4.6.1 |
| Renderizador | Forward+ |
| Repositório | `github.com/PatoGaymes/Jogo-Rei-Amarelo` (nome antigo, anterior à definição do título) |

Detalhes e pendências em [docs/AMBIENTE.md](docs/AMBIENTE.md).

---

## Estrutura de pastas

```
ContosDeOutrora/                  # projeto Godot
├── Scenes/                       # cenas .tscn — Characters/, UI/, Combat/, Levels/
├── Scripts/                      # código .cs  — Characters/, Combat/, UI/, Systems/
├── Assets/                       # arte e som, organizados POR PERSONAGEM
│   ├── Art/  Characters/ Bosses/ Enemies/ Npcs/ Cenarios/ UI/
│   ├── Audio/ Music/{Personagens,Bosses,Ambiente}/ SFX/
│   └── README.md                 # regra de organização — ler antes de adicionar arte
├── Resources/                    # dados .tres — heróis, habilidades, inimigos
├── ContosDeOutrora.csproj
└── project.godot
docs/                             # documentação interna + espelho da externa
```

**Cada personagem, chefe, inimigo e NPC tem pasta própria**, com `Combat/`, `Dialogue/`,
`Map/` e `Provisorio/` dentro. Pastas em `NomePróprio_Classe`, **sem espaço e sem acento**.
As regras completas estão em [ContosDeOutrora/Assets/README.md](ContosDeOutrora/Assets/README.md)
— ler antes de adicionar qualquer arte ou som.

### Formatos de arquivo (decidido em 12/09/2026)

| Tipo | Formato | Observação |
|---|---|---|
| Imagem | **`.webp`** sem perda | Metade do tamanho do PNG, pixels idênticos |
| Música | **`.ogg`** | Exportar já em ogg; nunca `.wav` para música |
| Efeito sonoro curto | `.wav` | Toca sem descomprimir |
| Animação | **nunca `.gif`** | O Godot não importa GIF, e GIF só tem 256 cores e transparência "tudo ou nada" |

**Animação:** personagens, chefes, inimigos e NPCs são animados **por ossos/recortes** (o
personagem é desenhado em partes e a animação move as partes — método do Darkest Dungeon).
**Efeitos** (explosão, magia, fogo) são **quadro a quadro** em sprite sheet, em `Art/Effects/`.

**`Resources/` é importante:** heróis, habilidades e inimigos devem ser arquivos de dados
(`.tres`), não valores fixos no código. Isso permite que o game designer balanceie o jogo
sem programar — é o mesmo padrão do Darkest Dungeon, e o GDD tem dezenas de habilidades
por personagem, o que torna inviável deixá-las no código.

---

## Comandos

```bash
# Compilar o C#
dotnet build ContosDeOutrora/ContosDeOutrora.csproj

# Rodar o jogo
"$GODOT" --path ContosDeOutrora

# Rodar sem abrir janela (para testes automatizados)
"$GODOT" --headless --path ContosDeOutrora --quit-after 60 res://Scenes/player.tscn

# Reimportar assets depois de adicionar arte ou som
"$GODOT" --headless --path ContosDeOutrora --import
```

No VS Code, **F5** roda o jogo já com depuração de C# ligada.

---

## Documentação externa

A documentação fica no Trello e no Google Drive — **ambos privados**, o Claude não acessa
diretamente. A equipe exporta para `docs/externo/`:

| Arquivo | Conteúdo |
|---|---|
| `Infos/GDD Rei de Amarelo.md` | **Documento principal** — lore, mapa, combate, atributos, progressão, personagens |
| `Infos/Árvores de Habilidades.md` | Habilidades e caminhos de cada personagem |
| `trello-board.json` | Quadro de tarefas da equipe |

Instruções de exportação em [docs/README.md](docs/README.md).

Se uma informação sobre o jogo não estiver em `docs/externo/`, **perguntar à equipe em vez
de supor.**

---

## Estado atual do código

**Mecânicas base de exploração — feitas em 12/09/2026** (card do Trello *"Criação das mecânicas
base - Câmera e Movimentação"*).

| Arquivo | O que faz |
|---|---|
| [Player.cs](ContosDeOutrora/Scripts/Characters/Player.cs) | Movimentação lateral com aceleração e desaceleração, gravidade para manter no chão, e vira o sprite para o lado em que anda |
| [CameraCorredor.cs](ContosDeOutrora/Scripts/Systems/CameraCorredor.cs) | Câmera que acompanha o personagem **só na horizontal**, com a altura travada |
| [Player.tscn](ContosDeOutrora/Scenes/Characters/Player.tscn) | Cena do personagem, no grupo `player`. Usa a arte provisória da Khalid como placeholder |
| [CorredorTeste.tscn](ContosDeOutrora/Scenes/Levels/CorredorTeste.tscn) | Corredor de 4000px com chão, paredes e marcadores de distância. É a cena inicial do jogo hoje |

**Como a câmera funciona:** ela desliza para o lado acompanhando quem anda, mas **nunca sobe nem
desce** — é o enquadramento de "quadro de teatro" do Darkest Dungeon. Quem faz isso é o
`CameraCorredor`, que copia só a posição horizontal do alvo. A câmera encontra o personagem
sozinha pelo grupo `player`, então funciona mesmo que os nós sejam renomeados ou movidos.

**Ainda são placeholders:** a arte da Khalid, o cenário (formas coloridas simples) e os
marcadores de distância. Tudo isso sai quando a arte real chegar.

**Próximos passos naturais:** interações do cenário (portas, cadáveres, Estátuas do Rei) e a
cena de menu inicial — hoje o jogo abre direto no corredor de teste.

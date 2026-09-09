# Rei Amarrilho — contexto do projeto

Jogo de RPG desenvolvido pela equipe **PatoGaymes** na engine **Godot 4.6**, com **C#**.

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

O Trello, o OneNote, o Canva e o Google Drive são a **fonte de verdade** do projeto e não
podem ser alterados pelo Claude. Se algo precisar mudar lá, escrever a sugestão em
[docs/SUGESTOES-PARA-DOCUMENTACAO.md](docs/SUGESTOES-PARA-DOCUMENTACAO.md) para o dev
revisar e subir manualmente.

---

## Sobre o jogo

- **Gênero:** RPG por turnos
- **Referência:** **Darkest Dungeon** — na disposição de tela e nas mecânicas. A arte e o
  design são próprios da equipe.
- **Estilo visual:** arte 2D em **alta resolução** (desenhada, **não** pixel art)
- **Resolução base:** 1920x1080
- **Câmera:** fixa por cena/corredor — **não** segue o personagem

**Consequências práticas dessas escolhas** (não mudar sem falar com a equipe):

- Filtro de textura **Linear**, nunca `Nearest` (`Nearest` é para pixel art e deixaria a arte serrilhada)
- Escala **fracionária** (`fractional`), nunca `integer`
- Snap de pixel **desligado**
- UI pesada, com âncoras bem definidas (o modo de tela é `expand`, então a tela pode
  ficar mais larga em monitores ultrawide)
- Combate por turnos: a lógica é de **máquina de estados**, não de física em tempo real

---

## Linguagens

- **C#** é a linguagem principal do projeto
- **GDScript** pode ser usado pontualmente em scripts pequenos de cena

**Godot não suporta JavaScript, Lua nem Python.** Se alguém pedir código nessas linguagens
para dentro do jogo, avisar que não é possível e oferecer C# ou GDScript.

---

## Ambiente (validado e funcionando)

| Item | Versão / caminho |
|---|---|
| Godot | 4.6.1-stable **mono** (`Documents/Godot/Godot_v4.6.1-stable_mono_win64.exe`) |
| .NET SDK | 10.0.300 |
| Alvo do projeto C# | `net8.0` — **testado e rodando** com o SDK 10, não precisa instalar o .NET 8 |
| Renderizador | Forward+ |
| Repositório | `github.com/PatoGaymes/Jogo-Rei-Amarelo` |

Detalhes e decisões pendentes em [docs/AMBIENTE.md](docs/AMBIENTE.md).

---

## Estrutura de pastas

```
ReiDoAmarrilho/              # projeto Godot
├── Scenes/                  # cenas .tscn — Characters/, UI/, Combat/, Levels/
├── Scripts/                 # código .cs  — Characters/, Combat/, UI/, Systems/
├── Assets/                  # Art/, Audio/, Fonts/
├── Resources/               # dados .tres — heróis, habilidades, inimigos
├── ReiAmarrilho.csproj      # configuração do projeto C#
└── project.godot            # configurações do jogo
docs/                        # documentação interna e espelho da externa
```

**`Resources/` é importante:** heróis, habilidades e inimigos devem ser arquivos de dados
(`.tres`), não valores fixos no código. Isso permite que o game designer balanceie o jogo
sem precisar programar — é o mesmo padrão do Darkest Dungeon.

---

## Comandos

```bash
# Compilar o C#
dotnet build ReiDoAmarrilho/ReiAmarrilho.csproj

# Rodar o jogo
"$GODOT" --path ReiDoAmarrilho

# Rodar sem abrir janela (para testes automatizados)
"$GODOT" --headless --path ReiDoAmarrilho --quit-after 60 res://Scenes/player.tscn
```

No VS Code, **F5** roda o jogo já com depuração de C# ligada.

---

## Documentação externa

A documentação do jogo está espalhada em Trello, OneNote, Canva e Google Drive — **todos
privados**, o Claude não consegue acessá-los diretamente. A equipe exporta o conteúdo para
`docs/externo/`, e é de lá que o Claude lê. Instruções de exportação em
[docs/README.md](docs/README.md).

Se uma informação sobre o jogo não estiver em `docs/externo/`, **perguntar à equipe em vez
de supor.**

---

## Estado atual do código

O projeto está no começo. Existe apenas [player.tscn](ReiDoAmarrilho/Scenes/player.tscn) com
[Player.cs](ReiDoAmarrilho/Scripts/Characters/Player.cs), que é um **teste de ambiente** —
usa o ícone padrão do Godot como imagem e tem um movimento lateral provisório. Não é o
personagem real do jogo e deve ser substituído quando o design estiver definido.

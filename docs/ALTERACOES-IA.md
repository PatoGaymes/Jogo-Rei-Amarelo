# Registro de alterações feitas por IA

A Regra 2 do projeto exige que toda alteração feita pelo Claude seja marcada com o
comentário `Alteração de IA - Revisar` e explicada.

Alguns arquivos **não aceitam comentário** — ou aceitam, mas o Godot os apaga ao salvar o
arquivo pelo editor. Para esses, a explicação fica registrada aqui.

Arquivos nessa situação:
- Cenas `.tscn` e recursos `.tres` — o Godot reescreve o arquivo e remove comentários
- JSON puro

---

## 09/09/2026 — Preparação do ambiente

### `ReiDoAmarrilho/Scenes/player.tscn`

**O que foi feito:** ligado o script `Player.cs` ao personagem da cena (duas linhas: uma
declarando o arquivo do script, outra ligando-o ao nó `Player`).

**Por quê:** era necessário para testar se o C# realmente funciona dentro do jogo, e não
apenas se compila. Foi o teste que revelou o problema do espaço no nome do assembly
(descrito em [AMBIENTE.md](AMBIENTE.md)).

**O que revisar:** o `Camera2D` dessa cena provavelmente deve sair — ver pendência 4 em
[AMBIENTE.md](AMBIENTE.md).

### `ReiDoAmarrilho/default_bus_layout.tres` (novo)

**O que foi feito:** criados os canais de som `Music`, `SFX`, `UI` e `Ambience`, todos
ligados ao canal principal (`Master`).

**Por quê:** separar os tipos de som é o que permite ter controles de volume independentes
no menu de opções (música baixa, efeitos altos, por exemplo). Sem essa separação, só
existe um volume geral. Num jogo no estilo Darkest Dungeon, trilha e ambientação pesam
bastante na experiência, então vale ter o controle separado desde o início.

**O que revisar:** os volumes estão todos em 0 dB (volume normal). Ajustar conforme a
mixagem do jogo for definida.

### `ReiDoAmarrilho/ReiAmarrilho.sln` (novo)

**O que foi feito:** criado o arquivo de "solução" do C#, que agrupa o projeto.

**Por quê:** normalmente o Godot cria esse arquivo sozinho ao criar o primeiro script C#.
Como ele foi criado manualmente para adiantar a validação do ambiente, fica o registro.
O identificador interno do projeto foi gerado aleatoriamente, como o Godot faria.

**O que revisar:** nada em especial — é um arquivo de estrutura, não de lógica do jogo.

### `.vscode/*.json` (novos)

Estes aceitam comentário e **estão comentados no próprio arquivo**. Registrados aqui apenas
para a lista ficar completa:

- `settings.json` — caminho do Godot, projeto C# padrão, pastas ocultas na busca
- `tasks.json` — comando de compilação
- `launch.json` — rodar o jogo com F5 e depuração
- `extensions.json` — extensões recomendadas para a equipe

**O que revisar:** o caminho do Godot em `settings.json` é o da máquina do Eric. Cada dev
precisa ajustar para o caminho da sua própria instalação.

### `.claude/settings.json` (novo)

**O que foi feito:** bloqueada a execução de `git commit`, `git push`, `git merge` e
`git rebase` pelo Claude. Liberados sem pedir confirmação: `dotnet build`,
`dotnet restore` e comandos de leitura do Git (`status`, `diff`, `log`).

**Por quê:** a Regra 1 diz que o Claude nunca deve commitar. Escrever a regra num texto
depende do Claude lembrar dela. Colocá-la aqui faz o próprio programa recusar o comando —
vira uma trava de verdade, não uma promessa. As liberações são só para reduzir a
quantidade de confirmações em comandos que não alteram nada além de compilar.

**O que revisar:** se a equipe quiser bloquear mais comandos, é só acrescentar à lista
`deny`. Este arquivo vai para o GitHub e vale para todos os devs.

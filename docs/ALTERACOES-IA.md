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

---

## 11/09/2026 — Nome oficial e organização dos assets

### Renomeação para o nome oficial do jogo

**O que foi feito:** a pasta do projeto passou de `ReiDoAmarrilho` para `ContosDeOutrora`, e os
arquivos do projeto C# de `ReiAmarrilho.csproj`/`.sln` para `ContosDeOutrora.csproj`/`.sln`.
Os caminhos no VS Code foram ajustados junto.

**Por quê:** a equipe definiu o nome oficial **Contos de Outrora: O Rei De Amarelo**. O nome
completo, com espaços e dois-pontos, ficou só em `config/name`, que é o título que o jogador vê.
Pasta e programa usam a versão sem espaço porque espaço, acento e dois-pontos quebram caminhos
de arquivo e comandos de linha.

**O que revisar:** depois de dar `pull`, o VS Code pode continuar apontando para a pasta antiga.
Fechar e reabrir o projeto resolve. O repositório no GitHub continua com o nome antigo — renomear
lá é decisão e tarefa da equipe.

### `ReiAmarrilho.csproj.old` — apagado

**O que foi feito:** o arquivo foi removido.

**Por quê:** era uma cópia de segurança que o próprio Godot criou sozinho ao atualizar a versão
do Godot.NET.Sdk de 4.6.0 para 4.6.1. O conteúdo era idêntico ao arquivo atual, só com a versão
antiga. Não tinha nada que já não estivesse no arquivo em uso.

### Assets movidos e organizados por personagem

**O que foi feito:** os 37 PNGs provisórios e as 4 músicas saíram de `docs/externo/` e foram para
`ContosDeOutrora/Assets/`, cada um na pasta do seu personagem, chefe, inimigo ou NPC. Os nomes de
pasta e de arquivo foram padronizados sem espaço e sem acento.

**Por quê:** em `docs/externo/` eles eram só documentação; dentro de `Assets/` o Godot os enxerga
e eles podem ser usados no jogo. A separação por personagem evita que, com centenas de imagens,
ninguém mais ache nada. As regras completas estão em
[../ContosDeOutrora/Assets/README.md](../ContosDeOutrora/Assets/README.md).

**O que revisar:**
- A arte provisória está em `Provisorio/` dentro de cada pasta. **Apagar quando a definitiva ficar pronta.**
- O PNG **"Criança"** não corresponde a nenhum personagem do GDD. Está em
  `Characters/_SemNomeNoGDD_Crianca` até alguém confirmar quem é.
- A música **"Marioneteira"** não aparece no GDD nem entre os PNGs de chefe. Foi colocada em
  `Audio/Music/Bosses/` por suposição — confirmar se é chefe mesmo.
- As pastas de personagem usam `NomePróprio_Classe`. O GDD tem grafias conflitantes para três
  personagens (ver Bloco B das sugestões); se a equipe decidir outra grafia, as pastas mudam.

### Cache do Godot (`.godot/`) apagado e recriado

**O que foi feito:** a pasta de cache foi apagada e gerada de novo.

**Por quê:** ela guarda os caminhos dos arquivos, e todos mudaram com a renomeação. Um cache com
caminhos velhos faz o Godot reclamar de arquivos que não existem mais. Essa pasta é descartável:
o Godot a refaz sozinha, e ela nem vai para o Git.

---

## 12/09/2026 — Formatos de arquivo e nomes oficiais

### Músicas convertidas de `.wav` para `.ogg`

**O que foi feito:** `Ciclope.wav` (38,2 MB) e `Elevador.wav` (27,2 MB) viraram `.ogg` de
4,7 MB e 3,5 MB. Os `.wav` originais foram apagados da pasta do projeto.

**Por quê:** o `.wav` guarda o som sem nenhuma compressão, e o Git guarda uma cópia inteira de
cada versão de um arquivo. Cinco ajustes numa música de 38 MB virariam 190 MB no repositório,
para sempre, para todos. Foi feito antes do primeiro commit desses arquivos, então nada disso
entrou no histórico.

**O que revisar:** os originais em `.wav` continuam no Drive da equipe — nada foi perdido. Se
alguém achar que a qualidade caiu, dá para reexportar. As durações ficaram idênticas e a
diferença não é audível num jogo.

Os dois `.mp3` **não** foram convertidos: já estão comprimidos em 192 kbps, e comprimir de novo
perderia qualidade sem economizar muito.

### Imagens convertidas de `.png` para `.webp`

**O que foi feito:** as 37 imagens provisórias viraram `.webp` sem perda. No total, 4,39 MB
viraram 2,46 MB. Os `.png` foram apagados.

**Por quê:** o `.webp` sem perda guarda **exatamente** os mesmos pixels do PNG ocupando cerca
da metade do espaço. Cada imagem foi conferida uma a uma comparando os pixels antes e depois.

**O que revisar — vale saber:** em 8 das 37 imagens, a comparação acusou diferença. Ao
investigar, **todas as diferenças estavam em pontos 100% transparentes** — o WebP apaga a cor
guardada "por baixo" de pontos invisíveis, porque ela não aparece na tela. Nenhum ponto visível
mudou em nenhuma das 37 imagens. O Godot também corrige as bordas transparentes sozinho na
importação (opção `fix_alpha_border`, que já vem ligada).

### `Thao` renomeado para `Tao`

**O que foi feito:** as pastas e arquivos do Peregrino passaram de `Thao_Peregrino` para
`Tao_Peregrino`, em `Art/Characters/` e `Audio/Music/Personagens/`.

**Por quê:** o PO confirmou em 12/09/2026 que a grafia oficial é **Tao**. O GDD usa as duas
formas em trechos diferentes. As outras duas grafias confirmadas (**Xamã** e **Jedara**) já
estavam corretas nas pastas.

### Pasta `Art/Effects/` criada

**O que foi feito:** nova pasta para explosões, magias, fogo e partículas.

**Por quê:** ficou definido que personagens são animados por ossos (partes do corpo que se
movem), mas efeitos não funcionam bem assim — a forma deles muda inteira a cada quadro. Efeitos
usam sprite sheet quadro a quadro, e precisam de um lugar próprio, separado dos personagens.

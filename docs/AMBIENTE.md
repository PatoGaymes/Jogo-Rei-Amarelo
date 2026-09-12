# Ambiente de desenvolvimento — Contos de Outrora: O Rei De Amarelo

Estado do ambiente em **11/09/2026**, montado e testado na máquina do Eric.

---

## O que está instalado e validado

| Item | Versão | Situação |
|---|---|---|
| Godot | 4.6.1-stable **mono** (win64) | OK |
| .NET SDK | 10.0.103 e 10.0.300 | OK |
| Alvo do projeto C# | `net8.0` | **OK — testado rodando** |
| Godot.NET.Sdk | 4.6.1 | Atualizado pelo próprio Godot em 11/09 |
| Renderizador | Forward+ | Funcionando (ver pendência 1) |
| VS Code | com C#, C# Dev Kit e godot-tools | OK |

### Sobre a questão do .NET 8 vs .NET 10

A máquina tem **apenas o .NET 10**, e o Godot 4.6 gera projetos mirando a versão 8. Havia o
risco de o jogo compilar mas não abrir. **Foi testado e funciona** — não é necessário instalar
o .NET 8 nem mudar o projeto para `net10.0`.

Se um dev tiver o erro *"você precisa instalar o .NET"*, a solução é instalar o **.NET SDK 10**.

---

## ✅ RESOLVIDO — músicas convertidas para `.ogg` (12/09/2026)

O problema era que os `.wav` guardam o som **sem nenhuma compressão**, e o Git armazena **uma
cópia inteira de cada versão** de um arquivo. Cinco ajustes numa música de 38 MB virariam
190 MB no repositório, para sempre, para todo mundo que clonar.

**Foi resolvido antes do primeiro commit dos arquivos**, então nada disso entrou no histórico.

| Arquivo | Antes | Depois | Redução |
|---|---|---|---|
| `Ciclope` | 38,2 MB (WAV) | **4,7 MB** (OGG) | 87% |
| `Elevador` | 27,2 MB (WAV) | **3,5 MB** (OGG) | 87% |

Conversão feita com ffmpeg (`libvorbis -q:a 6`, ~175 kbps). As durações dos arquivos ficaram
idênticas, e a diferença de qualidade não é audível num jogo.

**Os dois `.mp3` não foram convertidos de propósito.** `Marioneteira.mp3` e `Tao_Peregrino.mp3`
já estão comprimidos em 192 kbps. Converter MP3 para OGG seria comprimir o que já foi
comprimido — perde qualidade de novo e economiza pouco. O Godot toca MP3 sem problema.

> **Se quiserem tudo em `.ogg`**, o certo é **reexportar do projeto original** da música, não
> converter o MP3. Exportar direto em `.ogg` evita a perda dupla.

**Daqui para frente:** exportar música já em `.ogg`. A regra completa está em
[Assets/README.md](../ContosDeOutrora/Assets/README.md).

**Total da pasta de áudio:** era 80 MB, hoje são 23 MB.

---

## Problema encontrado e corrigido (09/09)

**O C# não funcionava por causa de um espaço no nome.**

A configuração `project/assembly_name` estava como `"Rei Amarrilho"`, com espaço. O Godot
procurava `Rei Amarrilho.dll`, mas o C# gerava outro nome. Como não batiam, **nenhum script C#
carregava**, com o erro:

```
Cannot instantiate C# script because the associated class could not be found.
```

Hoje o valor é `ContosDeOutrora`, sem espaço, e o C# roda normalmente.

---

## Renomeação para o nome oficial (11/09)

A equipe definiu o nome **Contos de Outrora: O Rei De Amarelo**. Foi replicado assim:

| Onde | Antes | Depois |
|---|---|---|
| Pasta do projeto | `ReiDoAmarrilho/` | `ContosDeOutrora/` |
| Projeto C# | `ReiAmarrilho.csproj` / `.sln` | `ContosDeOutrora.csproj` / `.sln` |
| `config/name` (título na tela) | `Rei Amarrilho` | `Contos de Outrora: O Rei De Amarelo` |
| `project/assembly_name` | `ReiAmarrilho` | `ContosDeOutrora` |
| Caminhos no VS Code | `ReiDoAmarrilho` | `ContosDeOutrora` |

**O nome com espaços e dois-pontos fica só em `config/name`**, que é o que o jogador vê. Pasta
e programa usam `ContosDeOutrora` porque espaço, acento e dois-pontos quebram caminhos de
arquivo e comandos de linha.

> **O repositório no GitHub continua `Jogo-Rei-Amarelo`.** Renomear lá é decisão da equipe e
> tem que ser feito por um dev — o Claude não mexe no GitHub (Regra 1). Renomear é seguro: o
> GitHub redireciona o endereço antigo automaticamente.

**Depois de dar `pull`:** o VS Code pode continuar apontando para a pasta antiga. Fechar e
reabrir o projeto resolve. Se o C# reclamar, rodar `dotnet build-server shutdown`.

---

## Configurações aplicadas no projeto (`project.godot`)

Valem para **todos os devs**, porque estão versionadas no Git.

| Configuração | Antes | Depois | Motivo |
|---|---|---|---|
| `config/name` | `Rei Amarrilho` | `Contos de Outrora: O Rei De Amarelo` | Nome oficial definido pela equipe |
| `project/assembly_name` | `Rei Amarrilho` | `ContosDeOutrora` | O espaço quebrava todo o C# |
| `window/vsync/vsync_mode` | `0` (desligado) | `1` (ligado) | Desligado, a imagem rasga e a placa fica a 100% à toa |
| `window/size/window_width_override` | não existia | `1280` | Faz a janela de teste caber num monitor 1080p |
| `window/size/window_height_override` | não existia | `720` | Idem |
| `layer_names/2d_physics/*` | sem nome | `Player`, `Enemy`, `Interactable`, `Wall`, `Trigger` | Para não decorar o que é "camada 3" |
| `[input]` | vazio | 7 comandos nomeados | Permite trocar teclas sem mexer no código |

### Comandos criados no Input Map

| Comando | Teclas |
|---|---|
| `move_left` | A, seta esquerda |
| `move_right` | D, seta direita |
| `interact` | E |
| `combat_confirm` | Espaço |
| `combat_cancel` | X |
| `open_inventory` | I |
| `pause` | Esc |

Ponto de partida. Trocar em **Project Settings → Input Map**.

> Pelo GDD, o combate tem **5 posições** e uma ação de **Movimento** lateral. Quando o combate
> for implementado, provavelmente serão necessárias ações novas para navegar a formação.

### Configurações conferidas e corretas (não mexer)

Já estavam certas para arte 2D em alta resolução:

- `viewport_width` / `viewport_height` = **1920 x 1080**
- `stretch/mode` = **canvas_items** — escala arte e interface juntas mantendo a nitidez
- `stretch/aspect` = **expand** — em telas ultrawide mostra mais área lateral em vez de barras
  pretas. Exige atenção às âncoras da interface.
- `stretch/scale_mode` = **fractional** — correto para arte HD (`integer` é só pixel art)
- Filtro de textura = **Linear** — **não** trocar para `Nearest`, deixaria a arte serrilhada
- Snap de pixel = **desligado** — recurso de pixel art, atrapalharia o movimento suave

### Canais de áudio (`default_bus_layout.tres`)

`Master` → `Music`, `SFX`, `UI`, `Ambience`

Sem isso não há como fazer controles de volume separados no menu de opções.

---

## Configurações do Godot por máquina — ✅ aplicadas por Eric em 11/09

Não são versionadas: **cada dev novo precisa repetir na instalação dele.**

| Configuração | Valor | Motivo |
|---|---|---|
| **Dotnet → Editor → External Editor** | **Visual Studio Code** | **É o que faz os scripts C# abrirem no VS Code.** A opção `Text Editor → External` controla apenas GDScript |
| Text Editor → Behavior → Files → **Auto Reload Scripts on External Change** | Ativado | Sem isso, ao editar no VS Code e voltar ao Godot, ele mostra a versão antiga |
| Interface → Editor → **Save on Focus Loss** | Ativado | Salva ao alternar para o VS Code |
| Interface → Editor → **Update Continuously** | Desativado | Reduz consumo de energia e da placa de vídeo |
| Project Manager → Directory Naming Convention | **PascalCase** | Alinha com `Scenes`, `Scripts`, `Assets` |
| Idioma do editor | `[en] English` | Traduções atrapalham ao buscar ajuda online |

---

## Ferramentas instaladas para o projeto

| Ferramenta | Para que | Instalado em |
|---|---|---|
| **ffmpeg** 9.0.1 | Converter áudio e imagem (`.wav`→`.ogg`, `.png`→`.webp`) | 12/09/2026, via `winget install Gyan.FFmpeg` |

Se o comando `ffmpeg` não for reconhecido no terminal, feche e abra o terminal de novo — a
instalação altera o PATH e programas já abertos não enxergam a mudança.

---

## ✅ Pendências resolvidas

- ~~`net8.0` vs `net10.0`~~ → `net8.0`, testado e funcionando
- ~~Nome oficial do jogo~~ → **Contos de Outrora: O Rei De Amarelo**, replicado no projeto
- ~~Configurações do Godot por máquina~~ → aplicadas
- ~~Sugestões para a documentação~~ → aplicadas no Trello pela equipe
- ~~Formato das músicas~~ → **`.ogg`**, convertido (87% menor)
- ~~Formato das imagens~~ → **`.webp`** sem perda, convertido (44% menor)
- ~~Técnica de animação~~ → **ossos** para personagens, **quadro a quadro** para efeitos
- ~~GDD vs Árvore de Habilidades~~ → **a Árvore é a fonte oficial do combate** (decisão do PO)

## ⚠️ Pendências em aberto

### 1. Renderizador: Forward+ ou Compatibility

Hoje está em **Forward+**, um renderizador voltado para 3D avançado. Como o jogo é 2D puro, o
modo **Compatibility** geraria um jogo mais leve, que roda em placas de vídeo fracas e pode ser
publicado para navegador. As luzes 2D (tochas, ambientação escura) funcionam nos dois.

Quanto antes decidir, menos retrabalho.

### 2. Quais ações existem no turno de combate

O PO confirmou que é **1 ação por turno**, mas não quais ações existem: o GDD lista Movimento e
Conversar, a Árvore lista Fugir. Isso define as telas de combate e os controles.
Ver pendência 2 em [SUGESTOES-PARA-DOCUMENTACAO.md](SUGESTOES-PARA-DOCUMENTACAO.md).

### 3. Cena inicial do jogo

O jogo abre direto na cena do personagem de teste. O certo é criar `Scenes/Main.tscn` (menu ou
gerenciador) e apontar `run/main_scene` para ela.

### 4. A câmera dentro do personagem

A cena do player tem um `Camera2D` que segue o personagem. Pelo GDD, a exploração é **lateral,
só para frente e para trás**, e a câmera pertence à cena/corredor. Esse nó provavelmente sai
quando a movimentação real for implementada — que é justamente o próximo card do Trello.

### 5. Singletons (Autoload)

Ainda não criados. Candidatos: `GameManager`, `AudioManager`, `SaveSystem`.
Definir junto com a arquitetura do combate.

### 6. O GDD continua com as regras antigas de combate escritas

A Árvore de Habilidades virou a fonte oficial, mas o GDD — que é o documento principal e o
primeiro lugar onde alguém novo procura — ainda tem as regras antigas no texto. Enquanto os
dois coexistirem, a confusão se repete com a próxima pessoa que ler.
Sugestão de correção na pendência 1 de [SUGESTOES-PARA-DOCUMENTACAO.md](SUGESTOES-PARA-DOCUMENTACAO.md).

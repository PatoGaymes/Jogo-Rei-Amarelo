# Ambiente de desenvolvimento — Rei Amarrilho

Estado do ambiente em **09/09/2026**, montado e testado na máquina do Eric.

---

## O que está instalado e validado

| Item | Versão | Situação |
|---|---|---|
| Godot | 4.6.1-stable **mono** (win64) | OK |
| .NET SDK | 10.0.103 e 10.0.300 | OK |
| Alvo do projeto C# | `net8.0` | **OK — testado rodando** |
| Renderizador | Forward+ | Funcionando (ver pendência abaixo) |
| VS Code | com C#, C# Dev Kit e godot-tools | OK |

### Sobre a questão do .NET 8 vs .NET 10

A máquina tem **apenas o .NET 10** instalado, e o Godot 4.6 gera projetos C# mirando a
versão 8. Havia o risco de o jogo compilar mas não abrir.

**Foi testado e funciona.** O jogo compila e roda normalmente. **Não é necessário instalar
o .NET 8** nem mudar o projeto para `net10.0`.

Se algum dev tiver o erro *"você precisa instalar o .NET"* na máquina dele, a solução é
instalar o **.NET SDK 10** — o mesmo desta máquina.

---

## Problema encontrado e corrigido

**O C# não funcionava por causa de um espaço no nome.**

A configuração `project/assembly_name` estava como `"Rei Amarrilho"`, com espaço. O Godot
procurava por um arquivo chamado `Rei Amarrilho.dll`, mas o C# gerava `ReiAmarrilho.dll`.
Como os nomes não batiam, **nenhum script C# carregava**, com o erro:

```
Cannot instantiate C# script because the associated class could not be found.
```

Corrigido para `"ReiAmarrilho"`, sem espaço. Depois disso o C# passou a rodar.

---

## Configurações aplicadas no projeto (`project.godot`)

Estas valem para **todos os devs**, porque estão versionadas no Git.

| Configuração | Antes | Depois | Motivo |
|---|---|---|---|
| `project/assembly_name` | `Rei Amarrilho` | `ReiAmarrilho` | O espaço quebrava todo o C# (acima) |
| `window/vsync/vsync_mode` | `0` (desligado) | `1` (ligado) | Desligado, a imagem rasga e a placa de vídeo fica a 100% à toa |
| `window/size/window_width_override` | não existia | `1280` | Faz a janela de teste caber num monitor 1080p |
| `window/size/window_height_override` | não existia | `720` | Idem |
| `layer_names/2d_physics/*` | sem nome | `Player`, `Enemy`, `Interactable`, `Wall`, `Trigger` | Para não ter que decorar o que é "camada 3" |
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

São um ponto de partida. Trocar pelo painel do Godot em **Project Settings → Input Map**.

### Configurações que foram conferidas e estão corretas (não mexer)

Estas já estavam certas para arte 2D em alta resolução:

- `viewport_width` / `viewport_height` = **1920 x 1080**
- `stretch/mode` = **canvas_items** — escala arte e interface juntas mantendo a nitidez
- `stretch/aspect` = **expand** — em telas ultrawide mostra mais área lateral em vez de
  barras pretas. Exige atenção às âncoras da interface.
- `stretch/scale_mode` = **fractional** — correto para arte HD (`integer` é só para pixel art)
- Filtro de textura = **Linear** — **não** trocar para `Nearest`, que deixaria a arte serrilhada
- Snap de pixel = **desligado** — recurso de pixel art, atrapalharia o movimento suave

### Canais de áudio criados (`default_bus_layout.tres`)

`Master` → `Music`, `SFX`, `UI`, `Ambience`

Sem isso não há como fazer os controles de volume separados no menu de opções.

---

## Configurações do Godot que cada dev faz na sua máquina

Estas **não** são versionadas — cada pessoa precisa aplicar na sua instalação.

### Correção necessária

| Configuração | Mudar para | Motivo |
|---|---|---|
| Project Manager → Directory Naming Convention | **PascalCase** | Estava `camelCase`, mas as pastas do projeto (`Scenes`, `Scripts`) são PascalCase |

### O que falta configurar (importante)

| Configuração | Valor | Motivo |
|---|---|---|
| **Dotnet → Editor → External Editor** | **Visual Studio Code** | **É o que faz os scripts C# abrirem no VS Code.** A opção `Text Editor → External` controla apenas o GDScript — muita gente configura só ela e acha que terminou |
| Text Editor → Behavior → Files → **Auto Reload Scripts on External Change** | Ativado | Sem isso, ao editar no VS Code e voltar ao Godot, ele mostra a versão antiga do arquivo |
| Interface → Editor → **Save on Focus Loss** | Ativado | Salva ao alternar para o VS Code, evita perder trabalho |
| Interface → Editor → **Update Continuously** | Desativado | Reduz consumo de energia e da placa de vídeo |

### O que já está correto

- Idioma do editor: `[en] English`
- Text Editor → External → Use External Editor: ativado
- Exec Path apontando para o VS Code
- Exec Flags: `{project} --goto {file}:{line}:{col}`

---

## Pendências — precisam de decisão da equipe

### 1. Nome oficial do jogo

Existem **três grafias diferentes** hoje:

| Onde | Nome |
|---|---|
| Repositório no GitHub | `Jogo-Rei-Amarelo` |
| Pasta do projeto | `ReiDoAmarrilho` |
| Configuração do jogo | `Rei Amarrilho` |

Definir qual é o correto e alinhar os três.

### 2. Renderizador: Forward+ ou Compatibility

Hoje está em **Forward+**, que é um renderizador voltado para jogos 3D avançados. Como o
jogo é 2D puro, o modo **Compatibility** geraria um jogo mais leve, que roda em placas de
vídeo mais fracas e pode ser publicado para navegador. As luzes 2D (tochas, ambientação
escura) funcionam nos dois modos.

**Não é urgente**, mas quanto antes decidir, menos retrabalho.

### 3. Cena inicial do jogo

Hoje o jogo abre direto na cena do personagem de teste. O certo é criar uma
`Scenes/Main.tscn` (menu ou gerenciador) e apontar a configuração `run/main_scene` para ela.

### 4. A câmera dentro do personagem

A cena do player tem um `Camera2D` que segue o personagem. Em jogos no estilo Darkest
Dungeon a **câmera pertence à cena/corredor**, não ao personagem. Provavelmente esse nó
deve ser removido quando o design das telas estiver definido.

### 5. Singletons (Autoload)

Ainda não criados. Prováveis candidatos: `GameManager`, `AudioManager`, `SaveSystem`.
Definir junto com a arquitetura do jogo.

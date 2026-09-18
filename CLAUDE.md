# Contos de Outrora: O Rei De Amarelo

RPG tático por turnos da equipe **PatoGaymes**. **Godot 4.6** + **C#**.
Mapa **3D isométrico**, personagens **2D** — câmera e gameplay no estilo **Don't Starve Together**.

Detalhes de lore, personagens, combate e escopo da demo: **[docs/JOGO.md](docs/JOGO.md)**.

---

## Regras obrigatórias

### Regra 1 — Nunca commitar

Não executar `git commit`, `git push`, `git merge`, `git rebase` nem nada que altere o histórico ou
o repositório remoto. **O versionamento é da equipe de devs.** O trabalho do Claude termina no
arquivo salvo em disco.

### Regra 2 — Marcar toda alteração feita por IA

Todo trecho criado ou alterado leva o comentário `Alteração de IA - Revisar`, com **o que faz** e
**por quê**, em linguagem **clara e sem jargão técnico**.

```csharp
// Alteração de IA - Revisar
// O que faz: guarda a velocidade de caminhada do personagem.
// Por quê: o valor precisa ser ajustável no editor sem mexer no código.
```

Arquivos que **não aceitam comentário** (JSON puro, `.tscn`, `.tres` — o Godot apaga comentários ao
salvar): registrar em [docs/ALTERACOES-IA.md](docs/ALTERACOES-IA.md).

### Regra 3 — Nunca alterar a documentação externa

Trello e Google Drive são a fonte de verdade e o Claude não os altera. Sugestões vão para
[docs/SUGESTOES-PARA-DOCUMENTACAO.md](docs/SUGESTOES-PARA-DOCUMENTACAO.md).

**Arquivos com sufixo `DoRepositorio`:** são o espelho do que está no Drive. O Claude edita **apenas
os arquivos sem esse sufixo**. Havendo divergência, **perguntar qual lado vale antes de aplicar** —
nunca escolher sozinho.

**O caminho de volta para o Drive** (já que o Claude não escreve lá):

```bash
python docs/ferramentas/sincronizar-docs.py         # o que está diferente?
python docs/ferramentas/atualizar-documentacao.py   # gera os .docx atualizados
```

Os arquivos saem em `docs/para-repositorio/`, prontos para substituir os do Drive.

**Regra da formatação:** os documentos são lidos por outras pessoas da equipe, então os `.docx`
**nunca são gerados do zero** — o script abre o original e altera só o texto, mantendo estilos,
títulos, numeração e sumário intactos. Gerar a partir de markdown descarta tudo isso e entrega um
documento com cara diferente do resto da documentação.

As ferramentas estão em [docs/ferramentas/docx_editor.py](docs/ferramentas/docx_editor.py); o que
já foi alterado em cada documento fica em
[docs/MUDANCAS-DOCUMENTACAO.md](docs/MUDANCAS-DOCUMENTACAO.md).

Precisa do pandoc para conferir o resultado: `winget install JohnMacFarlane.Pandoc`.

### Regra 4 — Processar na máquina, não no contexto

**Todo dado que pode ser reduzido por um comando deve ser reduzido antes de virar contexto.**
Contar, filtrar, somar, comparar, converter e validar é trabalho da máquina. Interpretar o
resultado é trabalho do modelo.

Isso existe porque despejar arquivo bruto no contexto gasta a cota de uso à toa e ainda deixa a
resposta pior — o dado relevante se perde no meio do volume.

Na prática:

| Em vez de | Fazer |
|---|---|
| Ler vários arquivos para contar ocorrências | `rg -c padrao` |
| Ler um arquivo grande inteiro | `rg -n padrao arquivo`, `sed -n 'X,Yp'` |
| Comparar dois documentos lendo os dois | script que imprime só as diferenças |
| Inspecionar JSON grande | `jq` com o filtro do campo desejado |
| Verificar imagem, áudio ou vídeo | `ffprobe` / `ffmpeg` |
| Buscar arquivos | `fd` / `rg --files` com filtro |

**Máquina disponível:** Ryzen 7 5800H (8 núcleos / 16 threads), 15,4 GB de RAM, GTX 1650 4 GB.
Ferramentas: `rg`, `fd`, `jq`, `python`, `node`, `ffmpeg`, `git`, `dotnet`.

**Cuidado com a RAM:** costuma haver só ~5 GB livres. Processar arquivo grande em partes, não
carregar tudo de uma vez.

---

## Linguagens

**C#** é a principal; **GDScript** para scripts pequenos de cena.
**Godot não suporta JavaScript, Lua nem Python** dentro do jogo — se pedirem, avisar e oferecer C#
ou GDScript. (Python serve para ferramentas de apoio fora do jogo, como o script de sincronia.)

---

## Ambiente

| Item | Versão |
|---|---|
| Godot | 4.6.1-stable **mono** — `Documents/Godot/Godot_v4.6.1-stable_mono_win64.exe` |
| .NET SDK | 10.0.300 (projeto mira `net8.0`, testado e rodando) |
| Renderizador | Forward+ · Física: **Jolt** |
| Repositório | `github.com/PatoGaymes/Jogo-Rei-Amarelo` (nome anterior ao título oficial) |

Detalhes e pendências: [docs/AMBIENTE.md](docs/AMBIENTE.md).

---

## Estrutura

```
ContosDeOutrora/          # projeto Godot
├── Scenes/               # Characters/ UI/ Combat/ Levels/
├── Scripts/              # Characters/ Combat/ UI/ Systems/
├── Assets/               # arte e som POR PERSONAGEM — ver Assets/README.md
├── Resources/            # dados .tres — heróis, habilidades, inimigos
└── project.godot
docs/                     # documentação interna + espelho da externa
```

**Cada personagem, chefe, inimigo e NPC tem pasta própria** (`NomePróprio_Classe`, sem espaço nem
acento), com `Combat/`, `Dialogue/`, `Map/` e `Provisorio/`. Regras completas em
[Assets/README.md](ContosDeOutrora/Assets/README.md) — **ler antes de adicionar arte ou som**.

**`Resources/` é importante:** heróis, habilidades e inimigos são arquivos de dados (`.tres`), não
valores fixos no código, para o game designer balancear sem programar.

### Formatos

| Tipo | Formato |
|---|---|
| Imagem | **`.webp`** sem perda (metade do PNG, pixels idênticos) |
| Música | **`.ogg`** — exportar já em ogg, nunca `.wav` para música |
| Efeito sonoro curto | `.wav` |
| Animação | **nunca `.gif`** — o Godot não importa, e GIF só tem 256 cores |

**Animação:** personagens por **ossos/recortes**; efeitos **quadro a quadro** em `Art/Effects/`.

---

## Comandos

```bash
# Compilar
dotnet build ContosDeOutrora/ContosDeOutrora.csproj

# Rodar (GODOT = caminho do executável)
"$GODOT" --path ContosDeOutrora

# Sem janela, para teste automatizado
"$GODOT" --headless --path ContosDeOutrora --quit-after 120

# Reimportar depois de adicionar arte ou som
"$GODOT" --headless --path ContosDeOutrora --import

# Conferir sincronia da documentação
python docs/ferramentas/sincronizar-docs.py
```

No VS Code, **F5** roda com depuração de C#.

---

## Estado do código

Base de exploração 3D isométrica: câmera de 8 ângulos, personagem em billboard e movimentação
relativa à câmera. Arte e cenário ainda são provisórios.

Se uma informação sobre o jogo não estiver em `docs/externo/`, **perguntar à equipe em vez de
supor.**

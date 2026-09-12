# Documentação interna — Contos de Outrora: O Rei De Amarelo

Esta pasta existe porque **o Claude não consegue acessar a documentação do projeto**. Trello e
Google Drive são privados e exigem login. A equipe **exporta** o conteúdo para `docs/externo/`,
e é de lá que o Claude lê — ele abre PDF, Markdown, DOCX e JSON direto do repositório.

> Os sites continuam sendo a fonte de verdade. Esta pasta é uma **cópia de leitura**. O Claude
> nunca altera nada nos sites (Regra 3).

> **OneNote e Canva foram descartados** pela equipe em 11/09/2026 por estarem desatualizados.

---

## O que já está em `docs/externo/`

| Arquivo | Conteúdo | Situação |
|---|---|---|
| `Infos/GDD Rei de Amarelo.md` | **Documento principal** — lore, mapa, exploração, combate, atributos, progressão e os 10 personagens com a lore em 4 Atos | Atual |
| `Infos/Árvores de Habilidades.md` | Habilidades e caminhos de cada personagem | Atual, mas **conflita com o GDD** — ver [SUGESTOES-PARA-DOCUMENTACAO.md](SUGESTOES-PARA-DOCUMENTACAO.md) |
| `trello-board.json` | Quadro de tarefas da equipe | Atual |

A arte e as músicas **saíram desta pasta** e foram para `ContosDeOutrora/Assets/`, organizadas
por personagem. Ver [Assets/README.md](../ContosDeOutrora/Assets/README.md).

---

## Como exportar cada fonte

### Trello — quadro do jogo

1. Abrir o quadro
2. Menu **⋯** (canto superior direito) → **Print, Export and Share**
3. **Export as JSON**
4. Salvar como `docs/externo/trello-board.json`

O JSON traz listas, cards, descrições e checklists de uma vez.

### Google Docs / Drive

1. Abrir o documento
2. **Arquivo → Fazer download → Markdown (.md)**
3. Salvar em `docs/externo/Infos/`

**Markdown é o melhor formato** para o Claude: ele lê o texto exatamente como está escrito, sem
perder a estrutura de títulos e listas. Se baixar em `.docx`, também funciona, mas o Markdown é
mais confiável.

---

## Quando reexportar

Sempre que a documentação mudar de forma relevante. Não precisa ser a cada ajuste pequeno — mas
**antes de pedir ao Claude para trabalhar numa mecânica, vale reexportar a fonte dela**, senão
ele trabalha com informação velha.

---

## Arquivos desta pasta

| Arquivo | Para que serve |
|---|---|
| [AMBIENTE.md](AMBIENTE.md) | Como o ambiente foi montado, o que foi testado, e o que ainda precisa ser decidido |
| [ALTERACOES-IA.md](ALTERACOES-IA.md) | Registro das alterações do Claude em arquivos que não aceitam comentário |
| [SUGESTOES-PARA-DOCUMENTACAO.md](SUGESTOES-PARA-DOCUMENTACAO.md) | Correções sugeridas para a documentação, para um dev revisar e subir |
| `externo/` | Onde ficam os exports do Trello e do Drive |

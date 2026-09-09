# Documentação interna — Rei Amarrilho

Esta pasta existe porque **o Claude não consegue acessar a documentação do projeto**.
Trello, OneNote, Canva e Google Drive são todos privados e exigem login — foi testado, e
nenhum deles abre sem autenticação.

A solução é a equipe **exportar** o conteúdo para `docs/externo/`. O Claude lê PDF e JSON
direto do repositório, então esses arquivos funcionam como documentação viva.

> **Importante:** os sites continuam sendo a fonte de verdade. Esta pasta é só uma **cópia
> de leitura**. O Claude nunca altera nada nos sites (Regra 3).

---

## Como exportar cada fonte

### Trello — quadro "Jogo Rei Amarelo"

1. Abrir o quadro
2. Menu **⋯** (canto superior direito) → **Print, Export and Share**
3. Escolher **Export as JSON**
4. Salvar como `docs/externo/trello-board.json`

O JSON traz todas as listas, cards, descrições e checklists de uma vez.

### OneNote — caderno de personagens

1. Abrir o caderno no OneNote
2. **Arquivo → Exportar**
3. Escolher **Seção** ou **Bloco de anotações**, formato **PDF**
4. Salvar como `docs/externo/personagens.pdf`

### Canva — os três designs

Para cada um dos três links:

1. Abrir o design
2. **Compartilhar → Baixar**
3. Tipo de arquivo: **PDF Standard**
4. Salvar em `docs/externo/` com nome descritivo
   (ex: `canva-interface.pdf`, `canva-personagens.pdf`, `canva-mundo.pdf`)

### Google Drive

Baixar os arquivos relevantes (documentos, planilhas de balanceamento, referências de arte)
e colocar em `docs/externo/`.

---

## Quando reexportar

Sempre que a documentação mudar de forma relevante. Não precisa ser a cada alteração
pequena — mas se o Claude for trabalhar em cima de uma mecânica, vale reexportar a fonte
daquela mecânica antes, para ele não trabalhar com informação velha.

---

## Arquivos desta pasta

| Arquivo | Para que serve |
|---|---|
| [AMBIENTE.md](AMBIENTE.md) | Como o ambiente foi montado, o que foi testado e o que ainda precisa ser decidido |
| [ALTERACOES-IA.md](ALTERACOES-IA.md) | Registro das alterações feitas pelo Claude em arquivos que não aceitam comentário |
| [SUGESTOES-PARA-DOCUMENTACAO.md](SUGESTOES-PARA-DOCUMENTACAO.md) | Correções sugeridas para a documentação dos sites, para um dev revisar e subir |
| `externo/` | Onde ficam os exports do Trello, OneNote, Canva e Drive |

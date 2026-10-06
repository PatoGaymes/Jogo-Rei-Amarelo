# Sugestões para a documentação externa

A Regra 3 diz que o Claude **nunca altera** um documento oficial do Trello ou do Google Drive. Quando
ele identificar algo que precisa ser corrigido lá, a sugestão vem para cá, e um dev decide se
sobe ou não. Correção de documento já pronta vai para a pasta **Pato Games/Revisão** do Drive (desde
06/10/2026 o Claude tem acesso a ela) — ver a Regra 3 no `CLAUDE.md`. Aqui ficam as **perguntas**:
o que precisa de decisão da equipe antes de virar texto.

**Nada nesta página foi aplicado nos sites.**

---

# ✅ RESOLVIDAS

## 09/09/2026 — Preparação do ambiente

As 5 sugestões (linguagens do Godot, editor externo para C#, extensões do VS Code, convenção de
nomes de pastas e a observação sobre o .NET 10) foram **aplicadas no Trello** pela equipe em
11/09/2026. Confirmado na exportação do quadro.

## 11/09/2026 — Conflito entre GDD e Árvore de Habilidades

**Respondido pelo PO/PM em 12/09/2026.** As decisões estão registradas em
[CLAUDE.md](../CLAUDE.md) e resumidas abaixo.

### A Árvore de Habilidades é o documento oficial do combate

O PO explicou: *"os efeitos de status foram alterados, e conforme eu fui fazendo as árvores de
habilidade, eu dei algumas alteradas em certas coisas, e ao invés de puxar pro GDD, eu coloquei
no documento da árvore de habilidades, sendo assim, ele é o atualizado."*

| Item | Decisão |
|---|---|
| **Efeitos de status** | Vale a lista da Árvore. **Atualizada em 18/09:** Gelo, **Veneno**, Sangramento, Fogo, Corrosão, Raio, Escuridão, Luz — mais a condição **Espinhos** |
| **Vanguarda** | Quem tem Vanguarda **protege os aliados e toma o dano por eles**. Não era contradição — a Árvore explicou a mesma coisa com outras palavras |
| **Vulnerável** | Vale o da Árvore: **não pode receber buffs** |
| **Ações por turno** | **1 ação por turno**, salvo item ou habilidade que contorne isso |
| **Corrompido / Purificado** | São os nomes **antigos** de Escuridão e Luz. **Não entram no produto final** |
| **Personagens sem lore** | São recrutáveis só via gameplay. **Não precisam de lore extensa** — não é lacuna |

### Nomes oficiais definidos

**Xamã**, **Tao**, **Jedara**. Já aplicados nas pastas de assets do projeto
(`Amana_Xama`, `Tao_Peregrino`, `Jedara_Brutamonte`).

---

## 12/09/2026 — As 5 ações do turno

**Definido pelo PO:** Atacar, Habilidades, Defender, Itens, Fugir.

**Conversar foi removida** — *"não seria viável por boa parte do jogo"*. As sub-opções (Enganar,
Ameaçar, Furtar, Expor) saem junto.

Não existe ação de "mover": a movimentação na formação acontece pelas próprias habilidades
(*Avanço Tático* avança e empurra, etc.), como no Darkest Dungeon.

## 12/09/2026 — Aviso de seção desatualizada aplicado no GDD

Aplicado **na cópia local** (`docs/externo/Infos/GDD Rei de Amarelo.md`), nas seções *"Combate"*
e *"Status, Efeitos e Condições"*. Ver a pendência 1 abaixo — **falta replicar no Google Drive.**

## 12/09/2026 — Arte e música sem correspondência: resolvido

- **"Marioneteira"** → confirmada como **chefe** pelo PO. Já está em `Audio/Music/Bosses/`.
- **"Criança"** → **resolvida em 18/09**: a Demo confirma que é personagem recrutável. Pasta renomeada para `Crianca`.

---

## 18/09/2026 — Documentos sincronizados com o repositório

O repositório estava **mais novo** que nossa cópia. Aplicado localmente:
Ácido → **Veneno**, nova condição **Espinhos**, Chuva de Fogos → **Embuste**,
Sede de Sangue → **Morte lenta**, Retribuição Divina → **Retribuição**, e
Matadora de Yokais passou de "dano + cura" para "dano + veneno".

Gerado `GDD Rei de AmareloDoRepositorio.md`, **pronto para colar no Drive**, já com o aviso
de seção desatualizada aplicado. Conferência automática:
`python docs/ferramentas/sincronizar-docs.py`.

**"Criança" resolvida:** a Demo confirma que é personagem recrutável. Pasta renomeada de
`_SemNomeNoGDD_Crianca` para `Crianca`.

---

## 18/09/2026 — Todas as pendências respondidas pelo PO

| # | Pendência | Decisão |
|---|---|---|
| 1 | "Bufão Alegre" é o Abanur? | **Sim.** Pasta renomeada para `Abanur_Bufao` |
| 2 | Referências órfãs na Árvore | Nomes oficiais: **Morte lenta** e **Veneno**. Corrigido |
| 3 | Inimigos respawnam? | **Não na demo.** Fica previsto um item/mecânica de respawn **depois** dela |
| 4 | Quest do Ferreiro | **Saiu da demo.** Menções removidas — a demo tem **4** side-quests |
| 5 | "Caçador" no masculino | Erro de digitação. Agora é **Caçadora** em todas as referências |
| 6 | Documento para o Drive | Gerar **`.docx` pronto** para o analista revisar e colar |
| 7 | Atributos sem função | **Lábia, Intuição e Análise removidas.** Furtividade fica e ganha o **raio de agro** |
| 8 | Cards do Trello | **Movidos pela equipe** |

### O que a decisão 7 muda

A ficha do personagem passa de **11 para 8 atributos**. A **Furtividade** ganhou um segundo efeito:
além de reduzir o agro em combate, ela agora define **a que distância o inimigo percebe o
personagem** — quanto maior, menor o raio de agro.

Isso a conecta direto com dois sistemas da demo: os **perseguidores** e as **safe zones**. Um
personagem furtivo consegue atravessar trajetos vigiados que um barulhento não conseguiria — o
atributo deixou de ser secundário e virou escolha de estilo de jogo.

---

# ⚠️ PENDENTES

## 06/10/2026 — Combate básico: o que ficou provisório ou em aberto

Tudo isto já funciona no jogo com um valor provisório (ver `docs/JOGO.md`, seção "Combate"); a
equipe só precisa dizer se fica assim ou como deve ser.

| # | Pergunta | Como está hoje |
|---|---|---|
| 1 | **Quem tira 1 no dado de iniciativa** — o que acontece? (a frase da decisão ficou incompleta) | Nada; o registro só avisa "tirou 1" |
| 2 | Os inimigos que **não** entraram na luta: ficam parados até ela acabar, continuam a ronda, ou podem entrar se perceberem a luta? | Ficam parados onde estão |
| 3 | A **vida e a sanidade** continuam de uma luta para a outra? E o **Foco**: começa cheio a cada luta ou também continua? | Vida e sanidade continuam; Foco começa cheio |
| 4 | **Vanguarda**: quanto tempo dura a proteção da Protetora? Os **efeitos** do golpe (veneno, enraizar) também vão para a Vanguarda, ou só o dano? | 3 rodadas; só o dano vai para a Vanguarda |
| 5 | **Transfiguração térmica** "aplica gelo ou fogo": o jogador escolhe, ou é sorteado? | Sorteado |
| 6 | **Alucinações** (sanidade em 30% ou menos): a sanidade que conta é a de quem — do líder, da média do grupo, ou de cada personagem? | Ainda não implementado |
| 7 | Na exploração, os outros 4 da equipe **seguem o líder** pelo mapa, ou só aparecem quando a luta começa? | Só aparecem na luta, nas casas da formação |
| 8 | Começar a luta **pelo clique**: a que distância o jogador pode atacar um inimigo? (o ataque surpresa vem depois) | Até 8 m, com o inimigo à vista |
| 9 | Os efeitos que aparecem nas habilidades mas não são explicados (lista em `docs/JOGO.md`): maldição, medo, náusea, marca, fúria, taunt, camuflagem, frio, dano verdadeiro, dano massivo... | Só a maldição existe, sem efeito |
| 10 | **Nomes com duas grafias** na Árvore: Pouco Espaço / Espaço aberto; Fera / Besta vampírica; Negrosar / Necrosar; dois "Cruel" no Desgarrado; "Cartas de baralho" / "Roda dos 6 Desejos" na Xamã | Não alterados |
| 11 | **Rosaria**: "Aberração" (Árvore e projeto) ou "Abominação" (Demo e Interações)? | "Aberração" |
| 12 | **Iniciativa** é atributo de Corpo, Mente ou Essência? | Corpo (pela posição na lista do GDD) |
| 13 | A **Criança** (recrutável na Demo) não tem árvore de habilidades | — |

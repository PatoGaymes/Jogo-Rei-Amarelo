# Sugestões para a documentação externa

A Regra 3 diz que o Claude **nunca altera** a documentação do Trello e do Google Drive. Quando
ele identificar algo que precisa ser corrigido lá, a sugestão vem para cá, e um dev decide se
sobe ou não.

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

# ⚠️ PENDENTES

## 1. O "Bufão Alegre" é o Abanur? — **prioridade média**

O GDD do repositório ganhou um narrador: **Abanur, o deus da morte e das artes**, que *"aparece
para o jogador trajado como um bufão contador de histórias"* e **aparece em cena**, não só narra.

Nos assets existe um NPC chamado **"Bufão Alegre"** (`Art/Npcs/BufaoAlegre`). Pela descrição,
parece ser o próprio Abanur — mas **isso não está escrito em lugar nenhum**.

**Por que importa:** se forem a mesma pessoa, a pasta deveria se chamar `Abanur_Bufao` e o
personagem precisa de tratamento especial (aparece em vários pontos da história). Se forem
diferentes, são dois NPCs parecidos e isso vai confundir quem produzir a arte.

**O que decidir:** são o mesmo personagem?

## 2. Referências órfãs dentro da Árvore de Habilidades — **prioridade alta**

Ao renomear habilidades, **as que citam as renomeadas não foram atualizadas**. O documento
ficou se referindo a coisas que não existem mais:

| Onde | O que diz | Problema |
|---|---|---|
| `"Queime"` | *"aplica veneno em inimigos marcados por **sede de sangue**"* | "Sede de Sangue" virou **Morte lenta** |
| `"Dance"` | *"**Sede de sangue** agora marca todos os inimigos"* | idem |
| `Poço das moléstias` | *"deixa poças que aplicam **ácido** e corrosão"* | "Ácido" virou **Veneno** |

**Por que importa:** na hora de programar, uma habilidade que aponta para outra que não existe
vira erro. Alguém vai ter que adivinhar, e adivinhar errado custa retrabalho.

**Sugestão:** trocar "sede de sangue" por "Morte lenta" e "ácido" por "veneno" nesses 3 pontos.

## 3. Contradição entre a Demo e o GDD: inimigos respawnam ou não? — **prioridade alta**

| Documento | O que diz |
|---|---|
| **Demo** (mais recente) | *"Inimigos e loot pré determinados e fixos/**não respawna**"* |
| **GDD** | *"Transicionar de uma área para outra **reseta a sala anterior, com novos loots e Spawn de inimigos**"* |

**Adotamos a Demo**, por ser o documento mais recente. Mas o GDD precisa ser corrigido, senão
a contradição volta.

**Isso muda o desenho do jogo:** sem respawn, não dá para "moer" inimigos para juntar recursos.
Os itens de progressão (lascas de Euduroh) passam a ser **finitos na demo**, o que torna o
balanceamento muito mais sensível — faltou recurso, o jogador trava.

## 4. A Demo diz "4 side-quests" mas lista 5 — **prioridade média**

As listadas são: **Brutamonte, Xamã, Hemomante, Ferreiro e Ocultistas**. Além disso, a
**"Quest do ferreiro" está com o título escrito e o corpo vazio**.

Definir: são 4 ou 5? E qual é a do ferreiro?

## 5. "Caçador" no masculino — **prioridade baixa**

A Demo lista "Caçador" entre os jogáveis, mas a personagem é **Emi Matsunaga**, a **Caçadora**.
Provavelmente só um erro de digitação, mas vale alinhar porque o nome vira pasta e código.

## 6. Replicar o aviso no Google Drive — **prioridade alta**

O aviso de "seção desatualizada" foi escrito **só na cópia local** do GDD, dentro do repositório.
O Claude não tem acesso ao Google Drive (Regra 3), então **o documento no Drive continua com as
regras antigas**.

**Isso importa por um motivo prático:** na próxima vez que alguém exportar o GDD do Drive para
`docs/externo/`, o arquivo exportado **vai sobrescrever a cópia local** e o aviso desaparece.

**O que fazer:** um dev abre o GDD no Drive e cola o mesmo aviso nas duas seções:

> **ATENÇÃO — seção desatualizada, substituída em 12/09/2026.**
>
> **Sistema de status, condições e ações de combate:** ver o documento *Árvores de Habilidades*,
> que é a fonte oficial e mais atualizada. As regras que ficavam aqui estão desatualizadas.

## 7. Três atributos ficaram sem função — **prioridade média**

Com a remoção da ação **Conversar**, três dos onze atributos perderam o motivo de existir. O GDD
os define assim, na categoria **Mente**:

| Atributo | Definição no GDD | Situação |
|---|---|---|
| **Lábia** | *"Aumenta a chance de sucesso em Ameaçar"* | Ameaçar não existe mais |
| **Intuição** | *"Aumenta a chance de sucesso em Enganar"* | Enganar não existe mais |
| **Análise** | *"Aumenta a chance de sucesso em Expor"* | Expor não existe mais |

Também some a utilidade da perícia **Furtividade**, que segundo o GDD *"aumenta a chance de
sucesso de furto"* — embora ela continue útil por reduzir o agro inimigo.

**O que decidir:** dar uma nova função a esses três atributos (por exemplo, ligá-los a
habilidades, diálogos fora de combate ou eventos de exploração), ou removê-los da ficha do
personagem. Do jeito que está, o jogador distribui pontos em atributos que não fazem nada.

Isso afeta diretamente a tela de ficha do personagem e o balanceamento da skill tree.

## 8. Cards do Trello que já podem mudar de coluna — **prioridade baixa**

**"Preparar o ambiente de programação - Godot"** e **"...- Visual Studio Code"** estão em
*Em andamento*. O ambiente está montado, testado e rodando — podem ir para *Revisão* ou
*Concluído*.

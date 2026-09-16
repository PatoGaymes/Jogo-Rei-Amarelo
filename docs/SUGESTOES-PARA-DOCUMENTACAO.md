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
| **Efeitos de status** | Vale a lista da Árvore: Gelo, Ácido, Sangramento, Fogo, Corrosão, Raio, Escuridão, Luz |
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
- **"Criança"** → fica como está (`_SemNomeNoGDD_Crianca`) por decisão da equipe.

---

# ⚠️ PENDENTES

## 1. Replicar o aviso no Google Drive — **prioridade alta**

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

## 2. Três atributos ficaram sem função — **prioridade média**

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

## 3. Cards do Trello que já podem mudar de coluna — **prioridade baixa**

**"Preparar o ambiente de programação - Godot"** e **"...- Visual Studio Code"** estão em
*Em andamento*. O ambiente está montado, testado e rodando — podem ir para *Revisão* ou
*Concluído*.

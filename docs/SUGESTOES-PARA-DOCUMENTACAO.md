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

# ⚠️ PENDENTES

## 1. O GDD precisa ser atualizado ou marcado como desatualizado — **prioridade alta**

Ficou definido que a Árvore de Habilidades é o documento oficial do combate. Só que **o GDD
continua com as regras antigas escritas**, e ele é o documento principal do projeto — é o
primeiro lugar onde alguém novo vai procurar.

Enquanto os dois textos coexistirem, a confusão vai se repetir com a próxima pessoa que ler.

**Sugestão:** nas seções *"Status, Efeitos e Condições"* e *"Combate"* do GDD, substituir o
conteúdo pelo da Árvore de Habilidades — ou, se for mais rápido, apagar essas seções e deixar
um aviso no lugar:

> **Sistema de status, condições e ações de combate:** ver o documento *Árvores de Habilidades*,
> que é a fonte oficial e mais atualizada. As regras que ficavam aqui estão desatualizadas.

## 2. Quais ações existem no turno — **prioridade alta, ainda em aberto**

O PO respondeu **quantas** ações o personagem tem (uma por turno), mas não **quais** ações
existem. Os dois documentos continuam divergindo nisso:

| GDD | Árvore de Habilidades |
|---|---|
| Atacar, Habilidades, **Movimento**, Itens, Defender, **Conversar** | Atacar/Habilidades, Itens, Defender, **Fugir** |

**Por que isso não é detalhe:**

- **Movimento** parece continuar existindo, porque a própria Árvore tem habilidades que dependem
  de posição: *"Jogo de pés: você pode usar suas habilidades a qualquer distância"* e
  *"Avanço Tático: Avança, causa dano e empurra"*. Sem movimentação, essas habilidades não fazem
  sentido.
- **Conversar** (Enganar, Ameaçar, Furtar, Conversar, Expor) é uma mecânica grande e detalhada no
  GDD, e **três atributos existem só para servir a ela**: Lábia, Intuição e Análise. Se Conversar
  sair, esses três atributos ficam sem função.
- **Fugir** aparece só na Árvore e não existe no GDD.

**O que precisamos saber:** a lista final de ações do turno. Ela define diretamente as telas de
combate e os comandos de controle.

## 3. Arte e música sem correspondência no GDD — **prioridade baixa**

- **"Criança"** — existe PNG em personagens recrutáveis, mas não aparece no GDD. A pasta está como
  `_SemNomeNoGDD_Crianca` até alguém confirmar quem é.
- **"Marioneteira"** — existe música, mas não aparece no GDD nem entre os PNGs de chefe. Está em
  `Audio/Music/Bosses/` por suposição. Confirmar se é chefe.

## 4. Cards do Trello que já podem mudar de coluna — **prioridade baixa**

**"Preparar o ambiente de programação - Godot"** e **"...- Visual Studio Code"** estão em
*Em andamento*. O ambiente está montado, testado e rodando — podem ir para *Revisão* ou
*Concluído*.

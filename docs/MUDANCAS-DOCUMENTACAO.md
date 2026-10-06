# Mudanças aplicadas na documentação

Registro do que foi alterado nos documentos oficiais e por quê. Os arquivos prontos ficam em
`docs/para-repositorio/` (a rodada de 06/10/2026 em `docs/para-repositorio/2026-10-06/`), com a
**formatação original preservada** — o dev confere e sobe para a pasta Revisão do Drive.

Para regerar: `python docs/ferramentas/atualizar-documentacao.py` (18/09) e
`python docs/ferramentas/revisao-2026-10-06.py --gdd "<GDD baixado do Drive>"` (06/10).

---

## 06/10/2026 — decisões de combate e grafias oficiais

Base: a versão **atual do Drive** de cada documento (a Árvore está igual à de 18/09; o GDD do Drive
ganhou o atributo Iniciativa em 06/10). Arquivos corrigidos em `docs/para-repositorio/2026-10-06/`,
gerados por `python docs/ferramentas/revisao-2026-10-06.py --gdd "<GDD baixado do Drive>"`. A lista
abaixo também foi para a pasta **Pato Games/Revisão** do Drive.

**Por quê:** desde 06/10/2026 a documentação do projeto vale mais que a do Drive quando divergem (ver
a Regra 3 do `CLAUDE.md`); o que estava diferente no Drive foi corrigido para revisão.

### Árvores de Habilidades

| Onde | Antes | Agora | Motivo |
|---|---|---|---|
| Ações | "Os personagens tem 4 ações: atacar/usar habilidades, usar itens, defender, fugir" | "Os personagens têm 5 ações: atacar, usar habilidades, usar itens, defender e fugir" | As 5 ações combinadas em 12/09 |
| Ações (linhas novas, depois de "uma ação por turno") | — | Mover até 3 casas por turno sem gastar a ação · alcance contado em casas · iniciativa d20 + Iniciativa, quem tira 20 joga 2 turnos seguidos · o recurso das habilidades é o Foco | Decisões do PO em 06/10 |
| Condição Vanguarda | "a unidade com essa condição é protegida de qualquer ataque lançado a ela" | "condição de quem protege. O aliado protegido fica 'Protegido': todo o dano que ele receberia vai para a Vanguarda; em dano de área, metade e metade. Dá para proteger vários aliados, um por uso" | Decisão do PO em 06/10 |
| 14 descrições (Energia comprimida, Renovação, Manipulação de energia, Sangue grosso, Matriz encantada, Foco total, Destrinchar, Feitiço inato, Verde, Mago, Em nome do filho, Puritano, Caminhar dos anjos, Sacramento) | "recupera / gasta energia", "pontos de energia" | "recupera / gasta Foco", "pontos de Foco" | O recurso das habilidades é o Foco. Os **nomes** "Energia comprimida" e "Manipulação de energia" e o **atributo** Energia ficaram |
| Louco (Xamã) | "sanidade da cartomante" | "sanidade da xamã" | Grafia oficial |
| Título do Escudeiro | "-Escurdeiro-" | "-Escudeiro-" | Erro de digitação |

### GDD

| Onde | Antes | Agora | Motivo |
|---|---|---|---|
| Exploração | "A movimentação é de apenas para trás e frente" | "A movimentação é livre pelo mapa 3D isométrico, com a câmera girando em 8 ângulos em volta do personagem (referência: Don't Starve Together)" | O jogo virou 3D isométrico em 18/09 |
| Sumário e título | "Thao A'Bajal (Peregrino)", "Thao desde cedo" | "Tao" | Grafia oficial |
| Sumário | "Jedah, Filho de Tauron" | "Jedara" | Grafia oficial |
| Título da Amana | "Amana K'Ushim(Cartomante)" | "Amana K'Ushim (Xamã)" | Grafia oficial |

**Conferido antes de entregar:** a estrutura interna dos dois arquivos é igual à do original (estilos,
sumário, numeração), o texto antigo sumiu e o que deveria ficar (lore, personagens, regiões) está lá.

**Não mexido de propósito:** os erros de digitação dos nomes de habilidades (Pouco Espaço / Espaço
aberto, Fera / Besta vampírica, Negrosar / Necrosar, os dois "Cruel" do Desgarrado) — nomes viram
identificadores, e escolher o certo é da equipe. Ver as pendências em `SUGESTOES-PARA-DOCUMENTACAO.md`.

**A Demo e Interações não tinham nada a corrigir.** (Interações chama a Rosaria de "Abominação", e a
Demo fala em "Escudeiro e abominação"; a documentação local usa "Aberração", que é o nome da Árvore.
Ficou como pergunta, porque Interações é mais novo que a Árvore.)

---

## 18/09/2026

### Árvores de Habilidades

| Onde | Antes | Agora |
|---|---|---|
| Habilidade "Queime" | *"marcados por **sede de sangue**"* | *"marcados por **Morte lenta**"* |
| Habilidade "Dance" | *"**Sede de sangue** agora marca todos"* | *"**Morte lenta** agora marca todos"* |
| Poço das moléstias | *"poças que aplicam **ácido** e corrosão"* | *"poças que aplicam **veneno** e corrosão"* |

**Motivo:** as habilidades foram renomeadas (Ácido virou Veneno, Sede de Sangue virou Morte
lenta), mas as que as citam pelo nome ficaram para trás. Uma habilidade apontando para outra que
não existe mais vira erro na hora de programar.

### Demo

| Mudança | Motivo |
|---|---|
| **Quest do Ferreiro removida** | Saiu do escopo da demo. Era ela que fazia a conta não fechar — o texto dizia "4 side-quests" mas listava 5. Agora são 4 de fato. |
| **"Caçador" → "Caçadora"** | A personagem é Emi Matsunaga, a Caçadora. Erro de digitação. |

### GDD

| Mudança | Motivo |
|---|---|
| **Aviso de seção desatualizada** em *Combate* e *Status, Efeitos e Condições* | As regras oficiais de combate agora ficam na Árvore de Habilidades. Manter as antigas escritas aqui faz a próxima pessoa seguir regra errada. |
| **Lábia, Intuição e Análise removidas** | Só serviam à ação Conversar, que saiu do jogo. Ficariam na ficha sem fazer nada. |
| **Atributo Mente** passou a listar só Sanidade e Vontade | Consequência da remoção acima. |
| **Furtividade ganhou segundo efeito** | Além de reduzir agro em combate, define o raio em que o inimigo percebe o personagem. Liga o atributo aos perseguidores e às safe zones. |
| **Respawn corrigido** | O GDD dizia que sair da sala reseta com novo spawn. A Demo define que **não há respawn**, com a ressalva do item previsto para depois dela. |

**Conferido antes de entregar:** as 8 regiões do mundo e os 10 personagens continuam com as
mesmas contagens do original. A redução de tamanho corresponde exatamente às seções substituídas
pelo aviso — nada de lore se perdeu.

---

## Como isso é feito

Os documentos **não são gerados do zero**. O script abre o `.docx` original e mexe só no texto,
copiando estilos, tema, numeração e cabeçalho intactos — por isso a aparência continua a mesma.

Cada alteração passa por uma conferência automática que checa duas coisas: que o texto antigo
sumiu, e que o conteúdo que deveria ficar (lore, personagens, seções) continua lá. Se uma troca
não encontrar o texto esperado, ela é **ignorada e reportada**, em vez de alterar o lugar errado
em silêncio.

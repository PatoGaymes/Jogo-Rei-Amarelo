# Mudanças aplicadas na documentação

Registro do que foi alterado nos documentos oficiais e por quê. Os arquivos prontos ficam em
`docs/para-repositorio/`, com a **formatação original preservada** — é só substituir no Drive.

Para regerar: `python docs/ferramentas/atualizar-documentacao.py`

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

# Registro de alterações feitas por IA

A Regra 2 do projeto exige que toda alteração feita pelo Claude seja marcada com o
comentário `Alteração de IA - Revisar` e explicada.

Alguns arquivos **não aceitam comentário** — ou aceitam, mas o Godot os apaga ao salvar o
arquivo pelo editor. Para esses, a explicação fica registrada aqui.

Arquivos nessa situação:
- Cenas `.tscn` e recursos `.tres` — o Godot reescreve o arquivo e remove comentários
- JSON puro

---

## 09/09/2026 — Preparação do ambiente

### `ReiDoAmarrilho/Scenes/player.tscn`

**O que foi feito:** ligado o script `Player.cs` ao personagem da cena (duas linhas: uma
declarando o arquivo do script, outra ligando-o ao nó `Player`).

**Por quê:** era necessário para testar se o C# realmente funciona dentro do jogo, e não
apenas se compila. Foi o teste que revelou o problema do espaço no nome do assembly
(descrito em [AMBIENTE.md](AMBIENTE.md)).

**O que revisar:** o `Camera2D` dessa cena provavelmente deve sair — ver pendência 4 em
[AMBIENTE.md](AMBIENTE.md).

### `ReiDoAmarrilho/default_bus_layout.tres` (novo)

**O que foi feito:** criados os canais de som `Music`, `SFX`, `UI` e `Ambience`, todos
ligados ao canal principal (`Master`).

**Por quê:** separar os tipos de som é o que permite ter controles de volume independentes
no menu de opções (música baixa, efeitos altos, por exemplo). Sem essa separação, só
existe um volume geral. Num jogo no estilo Darkest Dungeon, trilha e ambientação pesam
bastante na experiência, então vale ter o controle separado desde o início.

**O que revisar:** os volumes estão todos em 0 dB (volume normal). Ajustar conforme a
mixagem do jogo for definida.

### `ReiDoAmarrilho/ReiAmarrilho.sln` (novo)

**O que foi feito:** criado o arquivo de "solução" do C#, que agrupa o projeto.

**Por quê:** normalmente o Godot cria esse arquivo sozinho ao criar o primeiro script C#.
Como ele foi criado manualmente para adiantar a validação do ambiente, fica o registro.
O identificador interno do projeto foi gerado aleatoriamente, como o Godot faria.

**O que revisar:** nada em especial — é um arquivo de estrutura, não de lógica do jogo.

### `.vscode/*.json` (novos)

Estes aceitam comentário e **estão comentados no próprio arquivo**. Registrados aqui apenas
para a lista ficar completa:

- `settings.json` — caminho do Godot, projeto C# padrão, pastas ocultas na busca
- `tasks.json` — comando de compilação
- `launch.json` — rodar o jogo com F5 e depuração
- `extensions.json` — extensões recomendadas para a equipe

**O que revisar:** o caminho do Godot em `settings.json` é o da máquina do Eric. Cada dev
precisa ajustar para o caminho da sua própria instalação.

### `.claude/settings.json` (novo)

**O que foi feito:** bloqueada a execução de `git commit`, `git push`, `git merge` e
`git rebase` pelo Claude. Liberados sem pedir confirmação: `dotnet build`,
`dotnet restore` e comandos de leitura do Git (`status`, `diff`, `log`).

**Por quê:** a Regra 1 diz que o Claude nunca deve commitar. Escrever a regra num texto
depende do Claude lembrar dela. Colocá-la aqui faz o próprio programa recusar o comando —
vira uma trava de verdade, não uma promessa. As liberações são só para reduzir a
quantidade de confirmações em comandos que não alteram nada além de compilar.

**O que revisar:** se a equipe quiser bloquear mais comandos, é só acrescentar à lista
`deny`. Este arquivo vai para o GitHub e vale para todos os devs.

---

## 11/09/2026 — Nome oficial e organização dos assets

### Renomeação para o nome oficial do jogo

**O que foi feito:** a pasta do projeto passou de `ReiDoAmarrilho` para `ContosDeOutrora`, e os
arquivos do projeto C# de `ReiAmarrilho.csproj`/`.sln` para `ContosDeOutrora.csproj`/`.sln`.
Os caminhos no VS Code foram ajustados junto.

**Por quê:** a equipe definiu o nome oficial **Contos de Outrora: O Rei De Amarelo**. O nome
completo, com espaços e dois-pontos, ficou só em `config/name`, que é o título que o jogador vê.
Pasta e programa usam a versão sem espaço porque espaço, acento e dois-pontos quebram caminhos
de arquivo e comandos de linha.

**O que revisar:** depois de dar `pull`, o VS Code pode continuar apontando para a pasta antiga.
Fechar e reabrir o projeto resolve. O repositório no GitHub continua com o nome antigo — renomear
lá é decisão e tarefa da equipe.

### `ReiAmarrilho.csproj.old` — apagado

**O que foi feito:** o arquivo foi removido.

**Por quê:** era uma cópia de segurança que o próprio Godot criou sozinho ao atualizar a versão
do Godot.NET.Sdk de 4.6.0 para 4.6.1. O conteúdo era idêntico ao arquivo atual, só com a versão
antiga. Não tinha nada que já não estivesse no arquivo em uso.

### Assets movidos e organizados por personagem

**O que foi feito:** os 37 PNGs provisórios e as 4 músicas saíram de `docs/externo/` e foram para
`ContosDeOutrora/Assets/`, cada um na pasta do seu personagem, chefe, inimigo ou NPC. Os nomes de
pasta e de arquivo foram padronizados sem espaço e sem acento.

**Por quê:** em `docs/externo/` eles eram só documentação; dentro de `Assets/` o Godot os enxerga
e eles podem ser usados no jogo. A separação por personagem evita que, com centenas de imagens,
ninguém mais ache nada. As regras completas estão em
[../ContosDeOutrora/Assets/README.md](../ContosDeOutrora/Assets/README.md).

**O que revisar:**
- A arte provisória está em `Provisorio/` dentro de cada pasta. **Apagar quando a definitiva ficar pronta.**
- O PNG **"Criança"** não corresponde a nenhum personagem do GDD. Está em
  `Characters/_SemNomeNoGDD_Crianca` até alguém confirmar quem é.
- A música **"Marioneteira"** não aparece no GDD nem entre os PNGs de chefe. Foi colocada em
  `Audio/Music/Bosses/` por suposição — confirmar se é chefe mesmo.
- As pastas de personagem usam `NomePróprio_Classe`. O GDD tem grafias conflitantes para três
  personagens (ver Bloco B das sugestões); se a equipe decidir outra grafia, as pastas mudam.

### Cache do Godot (`.godot/`) apagado e recriado

**O que foi feito:** a pasta de cache foi apagada e gerada de novo.

**Por quê:** ela guarda os caminhos dos arquivos, e todos mudaram com a renomeação. Um cache com
caminhos velhos faz o Godot reclamar de arquivos que não existem mais. Essa pasta é descartável:
o Godot a refaz sozinha, e ela nem vai para o Git.

---

## 12/09/2026 — Formatos de arquivo e nomes oficiais

### Músicas convertidas de `.wav` para `.ogg`

**O que foi feito:** `Ciclope.wav` (38,2 MB) e `Elevador.wav` (27,2 MB) viraram `.ogg` de
4,7 MB e 3,5 MB. Os `.wav` originais foram apagados da pasta do projeto.

**Por quê:** o `.wav` guarda o som sem nenhuma compressão, e o Git guarda uma cópia inteira de
cada versão de um arquivo. Cinco ajustes numa música de 38 MB virariam 190 MB no repositório,
para sempre, para todos. Foi feito antes do primeiro commit desses arquivos, então nada disso
entrou no histórico.

**O que revisar:** os originais em `.wav` continuam no Drive da equipe — nada foi perdido. Se
alguém achar que a qualidade caiu, dá para reexportar. As durações ficaram idênticas e a
diferença não é audível num jogo.

Os dois `.mp3` **não** foram convertidos: já estão comprimidos em 192 kbps, e comprimir de novo
perderia qualidade sem economizar muito.

### Imagens convertidas de `.png` para `.webp`

**O que foi feito:** as 37 imagens provisórias viraram `.webp` sem perda. No total, 4,39 MB
viraram 2,46 MB. Os `.png` foram apagados.

**Por quê:** o `.webp` sem perda guarda **exatamente** os mesmos pixels do PNG ocupando cerca
da metade do espaço. Cada imagem foi conferida uma a uma comparando os pixels antes e depois.

**O que revisar — vale saber:** em 8 das 37 imagens, a comparação acusou diferença. Ao
investigar, **todas as diferenças estavam em pontos 100% transparentes** — o WebP apaga a cor
guardada "por baixo" de pontos invisíveis, porque ela não aparece na tela. Nenhum ponto visível
mudou em nenhuma das 37 imagens. O Godot também corrige as bordas transparentes sozinho na
importação (opção `fix_alpha_border`, que já vem ligada).

### `Thao` renomeado para `Tao`

**O que foi feito:** as pastas e arquivos do Peregrino passaram de `Thao_Peregrino` para
`Tao_Peregrino`, em `Art/Characters/` e `Audio/Music/Personagens/`.

**Por quê:** o PO confirmou em 12/09/2026 que a grafia oficial é **Tao**. O GDD usa as duas
formas em trechos diferentes. As outras duas grafias confirmadas (**Xamã** e **Jedara**) já
estavam corretas nas pastas.

### Pasta `Art/Effects/` criada

**O que foi feito:** nova pasta para explosões, magias, fogo e partículas.

**Por quê:** ficou definido que personagens são animados por ossos (partes do corpo que se
movem), mas efeitos não funcionam bem assim — a forma deles muda inteira a cada quadro. Efeitos
usam sprite sheet quadro a quadro, e precisam de um lugar próprio, separado dos personagens.

---

## 12/09/2026 — Mecânicas base: câmera e movimentação

### Aviso de seção desatualizada aplicado no GDD local

**O que foi feito:** nas seções *"Combate"* e *"Status, Efeitos e Condições"* do arquivo
`docs/externo/Infos/GDD Rei de Amarelo.md`, o conteúdo antigo foi substituído por um aviso
apontando para o documento *Árvores de Habilidades*.

**Por quê:** o PO definiu que a Árvore de Habilidades é a fonte oficial do combate, mas o GDD
continuava com as regras antigas escritas. Como ele é o documento principal, a confusão se
repetiria com a próxima pessoa que o lesse.

**O que revisar — importante:** isso foi feito **só na cópia local**, dentro do repositório. O
documento no Google Drive **continua com as regras antigas**. Na próxima vez que alguém exportar
o GDD do Drive, o arquivo exportado sobrescreve esta cópia e o aviso some. O texto para colar no
Drive está em [SUGESTOES-PARA-DOCUMENTACAO.md](SUGESTOES-PARA-DOCUMENTACAO.md), pendência 1.

### `Scenes/player.tscn` substituída

**O que foi feito:** a cena de teste de ambiente foi apagada e no lugar entraram duas cenas:
`Scenes/Characters/Player.tscn` (o personagem) e `Scenes/Levels/CorredorTeste.tscn` (o cenário).

**Por quê:** a cena antiga era só um teste para confirmar que o C# rodava, usando o ícone do
Godot como imagem. Separar o personagem do cenário permite reaproveitar o mesmo personagem em
vários corredores, sem copiar e colar.

**O que revisar:** a cena do personagem usa a **arte provisória da Khalid** como placeholder, e
o cenário são formas coloridas simples. Ambos saem quando a arte real chegar.

### Cena inicial do jogo alterada

**O que foi feito:** a configuração `run/main_scene` passou a apontar para o corredor de teste.

**Por quê:** antes o jogo abria direto no personagem solto, sem chão nem cenário. Agora abre num
corredor onde dá para conferir a movimentação e a câmera funcionando juntas.
**Quando existir menu inicial, trocar para a cena do menu.**

### Personagem colocado no grupo "player"

**O que foi feito:** a cena do personagem foi marcada com o grupo `player`.

**Por quê:** é assim que a câmera acha o personagem sozinha, sem depender do caminho dele dentro
da cena. Se alguém renomear ou mover os nós do corredor, a câmera continua funcionando.
**Todo personagem controlado pelo jogador precisa estar nesse grupo.**

### Enquadramento da câmera ajustado

**O que foi feito:** a câmera recebeu aproximação (zoom 1.6) e teve a altura travada em 600.

**Por quê:** no primeiro teste o personagem aparecia pequeno no canto inferior, com muito espaço
vazio em cima. Com a aproximação, ele ocupa a tela de forma parecida com o Darkest Dungeon.

**O que revisar:** esses dois valores definem o enquadramento e **precisam ser conferidos a olho
junto com a arte real do cenário**. O espaço que sobra em cima é onde entra a arte de teto e
arquitetura.


---

## 18/09/2026 — Ferramentas e virada para 3D isométrico

### Documentos sincronizados com o repositório

**O que foi feito:** o `Árvores de Habilidades.md` local foi regerado a partir da versão do
repositório, que estava mais nova. Criado `docs/ferramentas/sincronizar-docs.py` para conferir
isso automaticamente daqui em diante.

**Por quê:** a equipe mantém a documentação em dois lugares. Sem conferência, uma decisão tomada
num lugar se perde no outro. O script roda na máquina e devolve só o resumo das diferenças.

**O que revisar:** as mudanças foram Ácido → Veneno, nova condição Espinhos, e três habilidades
da Caçadora renomeadas. **O documento do repositório tem referências órfãs** — habilidades que
citam outras pelo nome antigo. Está na pendência 1 das sugestões.

### `.mcp.json` (novo) e ferramentas instaladas

**O que foi feito:** instalados o `codebase-memory-mcp` (indexa o projeto para eu achar as coisas
sem ler arquivo por arquivo) e o `godot-mcp` (me deixa enxergar o projeto pelo próprio Godot).
Também o plugin de skills do mattpocock, e os programas `fd` e `jq`.

**Por quê:** reduzem a quantidade de arquivo que preciso ler para responder, o que economiza a
cota de uso. O binário do codebase-memory foi baixado do GitHub e **conferido pelo checksum
oficial** antes de instalar, em vez de rodar o script de instalação direto da internet.

**O que revisar:** o `godot-mcp` não é atualizado desde fevereiro. **Foi testado com o Godot
4.6.1 e funcionou**, mas se um dia der problema, é o primeiro a desconfiar — basta apagar a
entrada "godot" do `.mcp.json`.

### `.claudeignore` (novo)

**O que foi feito:** lista de arquivos que eu devo ignorar ao procurar coisas no projeto.

**Por quê:** pastas geradas automaticamente e arquivos duplicados (os `.docx` que já têm `.md`).
Ler isso gasta a cota de uso sem trazer nada útil.

### Código 2D apagado, base 3D no lugar

**O que foi feito:** `Player.cs`, `CameraCorredor.cs` e as duas cenas 2D foram apagados. No lugar
entraram `CameraIsometrica.cs`, `PlayerIsometrico.cs`, a nova `Player.tscn` (3D) e `MapaTeste.tscn`.

**Por quê:** o jogo deixou de ser 2D lateral e passou a ser 3D isométrico no estilo Don't Starve
Together. Câmera, movimentação e colisão mudaram por completo — não havia o que aproveitar.
O histórico do Git preserva o código antigo.

**O que revisar:** o personagem usa a arte provisória da Khalid, e o cenário são caixas coloridas.
Tudo isso sai quando a arte real chegar.

### Comandos do teclado reorganizados

**O que foi feito:** adicionados `move_forward` (W) e `move_back` (S), e as ações de girar a
câmera com Q e E. **A tecla de interagir mudou de E para F.**

**Por quê:** o personagem agora anda nas quatro direções, e não só para os lados. O E precisou
ser liberado porque passou a girar a câmera.

**O que revisar:** quem já estava acostumado com o E para interagir precisa saber da mudança.

### Erro de sinal encontrado no movimento

**O que foi feito:** corrigido o sentido da rotação que converte o comando do teclado em
movimento no mundo.

**Por quê:** no primeiro teste, o W funcionava com a câmera ao norte ou ao sul, mas **invertia**
quando ela estava a leste ou a oeste — o personagem andava na direção da câmera em vez de para
longe dela. Só apareceu porque o teste checou os 8 ângulos, e não apenas o inicial.

**O que revisar:** nada pendente — os 8 ângulos foram testados e todos passaram.


---

## 18/09/2026 (tarde) — Decisões do PO aplicadas

### NPC "Bufão Alegre" renomeado para `Abanur_Bufao`

**O que foi feito:** a pasta e a imagem em `Assets/Art/Npcs/` mudaram de nome.

**Por quê:** o PO confirmou que o "Bufão Alegre" **é o Abanur**, o deus da morte e das artes que
narra o jogo. Eram a mesma pessoa com dois nomes, o que confundiria quem fosse produzir a arte.

**O que revisar:** a arte provisória continua a mesma, só o nome mudou.

### Atributos removidos da ficha do personagem

**O que foi feito:** **Lábia, Intuição e Análise** saíram. A ficha passou de 11 para 8 atributos.
A **Furtividade** ganhou um segundo efeito.

**Por quê:** os três só serviam à ação Conversar, que foi removida do jogo. Ficariam na ficha sem
fazer nada, e o jogador gastaria pontos à toa.

A Furtividade agora também define **a que distância o inimigo percebe o personagem** — quanto
maior, menor o raio de agro. Isso a liga aos perseguidores e às safe zones da demo.

**O que revisar:** o raio de agro ainda **não existe no código** — é só decisão de design por
enquanto. Entra quando o sistema de inimigos for feito.

### Quest do Ferreiro retirada da demo

**O que foi feito:** as menções foram removidas dos documentos.

**Por quê:** decisão do PO. Era essa quest que fazia a conta não fechar — o documento dizia
"4 side-quests" mas listava 5, e a do Ferreiro estava sem conteúdo.

### `docs/para-revisao/` (nova pasta)

**O que foi feito:** três `.docx` prontos para o analista revisar e colar no Drive, mais o script
`gerar-docx-revisao.py` que os regera.

**Por quê:** o Claude não tem acesso ao Google Drive (Regra 3), então tudo que ele corrige aqui
precisava ser repassado à mão. Agora sai um arquivo pronto, e **cada um começa com uma tabela do
que mudou e por quê** — o analista revisa só o que é novo.

**O que revisar:** os `.docx` são **gerados**, não editados à mão. Quem manda é o `.md` ao lado.
Se alguém editar o `.docx` direto, a alteração se perde na próxima geração.

### pandoc instalado

**O que foi feito:** instalado o pandoc (`winget install JohnMacFarlane.Pandoc`).

**Por quê:** converte entre `.docx` e `.md` nos dois sentidos. Serve tanto para gerar os arquivos
de revisão quanto para ler os `.docx` que a equipe exporta do Drive — antes isso era feito
descompactando o arquivo na mão.


### `docs/para-repositorio/` (nova pasta)

**O que foi feito:** os mesmos três documentos, agora **completos e sem a tabela de mudanças** —
prontos para substituir os oficiais no Google Drive.

**Por quê:** a tabela "o que mudou" serve para a revisão, mas não deve entrar no documento oficial.
São duas finalidades diferentes, então viraram duas pastas. A fonte continua sendo um arquivo só:
o `.md` em `docs/para-revisao/`. A versão limpa é gerada a partir dele.

**O que revisar — foi conferido antes de gerar:** comparei os documentos novos com os originais
para garantir que nada se perdeu. O GDD ficou **3.554 caracteres menor**, e a conta fecha: as
seções de Combate (1.830) e Status (2.212) foram substituídas pelo aviso (~490). Toda a lore de
mundo e de personagens continua presente — foi conferida item por item.

**Atenção:** os `.docx` são gerados, não editados. Quem editar o `.docx` direto perde a alteração
na próxima vez que o script rodar.


---

## 18/09/2026 (noite) — Sistema de detecção dos inimigos

### Camadas de colisão 3D nomeadas

**O que foi feito:** as camadas de colisão 3D ganharam nome (Player, Enemy, Interactable, Wall,
Trigger). Só as 2D tinham.

**Por quê:** o jogo virou 3D e as camadas 3D estavam sem nome. A camada **Parede** é a mais
importante: é ela que o sistema de detecção usa para saber que o inimigo não enxerga através de
um muro.

### Cenas e scripts novos

`SensorDeteccao.cs`, `InimigoIA.cs`, `RotaRonda.cs`, `DebugDeteccao.cs`, `GerarNavegacao.cs`,
`Inimigo.tscn` e `SalaTeste.tscn`. A `MapaTeste.tscn` antiga foi apagada — a sala nova faz o
mesmo e ainda tem paredes.

**O que revisar:** o inimigo usa a arte provisória do Rasgador; a sala são caixas coloridas.

### A tecla de interagir continua sendo F

A ação `debug_raios` foi para o **F3**, sem conflito com as outras teclas.

### Dois erros encontrados durante os testes

**1. O mapa de caminhos não era gerado.** O Godot avisou que, com o jogo rodando, o cálculo não
pode partir das imagens do cenário — precisa partir das formas de colisão. Corrigido apontando a
fonte para as formas de colisão da camada Parede.

**2. O teste não conseguia chamar a checagem de parede.** O método usava um tipo de número que o
Godot não expõe para fora do C#, então o teste automatizado não alcançava a função. Trocado por um
número comum — o que também deixou a camada configurável, caso um dia uma grade precise deixar ver
através.

**O que revisar:** os dois foram pegos pelos testes, não em produção. Vale manter o hábito.


---

## 18/09/2026 (noite, parte 2) — Reação ao barulho

### Novo modo "Ouviu algo" entre a ronda e a busca

**O que foi feito:** o contato dos círculos de detecção não manda mais o inimigo direto para a
busca. Agora ele entra num estado intermediário em que para, mostra um "?" e olha em volta antes
de decidir.

**Por quê:** do jeito anterior, o inimigo virava para o jogador no mesmo instante em que os
círculos se tocavam — o cone de visão caía em cima dele e a perseguição começava sem nenhuma
chance de reação. Não dava para jogar de furtividade.

**O que revisar:** o tempo de cada etapa (`TempoDeCadaEtapaDoAlerta`, padrão 0,9 s) é **o número
mais importante do sistema de furtividade**. São quatro etapas, então o total é cerca de 4
segundos — a janela que o jogador tem para se esconder. Vale testar jogando antes de fechar.

### A cabeça passou a girar aos poucos

**O que foi feito:** o inimigo agora tem uma "direção desejada" e gira até ela, em vez de mudar
o olhar de um quadro para o outro.

**Por quê:** girar instantaneamente entrega a posição do jogador no mesmo instante do barulho.
Girando devagar, o cone varre o ambiente de forma visível e dá para reagir.

**O que revisar:** a velocidade (`VelocidadeDeVirar`) afeta tanto a reação ao barulho quanto o
andar normal na ronda.

### `BalaoAviso.cs` (novo)

**O que foi feito:** o balãozinho com "?" e "!" sobre a cabeça do inimigo.

**Por quê:** sem ele, o jogador não tem como saber que foi ouvido. É o mesmo recurso de Metal Gear
e Assassin's Creed: o aviso é o que transforma a situação em algo jogável em vez de injusto.

**O que revisar — importante:** isto **não é ferramenta de teste**. Diferente do desenho dos raios
(tecla F3), o balão faz parte do jogo e continua ligado na versão publicada. Quando houver arte,
o Label3D pode virar um sprite sem mexer na lógica.

---

## 18/09/2026 (noite, parte 3) — Barra de suspeita

### `MedidorDeSuspeita.cs` (novo) e o nó novo em `Inimigo.tscn`

**O que foi feito:** a percepção deixou de ser um interruptor e virou uma barra de 0% a 100%.
Todos os alcances (ouvir, chegar perto, ver) alimentam a mesma barra; aos 10% o inimigo estranha,
aos 100% ele reconhece o jogador. Foi acrescentado o nó `MedidorDeSuspeita` na cena do inimigo —
como `.tscn` não aceita comentário, fica registrado aqui.

**Por quê:** do jeito anterior, bastava o jogador entrar e sair do alcance de propósito para o
inimigo ficar preso parando e olhando em volta, sem nunca voltar à ronda. Era uma trava que o
jogador aprenderia a explorar. Com a barra, o entra-e-sai acumula em vez de reiniciar.

**O que revisar:** os valores de partida são `VelocidadeAoOuvir = 20`, `TetoAoOuvir = 50`,
`VelocidadeAoChegarPerto = 70`, `VelocidadeDeEsquecer = 8`, `EsperaParaEsquecer = 1,5 s` e
`LimiteParaEstranhar = 10`. São chutes iniciais, feitos para serem ajustados jogando — o próprio
PO já pediu para olhar os números depois.

### Por ouvir, a barra para em 50%

**O que foi feito:** o contato dos círculos externos enche a barra só até a metade, por mais tempo
que dure.

**Por quê:** som não identifica ninguém. Para completar a detecção, o inimigo precisa chegar perto
ou enxergar de fato. É o que garante que dá para atravessar uma sala sem ser pego, desde que não
se entre no campo de visão.

**O que revisar:** o teto é por inimigo (`TetoAoOuvir`). Um inimigo com audição muito apurada
poderia ter um teto maior — é uma alavanca de design que já está pronta para uso.

### A suspeita acumulada acelera o reconhecimento

**O que foi feito:** quanto mais cheia a barra, mais rápido ela sobe (`AceleracaoPelaSuspeita`).

**Por quê:** quem já ouviu barulho não precisa de um olhar demorado para confirmar. Medido: 0,85 s
com a barra zerada contra 0,47 s com a barra na metade.

**O que revisar:** em 0, o inimigo não aproveita nada do que ouviu antes. Em valores altos, a
segunda chance do jogador praticamente some. 1,0 é um meio-termo de partida.

### A reação de espanto não se repete enquanto ele não se acalmar

**O que foi feito:** o "?" e a sequência de olhar em volta só disparam quando a barra **cruza** os
10% de baixo para cima. Enquanto ela ficar acima disso, não disparam de novo.

**Por quê:** é a proteção contra travar o inimigo. Ele segue a ronda desconfiado em vez de ficar
parado olhando em volta sem parar.

**O que revisar:** a consequência é que um inimigo que já estranhou não estranha de novo tão cedo —
ele precisa esquecer primeiro (cerca de 6,5 s longe, com os valores atuais). Se na prática parecer
que ele "ignora" o jogador, o caminho é aumentar `VelocidadeDeEsquecer`, não voltar ao interruptor.

### O balão passou a seguir a barra

**O que foi feito:** o "?" e o "!" eram ligados e desligados em vários pontos do código; agora há
um lugar só (`AtualizarBalao`), que olha para a barra e para o modo.

**Por quê:** era fácil esquecer um dos pontos e deixar o balão preso na tela.

**O que revisar:** o "?" agora aparece a partir de 10% de suspeita **mesmo quando o inimigo está
vendo o jogador de longe** — antes só aparecia ao ouvir. Se a equipe preferir o "?" restrito ao
barulho, é uma linha em `AtualizarBalao`.

### `RotaRonda.cs` e `SalaTeste.tscn` — o inimigo não saía do lugar

**O que foi feito:** o `RotaRonda` ganhou o campo `CaminhoDaFase`, que aponta para um nó do mapa
cujos filhos são os pontos da ronda. Na sala de teste ele foi apontado para o `MarcasDaRonda`, que
é o nó das quatro marcas verdes que já estavam no chão.

**Por quê:** as marcas verdes eram só enfeite — nada as ligava ao inimigo, e o `RotaRonda` só lia os
**próprios nós filhos**. Na prática o inimigo ficava parado no canto da sala a partida inteira, e o
jogo soltava um aviso a cada início. Sem ronda, a sala de teste não mostra a mecânica: não dá para
ver um inimigo em patrulha ouvir o jogador e se assustar.

**O que revisar:** o campo é um **caminho em texto**, e não uma ligação arrastada para o nó. Foi
preciso assim porque o inimigo é uma cena montada à parte: a ligação direta feita de dentro da fase
não chegava a ser resolvida e o campo chegava vazio. Quem for montar as fases da demo aponta o
`CaminhoDaFase` de cada inimigo para o nó de marcas da sala dele.

Continua em aberto, do trabalho anterior: **o estilo da ronda** (`Circuito`, dando voltas, ou
`VaiEVolta`, refazendo o caminho de trás para frente) para as minas da demo. Hoje está em
`Circuito`.

### O inimigo parava em cima do jogador e não saía mais dali

**O que foi feito:** ao chegar na distância de encontro, ele agora **para e encara** o jogador, em
vez de continuar andando até a distância zero. E o aviso `CombateDeveComecar` passou a ser emitido
**uma vez por encontro**, não a cada quadro.

**Por quê:** dois defeitos juntos. Ele perseguia até atravessar o jogador e parava por cima dele,
o que na tela parecia um travamento. E o aviso de combate saía 60 vezes por segundo — quando
alguém passar a escutá-lo, o combate seria iniciado sem parar.

**O que revisar — importante:** enquanto o combate por turnos não existir, o inimigo **fica parado
encarando o jogador** depois de alcançá-lo. Isso **não é travamento**, é o ponto de entrega para o
sistema de combate: ele chegou ao alcance da arma e está esperando. Confirmado que ele sai desse
estado sozinho — se o jogador corre, ele volta a perseguir; se o jogador some, ele desiste e
retoma a ronda.

A distância em que ele para vem do `RaioEncontro` do inimigo somado ao raio interno do jogador —
hoje 1,8 + 2,5 = 4,3 m para parar, e ele freia por volta de 3,5 m. É a alavanca para diferenciar
um inimigo de arco de um de espada.

---

## 18/09/2026 (noite, parte 4) — O inimigo não andava

Três causas diferentes, empilhadas. As três estavam presentes ao mesmo tempo, e cada uma sozinha
já deixava o inimigo plantado no lugar.

### 1. A malha de navegação flutua meio metro acima do chão

**O que foi feito:** o inimigo agora **mede**, ao entrar na fase, a que altura fica o chão de
navegação em relação aos pés dele, e avisa essa diferença ao sistema de navegação
(`AlinharAlturaDaNavegacao`).

**Por quê:** a malha gerada não fica em cima do chão — fica em `y = 0,500`, enquanto o inimigo
pisa em `y = 0,000`. O sistema decide que ele chegou a um ponto do trajeto medindo a distância
**em três dimensões**, e essa distância nunca ficava abaixo do meio metro exigido, porque a
diferença de altura sozinha já valia meio metro exato. O trajeto era calculado certinho e nunca
avançava do primeiro ponto. Empate na casa decimal.

**O que revisar:** a diferença é **medida no próprio mapa**, não escrita à mão, para continuar
valendo se as fases mudarem. A medição é feita **uma vez só**, quando o inimigo pousa no chão.
Fases com andares em alturas diferentes vão precisar medir de novo a cada andar — vale lembrar
disso ao montar as minas em vários níveis.

### 2. O trajeto era refeito a cada quadro ao perseguir

**O que foi feito:** o destino só é reenviado ao sistema de navegação quando o alvo já se afastou
`DistanciaParaRefazerOTrajeto` (meio metro) do destino anterior.

**Por quê:** perseguir alguém que anda mandava um destino novo a cada quadro, e cada destino novo
refaz o caminho inteiro do zero. O "próximo ponto do trajeto" acabava caindo **atrás** do inimigo:
ele dava um passo, passava do ponto, voltava, e ficava tremendo no lugar. Medido antes da
correção: **0,27 m percorridos em 6 segundos**.

**O que revisar:** meio metro é curto o bastante para a perseguição continuar convincente e longo
o bastante para o caminho durar alguns quadros. Se o inimigo parecer "atrasado" ao seguir o
jogador, este é o número a diminuir.

### 3. Ele passava do ponto e voltava

**O que foi feito:** o passo de cada quadro é limitado à distância que realmente falta até o
próximo ponto do trajeto. E "ficar parado" deixou de ser feito mandando o inimigo ir até a
**própria posição** — agora existe um estado de "sem destino".

**Por quê:** a velocidade cheia fazia ele passar do ponto quando o ponto estava perto. E mandar
alguém ir até onde ele já está é um destino novo a cada quadro, porque a posição muda um
pouquinho sempre — era outra porta de entrada para a mesma vibração.

### Aviso de processo — o editor do Godot desfaz alterações em `.tscn`

**Aconteceu duas vezes hoje:** o arquivo `SalaTeste.tscn` foi alterado em disco e, minutos depois,
o editor do Godot salvou por cima com a versão que tinha em memória, apagando a alteração sem
avisar. Foi por isso que a ronda "voltou a não funcionar" depois de corrigida.

**Como evitar:** com o editor aberto numa cena, ele reescreve o arquivo ao salvar ou ao rodar o
jogo. Depois de qualquer alteração feita fora do editor, usar **Scene → Reload Saved Scene** antes
de mexer na cena, ou manter o editor fechado enquanto as alterações são feitas.

---

## 18/09/2026 (noite, parte 5) — A ronda achada por combinação de nome

### `RotaRonda.cs` — a ligação saiu da cena e foi para o código

**O que foi feito:** o `RotaRonda` agora procura sozinho um nó da fase chamado **`MarcasDaRonda`**
e usa os filhos dele como pontos da ronda. Nada precisa ser ligado na cena. A ordem de prioridade
é: (1) o nó indicado à mão em `CaminhoDaFase`, (2) os próprios nós filhos, (3) a procura pelo nome.

**Por quê:** a correção anterior dependia de uma propriedade gravada em `SalaTeste.tscn`, e essa
propriedade foi **apagada três vezes** pelo editor do Godot. O arquivo voltava sempre ao mesmo
tamanho (7033 bytes), inclusive depois de fechar o editor, reabrir e usar *Reload Saved Scene*.
A explicação mais provável é que o editor descarta propriedades que ele ainda não conhece — o
código havia mudado, mas o editor estava com a versão anterior carregada — e regrava o arquivo sem
elas, sem avisar nada.

Tirar a ligação da cena e colocá-la no código resolve de vez: não há o que perder ao salvar.

**O que revisar:** **a combinação é o nome `MarcasDaRonda`.** Ao montar as salas da demo, o nó com
os pontos da ronda deve se chamar assim para o inimigo achar sozinho. Salas com **mais de uma
rota** precisam indicar cada uma à mão pelo `CaminhoDaFase`, que continua tendo prioridade. O nome
procurado é ajustável por inimigo em `NomeDoCaminhoNaFase`.

Verificado com o arquivo da cena **exatamente como o editor o deixou**, sem nenhuma alteração: 4
pontos encontrados e o inimigo andando desde o primeiro instante (10,26 m nos primeiros 5
segundos), tanto na execução sem tela quanto com janela.


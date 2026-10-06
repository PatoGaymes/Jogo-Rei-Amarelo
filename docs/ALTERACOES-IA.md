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

---

## 21/09/2026 — Mecânica de agachar

### `SensorDeteccao.cs` — dois efeitos novos

**O que foi feito:** o sensor ganhou o estado `Agachado` e dois números:
`FatorDosRaiosAgachado` (0,55) encolhe os círculos de detecção, e `AumentoDoTempoAgachado` (1,8)
aumenta o tempo necessário para o personagem ser reconhecido dentro do campo de visão.

**Por quê:** era o pedido — agachar diminui todo o alcance de detecção do jogador e, mesmo quando
ele está dentro das regras de detecção, o inimigo demora mais para perceber.

**O que revisar:** os dois números são de partida e valem por personagem. O estado mora no
**sensor**, e não no script do jogador, para que um inimigo furtivo também possa se agachar sem
nenhum código novo.

Medido: raios de 5,00 → 2,75 m; tempo de 1,20 → 2,16 s; distância em que os círculos se tocam de
10,00 → 7,75 m; e, na prática, dentro do cone de visão a 9 m, perseguição em 0,85 s em pé contra
1,52 s agachado.

### O alcance da própria visão **não** encolhe

**O que foi feito:** `AlcanceVisaoEfetivo` é o único dos três alcances que não leva o fator de
agachar.

**Por quê:** agachar esconde o personagem, não cega ele. Quem se abaixa continua enxergando o
mesmo tanto à frente.

### A altura dos olhos abaixa

**O que foi feito:** agachado, a checagem de parede sai de 0,80 m em vez de 1,50 m.

**O que revisar:** **hoje isso não muda nada**, porque as paredes da sala de teste vão do chão ao
teto. Foi incluído porque é o que vai permitir se esconder atrás de mureta, caixa ou parapeito —
assunto que o PO já sinalizou que quer detalhar depois.

### `PlayerIsometrico.cs` — a tecla, o preço e o desenho

**O que foi feito:** segurar `Ctrl` (ou `C`) agacha. Agachado, o personagem anda a 45% da
velocidade, e o desenho abaixa e achata um pouco, de forma gradual.

**Por quê — importante, é decisão de design:** a penalidade de velocidade **não foi pedida**. Foi
incluída porque sem um preço agachar seria sempre melhor que andar em pé, e o jogador passaria o
jogo inteiro agachado — a mecânica viraria uma tecla obrigatória em vez de uma escolha. Em
`FatorDeVelocidadeAgachado = 1.0` a penalidade some.

**O que revisar:** é **segurar**, não liga/desliga, para o jogador não esquecer que está agachado.
Para virar liga/desliga, é trocar `IsActionPressed` por `IsActionJustPressed` e inverter o valor.
O abaixamento do desenho é só aparência — quando houver arte de agachado, vira troca de imagem sem
mexer na mecânica.

### `project.godot` — ação `agachar`

**O que foi feito:** ação nova ligada a **Ctrl** e a **C**, as duas como segurar. Como `.godot`
não aceita comentário, fica registrado aqui.

**O que revisar:** duas teclas de propósito — Ctrl é o costume do gênero, e C fica de reserva para
quem achar Ctrl desconfortável junto com o WASD. Verificado que as duas funcionam.

---

## 21/09/2026 (parte 2) — Mira no mouse e névoa de guerra

### `project.godot` — a tecla Ctrl de agachar não funcionava

**O que foi feito:** a ação `agachar` passou a ter **três** eventos: Ctrl com o modificador
marcado, Ctrl sem ele, e C.

**Por quê:** o Ctrl é um modificador, e não dá para saber de fora se o Godot marca ou não o
modificador no evento da própria tecla Ctrl. Se a ação exigir a marcação e o evento não trouxer,
a tecla nunca funciona; se for o contrário, ela funciona ao apertar mas não ao soltar, e o
personagem fica agachado para sempre. Registrar as duas variações cobre os dois casos.

**O que revisar — honestidade sobre o teste anterior:** o teste que fiz antes **passou sem
provar nada**. Eu construí o evento de teclado com a mesma marcação que tinha escrito na ação, de
modo que ele só confirmou que a ação combina consigo mesma. Como não dá para apertar uma tecla de
verdade num teste automatizado, este caso precisa ser confirmado jogando.

### `PlayerIsometrico.cs` — segurar ou alternar

**O que foi feito:** `ModoDeAgachar` aceita `Segurar` (padrão) ou `Alternar`.

**Por quê:** foi pedido que a escolha ficasse disponível para o **módulo de configurações** que a
equipe vai fazer. Deixando as duas prontas, aquele módulo só vai precisar escrever neste campo.

### A visão do personagem segue o cursor do mouse

**O que foi feito:** o personagem vira para onde o cursor aponta no chão. A câmera **não** gira
junto: continua nos 8 ângulos fixos de Q e E.

**Por quê:** separa "para onde ele anda" de "para onde ele olha". Sem isso a névoa seria
injogável, porque o jogador não teria como escolher o que vigiar enquanto se desloca.

**O que revisar:** verificado nos 8 ângulos da câmera, com o cursor sobre um ponto fixo do mapa —
o personagem encara esse ponto em todos, com erro máximo de 0,2°. Em `ApontarComOMouse = false`
volta o comportamento anterior.

### `SensorDeteccao.cs` — três níveis no cone e duas faixas de visão passiva

**O que foi feito:** o cone ganhou três faixas (claro, embaçado, vulto) e o personagem ganhou duas
faixas de visão em volta que não dependem de para onde ele olha. O alcance do jogador subiu de 10
para 16 m.

**O que revisar — duas decisões:**

1. **A numeração do cone é ao contrário da numeração da detecção.** No cone, nível 1 é o mais
   perto; nos raios de detecção, nível 1 é o de fora. Foi assim que a equipe descreveu cada um.
   É o tipo de coisa que confunde na leitura do código — está avisado nos comentários dos dois.
2. **Os alcances da visão são números próprios**, e não os raios de detecção. Ver e ser visto são
   coisas diferentes, e os raios de detecção encolhem ao agachar: se fossem os mesmos, agachar
   cegaria o jogador, o contrário do que foi combinado.

### `NevoaDeGuerra.cs` + `Shaders/NevoaDeGuerra.gdshader` (novos)

**O que foi feito:** o mundo fica coberto de névoa e ela abre só onde o personagem enxerga.
Funciona em cima da imagem pronta: para cada ponto da tela, descobre a que lugar do mundo ele
corresponde e pergunta se o personagem vê aquele lugar.

**Por quê:** num mapa 3D visto de cima, a câmera entrega o que está atrás das paredes e o mapa
inteiro se revela de graça.

**O que revisar:** a **cor da névoa é o que separa neblina de escuridão** — a mesma mecânica serve
para iluminação, mudando só a cor. A névoa se cria sozinha na câmera, em código, porque alterações
em arquivos de cena já se perderam três vezes por serem regravadas pelo editor.

### A parede esconde o que está atrás — medida com linhas imaginárias

**O que foi feito:** a cada passo da física, 128 linhas saem do personagem em roda e medem a que
distância está a parede em cada direção. Essas distâncias vão para o desenho da tela numa tira de
imagem, e a névoa respeita esse contorno.

**Por quê:** sem isto a névoa limpava tudo que estivesse perto e dentro do cone, **inclusive o
cômodo do outro lado do muro** — que é exatamente o problema que ela veio resolver. Verificado:
um inimigo a 11 m, dentro do cone de visão, atrás de uma divisória, fica invisível.

**O que revisar:** `QuantidadeDeRaios` (128) é o equilíbrio entre contorno fiel e custo; menos
linhas deixam os cantos serrilhados. `FolgaDaParede` (0,8 m) existe para a própria parede
continuar aparecendo — sem folga, a face dela ficaria escura e pareceria defeito.

### `VisibilidadeDoInimigo.cs` (novo)

**O que foi feito:** cada inimigo decide como aparece conforme o quanto o jogador o enxerga:
normal, apagado, vulto escuro, ou escondido.

**Por quê:** a névoa escurece o cenário por igual, mas um inimigo não pode escurecer junto — no
limite do alcance o jogador tem que **perceber que tem alguém ali sem saber quem é**. Esse vulto é
informação de jogo, não enfeite.

**O que revisar:** o interruptor `IgnoraANevoa` já está pronto para os **inimigos que a névoa não
afeta**, que a equipe avisou que vão existir. O componente é criado em código junto com o inimigo,
pelo mesmo motivo do medidor de suspeita.

---

## 21/09/2026 (parte 3) — Sandbox, arte do Desgarrado e memória visual

### `Sandbox.tscn` (nova) — o mapa de testes cresceu

**O que foi feito:** mapa novo de **70 x 70 m** (o anterior tinha 30 x 30), com uma casa de dois
andares, rampa externa subindo até o segundo piso, caixas espalhadas, duas divisórias soltas, e
**dois inimigos**: um em ronda por cinco pontos e outro parado em Idle dentro da casa. Virou a
cena principal do projeto.

**Por quê: cena nova em vez de alterar a antiga.** A `SalaTeste.tscn` já foi regravada três vezes
pelo editor do Godot, desfazendo alterações. Um arquivo que o editor nunca abriu não tem cópia
velha em memória para sobrescrever. A `SalaTeste.tscn` continua onde estava, sem uso — pode ser
apagada quando a equipe quiser.

**O que revisar:** a rampa sobe 3,40 m em 9,34 m (20°), por fora da parede leste, e chega num vão
do parapeito. Verificado que o jogador sobe até o segundo piso. A casa é feita de caixas sem
telhado, de propósito: com telhado a câmera de cima não mostraria nada do interior.

### `InimigoIA.cs` — escolher o modo inicial

**O que foi feito:** campo `ModoInicial`. Antes todo inimigo começava rondando e o modo Idle
existia no código mas não dava para escolher pela cena.

**O que revisar:** um inimigo que **só** fica parado precisa de `ModoInicial = Idle`,
`ChanceDeIdlePorSegundo = 0` e `TempoEmIdle = 0`. Se levar um susto ele sai do Idle e passa a
rondar, o que é proposital.

### Animação de andar do Desgarrado — 96 quadros inseridos

**O que foi feito:** os 96 arquivos entregues em `inserir/` foram recortados, renomeados e movidos
para `Assets/Art/Characters/Uzhan_Desgarrado/Map/Andar/{Frente,Costas,Lado}/`, 32 quadros cada.

**Por quê o recorte:** cada quadro vinha numa tela de 1850 x 910 com o personagem ocupando
539 x 399 no meio — **87% da imagem era vazio**. Em disco isso quase não pesa (o vazio comprime),
mas **bagunça a posição do personagem no jogo**, porque o motor centraliza a imagem inteira. Todos
os 96 quadros foram recortados com a **mesma moldura**, senão a animação trocaria de
enquadramento a cada quadro e o personagem tremeria.

**O que revisar — divergência consciente com o padrão:** o `Assets/README.md` manda animar
personagem **por ossos**, e isso continua valendo para o combate. Andar no mapa quadro a quadro
está certo neste caso (personagem visto de longe, direção resolvida trocando a imagem — a técnica
do Doom), e o README foi atualizado explicando a exceção. **Se a equipe discordar, é aqui que se
discute.**

### `inserir/` — caixa de entrada documentada

**O que foi feito:** a pasta ganhou um `README.md` explicando o que largar ali, o que acontece com
cada arquivo e o que não entra. Apontada também no `CLAUDE.md` e no `Assets/README.md`.

### Memória visual — o cenário fica, o inimigo desbota

**O que foi feito:** duas memórias diferentes, porque são duas coisas diferentes.

**Cenário (`NevoaDeGuerra`):** uma grade sobre o mapa anota tudo o que o personagem já viu. Lugar
já visto continua aparecendo, **apagado e sem cor**, mesmo de costas. Nunca é esquecido.

**Inimigo (`VisibilidadeDoInimigo`):** ao sair de vista, fica um **borrão** no último lugar onde
foi visto, que **envelhece e some em 5 segundos** — e some **na hora** se o jogador olhar para lá
e não achar ninguém.

**Por quê separado:** casa não anda, então saber onde ela está nunca fica errado. Inimigo anda,
então a lembrança dele apodrece. Tratar os dois igual daria um de dois defeitos: ou o mapa some
ao virar o rosto, ou o jogador passa a caçar um inimigo que já saiu dali.

**O que revisar:**
- `TempoDeMemoria` (5 s) **decide o quanto o jogador pode confiar na lembrança.** Curto demais e o
  borrão não ajuda; longo demais e ele vira armadilha.
- `PesoDaMemoria` (0,45) é o quanto um lugar lembrado aparece comparado a estar olhando.
- A grade da memória cobre **80 m** centrados na origem. **Mapa maior que isso não é lembrado nas
  bordas** — ao montar as fases da demo, conferir `TamanhoDoMapa`.
- O borrão é desenhado **por cima da névoa e atravessando parede**, de propósito: a lembrança está
  na cabeça do jogador, não no mundo. Se a equipe achar que atravessar parede confunde, é um
  campo a mudar.
- A cor do vulto (nível 3) passou a ser **mais clara que a névoa**, não mais escura. Vulto escuro
  sumia dentro da névoa escura.

---

## 29/09/2026 — Animação do Desgarrado, novo padrão de animação e reestruturação da névoa

### A animação do Desgarrado não funcionava — erro meu

**O que aconteceu:** na entrega anterior os 96 quadros foram movidos para a pasta certa e a inserção
foi dada como pronta. **Nada no jogo usava aqueles arquivos**: o personagem continuava com a imagem
provisória do Khalid, que só espelhava para os lados.

**O que foi feito:**

- `Uzhan_Desgarrado_Mapa.tres` (novo, na pasta `Map/` do personagem): o `SpriteFrames` com três
  animações — `andar_frente`, `andar_costas`, `andar_lado` — de 32 quadros a 24 por segundo.
- `Player.tscn`: o desenho do jogador passou a ser a animação do Desgarrado (era a imagem do Khalid).
  Como `.tscn` não aceita comentário, fica registrado aqui.
- `AnimacaoDirecional.cs` (novo): escolhe frente, costas ou lado comparando para onde o personagem
  olha com de onde a câmera olha, espelha o lado para a esquerda, e toca a animação no ritmo da
  caminhada. Verificado nos 8 ângulos da câmera × 4 direções do olhar: **32 combinações certas**.
- `PlayerIsometrico.cs`: o espelhamento saiu daqui (agora é do componente acima), e o personagem
  implementa `IPersonagemQueOlha`, que permite ao mesmo componente animar inimigos no futuro.

**O que revisar:**

- **Os quadros foram recortados de novo**, agora com os pés no centro de cada direção. Na arte
  entregue, o corpo de frente e de costas estava 0,9 m à direita do corpo de lado — o personagem
  pularia quase um metro ao virar. Conferido: pés no centro com erro de 0,4 pixel, e a marca no chão
  fica exatamente sob os pés nas quatro vistas. Nenhum pixel do desenho foi alterado, só o fundo
  transparente.
- `VelocidadeDeReferencia` (3 m/s) é a velocidade em que a animação toca no ritmo original. Se os
  pés parecerem patinar no chão, é esse número.
- Parado, o personagem fica no primeiro quadro, até chegar uma animação de parado própria.
- A escala foi ajustada para 1,85 m de altura (o Rasgador tem 1,83 m, o Khalid provisório 1,61 m).

### Padrão de animação: tudo quadro a quadro

**O que foi feito:** `Assets/README.md`, `CLAUDE.md` e `inserir/README.md` atualizados. A regra
antiga (personagens por ossos, efeitos quadro a quadro) foi substituída: **o jogo inteiro é quadro a
quadro**. A Quimera é produzida de outro jeito na ferramenta de arte, mas no jogo também chega
quadro a quadro.

**O que revisar — para a equipe de devs:** a regra antiga existia por causa do tamanho (estimativa
de ~6 MB por ossos contra ~216 MB quadro a quadro). O andar do Desgarrado sozinho tem 4,6 MB. Vale
considerar **Git LFS** para as pastas de arte antes que o repositório fique pesado para clonar.

### Lembrança do inimigo e memória do cenário — retiradas

**O que foi feito:** o borrão no último lugar em que o inimigo foi visto e a grade que lembrava o
cenário já visto foram retirados, a pedido do PO. Saíram também o `EnxergaOPonto` do sensor (só a
lembrança usava) e o `UsarNevoaDeGuerra` da câmera (quem decide agora é o ambiente da fase).

**Onde ficou registrado:** [IDEIAS-ARQUIVADAS.md](IDEIAS-ARQUIVADAS.md), com como funcionavam, os
números e o commit onde está o código (`7757945`).

### `AmbienteDaFase.cs` (novo) — cada fase escolhe o seu ambiente

**O que foi feito:** nó que diz se a fase é limpa, com névoa ou com escuridão. **Mapa sem esse nó é
limpo**: nada ofusca, e a camada da névoa nem aparece.

**O que revisar:** a tecla **F4** passou a percorrer os ambientes (limpo → névoa → escuridão → breu
total → limpo), no lugar de só ligar e desligar. Ferramenta de teste.

### `FonteDeLuz.cs` (novo) — a luz da escuridão

**O que foi feito:** fogueiras, lamparinas e tochas. Na escuridão, só se vê o que está iluminado e,
ao mesmo tempo, perto do personagem ou na direção em que ele olha. Sem luz, breu total — nem o
personagem aparece, como na referência do Don't Starve.

**O que revisar:** a luz **não atravessa parede** — medido: brilho 0,239 do lado da lâmpada, 0,000 do
outro lado da divisória, os dois dentro do alcance dela. Até 16 luzes contam ao mesmo tempo (as mais
perto do jogador).

### `NevoaDeGuerra.cs` e `NevoaDeGuerra.gdshader` — reescritos

**O que foi feito:** a camada agora **esconde de verdade** o que está fora da visão, em dois modos.

- **Escuridão:** preto onde não há luz.
- **Névoa:** clara, e **volátil** — encobre entre 70% e 100%, mudando com o tempo e de lugar para
  lugar. Medido num mesmo ponto em 12 segundos: 83% → 97% → 77%. Atrás de parede e longe do
  personagem, sempre 100%, para a névoa rala não virar janela.

**O que revisar — desempenho, que foi pedido explicitamente:**

| | Limpo | Névoa | Escuridão |
|---|---|---|---|
| Placa de vídeo | 1,12 ms | 1,51 ms | 1,52 ms |
| Processador, parado | 0 | 0,002 ms | 0,04 ms |
| Processador, andando | 0 | 0,17 ms | 0,33 ms |

A primeira versão custava **1,0 a 1,6 ms** de processador em todo passo da física, por causa das 128
linhas até as paredes. Ficou até 8 vezes mais barata com três medidas: a pergunta ao mundo passou a
ser reaproveitada em vez de criada 128 vezes por passo; as paredes só são medidas de novo quando o
jogador se move; e, andando, um passo sim, um não.

**Sobre as medições:** a janela de teste ficou presa em 30 quadros por segundo **em todos os modos,
inclusive o limpo** — limitação do ambiente de teste, não do jogo. Por isso os números acima são o
tempo gasto pela placa de vídeo (medido pelo próprio Godot) e o tempo do código da névoa (medido com
cronômetro), e não quadros por segundo.

### `VisibilidadeDoInimigo.cs` — simplificado

**O que foi feito:** quem esconde o inimigo agora é a camada da névoa, igual esconde o resto do mapa.
O script ficou só com o que ela não resolve: na névoa, o inimigo longe vira **silhueta escura**, que
aparece por um instante onde a névoa afina.

### `Sandbox.tscn` — ambiente e luzes

**O que foi feito:** o mapa de testes ganhou um `AmbienteDaFase` (começa na escuridão), cinco
luzes (fogueira perto do início, lamparinas nos dois andares da casa, numa esquina da ronda e atrás
de uma divisória) e uma lanterna na mão do jogador.

## 30/09/2026 — Bugs da luz e da névoa, névoa e escuridão nos inimigos, cenário no estilo Don't Starve

### Os bugs do vídeo (`debug/bugsiluminacao...mp4`)

| Bug relatado | Causa | O que mudou |
|---|---|---|
| A luz só aparecia olhando para ela | Na escuridão, o que aparecia era "iluminado **e** dentro do cone ou do círculo passivo" | Agora aparece tudo o que está iluminado e sem parede no meio, olhando ou não |
| Faixa escura entre a visão e a luz | O cone recortava a área iluminada com bordas retas | Some junto com o item acima. Entre duas luzes que não se alcançam continua escuro — é o Don't Starve |
| Artefatos ao olhar por quinas | A sombra da parede tinha corte seco, desenhando polígonos duros na névoa | Penumbra de pouco mais de 1 m; cada linha medida é comparada antes de misturar |
| Círculos visíveis na névoa | A névoa tinha degraus: círculo passivo, cone e anéis | Névoa contínua que fecha aos poucos com a distância, sem borda |

Medidas de antes e depois estão em `docs/JOGO.md`, seção "Névoa e escuridão".

### `AmbienteDaFase.cs` — passou a ser a fonte das contas do ambiente

**O que foi feito:** o desenho de ruído e o relógio da névoa saíram da camada da tela e vieram para
cá, junto com as contas "quanto de névoa existe aqui", "quanta luz chega aqui" e "o quanto se
enxerga através disso". A tela e os inimigos usam as mesmas contas.

**O que revisar:**

- **`DensidadeMinima` e `DensidadeMaxima` mudaram de sentido.** Antes eram a porcentagem encoberta
  (0,70 e 1,0); agora dizem o quão grossa a névoa está (**0,6 e 1,6**, sendo 1 a normal). Nenhuma
  cena mudava esses números, então nada quebrou — mas quem for ajustar precisa saber.
- `NevoaColada` (5%): o véu que existe até colado no personagem, para ele estar **dentro** da névoa.
- `CurvaDaNevoa` (3): o formato com que a névoa fecha com a distância.

### `NevoaDeGuerra.gdshader` e `NevoaDeGuerra.cs` — a visão virou olhos, e não lanterna

**O que foi feito:** a névoa e a escuridão foram reescritas (ver a tabela dos bugs). Também:

- As linhas que procuram paredes vão até **32 m** na escuridão (antes 16 m), para enxergar luzes
  longe, e até ~24 m na névoa (além disso tudo já some).
- Linhas isoladas que "vazam" pela emenda de duas paredes são encurtadas (`TirarEspinhos`).
- As paredes são medidas de novo a cada **12 cm andados**, e a tela sabe de onde a última medida foi
  feita. Com isso o custo no processador ficou igual ao de antes, mesmo com as linhas mais longas.

**O que revisar:** `LarguraDaPenumbra` (1,2 m) e `PassagemDaParede` (1,5 m). Maior deixa a quina mais
suave, mas deixa espiar um pouco mais além dela.

### `SensorDeteccao.cs` — quanto o ambiente deixa ver, e ajuste por tipo de inimigo

**O que foi feito:** `EnxergaNoEscuro` e `EnxergaNaNevoa` (0 a 1, padrão 0), os alcances na névoa
(saem dos alcances da visão que já existiam) e `QuantoOAmbienteDeixaVer`, a pergunta que o inimigo
faz antes de ver o jogador. `NivelDeVisaoDe` (as faixas 1, 2 e 3) continua existindo, mas a névoa
não usa mais — os degraus dele é que desenhavam os círculos na tela.

### `InimigoIA.cs` — a névoa e a escuridão valem para os inimigos

**O que foi feito:** a visão do inimigo passa pelo ambiente. Abaixo de 15% (`VisaoMinimaParaEnxergar`)
ele não vê; acima, vê, mas o tempo até reconhecer cresce na mesma proporção em que a visão piora.
Ouvir e "quase esbarrar" não mudaram.

**O que revisar:** os tempos medidos estão em `docs/JOGO.md`. **Falta a equipe decidir** quais
inimigos enxergam no escuro ou ignoram a névoa — os dois do sandbox estão como gente comum.

### `VisibilidadeDoInimigo.cs` — a silhueta ficou contínua

**O que foi feito:** o inimigo escurece aos poucos até virar vulto conforme a névoa entre ele e o
jogador engrossa, em vez de trocar de cor aos saltos por faixa (`NivelAtual` virou
`QuantoOJogadorVe`, de 0 a 1).

### `PlayerIsometrico.cs` e `DebugDeteccao.cs` — lanterna e informações de teste

**O que foi feito:** a tecla **L** acende e apaga a lanterna (a primeira `FonteDeLuz` presa ao
jogador). O jogador passou a medir a luz que chega nele (`LuzNoPersonagem`), a mesma conta do
inimigo. Com F3 ligado aparecem, sobre o jogador, a luz e o estado da lanterna; sobre o inimigo, o
quanto ele enxerga o jogador.

**O que revisar:** a lanterna ilumina o próprio jogador — acesa, ele é visto de longe no escuro.
É uma decisão de jogo que vale confirmar.

### `project.godot` (não aceita comentário)

- Ação nova **`lanterna`**, tecla **L**.
- Camada de física 6 batizada de **`Obstacle`**: o que bloqueia a passagem mas não a visão (hoje,
  as árvores).

### `Player.tscn` e `Inimigo.tscn` (não aceitam comentário)

- `collision_mask` passou de 8 para **40** (8 = Wall, mais 32 = Obstacle), para os personagens
  esbarrarem nas árvores.

### Cenário no estilo Don't Starve — `Scenes/Cenarios/` (novo)

**O que foi feito:**

- `Pinheiro.tscn`: imagem que encara a câmera, tronco que bloqueia a passagem (camada Obstacle) e
  sombra redonda no pé.
- `MuroDePedra.tscn`: um bloco de muro de 1 casa, que bloqueia passagem e visão (camada Wall). Blocos
  vizinhos se sobrepõem e formam um muro contínuo.
- Nos dois, a imagem **não projeta sombra do sol** (virava um triângulo preto que mudava ao girar a
  câmera).
- Arte **provisória**, desenhada por código: `Assets/Art/Cenarios/Pinheiro/Provisorio/Pinheiro.webp`
  e `Assets/Art/Cenarios/MuroDePedra/Provisorio/MuroDePedra.webp`. Os `.import` usam a mesma
  compressão e as mesmas miniaturas (mipmaps) da arte dos personagens.

**O que revisar:** as imagens ficam em pé (como os personagens) e não inclinadas para a câmera como
no Don't Starve — o motivo está em `docs/JOGO.md`, seção "Cenário".

### `Sandbox.tscn` (não aceita comentário)

- **16 pinheiros** e uma **estrutura sem teto de 32 blocos** (9 × 8 casas, entrada ao sul, parede
  interna em L), a sudeste da casa.
- A malha de navegação passou a contornar também a camada Obstacle (`geometry_collision_mask` de 8
  para 40).
- Conferido: o jogador para encostado no muro e no tronco, o caminho do inimigo entra pela abertura
  da estrutura, e a ronda andou 112 m em 45 s por quatro pontos sem parar nenhuma vez.

### Documentação

- `Assets/README.md`: regras para arte de cenário (uma pasta por objeto, uma peça por casa nas
  estruturas, base no centro embaixo, desenhar ~1,5 vez mais alto por causa do ângulo da câmera).
- `CLAUDE.md`: a pasta `Scenes/Cenarios/` entrou na estrutura do projeto.
- `docs/JOGO.md`: seções "Cenário" e "Névoa e escuridão" reescritas com os números medidos.

## 01/10/2026 — Fim do cone de visão do jogador e da mira pelo mouse

**O que foi feito (decisão do PO):** o jogador deixou de ter cone de visão. Na névoa, ele enxerga
só num **círculo em volta** (`VisaoPassiva1` e `2`); na escuridão nada mudou (quem revela é a luz,
a lanterna). A **mira pelo mouse** saiu junto: o personagem volta a olhar sempre para onde anda, o
que acaba com o "moonwalk" (mouse para um lado, andando para o outro). **Os inimigos mantêm o cone.**

- `PlayerIsometrico.cs`: saíram `ApontarComOMouse`, `PontoParaOndeOlha` e a mira pelo mouse.
- `SensorDeteccao.cs`: saíram as três faixas do cone (`FracaoDaVisao1/2`, `AlcanceVisao1/2/3`) e
  `NivelDeVisaoDe`. A conta do ambiente virou `QuantoEnxergaEmVolta` (jogador) e
  `QuantoEnxergaPeloCone` (inimigos).
- `NevoaDeGuerra.cs` e `.gdshader`: a névoa usa um alcance só, igual em todas as direções.
- `DebugDeteccao.cs`: o cone (F3) aparece só nos inimigos, numa faixa só.
- `Player.tscn` (não aceita comentário): saíram `AlcanceVisao = 16` e `AberturaVisao = 80`, que só
  serviam ao cone do jogador.

**Conferido:** o personagem olha para onde anda com o mouse do lado oposto (direita = lado, esquerda
= lado espelhado, W = costas, S = frente); a névoa ficou igual em todas as direções (13 a 15% aos
3,5 m, 40 a 49% aos 6 m, diferença máxima de 4 pontos olhando para lados opostos); os tempos dos
inimigos ficaram idênticos; névoa 1,50 ms e escuridão 1,56 ms na placa de vídeo.

**Documentação:** `docs/JOGO.md` atualizado (a seção da mira pelo mouse saiu; a névoa descreve só o
círculo; a tabela "Os raios" e a legenda do F3 dizem que o cone é só dos inimigos). O cone do jogador
e a mira pelo mouse foram registrados em `docs/IDEIAS-ARQUIVADAS.md`.

### Regra 3 — o Claude pode gravar no Drive, só na pasta "Para revisão"

**O que foi feito (decisão do PO):** a Regra 3 do `CLAUDE.md` mudou. O Claude continua sem alterar
nenhum documento oficial, mas pode gravar alterações na pasta **"Para revisão"** do Drive; um dev
revisa e, dando o ok, leva para a pasta oficial. Do Trello, só lê. Também ajustados os textos que
repetiam a regra antiga: `docs/README.md` e `docs/SUGESTOES-PARA-DOCUMENTACAO.md`.

**O que revisar:** em 01/10/2026 o Claude ainda **não tem acesso** ao Drive nem ao Trello neste
ambiente (VS Code), então por enquanto vale o caminho manual de sempre (`docs/para-repositorio/`). O
nome e o lugar exatos da pasta "Para revisão" no Drive precisam ser definidos quando o acesso
funcionar.

## 06/10/2026 — Combate básico em hexágonos

Decisões do PO de 06/10/2026, com o Pit People como referência. O que vale como regra está em
`docs/JOGO.md`, seção "Combate"; aqui fica o que foi feito em cada arquivo e o que revisar.

### Scripts novos (`Scripts/Combat/` e `Scripts/UI/`)

| Arquivo | O que faz |
|---|---|
| `Combat/Hex.cs` | O endereço de uma casa hexagonal (topo reto), distância, vizinhas, giro de 60° e conversão para o mapa |
| `Combat/GradeDeCombate.cs` | A grade do mapa: calculada uma vez quando a fase carrega, com andares, rampas, paredes e obstáculos; caminho, alcance e linha de visão |
| `Combat/DesenhoDaGrade.cs` | Desenha os hexágonos no chão (só no combate, ou com F6) |
| `Combat/Dados/FichaDeCombatente.cs`, `Habilidade.cs`, `EfeitoDeHabilidade.cs`, `EquipeInicial.cs`, `TipoDeEfeito.cs` | Os tipos dos arquivos de dados: ficha, habilidade, as peças de cada habilidade, a equipe inicial e a lista de efeitos |
| `Combat/RegrasDeCombate.cs` | Todos os números das regras num lugar só — **simbólicos** |
| `Combat/Combatente.cs` | O estado de cada lutador durante a luta (vida, Foco, efeitos, iniciativa, movimento, ação) |
| `Combat/ExecutorDeHabilidades.cs` | Usa uma habilidade: acerto, dano, Vanguarda, Couraça, efeitos, empurrão, cura, contra-ataque |
| `Combat/AnimacaoDeAtaque.cs` | O plano do golpe (distância até o alvo, encaixe da arma, quanto andar) e as animações provisórias |
| `Combat/IaDeCombate.cs` | O turno do inimigo: escolhe o alvo (Furtividade pesa), anda e ataca |
| `Combat/GerenciadorDeCombate.cs` | Comanda a luta: grito de aviso, formação, iniciativa, turnos, mouse, fim. Nó global "Combate" |
| `Combat/Equipe.cs` | A equipe (5 personagens, formação, líder, quem morreu, vida entre lutas). Nó global "Equipe" |
| `Combat/CorpoDeCombate.cs` | O corpo dos membros da equipe que aparecem só na luta |
| `Combat/MarcadorDeCombate.cs` | Barra de vida, nome, efeitos e números de dano sobre a cabeça |
| `UI/InterfaceDeCombate.cs` | Ordem da iniciativa, painel da vez, botões das ações, lista de habilidades, registro |
| `UI/MenuDeEquipe.cs` | O menu de equipe (T) com a formação arrastável |
| `UI/TelaDeFimDeJogo.cs` | A tela de fim de jogo provisória |

### Scripts alterados

- `PlayerIsometrico.cs`: `ControladoPeloCombate` (na luta não lê o teclado), `OlharPara` e
  `TrocarDesenho` (quando o líder morre, o próximo assume).
- `InimigoIA.cs`: `Ficha` (a ficha de combate), `EmCombate` (para de rondar) e `OlharPara`.
- `CameraIsometrica.cs`: a aproximação ao começar a luta (`DefinirModo` agora faz algo), a câmera
  desliza até quem está na vez, a roda do mouse afasta no combate, e girar com Q/E vale também na luta.
  **Na exploração ela continua colada no personagem, como antes.**

### Arquivos que não aceitam comentário

- `project.godot`: dois nós globais (autoload) — `Equipe` e `Combate` — e três teclas novas:
  `menu_equipe` (T), `combate_encerrar_turno` (Enter) e `debug_grade` (F6).
- `Scenes/Characters/Inimigo.tscn`: o campo `Ficha` aponta para `Resources/Inimigos/Rasgador.tres`.
- `Scenes/Levels/Sandbox.tscn`: o nó `GradeDeCombate`, **deslocado 0,5 m para oeste** (sem isso a
  coluna de hexágonos caía na beirada da laje e a rampa não chegava ao andar de cima — testados 28
  deslocamentos, este deixa rampa, porta e muro funcionando em todos os testes), e dois inimigos novos,
  `InimigoGrupo1` e `InimigoGrupo2`, parados a oeste do ponto de partida, para testar o grito de aviso.
- `Resources/` (novos, gravados pelo próprio Godot para o formato sair certo):
  `Personagens/` (5 fichas), `Inimigos/Rasgador.tres`, `Habilidades/<personagem>/` (21 habilidades) e
  `Equipe/EquipeInicial.tres` (Uzhan líder, Khalid, Lancelot, Emi, Tao, com a formação inicial).

### Ferramentas e documentação

- `docs/ferramentas/revisao-2026-10-06.py` (novo): gera os `.docx` corrigidos a partir da versão atual
  do Drive. Saída em `docs/para-repositorio/2026-10-06/`.
- `CLAUDE.md`: Regra 3 reescrita (a documentação local vale mais; o que o conector do Drive faz e não
  faz) e "Estado do código".
- `docs/JOGO.md`: seção "Combate" reescrita; câmera, sandbox e "Ainda não implementado" atualizados.
- `docs/externo/Infos/Árvores de Habilidades.md` e `GDD Rei de Amarelo.md` (as nossas cópias): as mesmas
  correções do Drive, e o GDD local recebeu também as decisões de 18/09 que só tinham ido para o
  `.docx` (Lábia, Intuição e Análise fora, Furtividade, respawn) e o atributo Iniciativa.
- `docs/MUDANCAS-DOCUMENTACAO.md`, `docs/SUGESTOES-PARA-DOCUMENTACAO.md` e `docs/AMBIENTE.md`.
- No Drive, pasta `Pato Games/Revisão`: o documento "Revisão 06-10-2026 — o que está desatualizado no
  Drive", com a lista das correções.

### Dois defeitos achados no teste com janela (e corrigidos)

1. **A luta não começava sozinha.** Os nós globais ficam prontos **depois** de o mapa inteiro entrar em
   cena, então o combate não ouvia o aviso dos inimigos do mapa inicial. No teste automático o mapa
   era carregado depois, e por isso passou. Agora o combate começa a escutar na entrada e liga também
   os inimigos que já estavam lá.
2. **Clicar na casa de um inimigo não atacava** (só clicar no desenho dele), e dois desenhos vizinhos
   sobrepostos mandavam o clique para o personagem errado. Agora vale primeiro a casa do chão debaixo
   do mouse.

### Testes feitos

- **Teste automático sem janela** (temporário, já apagado): 54 verificações, todas ok — a grade (térreo,
  andar de cima pela rampa em 22 casas, porta, estrutura de pedra, nenhuma das 16 árvores numa casa
  válida), o grito (entra quem está a 4 m; não entra quem está a 6,5 m atrás de uma caixa nem quem está
  longe), a formação virada para os inimigos, a ordem da iniciativa, uma luta inteira jogada sozinha
  (vitória na 4ª rodada, todas as 20 habilidades usadas ao longo das rodadas de teste), a volta à
  exploração e a tela de fim de jogo.
- **Teste com janela, com mouse e teclado:** a luta começando sozinha, andar, atacar, Rasga-ossos, a
  área laranja da Voz amaldiçoada, o segundo turno de quem tirou 20, os inimigos jogando, o menu de
  equipe (arrastar e trocar) e a formação nova aplicada na luta seguinte.
- Grade do sandbox: 1.863 casas e 4.786 ligações, montadas em ~0,3 s quando a fase carrega.

### O que revisar

- **Os números são todos simbólicos** (`RegrasDeCombate.cs` e os `.tres`).
- **Decisões provisórias** para dar para testar, listadas no fim da seção "Combate" do `JOGO.md` e nas
  pendências de `SUGESTOES-PARA-DOCUMENTACAO.md`: inimigos fora da luta ficam parados; Foco cheio em
  cada luta; vida e sanidade continuam entre lutas; Protetora dura 3 rodadas; Transfiguração térmica
  sorteia Gelo ou Fogo; o que acontece com quem tira 1 na iniciativa (por enquanto, nada).
- **As alucinações da sanidade baixa ainda não foram feitas** — a regra está documentada.

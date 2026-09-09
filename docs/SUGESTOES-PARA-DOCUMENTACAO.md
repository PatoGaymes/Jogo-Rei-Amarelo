# Sugestões para a documentação externa

A Regra 3 diz que o Claude **nunca altera** a documentação do Trello, OneNote, Canva ou
Drive. Quando ele identificar algo que precisa ser corrigido lá, a sugestão vem para cá,
e um dev decide se sobe ou não.

**Nada nesta página foi aplicado nos sites.**

---

## 09/09/2026 — Correções na documentação de preparação do ambiente

### 1. Godot não roda JavaScript, Lua nem Python — **prioridade alta**

**O que está escrito:**

> "Como iremos utilizar alguns códigos em C# (Caso tenha necessidade, usaremos JavaScrypt e
> Lua também)"

**O problema:** o Godot 4 suporta oficialmente apenas **GDScript** e **C#**. Lua existe só
através de extensões feitas pela comunidade, que teriam que ser instaladas e mantidas pela
equipe. JavaScript não tem suporte real. Python também não.

**Sugestão de texto:**

> Como iremos utilizar C#, usaremos o Visual Studio Code como IDE externa. O Godot suporta
> oficialmente duas linguagens: **C#** (nossa principal) e **GDScript** (a linguagem própria
> da engine, útil para scripts pequenos de cena). Outras linguagens como Python podem ser
> usadas em ferramentas de apoio fora do jogo, mas não dentro dele.

---

### 2. Falta a configuração que faz o C# abrir no VS Code — **prioridade alta**

**O que está escrito:** a documentação manda configurar `Text Editor > External`.

**O problema:** essa configuração vale **apenas para GDScript**. Existe uma configuração
separada para C#, e sem ela os scripts C# continuam abrindo no editor interno do Godot —
exatamente o que a equipe queria evitar.

**Sugestão: acrescentar após o passo do Exec Flags:**

> **Configuração do editor externo para C#**
>
> A configuração acima vale apenas para GDScript. Para o C# abrir no VS Code:
>
> Vá em **Dotnet → Editor → External Editor** e selecione **Visual Studio Code**.
>
> Sem esse passo, os scripts C# continuam abrindo no editor interno do Godot.

---

### 3. A lista de extensões do VS Code está incompleta — **prioridade média**

**O que está escrito:** "Instale a extensão própria da Microsoft chamada C#. Só isso é
necessário, por enquanto."

**O problema:** sem a extensão **godot-tools**, o VS Code trata os arquivos de cena
(`.tscn`) como texto sem sentido e não consegue depurar o jogo. Na prática ela já está
instalada na máquina do Eric — a documentação é que está desatualizada.

**Sugestão de texto:**

> Instale as seguintes extensões:
>
> - **C#** (Microsoft) — para programar em C#
> - **godot-tools** (geequlim) — para o VS Code entender arquivos de cena do Godot e
>   permitir depuração
> - **C# Tools for Godot** (neikeq) — para pausar o código C# e investigar problemas
>   enquanto o jogo roda
> - **EditorConfig** — mantém o padrão de formatação do projeto
>
> Opcional: **C# Dev Kit** (Microsoft) — traz recursos extras. Atenção à licença: é
> gratuita para uso individual e empresas pequenas, mas exige licença do Visual Studio em
> empresas grandes.
>
> Ao abrir o projeto, o VS Code já sugere instalar essas extensões automaticamente.

---

### 4. Correção na convenção de nomes de pastas — **prioridade baixa**

**O que está escrito:** "Nas opções de Convenção de Nomenclatura de Diretórios coloque
camelCase"

**O problema:** as pastas que já existem no projeto (`Scenes`, `Scripts`, `Assets`) usam
**PascalCase** (primeira letra maiúscula), que também é o padrão do C#. Manter `camelCase`
faria as pastas novas saírem com um padrão diferente das antigas.

**Sugestão:** trocar `camelCase` por `PascalCase`.

---

### 5. Observação sobre o .NET 10 — **prioridade baixa, apenas informativa**

**O que está escrito:** "Baixe a versão .NET 10.0 e instale no disco onde está o Windows"

**Observação:** está correto e **foi testado funcionando**. Vale acrescentar uma nota,
porque é uma dúvida que costuma aparecer:

> O Godot 4.6 trabalha internamente com o .NET 8, mas o .NET 10 é compatível e foi testado
> no nosso projeto. Não é preciso instalar as duas versões — o .NET 10 basta.

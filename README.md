[![CI (.NET)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml)
![Coverage](badges/coverage.svg)
![GitHub release](https://img.shields.io/github/v/release/felipe-frc/chesssharp)
![GitHub repo size](https://img.shields.io/github/repo-size/felipe-frc/chesssharp)
![GitHub license](https://img.shields.io/github/license/felipe-frc/chesssharp)

# ChessSharp

ChessSharp e um jogo de xadrez desenvolvido em **C# com .NET**, com tres formas de jogar:

- **Console** para experimentar o motor de regras no terminal
- **Desktop WPF** para uma experiencia 2D nativa no Windows
- **Web com Blazor WebAssembly** para jogar no navegador com a mesma engine em C#

O projeto foi evoluido por releases incrementais e hoje entrega uma base jogavel e estavel com:

- regras reais de xadrez
- bot com minimax simples e poda alpha-beta
- cobertura de testes com CI no GitHub Actions
- interface desktop premium
- interface web estavel, responsiva e jogavel no navegador

---

## Visao Geral

O ChessSharp compartilha o mesmo motor de jogo entre as interfaces. Isso permite evoluir a experiencia visual sem duplicar regras, validacoes ou comportamento do bot.

### O que ja esta pronto

- xeque e xeque-mate
- afogamento
- roque pequeno e grande
- promocao de peao
- en passant
- validacao de movimentos legais
- protecao contra jogadas que deixam o proprio rei em xeque
- bot com avaliacao material e busca minimax

### Plataformas disponiveis

- **Console**: partida via terminal com entrada por coordenadas
- **Desktop WPF**: experiencia 2D premium para Windows
- **Blazor WebAssembly**: versao web jogavel com layout premium, capturas, historico, selecao de cor, animacao curta e feedback visual refinado

---

## Objetivo do Projeto

O projeto foi construido para praticar e demonstrar:

- desenvolvimento com C# e .NET
- programacao orientada a objetos
- modelagem de dominio
- implementacao de regras completas de xadrez
- IA simples para jogos
- testes automatizados com xUnit
- cobertura de testes com Coverlet
- integracao continua com GitHub Actions
- evolucao incremental guiada por releases

---

## Funcionalidades

### Motor do jogo

- tabuleiro 8x8
- representacao por coordenadas como `e2`, `e4`, `h8`
- controle de turno entre brancas e pretas
- validacao de movimentos legais
- validacao de caminho livre
- captura de pecas adversarias
- bloqueio de captura de peca da mesma cor
- bloqueio de jogadas que deixam o proprio rei em xeque
- encerramento correto por vitoria, empate ou partida encerrada

### Regras especiais

- roque pequeno
- roque grande
- promocao de peao
- promocao com escolha de peca
- en passant
- afogamento

### Bot

- busca de movimentos legais
- avaliacao material
- minimax simples
- poda alpha-beta
- respeito integral ao motor de regras

### Console

- jogavel no terminal
- pecas em Unicode
- entrada no formato `origem destino`
- escolha da cor do jogador
- promocao informando a peca desejada

Exemplo:

```txt
e2 e4
e7 e8 q
```

### Desktop WPF

- tabuleiro visual 2D
- pecas em PNG
- selecao por clique
- destaque de movimentos validos
- nova partida
- feedback visual premium

### Web com Blazor WebAssembly

- roda no navegador com a mesma engine em C#
- selecao de cor no inicio da partida
- tabuleiro premium com foco visual dominante
- lateral compacta com resumo da partida
- capturas separadas por cor
- historico recente com acesso ao historico completo
- animacao curta de movimento
- feedback sonoro opcional
- layout responsivo e pronto para demonstracao publica

---

## Tecnologias Utilizadas

| Camada | Tecnologia |
| --- | --- |
| Linguagem | C# |
| Plataforma | .NET |
| Console | .NET 9 |
| Desktop | WPF em .NET 9 para Windows |
| Web | Blazor WebAssembly em .NET 9 |
| Testes | xUnit |
| Cobertura | Coverlet |
| CI/CD | GitHub Actions |
| Versionamento | Git / GitHub |

---

## Estrutura do Projeto

```txt
ChessSharp/
|
|-- ChessSharp/
|   |-- UI/
|   `-- Program.cs
|
|-- ChessSharp.Core/
|   |-- AI/
|   |-- Board/
|   |-- Enums/
|   |-- Game/
|   |-- Pieces/
|   `-- ChessSharp.Core.csproj
|
|-- ChessSharp.Desktop/
|   |-- Assets/
|   |-- App.xaml
|   |-- MainWindow.xaml
|   `-- ChessSharp.Desktop.csproj
|
|-- ChessSharp.Web/
|   |-- Components/
|   |-- Layout/
|   |-- Pages/
|   |-- Services/
|   |-- ViewModels/
|   |-- wwwroot/
|   |-- App.razor
|   |-- Program.cs
|   `-- ChessSharp.Web.csproj
|
|-- ChessSharp.Tests/
|   |-- AI/
|   |-- Board/
|   |-- Game/
|   `-- Pieces/
|
|-- .github/
|-- badges/
|-- ChessSharp.sln
`-- README.md
```

### Arquitetura

- `ChessSharp.Core` centraliza toda a engine compartilhada: tabuleiro, regras, movimentos, pecas, enums e bot.
- `ChessSharp` ficou focado apenas na experiencia de terminal e usa o Core por referencia de projeto.
- `ChessSharp.Desktop` usa a mesma engine em uma camada WPF dedicada a eventos, renderizacao e interacao visual.
- `ChessSharp.Web` usa a mesma engine em Blazor WebAssembly, com `Home.razor` atuando como orquestrador e a interface separada em componentes como `ChessBoard`, `ChessSquare`, `GamePanel`, `MoveHistory` e modais.
- `ChessSharp.Tests` valida a engine compartilhada sem depender de UI.

---

## Como Executar

### Pre-requisitos

- .NET SDK 9
- Windows para executar a versao desktop
- Git

> A solution foi padronizada em `.NET 9`, mantendo apenas a variacao `net9.0-windows` no projeto WPF por necessidade da plataforma.

### 1. Clone o repositorio

```bash
git clone https://github.com/felipe-frc/chesssharp.git
cd chesssharp
```

### 2. Restaure as dependencias

```bash
dotnet restore
```

### 3. Execute a versao console

```bash
dotnet run --project ChessSharp/ChessSharp.csproj
```

### 4. Execute a versao desktop

```bash
dotnet run --project ChessSharp.Desktop/ChessSharp.Desktop.csproj
```

### 5. Execute a versao web

```bash
dotnet run --project ChessSharp.Web/ChessSharp.Web.csproj
```

Depois, abra:

```txt
http://localhost:5290/
```

---

## Como Jogar

### Console

- escolha jogar com brancas ou pretas
- informe movimentos no formato `origem destino`
- para promocao, informe a peca como terceiro argumento
- use `sair` para encerrar a partida

Exemplos:

```txt
e2 e4
e7 e8 q
sair
```

### Desktop

- clique em uma peca da sua cor
- as casas legais serao destacadas
- clique na casa de destino
- a maquina responde automaticamente

### Web

- escolha sua cor ao iniciar
- clique em uma peca para ver os movimentos
- clique na casa destacada para concluir a jogada
- use `Novo jogo` para recomecar
- use `Trocar cor` para abrir uma nova escolha de lado
- use `Ver historico completo` para revisar todos os lances

---

## Testes Automatizados

O projeto possui testes cobrindo:

- conversao de coordenadas
- movimentos das pecas
- regras gerais do jogo
- xeque e xeque-mate
- promocao de peao
- roque
- en passant
- comportamento do bot

Para executar:

```bash
dotnet test ChessSharp.Tests/ChessSharp.Tests.csproj
```

Para compilar a solution completa:

```bash
dotnet build ChessSharp.sln
```

Para compilar manualmente por interface:

```bash
dotnet build ChessSharp/ChessSharp.csproj
dotnet build ChessSharp.Desktop/ChessSharp.Desktop.csproj
dotnet build ChessSharp.Web/ChessSharp.Web.csproj
```

---

## Integracao Continua

O GitHub Actions executa automaticamente:

- restauracao de dependencias
- compilacao da solucao
- execucao dos testes
- coleta de cobertura
- atualizacao do badge de cobertura

---

## Melhorias e Proximos Passos

Proximas evolucoes recomendadas:

- refinamentos visuais finais na interface web
- configuracao de profundidade do bot
- regras adicionais de empate:
  - repeticao de posicao
  - regra dos 50 lances
  - material insuficiente
- reducao de responsabilidades visuais concentradas nas telas
- avaliacao de deploy publico da versao web

---

## Roadmap

### v1.0.0

- versao inicial jogavel no console
- bot simples
- testes automatizados principais

### v1.1.0

- melhorias visuais do tabuleiro no console

### v1.2.0

- escolha da cor do jogador

### v1.3.0

- xeque e xeque-mate reais

### v1.4.0

- roque e promocao de peao

### v1.5.0

- bot com minimax simples

### v1.6.0

- cobertura de testes com badge

### v1.7.0

- en passant

### v2.0.0

- primeira interface grafica 2D com WPF

### v2.1.0

- pecas PNG e refinamento visual premium

### v2.2.0

- promocao visual na interface grafica

### v2.3.0

- refinamento do footer e feedback visual da experiencia desktop

### v2.4.0

- otimização visual e cache de assets do jogo

### v2.5.0

- versao web estavel com Blazor WebAssembly
- selecao de cor no navegador
- historico recente e historico completo em modal
- capturas separadas por cor
- resumo de posicao e material em linguagem amigavel
- animacao curta de movimento
- feedback sonoro opcional
- layout premium responsivo para browser

---

## Releases

### v2.5.0 - ChessSharp Web estavel no navegador

Versao focada em consolidar a experiencia web do ChessSharp com Blazor WebAssembly, transformando o projeto em uma experiencia jogavel no navegador sem abrir mao da mesma engine em C#, da identidade premium do tabuleiro e da evolucao incremental que marcou as releases anteriores.

### v2.4.0 - Otimizacao visual e cache de assets

Versao focada em melhorar o carregamento e o reaproveitamento dos assets visuais do projeto, preparando a base para interfaces mais ricas e responsivas.

### v2.3.0 - Refinamento do footer e feedback visual

Versao focada no polimento visual da interface desktop, refinando o footer, os botoes de acao, o painel de status e o feedback de selecao e movimentos legais para uma experiencia mais consistente com a identidade premium do projeto.

### v2.2.0 - Fundo premium e refinamento da interface desktop

Versao focada na atmosfera visual da interface desktop, adicionando fundo premium em marmore escuro, consolidando o tabuleiro premium e refinando o layout geral da tela para um acabamento mais forte de portfolio.

### v2.1.0 - Tabuleiro premium e refinamento visual do jogo

Versao focada na substituicao do tabuleiro renderizado por codigo por um tabuleiro premium em imagem, com melhor integracao visual entre pecas, fundo, coordenadas e layout da interface.

### v2.0.1 - Refinamento visual da interface desktop

Versao focada em melhorar a apresentacao da primeira interface grafica do ChessSharp, com ajustes de paleta, logo, harmonia visual, acabamento do layout e preservacao do fluxo funcional ja existente.

### v2.0.0 - Interface grafica 2D com WPF

Versao focada na evolucao visual do ChessSharp, adicionando uma interface grafica desktop em WPF, tabuleiro 2D, movimentacao por mouse, integracao com o bot e marcadores visuais para movimentos legais.

### v1.7.0 - En passant

Versao focada na implementacao da regra especial en passant, completando as principais regras especiais do xadrez junto com roque e promocao de peao.

### v1.6.0 - Cobertura de testes com badge

Versao focada na validacao de qualidade do projeto, adicionando coleta de cobertura com Coverlet e badge de cobertura no README.

### v1.5.0 - Bot com minimax simples

Versao focada na evolucao da inteligencia da maquina, adicionando avaliacao de tabuleiro, simulacao de jogadas futuras, busca minimax simples e poda alpha-beta.

### v1.4.0 - Roque e promocao de peao

Versao focada na implementacao de regras especiais do xadrez, adicionando promocao de peao, roque pequeno, roque grande e validacoes especificas para impedir roques ilegais.

### v1.3.0 - Xeque e xeque-mate real

Versao focada na evolucao do motor de regras do ChessSharp, substituindo a logica simplificada de captura do rei por deteccao real de xeque e xeque-mate.

### v1.2.0 - Escolha de cor do jogador

Versao que permite ao jogador escolher se deseja jogar com as pecas brancas ou pretas antes do inicio da partida.

### v1.1.0 - Melhorias visuais no tabuleiro

Versao focada na melhoria visual do ChessSharp no console, com casas coloridas, pecas Unicode e melhor apresentacao do tabuleiro.

### v1.0.0 - Versao inicial jogavel no console

Primeira versao jogavel do ChessSharp, com tabuleiro, pecas, movimentacao, jogador contra maquina, bot simples, testes automatizados e integracao continua.

---

## Licenca

Este projeto esta sob a licenca MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## Autor

**Marcos Felipe Franca**

[LinkedIn](https://www.linkedin.com/in/marcosfelipefrc) · [GitHub](https://github.com/felipe-frc)

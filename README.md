[![CI (.NET)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml)
![Coverage](badges/coverage.svg)
![GitHub release](https://img.shields.io/github/v/release/felipe-frc/chesssharp)
![GitHub repo size](https://img.shields.io/github/repo-size/felipe-frc/chesssharp)
![GitHub license](https://img.shields.io/github/license/felipe-frc/chesssharp)

# ChessSharp

ChessSharp e um jogo de xadrez desenvolvido em **C# com .NET**, estruturado para compartilhar a mesma engine entre tres experiencias:

- **Console** para jogar no terminal
- **Desktop WPF** para uma interface grafica nativa no Windows
- **Web com Blazor WebAssembly** para jogar no navegador usando a mesma base em C#

O projeto foi evoluido em releases incrementais e hoje combina:

- regras reais de xadrez
- bot com minimax e poda alpha-beta
- engine centralizada e reutilizavel
- testes automatizados
- integracao continua no GitHub Actions
- interfaces Desktop e Web com identidade visual premium

---

## Visao Geral

O ChessSharp foi refatorado para que toda a logica de jogo fique em `ChessSharp.Core`, enquanto as interfaces ficam responsaveis apenas por apresentacao e interacao.

Isso permite:

- evoluir a experiencia visual sem duplicar regra de negocio
- manter Console, Desktop e Web sincronizados
- testar a engine separadamente da UI
- escalar o projeto com mais seguranca e organizacao

### O que ja esta pronto

- xeque
- xeque-mate
- afogamento
- roque pequeno e grande
- promocao de peao
- en passant
- validacao de movimentos legais
- bloqueio de jogadas que deixam o proprio rei em xeque
- bot com avaliacao material e busca minimax

---

## Arquitetura Atual

### Nucleo compartilhado

`ChessSharp.Core` centraliza:

- tabuleiro
- pecas
- enums
- regras do jogo
- regras especiais
- validacoes de movimento
- estrutura de jogadas
- IA / bot

### Interfaces

- `ChessSharp`:
  camada Console, focada em entrada e saida textual
- `ChessSharp.Desktop`:
  camada WPF, focada em renderizacao, eventos e interacao visual
- `ChessSharp.Web`:
  camada Blazor WebAssembly, focada em componentes, layout, feedback visual e experiencia no navegador

### Testes

`ChessSharp.Tests` cobre a engine compartilhada sem depender de UI.

---

## Estrutura do Projeto

```txt
ChessSharp/
|
|-- ChessSharp/
|   |-- UI/
|   |-- ChessSharp.csproj
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
|   |   `-- Images/
|   |-- App.xaml
|   |-- MainWindow.xaml
|   |-- MainWindow.xaml.cs
|   `-- ChessSharp.Desktop.csproj
|
|-- ChessSharp.Web/
|   |-- Components/
|   |   |-- CapturedPieces.razor
|   |   |-- ChessBoard.razor
|   |   |-- ChessPiece.razor
|   |   |-- ChessSquare.razor
|   |   |-- ColorSelectionModal.razor
|   |   |-- GamePanel.razor
|   |   |-- MoveHistory.razor
|   |   |-- PromotionModal.razor
|   |   `-- SoundToggle.razor
|   |-- Layout/
|   |-- Pages/
|   |   |-- Home.razor
|   |   `-- Home.razor.cs
|   |-- Services/
|   |   `-- ChessPresentationService.cs
|   |-- ViewModels/
|   |   |-- CapturedPieceView.cs
|   |   |-- MoveHistoryEntry.cs
|   |   `-- PendingAnimation.cs
|   |-- wwwroot/
|   |   |-- assets/
|   |   |-- css/
|   |   `-- js/
|   |-- App.razor
|   |-- Program.cs
|   `-- ChessSharp.Web.csproj
|
|-- ChessSharp.Tests/
|   |-- AI/
|   |-- Board/
|   |-- Game/
|   |-- Pieces/
|   `-- ChessSharp.Tests.csproj
|
|-- .github/
|   `-- workflows/
|-- badges/
|-- ChessSharp.sln
`-- README.md
```

---

## Funcionalidades

### Motor do jogo

- tabuleiro 8x8
- coordenadas como `e2`, `e4`, `h8`
- controle de turno entre brancas e pretas
- validacao completa de movimentos
- validacao de caminho livre
- capturas
- protecao contra captura de peca da mesma cor
- bloqueio de jogadas que deixam o proprio rei em xeque
- encerramento correto da partida

### Regras especiais

- roque pequeno
- roque grande
- promocao de peao
- promocao com escolha de peca
- en passant
- afogamento

### Bot

- avaliacao material
- busca de movimentos legais
- minimax
- poda alpha-beta

### Versao Console

- jogavel no terminal
- pecas em Unicode
- escolha da cor do jogador
- entrada por coordenadas

Exemplo:

```txt
e2 e4
e7 e8 q
```

### Versao Desktop WPF

- tabuleiro 2D
- pecas em PNG
- selecao por clique
- destaque de movimentos legais
- feedback visual premium

### Versao Web com Blazor WebAssembly

- roda diretamente no navegador
- usa a mesma engine em C#
- selecao de cor no inicio
- tabuleiro premium
- painel lateral compacto
- capturas separadas por cor
- historico recente e completo
- promocao em modal
- animacao curta de movimento
- feedback sonoro opcional

---

## Tecnologias Utilizadas

| Camada | Tecnologia |
| --- | --- |
| Linguagem | C# |
| Plataforma | .NET 9 |
| Console | .NET 9 |
| Desktop | WPF em .NET 9 para Windows |
| Web | Blazor WebAssembly em .NET 9 |
| Testes | xUnit |
| Cobertura | Coverlet |
| CI/CD | GitHub Actions |
| Versionamento | Git / GitHub |

---

## Como Executar

### Pre-requisitos

- .NET SDK 9
- Windows para executar a versao Desktop
- Git

> O projeto foi padronizado em `.NET 9`, com o Desktop usando `net9.0-windows`.

### 1. Clone o repositorio

```bash
git clone https://github.com/felipe-frc/chesssharp.git
cd chesssharp
```

### 2. Restaure as dependencias

```bash
dotnet restore
```

### 3. Execute a versao Console

```bash
dotnet run --project ChessSharp/ChessSharp.csproj
```

### 4. Execute a versao Desktop

```bash
dotnet run --project ChessSharp.Desktop/ChessSharp.Desktop.csproj
```

### 5. Execute a versao Web

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

### Desktop

- clique em uma peca da sua cor
- as casas validas serao destacadas
- clique na casa de destino
- o bot responde automaticamente

### Web

- escolha sua cor ao iniciar
- clique em uma peca para ver os movimentos
- clique na casa destacada para concluir a jogada
- use `Novo jogo` para recomecar
- use `Trocar cor` para abrir uma nova selecao
- use `Ver historico completo` para revisar a partida

---

## Testes

Os testes cobrem:

- conversao de coordenadas
- movimentos das pecas
- regras gerais do jogo
- xeque e xeque-mate
- roque
- promocao
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

---

## Integracao Continua

O GitHub Actions executa automaticamente:

- restauracao da solution
- compilacao
- execucao dos testes
- coleta de cobertura
- atualizacao do badge de cobertura

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

- otimizacao visual e cache de assets

### v2.5.0

- versao Web estavel com Blazor WebAssembly
- selecao de cor no navegador
- historico recente e historico completo
- capturas separadas por cor
- animacao curta de movimento
- feedback sonoro opcional

### v2.6.0

- refatoracao arquitetural da solution
- consolidacao definitiva da engine em `ChessSharp.Core`
- componentizacao da versao Web
- separacao entre UI e regra de negocio
- reorganizacao da estrutura do projeto
- reforco de testes e compatibilidade da pipeline

---

## Releases

### v2.6.0 - Refatoracao arquitetural e consolidacao multiplataforma

Versao focada em reorganizar a base do ChessSharp para deixar a engine compartilhada entre Console, Desktop WPF e Web Blazor WebAssembly mais limpa, reutilizavel e profissional. Esta release consolida `ChessSharp.Core` como centro da logica do jogo, componentiza a interface Web e melhora a manutencao geral da solution.

### v2.5.0 - ChessSharp Web estavel no navegador

Versao focada em consolidar a experiencia Web do ChessSharp com Blazor WebAssembly, transformando o projeto em uma experiencia jogavel no navegador sem abrir mao da mesma engine em C# e da identidade premium do tabuleiro.

### v2.4.0 - Otimizacao visual e cache de assets

Versao focada em melhorar o carregamento e o reaproveitamento dos assets visuais do projeto, preparando a base para interfaces mais ricas e responsivas.

### v2.3.0 - Refinamento do footer e feedback visual

Versao focada no polimento visual da interface desktop, refinando o footer, os botoes de acao, o painel de status e o feedback de selecao e movimentos legais.

### v2.2.0 - Fundo premium e refinamento da interface desktop

Versao focada na atmosfera visual da interface desktop, adicionando fundo premium em marmore escuro e refinando o layout geral da tela.

### v2.1.0 - Tabuleiro premium e refinamento visual do jogo

Versao focada na substituicao do tabuleiro renderizado por codigo por um tabuleiro premium em imagem.

### v2.0.1 - Refinamento visual da interface desktop

Versao focada em melhorar a apresentacao da primeira interface grafica do ChessSharp, com ajustes de paleta, logo e acabamento visual.

### v2.0.0 - Interface grafica 2D com WPF

Versao focada na evolucao visual do ChessSharp, adicionando uma interface grafica desktop em WPF com movimentacao por mouse e integracao com o bot.

### v1.7.0 - En passant

Versao focada na implementacao da regra especial en passant.

### v1.6.0 - Cobertura de testes com badge

Versao focada na validacao de qualidade do projeto, adicionando coleta de cobertura com Coverlet e badge no README.

### v1.5.0 - Bot com minimax simples

Versao focada na evolucao da inteligencia da maquina, adicionando avaliacao de tabuleiro, minimax e poda alpha-beta.

### v1.4.0 - Roque e promocao de peao

Versao focada na implementacao de regras especiais do xadrez.

### v1.3.0 - Xeque e xeque-mate real

Versao focada na substituicao da logica simplificada de captura do rei por deteccao real de xeque e xeque-mate.

### v1.2.0 - Escolha de cor do jogador

Versao que permite ao jogador escolher jogar com as pecas brancas ou pretas.

### v1.1.0 - Melhorias visuais no tabuleiro

Versao focada na melhoria visual do ChessSharp no console, com casas coloridas e pecas Unicode.

### v1.0.0 - Versao inicial jogavel no console

Primeira versao jogavel do ChessSharp, com tabuleiro, pecas, movimentacao, jogador contra maquina, bot simples, testes automatizados e integracao continua.

---

## Proximos Passos

Melhorias naturais para as proximas iteracoes:

- refinamentos visuais finais na interface Web
- configuracao de profundidade do bot
- regras adicionais de empate
- mais testes para cenarios estrategicos
- deploy publico da versao Web

---

## Licenca

Este projeto esta sob a licenca MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## Autor

**Marcos Felipe Franca**

[LinkedIn](https://www.linkedin.com/in/marcosfelipefrc) · [GitHub](https://github.com/felipe-frc)

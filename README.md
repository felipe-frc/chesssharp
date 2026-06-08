[![CI (.NET)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml)
![Coverage](badges/coverage.svg)
![GitHub release](https://img.shields.io/github/v/release/felipe-frc/chesssharp)
![GitHub repo size](https://img.shields.io/github/repo-size/felipe-frc/chesssharp)
![GitHub license](https://img.shields.io/github/license/felipe-frc/chesssharp)

# ♟️ ChessSharp

Jogo de xadrez desenvolvido em **C# com .NET**, com versão em console e interface gráfica 2D em **WPF**, focado em lógica de programação, orientação a objetos, validação de regras, inteligência artificial simples, testes automatizados, cobertura de testes, organização de código e evolução incremental por releases.

O projeto permite jogar uma partida de xadrez contra a máquina. Na versão console, o jogador utiliza comandos no formato padrão de coordenadas do tabuleiro, como `e2 e4`. Na versão desktop, o jogador movimenta as peças com o mouse em um tabuleiro visual 2D.

A versão atual conta com motor de regras mais completo, incluindo detecção de xeque e xeque-mate, roque, promoção de peão, en passant, empate por afogamento, bot com minimax simples, cobertura de testes integrada ao pipeline de CI e interface gráfica 2D com seleção de peças, movimentação por clique e destaque visual de movimentos legais.

---

## 🎯 Objetivo do Projeto

Este projeto foi desenvolvido com o objetivo de praticar e demonstrar conhecimentos em:

- Desenvolvimento de aplicações em C# com .NET;
- Desenvolvimento de interface gráfica desktop com WPF;
- Programação orientada a objetos;
- Modelagem de domínio com classes, enums e responsabilidades bem definidas;
- Implementação de regras de movimentação das peças de xadrez;
- Implementação de regras reais de xeque, xeque-mate e afogamento;
- Implementação de regras especiais como roque, promoção de peão e en passant;
- Validação de entradas do usuário;
- Manipulação de matrizes para representação do tabuleiro;
- Criação de um bot para jogar contra o usuário;
- Implementação de busca minimax simples com poda alpha-beta;
- Testes automatizados com xUnit;
- Coleta de cobertura de testes com Coverlet;
- Integração contínua com GitHub Actions;
- Organização de projeto para portfólio profissional.

---

## ✅ Funcionalidades

### ♟️ Motor do Jogo

- Representação de tabuleiro 8x8;
- Conversão de posições no formato de xadrez, como `e2`, `a1` e `h8`;
- Controle de turno entre peças brancas e pretas;
- Validação de movimentos legais;
- Validação de caminho livre para torre, bispo e rainha;
- Captura de peças adversárias;
- Bloqueio de captura de peças da mesma cor;
- Bloqueio de captura direta do rei;
- Bloqueio de movimentos que deixam o próprio rei em xeque;
- Encerramento correto da partida com base no estado do jogo.

### 🧩 Peças

- Implementação das peças principais do xadrez:
  - Peão;
  - Torre;
  - Cavalo;
  - Bispo;
  - Rainha;
  - Rei.
- Validação individual de movimentação por peça;
- Regras específicas para cada tipo de peça;
- Controle de peças que já se moveram, necessário para regras como roque.

### 🎮 Versão Console

- Execução da partida pelo terminal;
- Renderização do tabuleiro no console;
- Casas alternadas com cores diferentes;
- Peças representadas por símbolos Unicode;
- Exibição das coordenadas do tabuleiro;
- Entrada de movimentos no formato `origem destino`.

Exemplo:

```txt
e2 e4
```

- Escolha da cor do jogador:
  - Brancas;
  - Pretas.
- Máquina controla automaticamente a cor oposta;
- Promoção com peça opcional via notação:
  - `q` para rainha;
  - `r` para torre;
  - `b` para bispo;
  - `n` para cavalo.
- Encerramento manual da partida com o comando:

```txt
sair
```

### 🖥️ Interface Gráfica 2D

- Projeto desktop criado com WPF;
- Tabuleiro visual 8x8;
- Visual em tons de madeira;
- Peças renderizadas com imagens PNG;
- Seleção de peças com o mouse;
- Movimentação por clique na casa de destino;
- Botões de:
  - Nova partida;
  - Sair.
- Mensagens curtas de status da partida;
- Integração da interface gráfica com o motor de regras existente;
- Máquina joga automaticamente após o movimento do usuário;
- Destaque visual da peça selecionada;
- Destaque dos movimentos legais;
- Exibição apenas de movimentos válidos para a peça selecionada;
- Em situação de xeque, a interface mostra apenas movimentos que resolvem o xeque;
- Bloqueio de tentativas de movimento fora das casas destacadas.

### 👑 Xeque, Xeque-mate e Empate

- Detecção de xeque;
- Detecção de xeque-mate;
- Validação de casas sob ataque;
- Identificação da posição do rei;
- Simulação de movimentos para verificar a segurança do rei;
- Impedimento de movimentos que deixam o próprio rei em xeque;
- Encerramento correto da partida por xeque-mate;
- Detecção de empate por afogamento quando o jogador não está em xeque, mas não possui movimentos legais.

### 🏰 Regras Especiais

- Promoção de peão ao alcançar a última fileira;
- Promoção automática para rainha quando nenhuma peça é informada;
- Escolha da peça de promoção por notação na versão console;
- Roque pequeno;
- Roque grande;
- Roque disponível para peças brancas e pretas;
- Movimento automático da torre durante o roque;
- Bloqueio de roque quando o rei já se moveu;
- Bloqueio de roque quando a torre já se moveu;
- Bloqueio de roque quando há peças entre o rei e a torre;
- Bloqueio de roque quando o rei está em xeque;
- Bloqueio de roque quando o rei passa por casa atacada;
- Bloqueio de roque quando o rei termina em casa atacada;
- Captura en passant;
- En passant disponível apenas imediatamente após o avanço duplo de um peão adversário;
- Remoção correta do peão capturado por en passant.

### 🤖 Bot da Máquina

- Bot para jogar contra o usuário;
- Busca movimentos legais disponíveis;
- Avalia o material do tabuleiro;
- Simula jogadas futuras usando minimax simples;
- Usa profundidade padrão controlada para preservar desempenho;
- Utiliza poda alpha-beta para otimizar a busca;
- Considera consequências futuras antes de escolher uma jogada;
- Respeita xeque, xeque-mate e movimentos legais;
- Não escolhe movimentos que deixam o próprio rei em xeque;
- Não captura diretamente o rei;
- Mantém suporte a roque;
- Mantém suporte a promoção de peão;
- Mantém suporte a en passant;
- Promove peões automaticamente para rainha quando uma promoção estiver disponível.

### 🧪 Qualidade e Validação

- Testes automatizados com xUnit;
- Pipeline de CI com GitHub Actions;
- Coleta de cobertura com Coverlet;
- Badge de cobertura exibido no README;
- Relatório de cobertura disponibilizado pelo pipeline.

---

## 🛠️ Tecnologias Utilizadas

| Camada               | Tecnologia                 |
| -------------------- | -------------------------- |
| Linguagem            | C#                         |
| Plataforma principal | .NET                       |
| Aplicação console    | .NET 8                     |
| Aplicação desktop    | WPF em .NET 9 para Windows |
| Testes               | xUnit                      |
| Cobertura            | Coverlet                   |
| CI/CD                | GitHub Actions             |
| Versionamento        | Git / GitHub               |
| IDE recomendada      | Visual Studio 2022         |

---

## 📁 Estrutura do Projeto

```txt
ChessSharp/
│
├── ChessSharp/
│   ├── AI/
│   │   ├── BoardEvaluator.cs
│   │   └── ChessBot.cs
│   │
│   ├── Board/
│   │   ├── BoardPosition.cs
│   │   └── ChessBoard.cs
│   │
│   ├── Enums/
│   │   ├── PieceColor.cs
│   │   └── PieceType.cs
│   │
│   ├── Game/
│   │   ├── ChessGame.cs
│   │   ├── ChessRules.cs
│   │   ├── GameStatus.cs
│   │   ├── Move.cs
│   │   └── MoveResult.cs
│   │
│   ├── Pieces/
│   │   ├── Bishop.cs
│   │   ├── ChessPiece.cs
│   │   ├── King.cs
│   │   ├── Knight.cs
│   │   ├── Pawn.cs
│   │   ├── Queen.cs
│   │   └── Rook.cs
│   │
│   ├── UI/
│   │   ├── ConsoleRenderer.cs
│   │   └── PlayerColorSelector.cs
│   │
│   └── Program.cs
│
├── ChessSharp.Desktop/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── Assets/
│   │   └── Images/
│   └── ChessSharp.Desktop.csproj
│
├── ChessSharp.Tests/
│   ├── AI/
│   │   └── ChessBotTests.cs
│   │
│   ├── Board/
│   │   └── BoardPositionTests.cs
│   │
│   ├── Game/
│   │   ├── CastlingTests.cs
│   │   ├── ChessGameTests.cs
│   │   └── PawnPromotionTests.cs
│   │
│   └── Pieces/
│       ├── BishopTests.cs
│       ├── KingTests.cs
│       ├── KnightTests.cs
│       ├── PawnTests.cs
│       ├── QueenTests.cs
│       └── RookTests.cs
│
├── .github/
│   └── workflows/
│       └── dotnet-ci.yml
│
├── badges/
│   └── coverage.svg
│
├── ChessSharp.sln
├── README.md
└── LICENSE
```

---

## ▶️ Como Executar o Projeto

### Pré-requisitos

Antes de iniciar, é necessário ter instalado:

- .NET SDK 9;
- Windows para executar a versão desktop;
- Git;
- Visual Studio 2022 ou outro editor compatível com C#.

> O uso do SDK 9 simplifica a execução de toda a solução, porque o projeto possui uma aplicação `net8.0` e uma aplicação `net9.0-windows`.

---

### 1. Clone o repositório

```bash
git clone https://github.com/felipe-frc/chesssharp.git
```

Acesse a pasta do projeto:

```bash
cd chesssharp
```

---

### 2. Restaure as dependências

```bash
dotnet restore
```

---

### 3. Execute a versão console

```bash
dotnet run --project ChessSharp/ChessSharp.csproj
```

---

### 4. Execute a versão desktop 2D

```bash
dotnet run --project ChessSharp.Desktop/ChessSharp.Desktop.csproj
```

---

## 🎮 Como Jogar

### Versão Console

Ao iniciar o jogo no console, o usuário escolhe se deseja jogar com as peças brancas ou pretas.

```txt
1 - Brancas
2 - Pretas
```

Depois da escolha, o tabuleiro será exibido no terminal.

O jogador deve digitar os movimentos usando o formato:

```txt
origem destino
```

Exemplo:

```txt
e2 e4
```

Para promover um peão, é possível informar a peça desejada como terceiro argumento:

```txt
e7 e8 q
```

Opções disponíveis para promoção:

```txt
q - Rainha
r - Torre
b - Bispo
n - Cavalo
```

Para sair da partida no console, digite:

```txt
sair
```

---

### Versão Desktop

Na interface gráfica 2D:

- Clique em uma peça branca;
- As casas legais serão destacadas;
- Clique em uma das casas destacadas para mover;
- A máquina joga automaticamente após o seu movimento;
- Use o botão **New Game** para reiniciar;
- Use o botão **Exit** para fechar o jogo.

Quando o rei estiver em xeque, a interface destacará apenas os movimentos que resolvem o xeque.

### Observações importantes da versão desktop atual

- O jogador controla as peças brancas;
- O bot joga com as peças pretas;
- A promoção via interface gráfica ainda promove automaticamente para rainha;
- A escolha manual da peça de promoção está disponível apenas na versão console.

---

## 🧪 Testes Automatizados

O projeto possui testes automatizados com xUnit cobrindo:

- Conversão de posições do tabuleiro;
- Validação de posições válidas e inválidas;
- Movimentos do peão;
- Movimentos da torre;
- Movimentos do bispo;
- Movimentos do cavalo;
- Movimentos da rainha;
- Movimentos do rei;
- Regras gerais do jogo;
- Alternância de turno;
- Validação de movimentos inválidos;
- Encerramento da partida;
- Detecção de xeque;
- Detecção de xeque-mate;
- Bloqueio de captura direta do rei;
- Bloqueio de movimentos que deixam o próprio rei em xeque;
- Cenário de xeque-mate com Fool's Mate;
- Promoção de peão;
- Promoção para rainha, torre, bispo e cavalo;
- Bloqueio de promoção inválida;
- Roque pequeno;
- Roque grande;
- Bloqueio de roque inválido;
- Bloqueio de roque quando o rei está em xeque;
- Bloqueio de roque passando por casa atacada;
- Validação de movimentos legais do bot;
- Validação de promoção de peão pelo bot;
- Validação para impedir o bot de capturar diretamente o rei;
- Validação do bot com minimax simples.

Para executar os testes:

```bash
dotnet test ChessSharp.Tests/ChessSharp.Tests.csproj
```

Para compilar os projetos principais manualmente:

```bash
dotnet build ChessSharp/ChessSharp.csproj
dotnet build ChessSharp.Desktop/ChessSharp.Desktop.csproj
```

A cobertura dos testes é coletada automaticamente pelo GitHub Actions utilizando **Coverlet**. O badge de cobertura é atualizado no README após a execução do workflow na branch `main`.

---

## 🔄 Integração Contínua

O projeto utiliza **GitHub Actions** para executar automaticamente o fluxo de validação a cada alteração enviada para a branch `main`.

O workflow executa:

- Restauração das dependências;
- Compilação da solução;
- Execução dos testes automatizados;
- Coleta da cobertura de testes;
- Atualização do badge de cobertura;
- Publicação dos resultados do pipeline.

---

## 📊 Cobertura de Testes

A cobertura de testes é gerada automaticamente durante o pipeline de CI.

O processo utiliza:

- **Coverlet** para coletar cobertura dos testes;
- **GitHub Actions** para executar a coleta;
- Geração de badge SVG atualizado no repositório.

O badge de cobertura fica disponível no topo do README:

```md
![Coverage](badges/coverage.svg)
```

---

## 📌 Melhorias e Próximos Passos

Hoje, os principais pontos de evolução do projeto são:

- Permitir que a interface desktop escolha a cor do jogador;
- Adicionar seleção visual da peça de promoção na UI desktop;
- Exibir histórico de jogadas e última jogada realizada;
- Implementar regras adicionais de empate, como repetição de posição, regra dos 50 lances e material insuficiente;
- Permitir configuração de profundidade do bot;
- Separar mais responsabilidades da interface desktop, reduzindo o volume de lógica no `MainWindow.xaml.cs`.

---

## 🗺️ Roadmap

### v1.0.0

- Tabuleiro inicial;
- Peças posicionadas corretamente;
- Movimentação básica das peças;
- Jogador contra máquina;
- Bot simples com prioridade de captura;
- Fim de jogo simplificado;
- Testes automatizados principais;
- GitHub Actions configurado.

### v1.1.0

- Melhorias visuais no tabuleiro do console;
- Casas alternadas com cores;
- Peças Unicode representando peças reais de xadrez;
- Melhor espaçamento visual entre as casas;
- Ajuste de codificação UTF-8 no console;
- Melhor contraste visual das peças no tabuleiro.

### v1.2.0

- Permitir que o jogador escolha jogar com peças brancas ou pretas;
- Ajustar o fluxo da máquina conforme a cor escolhida;
- Melhorar as mensagens iniciais da partida;
- Ajustar a ordem de jogadas quando o usuário escolher jogar com peças pretas.

### v1.3.0

- Implementar detecção de xeque;
- Implementar detecção de xeque-mate;
- Substituir a lógica de captura do rei pelo fluxo correto de xadrez;
- Impedir movimentos que deixam o próprio rei em xeque;
- Impedir movimentos que não resolvem uma situação de xeque;
- Ajustar o bot para considerar apenas movimentos legais;
- Adicionar testes automatizados para xeque e xeque-mate.

### v1.4.0

- Implementar promoção de peão;
- Implementar roque pequeno;
- Implementar roque grande;
- Adicionar testes automatizados para promoção de peão;
- Adicionar testes automatizados para roque;
- Melhorar a cobertura das regras oficiais do xadrez.

### v1.5.0

- Evoluir o bot para minimax simples;
- Implementar avaliação material do tabuleiro;
- Adicionar simulação de jogadas futuras;
- Adicionar poda alpha-beta;
- Melhorar a tomada de decisão da máquina;
- Adicionar testes para a lógica de decisão do bot.

### v1.6.0

- Adicionar coleta de cobertura de testes com Coverlet;
- Adicionar badge de cobertura no README;
- Atualizar o workflow de CI para publicar o badge automaticamente;
- Reforçar a validação de qualidade do projeto.

### v1.7.0

- Implementar en passant;
- Completar as principais regras especiais do xadrez.

### v2.0.0

- Criar projeto desktop com WPF;
- Adicionar interface gráfica 2D;
- Renderizar tabuleiro visual;
- Renderizar peças na interface;
- Permitir seleção de peças com mouse;
- Permitir movimentação por clique;
- Integrar o bot à interface desktop;
- Adicionar botão de nova partida;
- Adicionar botão de sair;
- Melhorar mensagens da interface;
- Destacar peça selecionada;
- Destacar movimentos legais;
- Exibir apenas movimentos válidos em situação de xeque;
- Manter compatibilidade com o motor de regras existente.

### v2.1.0

- Substituir peças Unicode por imagens PNG;
- Melhorar o visual das peças;
- Adicionar texturas ao tabuleiro;
- Criar uma experiência visual mais próxima de jogos de xadrez modernos.

### v2.2.0

- Adicionar modal visual para promoção de peão;
- Permitir escolher a peça promovida diretamente na interface gráfica;
- Melhorar o fluxo visual de regras especiais.

### v3.0.0

- Estudar uma versão 3D do tabuleiro;
- Avaliar o uso de engine gráfica;
- Adicionar modelos 3D de peças;
- Implementar câmera, iluminação e movimentação visual das peças.

---

## 📦 Releases

### v2.3.0 - Refinamento do footer e feedback visual

Versão focada no polimento visual da interface desktop, refinando o footer, os botões de ação, o painel de status e o feedback de seleção e movimentos legais para uma experiência mais consistente com a identidade premium do projeto.

### v2.2.0 - Fundo premium e refinamento da interface desktop

Versão focada na atmosfera visual da interface desktop, adicionando fundo premium em mármore escuro, consolidando o tabuleiro premium e refinando o layout geral da tela para um acabamento mais forte de portfólio.

### v2.1.0 - Tabuleiro premium e refinamento visual do jogo

Versão focada na substituição do tabuleiro renderizado por código por um tabuleiro premium em imagem, com melhor integração visual entre peças, fundo, coordenadas e layout da interface.

### v2.0.1 - Refinamento visual da interface desktop

Versão focada em melhorar a apresentação da primeira interface gráfica do ChessSharp, com ajustes de paleta, logo, harmonia visual, acabamento do layout e preservação do fluxo funcional já existente.

### v2.0.0 - Interface gráfica 2D com WPF

Versão focada na evolução visual do ChessSharp, adicionando uma interface gráfica desktop em WPF, tabuleiro 2D, movimentação por mouse, integração com o bot e marcadores visuais para movimentos legais.

### v1.7.0 - En passant

Versão focada na implementação da regra especial en passant, completando as principais regras especiais do xadrez junto com roque e promoção de peão.

### v1.6.0 - Cobertura de testes com badge

Versão focada na validação de qualidade do projeto, adicionando coleta de cobertura com Coverlet e badge de cobertura no README.

### v1.5.0 - Bot com minimax simples

Versão focada na evolução da inteligência da máquina, adicionando avaliação de tabuleiro, simulação de jogadas futuras, busca minimax simples e poda alpha-beta.

### v1.4.0 - Roque e promoção de peão

Versão focada na implementação de regras especiais do xadrez, adicionando promoção de peão, roque pequeno, roque grande e validações específicas para impedir roques ilegais.

### v1.3.0 - Xeque e xeque-mate real

Versão focada na evolução do motor de regras do ChessSharp, substituindo a lógica simplificada de captura do rei por detecção real de xeque e xeque-mate.

### v1.2.0 - Escolha de cor do jogador

Versão que permite ao jogador escolher se deseja jogar com as peças brancas ou pretas antes do início da partida.

### v1.1.0 - Melhorias visuais no tabuleiro

Versão focada na melhoria visual do ChessSharp no console, com casas coloridas, peças Unicode e melhor apresentação do tabuleiro.

### v1.0.0 - Versão inicial jogável no console

Primeira versão jogável do ChessSharp, com tabuleiro, peças, movimentação, jogador contra máquina, bot simples, testes automatizados e integração contínua.

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👨🏻‍💻 Autor

**Marcos Felipe França**

[LinkedIn](https://www.linkedin.com/in/marcosfelipefrc) · [GitHub](https://github.com/felipe-frc)

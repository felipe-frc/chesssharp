[![CI (.NET)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml)
![GitHub release](https://img.shields.io/github/v/release/felipe-frc/chesssharp)
![GitHub repo size](https://img.shields.io/github/repo-size/felipe-frc/chesssharp)
![GitHub license](https://img.shields.io/github/license/felipe-frc/chesssharp)

# ♟️ ChessSharp

Jogo de xadrez desenvolvido em **C# com .NET**, executado no console, com foco em lógica de programação, orientação a objetos, validação de regras, testes automatizados, organização de código e evolução incremental por releases.

O projeto permite jogar uma partida de xadrez contra a máquina, utilizando comandos no formato padrão de coordenadas do tabuleiro, como `e2 e4`. O jogador pode escolher se deseja jogar com as peças brancas ou pretas, enquanto a máquina controla a cor oposta por meio de um bot com busca minimax simples.

A versão atual conta com renderização visual aprimorada no console, peças Unicode, escolha de cor do jogador, detecção de xeque e xeque-mate, regras especiais como **roque** e **promoção de peão**, além de um bot com avaliação de tabuleiro e simulação de jogadas futuras.

---

## 🎯 Objetivo do Projeto

Este projeto foi desenvolvido com o objetivo de praticar e demonstrar conhecimentos em:

- Desenvolvimento de aplicações em C# com .NET;
- Programação orientada a objetos;
- Modelagem de domínio com classes, enums e responsabilidades bem definidas;
- Implementação de regras de movimentação das peças de xadrez;
- Implementação de regras reais de xeque e xeque-mate;
- Implementação de regras especiais como roque e promoção de peão;
- Manipulação de matrizes para representação de tabuleiro;
- Validação de entradas do usuário;
- Criação de um bot para jogar contra o usuário;
- Implementação de busca minimax simples;
- Avaliação material do tabuleiro;
- Simulação de jogadas futuras;
- Separação entre regra de negócio, renderização, controle do jogo e inteligência da máquina;
- Testes automatizados com xUnit;
- Integração contínua com GitHub Actions;
- Versionamento com Git e GitHub;
- Organização de projeto para portfólio profissional.

---

## ✅ Funcionalidades

### ♟️ Tabuleiro

- Representação de tabuleiro 8x8;
- Conversão de posições no formato de xadrez, como `e2`, `a1` e `h8`;
- Renderização do tabuleiro no console;
- Casas alternadas com cores diferentes;
- Peças representadas por símbolos Unicode reais de xadrez;
- Exibição das coordenadas do tabuleiro;
- Melhor espaçamento visual entre as casas.

### 🧩 Peças

- Implementação das peças principais do xadrez:
  - Peão;
  - Torre;
  - Cavalo;
  - Bispo;
  - Rainha;
  - Rei.
- Validação individual de movimentação por peça;
- Validação de caminho livre para torre, bispo e rainha;
- Captura de peças adversárias;
- Bloqueio de captura de peças da mesma cor;
- Bloqueio de captura direta do rei.

### 🎮 Jogabilidade

- Jogador pode escolher jogar com peças brancas ou pretas;
- Máquina controla automaticamente a cor oposta;
- Quando o jogador escolhe as brancas, ele inicia a partida;
- Quando o jogador escolhe as pretas, a máquina faz o primeiro movimento;
- Entrada de movimentos no formato `origem destino`.

Exemplo:

```txt
e2 e4
```

- Validação de:
  - Movimento inválido;
  - Posição inexistente;
  - Casa de origem vazia;
  - Tentativa de mover peça adversária;
  - Tentativa de capturar peça da mesma cor;
  - Tentativa de capturar diretamente o rei;
  - Turno incorreto;
  - Movimento que deixa o próprio rei em xeque;
  - Movimento que não resolve uma situação de xeque.

### 👑 Xeque e Xeque-mate

- Detecção de xeque;
- Detecção de xeque-mate;
- Validação de casas sob ataque;
- Identificação da posição do rei;
- Simulação de movimentos para verificar a segurança do rei;
- Impedimento de movimentos que deixam o próprio rei em xeque;
- Encerramento correto da partida por xeque-mate.

### 🏰 Regras Especiais

- Promoção de peão ao alcançar a última fileira;
- Promoção automática para rainha quando nenhuma peça é informada;
- Escolha da peça de promoção por notação:
  - `q` para rainha;
  - `r` para torre;
  - `b` para bispo;
  - `n` para cavalo.
- Roque pequeno;
- Roque grande;
- Roque disponível para peças brancas e pretas;
- Movimento automático da torre durante o roque;
- Bloqueio de roque quando o rei já se moveu;
- Bloqueio de roque quando a torre já se moveu;
- Bloqueio de roque quando há peças entre o rei e a torre;
- Bloqueio de roque quando o rei está em xeque;
- Bloqueio de roque quando o rei passa por casa atacada;
- Bloqueio de roque quando o rei termina em casa atacada.

### 🤖 Bot da Máquina

- Bot para jogar contra o usuário;
- Busca movimentos legais disponíveis;
- Avalia o material do tabuleiro;
- Simula jogadas futuras usando minimax simples;
- Usa profundidade padrão controlada para preservar desempenho no console;
- Utiliza poda alpha-beta para otimizar a busca;
- Considera consequências futuras antes de escolher uma jogada;
- Respeita xeque, xeque-mate e movimentos legais;
- Não escolhe movimentos que deixam o próprio rei em xeque;
- Não captura diretamente o rei;
- Mantém suporte a roque;
- Mantém suporte a promoção de peão;
- Promove peões automaticamente para rainha quando uma promoção estiver disponível.

### 🏁 Fim de Jogo

- Vitória das brancas por xeque-mate;
- Vitória das pretas por xeque-mate;
- Vitória do jogador caso a máquina não possua movimentos legais;
- Encerramento manual da partida com o comando:

```txt
sair
```

---

## 🛠️ Tecnologias Utilizadas

| Camada          | Tecnologia         |
| --------------- | ------------------ |
| Linguagem       | C#                 |
| Plataforma      | .NET               |
| Interface       | Console            |
| Testes          | xUnit              |
| CI/CD           | GitHub Actions     |
| Versionamento   | Git / GitHub       |
| IDE recomendada | Visual Studio 2022 |

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
├── ChessSharp.sln
├── README.md
└── LICENSE
```

---

## ▶️ Como Executar o Projeto

### Pré-requisitos

Antes de iniciar, é necessário ter instalado:

- .NET SDK;
- Git;
- Visual Studio 2022 ou outro editor compatível com C#.

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

### 3. Execute o projeto

```bash
dotnet run --project ChessSharp
```

---

## 🎮 Como Jogar

Ao iniciar o jogo, o usuário escolhe se deseja jogar com as peças brancas ou pretas.

```txt
1 - Brancas
2 - Pretas
```

Depois da escolha, o tabuleiro será exibido no console.

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

Após o movimento do jogador, a máquina joga automaticamente com a cor oposta.

Para sair da partida, digite:

```txt
sair
```

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
dotnet test
```

---

## 🔄 Integração Contínua

O projeto utiliza **GitHub Actions** para executar automaticamente o fluxo de validação a cada alteração enviada para a branch `main`.

O workflow executa:

- Restauração das dependências;
- Execução dos testes automatizados;
- Validação da solução no ambiente do GitHub Actions.

---

## 📌 Roadmap

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
- Gerar relatório de cobertura;
- Adicionar badge de cobertura no README;
- Reforçar a validação de qualidade do projeto.

### v1.7.0

- Implementar en passant;
- Adicionar testes automatizados para en passant;
- Completar as principais regras especiais do xadrez.

### v2.0.0

- Criar uma interface gráfica 2D;
- Adicionar tabuleiro visual com aparência de madeira;
- Adicionar peças em imagem;
- Permitir movimentação com mouse;
- Substituir comandos digitados por clique nas casas;
- Melhorar a experiência visual da partida.

### v2.1.0

- Destacar casas disponíveis ao selecionar uma peça;
- Destacar casa de origem e destino;
- Destacar capturas possíveis;
- Melhorar feedback visual para movimentos inválidos;
- Tornar a jogabilidade mais intuitiva.

### v3.0.0

- Estudar uma versão 3D do tabuleiro;
- Avaliar o uso de engine gráfica;
- Adicionar modelos 3D de peças;
- Implementar câmera, iluminação e movimentação visual das peças.

---

## 📦 Releases

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

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

## 👨🏻‍💻 Autor

**Marcos Felipe França**

[LinkedIn](https://www.linkedin.com/in/marcosfelipefrc) · [GitHub](https://github.com/felipe-frc)

[![CI (.NET)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml)
![GitHub release](https://img.shields.io/github/v/release/felipe-frc/chesssharp)
![GitHub repo size](https://img.shields.io/github/repo-size/felipe-frc/chesssharp)
![GitHub license](https://img.shields.io/github/license/felipe-frc/chesssharp)

# ♟️ ChessSharp

Jogo de xadrez desenvolvido em **C# com .NET**, executado no console, com foco em lógica de programação, orientação a objetos, validação de regras, testes automatizados, organização de código e evolução incremental por releases.

O projeto permite jogar uma partida de xadrez simplificada contra a máquina, utilizando comandos no formato padrão de coordenadas do tabuleiro, como `e2 e4`. O jogador controla as peças brancas, enquanto a máquina controla as peças pretas por meio de um bot simples que prioriza capturas de maior valor.

A versão atual também conta com uma renderização visual aprimorada no console, usando casas coloridas e símbolos Unicode para representar as peças reais do xadrez.

---

## 🎯 Objetivo do Projeto

Este projeto foi desenvolvido com o objetivo de praticar e demonstrar conhecimentos em:

- Desenvolvimento de aplicações em C# com .NET;
- Programação orientada a objetos;
- Modelagem de domínio com classes, enums e responsabilidades bem definidas;
- Implementação de regras de movimentação das peças de xadrez;
- Manipulação de matrizes para representação de tabuleiro;
- Validação de entradas do usuário;
- Criação de um bot simples para jogar contra o usuário;
- Separação entre regra de negócio, renderização e controle do jogo;
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
- Bloqueio de captura de peças da mesma cor.

### 🎮 Jogabilidade

- Jogador controla as peças brancas;
- Máquina controla as peças pretas;
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
  - Turno incorreto.

### 🤖 Bot da Máquina

- Bot simples para jogar com as peças pretas;
- Busca todos os movimentos válidos disponíveis;
- Prioriza capturas de peças adversárias;
- Escolhe a captura com maior valor de peça;
- Caso não exista captura, realiza um movimento válido aleatório.

### 🏁 Fim de Jogo Simplificado

- Vitória das brancas quando o rei preto é capturado;
- Vitória das pretas quando o rei branco é capturado;
- Vitória do jogador caso a máquina não tenha movimentos válidos;
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
│   │   └── ConsoleRenderer.cs
│   │
│   └── Program.cs
│
├── ChessSharp.Tests/
│   ├── Board/
│   │   └── BoardPositionTests.cs
│   │
│   ├── Game/
│   │   └── ChessGameTests.cs
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

Ao iniciar o jogo, o tabuleiro será exibido no console.

O jogador controla as peças brancas e deve digitar os movimentos usando o formato:

```txt
origem destino
```

Exemplo:

```txt
e2 e4
```

Após o movimento do jogador, a máquina joga automaticamente com as peças pretas.

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
- Encerramento simplificado da partida.

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

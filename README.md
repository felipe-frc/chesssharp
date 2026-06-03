[![CI (.NET)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/felipe-frc/chesssharp/actions/workflows/dotnet-ci.yml)
![GitHub release](https://img.shields.io/github/v/release/felipe-frc/chesssharp)
![GitHub repo size](https://img.shields.io/github/repo-size/felipe-frc/chesssharp)
![GitHub license](https://img.shields.io/github/license/felipe-frc/chesssharp)

# ♟️ ChessSharp

Jogo de xadrez desenvolvido em **C# com .NET**, executado no console, com foco em lógica de programação, orientação a objetos, validação de regras, testes automatizados e organização de código.

O projeto permite jogar uma partida de xadrez simplificada contra a máquina, utilizando comandos no formato padrão de coordenadas do tabuleiro, como `e2 e4`. O jogador controla as peças brancas, enquanto a máquina controla as peças pretas por meio de um bot simples que prioriza capturas de maior valor.

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
- Versionamento com Git e GitHub;
- Organização de projeto para portfólio profissional.

---

## ✅ Funcionalidades

### ♟️ Tabuleiro

- Representação de tabuleiro 8x8;
- Conversão de posições no formato de xadrez, como `e2`, `a1` e `h8`;
- Renderização do tabuleiro no console;
- Exibição das peças brancas em letras maiúsculas;
- Exibição das peças pretas em letras minúsculas.

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

## 📌 Roadmap

### v1.0.0

- Tabuleiro inicial;
- Peças posicionadas corretamente;
- Movimentação básica das peças;
- Jogador contra máquina;
- Bot simples com prioridade de captura;
- Fim de jogo simplificado;
- Testes automatizados principais.

### Melhorias futuras

- Implementar xeque;
- Implementar xeque-mate real;
- Implementar promoção de peão;
- Implementar roque;
- Implementar en passant;
- Melhorar inteligência da máquina;
- Adicionar histórico de movimentos;
- Melhorar interface visual do console;
- Criar versão gráfica futuramente.

---

## 📦 Releases

### v1.0.0 - Versão inicial jogável no console

Primeira versão jogável do ChessSharp, com tabuleiro, peças, movimentação, jogador contra máquina, bot simples e testes automatizados.

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

## 👨🏻‍💻 Autor

**Marcos Felipe França**

[LinkedIn](https://www.linkedin.com/in/marcosfelipefrc) · [GitHub](https://github.com/felipe-frc)

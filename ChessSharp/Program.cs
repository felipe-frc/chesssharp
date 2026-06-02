using ChessSharp.Board;
using ChessSharp.UI;

var board = new ChessBoard();
board.SetupInitialPosition();

ConsoleRenderer.RenderBoard(board);

Console.WriteLine();
Console.WriteLine("ChessSharp iniciado com peças na posição inicial.");
Console.WriteLine("Pressione qualquer tecla para sair...");
Console.ReadKey();
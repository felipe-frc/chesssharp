using ChessSharp.Board;
using ChessSharp.UI;

var board = new ChessBoard();

ConsoleRenderer.RenderBoard(board);

Console.WriteLine();
Console.WriteLine("ChessSharp iniciado com sucesso.");
Console.WriteLine("Pressione qualquer tecla para sair...");
Console.ReadKey();
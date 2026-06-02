using ChessSharp.Game;
using ChessSharp.UI;

var game = new ChessGame();
string lastMessage = "ChessSharp iniciado. Digite um movimento no formato: e2 e4.";

while (true)
{
    ConsoleRenderer.RenderBoard(game.Board);

    Console.WriteLine();
    Console.WriteLine(lastMessage);
    Console.WriteLine();
    Console.WriteLine($"Vez das peças {ChessGame.GetTurnName(game.CurrentTurn)}.");
    Console.Write("Digite seu movimento ou 'sair': ");

    string? input = Console.ReadLine();

    if (string.Equals(input, "sair", StringComparison.OrdinalIgnoreCase))
        break;

    var result = game.TryMove(input ?? string.Empty);
    lastMessage = result.Message;

    if (!result.Success)
    {
        Console.WriteLine();
        Console.WriteLine(result.Message);
        Console.WriteLine("Pressione qualquer tecla para tentar novamente...");
        Console.ReadKey();
    }
}

Console.WriteLine();
Console.WriteLine("Jogo encerrado.");
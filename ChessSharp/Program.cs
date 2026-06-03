using ChessSharp.AI;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.UI;

var game = new ChessGame();
var bot = new ChessBot(PieceColor.Black);

string lastMessage = "ChessSharp iniciado. Você joga com as peças brancas. Digite um movimento no formato: e2 e4.";

while (true)
{
    ConsoleRenderer.RenderBoard(game.Board);

    Console.WriteLine();
    Console.WriteLine(lastMessage);
    Console.WriteLine();

    if (game.CurrentTurn == PieceColor.White)
    {
        Console.WriteLine("Sua vez.");
        Console.Write("Digite seu movimento ou 'sair': ");

        string? input = Console.ReadLine();

        if (string.Equals(input, "sair", StringComparison.OrdinalIgnoreCase))
            break;

        var playerMoveResult = game.TryMove(input ?? string.Empty);
        lastMessage = playerMoveResult.Message;

        if (!playerMoveResult.Success)
        {
            Console.WriteLine();
            Console.WriteLine(playerMoveResult.Message);
            Console.WriteLine("Pressione qualquer tecla para tentar novamente...");
            Console.ReadKey();
            continue;
        }
    }

    if (game.CurrentTurn == PieceColor.Black)
    {
        ConsoleRenderer.RenderBoard(game.Board);

        Console.WriteLine();
        Console.WriteLine(lastMessage);
        Console.WriteLine();
        Console.WriteLine("A máquina está pensando...");
        Thread.Sleep(700);

        var botMove = bot.ChooseMove(game.Board);

        if (botMove is null)
        {
            lastMessage = "A máquina não possui movimentos válidos. Fim de jogo.";
            break;
        }

        var botMoveResult = game.TryMove(botMove.Value);

        lastMessage = botMoveResult.Success
            ? $"Máquina jogou: {botMove.Value}. {botMoveResult.Message}"
            : $"A máquina tentou um movimento inválido: {botMoveResult.Message}";
    }
}

ConsoleRenderer.RenderBoard(game.Board);

Console.WriteLine();
Console.WriteLine(lastMessage);
Console.WriteLine("Jogo encerrado.");
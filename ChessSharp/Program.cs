using System.Text;
using ChessSharp.AI;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.UI;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var playerColor = PlayerColorSelector.AskPlayerColor();
var botColor = playerColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

var game = new ChessGame();
var bot = new ChessBot(botColor);

string lastMessage = playerColor == PieceColor.White
    ? "ChessSharp iniciado. Você joga com as peças brancas. Digite um movimento no formato: e2 e4."
    : "ChessSharp iniciado. Você joga com as peças pretas. A máquina fará o primeiro movimento.";

while (!game.IsFinished)
{
    ConsoleRenderer.RenderBoard(game.Board, game.CurrentTurn);

    Console.WriteLine();
    Console.WriteLine(lastMessage);
    Console.WriteLine();

    if (game.CurrentTurn == playerColor)
    {
        Console.WriteLine($"Sua vez. Você está jogando com as peças {ChessGame.GetTurnName(playerColor)}.");
        Console.Write("Digite seu movimento ou 'sair': ");

        string? input = Console.ReadLine();

        if (string.Equals(input, "sair", StringComparison.OrdinalIgnoreCase))
        {
            game.FinishByPlayerQuit();
            lastMessage = ChessGame.GetStatusMessage(game.Status);
            break;
        }

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

    if (game.IsFinished)
        break;

    if (game.CurrentTurn == botColor)
    {
        ConsoleRenderer.RenderBoard(game.Board, game.CurrentTurn);

        Console.WriteLine();
        Console.WriteLine(lastMessage);
        Console.WriteLine();
        Console.WriteLine($"A máquina está jogando com as peças {ChessGame.GetTurnName(botColor)}...");
        Thread.Sleep(700);

        var botMove = bot.ChooseMove(game.Board);

        if (botMove is null)
        {
            if (playerColor == PieceColor.White)
            {
                game.FinishWithWhiteWin();
                lastMessage = "A máquina não possui movimentos válidos. Você venceu.";
            }
            else
            {
                lastMessage = "A máquina não possui movimentos válidos. Fim de jogo.";
            }

            break;
        }

        var botMoveResult = game.TryMove(botMove.Value);

        lastMessage = botMoveResult.Success
            ? $"Máquina jogou: {botMove.Value}. {botMoveResult.Message}"
            : $"A máquina tentou um movimento inválido: {botMoveResult.Message}";
    }
}

ConsoleRenderer.RenderBoard(game.Board, game.CurrentTurn);

Console.WriteLine();
Console.WriteLine(lastMessage);
Console.WriteLine(ChessGame.GetStatusMessage(game.Status));
Console.WriteLine("Jogo encerrado.");

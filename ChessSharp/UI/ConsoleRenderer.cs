using ChessSharp.Board;

namespace ChessSharp.UI;

public static class ConsoleRenderer
{
    public static void RenderBoard(ChessBoard board)
    {
        Console.Clear();

        Console.WriteLine("   a  b  c  d  e  f  g  h");
        Console.WriteLine("  -------------------------");

        for (int row = 0; row < 8; row++)
        {
            int rank = 8 - row;

            Console.Write($"{rank} |");

            for (int column = 0; column < 8; column++)
            {
                var position = new BoardPosition(row, column);
                var piece = board.GetPieceAt(position);

                string content = piece?.Symbol ?? ".";

                Console.Write($" {content} ");
            }

            Console.WriteLine($"| {rank}");
        }

        Console.WriteLine("  -------------------------");
        Console.WriteLine("   a  b  c  d  e  f  g  h");
    }
}
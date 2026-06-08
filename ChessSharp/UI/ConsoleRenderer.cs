using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.UI;

public static class ConsoleRenderer
{
    public static void RenderBoard(ChessBoard board)
    {
        Console.Clear();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ♟ ChessSharp");
        Console.ResetColor();

        Console.WriteLine();

        RenderFilesHeader();

        for (int row = 0; row < 8; row++)
        {
            int rank = 8 - row;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {rank} ");

            for (int column = 0; column < 8; column++)
            {
                var position = new BoardPosition(row, column);
                var piece = board.GetPieceAt(position);

                bool isLightSquare = (row + column) % 2 == 0;

                Console.BackgroundColor = isLightSquare
                    ? ConsoleColor.Gray
                    : ConsoleColor.DarkYellow;

                if (piece is null)
                {
                    Console.ForegroundColor = isLightSquare
                        ? ConsoleColor.Gray
                        : ConsoleColor.DarkYellow;

                    Console.Write("     ");
                }
                else
                {
                    Console.ForegroundColor = piece.PieceColor == PieceColor.White
                        ? ConsoleColor.White
                        : ConsoleColor.Black;

                    Console.Write($"  {piece.Symbol}  ");
                }

                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {rank}");
            Console.ResetColor();
            Console.WriteLine();
        }

        RenderFilesHeader();

        Console.WriteLine();
    }

    private static void RenderFilesHeader()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("    ");

        for (char file = 'a'; file <= 'h'; file++)
        {
            Console.Write($"  {file}  ");
        }

        Console.ResetColor();
        Console.WriteLine();
    }
}

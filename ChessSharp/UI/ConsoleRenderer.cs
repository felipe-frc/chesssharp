using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.UI;

public static class ConsoleRenderer
{
    private static readonly Dictionary<PieceType, int> InitialPieceCount = new()
    {
        [PieceType.Pawn] = 8,
        [PieceType.Rook] = 2,
        [PieceType.Knight] = 2,
        [PieceType.Bishop] = 2,
        [PieceType.Queen] = 1,
        [PieceType.King] = 1
    };

    public static void RenderBoard(ChessBoard board, PieceColor currentTurn)
    {
        Console.Clear();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ♙ ChessSharp");
        Console.ResetColor();

        Console.WriteLine();

        RenderFilesHeader();

        var lastMove = board.LastMove;

        for (int row = 0; row < 8; row++)
        {
            int rank = 8 - row;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {rank} ");

            for (int column = 0; column < 8; column++)
            {
                var position = new BoardPosition(row, column);
                var piece = board.GetPieceAt(position);

                bool isLightSquare = (row + column) % 2 == 0;
                bool isLastMoveSquare =
                    lastMove is not null &&
                    (lastMove.Value.Origin == position || lastMove.Value.Target == position);

                Console.BackgroundColor = GetSquareBackground(isLightSquare, isLastMoveSquare);

                if (piece is null)
                {
                    Console.ForegroundColor = Console.BackgroundColor;
                    Console.Write("     ");
                }
                else
                {
                    Console.ForegroundColor = GetPieceColor(piece.PieceColor);
                    Console.Write($"  {GetPieceSymbol(piece.PieceType, piece.PieceColor)}  ");
                }

                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {rank}");
            Console.ResetColor();
            Console.WriteLine();
        }

        RenderFilesHeader();

        Console.WriteLine();

        RenderCheckStatus(board, currentTurn);
        RenderCapturedPieces(board);
    }

    private static void RenderFilesHeader()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("    ");

        for (char file = 'a'; file <= 'h'; file++)
        {
            Console.Write($"  {file}  ");
        }

        Console.ResetColor();
        Console.WriteLine();
    }

    private static ConsoleColor GetSquareBackground(bool isLightSquare, bool isLastMoveSquare)
    {
        if (isLastMoveSquare)
            return ConsoleColor.DarkYellow;

        return isLightSquare
            ? ConsoleColor.Gray
            : ConsoleColor.DarkGray;
    }

    private static ConsoleColor GetPieceColor(PieceColor pieceColor)
    {
        return pieceColor == PieceColor.White
            ? ConsoleColor.White
            : ConsoleColor.Black;
    }

    private static void RenderCheckStatus(ChessBoard board, PieceColor currentTurn)
    {
        if (!ChessRules.IsKingInCheck(board, currentTurn))
            return;

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" XEQUE! O rei está sob ataque.");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void RenderCapturedPieces(ChessBoard board)
    {
        var capturedByWhite = GetCapturedPieces(board, PieceColor.Black);
        var capturedByBlack = GetCapturedPieces(board, PieceColor.White);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(" Capturadas pelas Brancas: ");
        RenderCapturedSymbols(capturedByWhite, PieceColor.Black);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(" Capturadas pelas Pretas:  ");
        RenderCapturedSymbols(capturedByBlack, PieceColor.White);

        Console.WriteLine();
        Console.ResetColor();
    }

    private static List<PieceType> GetCapturedPieces(ChessBoard board, PieceColor capturedColor)
    {
        var currentCounts = InitialPieceCount.ToDictionary(pair => pair.Key, _ => 0);

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var piece = board.GetPieceAt(new BoardPosition(row, column));

                if (piece?.PieceColor == capturedColor &&
                    piece.PieceType != PieceType.King &&
                    currentCounts.ContainsKey(piece.PieceType))
                {
                    currentCounts[piece.PieceType]++;
                }
            }
        }

        var capturedPieces = new List<PieceType>();

        foreach (var (pieceType, initialCount) in InitialPieceCount)
        {
            if (pieceType == PieceType.King)
                continue;

            int capturedCount = initialCount - currentCounts[pieceType];

            for (int count = 0; count < capturedCount; count++)
                capturedPieces.Add(pieceType);
        }

        return capturedPieces;
    }

    private static void RenderCapturedSymbols(List<PieceType> pieces, PieceColor capturedColor)
    {
        if (pieces.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("nenhuma");
            return;
        }

        if (capturedColor == PieceColor.Black)
        {
            foreach (var pieceType in pieces)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($" {GetPieceSymbol(pieceType, capturedColor)} ");
                Console.ResetColor();
                Console.Write(" ");
            }

            Console.WriteLine();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;

        foreach (var pieceType in pieces)
            Console.Write($"{GetPieceSymbol(pieceType, capturedColor)} ");

        Console.ResetColor();
        Console.WriteLine();
    }

    private static string GetPieceSymbol(PieceType pieceType, PieceColor pieceColor)
    {
        return (pieceType, pieceColor) switch
        {
            (PieceType.Pawn, PieceColor.White) => "♙",
            (PieceType.Rook, PieceColor.White) => "♖",
            (PieceType.Knight, PieceColor.White) => "♘",
            (PieceType.Bishop, PieceColor.White) => "♗",
            (PieceType.Queen, PieceColor.White) => "♕",
            (PieceType.King, PieceColor.White) => "♔",
            (PieceType.Pawn, PieceColor.Black) => "♙",
            (PieceType.Rook, PieceColor.Black) => "♜",
            (PieceType.Knight, PieceColor.Black) => "♞",
            (PieceType.Bishop, PieceColor.Black) => "♝",
            (PieceType.Queen, PieceColor.Black) => "♛",
            (PieceType.King, PieceColor.Black) => "♚",
            _ => string.Empty
        };
    }
}

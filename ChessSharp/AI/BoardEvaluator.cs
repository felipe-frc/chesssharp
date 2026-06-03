using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;

namespace ChessSharp.AI;

public static class BoardEvaluator
{
    public static int Evaluate(ChessBoard board, PieceColor perspectiveColor)
    {
        int score = 0;

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var piece = board.GetPieceAt(new BoardPosition(row, column));

                if (piece is null)
                    continue;

                int pieceValue = GetPieceValue(piece);

                score += piece.PieceColor == perspectiveColor
                    ? pieceValue
                    : -pieceValue;
            }
        }

        return score;
    }

    public static int GetPieceValue(ChessPiece piece)
    {
        return piece.PieceType switch
        {
            PieceType.Pawn => 100,
            PieceType.Knight => 320,
            PieceType.Bishop => 330,
            PieceType.Rook => 500,
            PieceType.Queen => 900,
            PieceType.King => 20_000,
            _ => 0
        };
    }
}

using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Game;

public readonly record struct Move(
    BoardPosition Origin,
    BoardPosition Target,
    PieceType? PromotionPieceType = null
)
{
    public override string ToString()
    {
        return PromotionPieceType is null
            ? $"{Origin} {Target}"
            : $"{Origin} {Target} {GetPromotionNotation(PromotionPieceType.Value)}";
    }

    private static string GetPromotionNotation(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Queen => "q",
            PieceType.Rook => "r",
            PieceType.Bishop => "b",
            PieceType.Knight => "n",
            _ => string.Empty
        };
    }
}
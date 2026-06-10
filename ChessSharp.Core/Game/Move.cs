using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Game;

public readonly record struct Move(
    BoardPosition Origin,
    BoardPosition Target,
    PieceType? PromotionPieceType = null,
    bool IsEnPassant = false
)
{
    public override string ToString()
    {
        var baseNotation = PromotionPieceType is null
            ? $"{Origin} {Target}"
            : $"{Origin} {Target} {GetPromotionNotation(PromotionPieceType.Value)}";

        return IsEnPassant
            ? $"{baseNotation} e.p."
            : baseNotation;
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

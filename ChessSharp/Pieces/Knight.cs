using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Pieces;

public class Knight : ChessPiece
{
    public Knight(PieceColor pieceColor)
    : base(pieceColor, PieceType.Knight, pieceColor == PieceColor.White ? "N" : "n")
    {
    }

    public override bool IsValidMove(
        BoardPosition currentPosition,
        BoardPosition targetPosition,
        ChessBoard board
    )
    {
        int rowDifference = Math.Abs(targetPosition.Row - currentPosition.Row);
        int columnDifference = Math.Abs(targetPosition.Column - currentPosition.Column);

        return rowDifference == 2 && columnDifference == 1 ||
               rowDifference == 1 && columnDifference == 2;
    }
}
using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Pieces;

public class Bishop : ChessPiece
{
    public Bishop(PieceColor pieceColor)
        : base(pieceColor, PieceType.Bishop, pieceColor == PieceColor.White ? "♗" : "♝")
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

        bool isDiagonalMove = rowDifference == columnDifference;

        if (!isDiagonalMove)
            return false;

        return board.IsPathClear(currentPosition, targetPosition);
    }
}
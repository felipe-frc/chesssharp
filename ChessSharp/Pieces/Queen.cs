using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Pieces;

public class Queen : ChessPiece
{
    public Queen(PieceColor pieceColor)
    : base(pieceColor, PieceType.Queen, pieceColor == PieceColor.White ? "Q" : "q")
    {
    }

    public override bool IsValidMove(
        BoardPosition currentPosition,
        BoardPosition targetPosition,
        ChessBoard board
    )
    {
        bool isHorizontalMove = currentPosition.Row == targetPosition.Row;
        bool isVerticalMove = currentPosition.Column == targetPosition.Column;

        int rowDifference = Math.Abs(targetPosition.Row - currentPosition.Row);
        int columnDifference = Math.Abs(targetPosition.Column - currentPosition.Column);
        bool isDiagonalMove = rowDifference == columnDifference;

        if (!isHorizontalMove && !isVerticalMove && !isDiagonalMove)
            return false;

        return board.IsPathClear(currentPosition, targetPosition);
    }
}
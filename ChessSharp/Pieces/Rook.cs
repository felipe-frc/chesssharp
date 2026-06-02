using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Pieces;

public class Rook : ChessPiece
{
    public Rook(PieceColor pieceColor)
      : base(pieceColor, PieceType.Rook, pieceColor == PieceColor.White ? "R" : "r")
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

        if (!isHorizontalMove && !isVerticalMove)
            return false;

        return board.IsPathClear(currentPosition, targetPosition);
    }
}
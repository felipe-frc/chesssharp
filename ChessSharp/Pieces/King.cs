using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Pieces;

public class King : ChessPiece
{
    public King(PieceColor pieceColor)
        : base(pieceColor, PieceType.King, pieceColor == PieceColor.White ? "♔" : "♚")
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

        return rowDifference <= 1 && columnDifference <= 1;
    }
}
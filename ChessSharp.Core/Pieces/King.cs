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

        bool isStandardKingMove = rowDifference <= 1 && columnDifference <= 1;

        if (isStandardKingMove)
            return true;

        bool isCastlingAttempt =
            rowDifference == 0 &&
            columnDifference == 2 &&
            currentPosition.Column == 4 &&
            targetPosition.Column is 2 or 6 &&
            currentPosition.Row is 0 or 7;

        return isCastlingAttempt;
    }
}

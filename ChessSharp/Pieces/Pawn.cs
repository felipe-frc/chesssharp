using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Pieces;

public class Pawn : ChessPiece
{
    public Pawn(PieceColor pieceColor)
     : base(pieceColor, PieceType.Pawn, pieceColor == PieceColor.White ? "P" : "p")
    {
    }

    public override bool IsValidMove(
        BoardPosition currentPosition,
        BoardPosition targetPosition,
        ChessBoard board
    )
    {
        int direction = PieceColor == PieceColor.White ? -1 : 1;
        int startRow = PieceColor == PieceColor.White ? 6 : 1;

        int rowDifference = targetPosition.Row - currentPosition.Row;
        int columnDifference = targetPosition.Column - currentPosition.Column;

        bool isMovingForwardOneSquare =
            columnDifference == 0 &&
            rowDifference == direction &&
            board.IsEmpty(targetPosition);

        if (isMovingForwardOneSquare)
            return true;

        bool isMovingForwardTwoSquares =
            columnDifference == 0 &&
            currentPosition.Row == startRow &&
            rowDifference == direction * 2;

        if (isMovingForwardTwoSquares)
        {
            var intermediatePosition = new BoardPosition(
                currentPosition.Row + direction,
                currentPosition.Column
            );

            return board.IsEmpty(intermediatePosition) && board.IsEmpty(targetPosition);
        }

        bool isCapturingDiagonally =
            Math.Abs(columnDifference) == 1 &&
            rowDifference == direction &&
            !board.IsEmpty(targetPosition);

        if (isCapturingDiagonally)
        {
            var targetPiece = board.GetPieceAt(targetPosition);
            return targetPiece is not null && targetPiece.PieceColor != PieceColor;
        }

        return false;
    }
}
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;

namespace ChessSharp.Game;

public static class ChessRules
{
    public static bool IsKingInCheck(ChessBoard board, PieceColor kingColor)
    {
        var kingPosition = board.FindKingPosition(kingColor);

        if (kingPosition is null)
            return false;

        return IsSquareUnderAttack(board, kingPosition.Value, GetOpponentColor(kingColor));
    }

    public static bool IsCheckmate(ChessBoard board, PieceColor kingColor)
    {
        return IsKingInCheck(board, kingColor) && !HasAnyLegalMove(board, kingColor);
    }

    public static bool HasAnyLegalMove(ChessBoard board, PieceColor pieceColor)
    {
        return GetLegalMoves(board, pieceColor).Count > 0;
    }

    public static List<Move> GetLegalMoves(ChessBoard board, PieceColor pieceColor)
    {
        var legalMoves = new List<Move>();

        for (int originRow = 0; originRow < 8; originRow++)
        {
            for (int originColumn = 0; originColumn < 8; originColumn++)
            {
                var origin = new BoardPosition(originRow, originColumn);
                var piece = board.GetPieceAt(origin);

                if (piece is null || piece.PieceColor != pieceColor)
                    continue;

                for (int targetRow = 0; targetRow < 8; targetRow++)
                {
                    for (int targetColumn = 0; targetColumn < 8; targetColumn++)
                    {
                        var target = new BoardPosition(targetRow, targetColumn);
                        var move = new Move(origin, target);

                        if (IsLegalMove(board, move, pieceColor))
                            legalMoves.Add(move);
                    }
                }
            }
        }

        return legalMoves;
    }

    public static bool IsLegalMove(ChessBoard board, Move move, PieceColor currentTurn)
    {
        if (move.Origin == move.Target)
            return false;

        var piece = board.GetPieceAt(move.Origin);

        if (piece is null || piece.PieceColor != currentTurn)
            return false;

        var targetPiece = board.GetPieceAt(move.Target);

        if (targetPiece is not null && targetPiece.PieceColor == piece.PieceColor)
            return false;

        if (targetPiece is not null && targetPiece.PieceType == PieceType.King)
            return false;

        if (!piece.IsValidMove(move.Origin, move.Target, board))
            return false;

        var simulatedBoard = board.Clone();
        simulatedBoard.MovePiece(move.Origin, move.Target);

        return !IsKingInCheck(simulatedBoard, currentTurn);
    }

    public static bool IsSquareUnderAttack(
        ChessBoard board,
        BoardPosition square,
        PieceColor attackingColor
    )
    {
        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var origin = new BoardPosition(row, column);
                var attackingPiece = board.GetPieceAt(origin);

                if (attackingPiece is null || attackingPiece.PieceColor != attackingColor)
                    continue;

                if (CanPieceAttackSquare(board, origin, square, attackingPiece))
                    return true;
            }
        }

        return false;
    }

    public static PieceColor GetOpponentColor(PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    private static bool CanPieceAttackSquare(
        ChessBoard board,
        BoardPosition origin,
        BoardPosition target,
        ChessPiece attackingPiece
    )
    {
        if (origin == target)
            return false;

        return attackingPiece.PieceType switch
        {
            PieceType.Pawn => CanPawnAttackSquare(origin, target, attackingPiece.PieceColor),
            PieceType.King => CanKingAttackSquare(origin, target),
            _ => attackingPiece.IsValidMove(origin, target, board)
        };
    }

    private static bool CanPawnAttackSquare(
        BoardPosition origin,
        BoardPosition target,
        PieceColor pawnColor
    )
    {
        int direction = pawnColor == PieceColor.White ? -1 : 1;
        int rowDifference = target.Row - origin.Row;
        int columnDifference = Math.Abs(target.Column - origin.Column);

        return rowDifference == direction && columnDifference == 1;
    }

    private static bool CanKingAttackSquare(BoardPosition origin, BoardPosition target)
    {
        int rowDifference = Math.Abs(target.Row - origin.Row);
        int columnDifference = Math.Abs(target.Column - origin.Column);

        return rowDifference <= 1 && columnDifference <= 1;
    }
}

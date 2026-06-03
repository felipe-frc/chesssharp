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

    public static bool IsStalemate(ChessBoard board, PieceColor color)
    {
        return !IsKingInCheck(board, color) && !HasAnyLegalMove(board, color);
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
                        var move = CreateMove(board, piece, origin, target);

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

        if (IsCastlingMove(board, move, currentTurn))
            return IsCastlingLegal(board, move, currentTurn);

        bool isEnPassant = IsEnPassantMove(board, move, currentTurn);

        if (!isEnPassant && !piece.IsValidMove(move.Origin, move.Target, board))
            return false;

        var simulatedBoard = board.Clone();

        if (isEnPassant)
        {
            var capturedPawnPosition = board.LastMove!.Value.Target;
            simulatedBoard.SetPieceAt(capturedPawnPosition, null);
        }

        simulatedBoard.MovePiece(move.Origin, move.Target);

        if (IsPawnPromotionMove(piece, move.Target))
        {
            var promotionPiece = CreatePromotedPiece(
                piece.PieceColor,
                move.PromotionPieceType ?? PieceType.Queen
            );

            simulatedBoard.SetPieceAt(move.Target, promotionPiece);
        }

        return !IsKingInCheck(simulatedBoard, currentTurn);
    }

    public static bool IsCastlingMove(ChessBoard board, Move move, PieceColor currentTurn)
    {
        var piece = board.GetPieceAt(move.Origin);

        if (piece is null || piece.PieceColor != currentTurn || piece.PieceType != PieceType.King)
            return false;

        int rowDifference = Math.Abs(move.Target.Row - move.Origin.Row);
        int columnDifference = Math.Abs(move.Target.Column - move.Origin.Column);

        return rowDifference == 0 && columnDifference == 2;
    }

    public static bool IsEnPassantMove(ChessBoard board, Move move, PieceColor currentTurn)
    {
        var piece = board.GetPieceAt(move.Origin);

        if (piece is null || piece.PieceColor != currentTurn || piece.PieceType != PieceType.Pawn)
            return false;

        if (!board.IsEmpty(move.Target))
            return false;

        int direction = currentTurn == PieceColor.White ? -1 : 1;
        int rowDifference = move.Target.Row - move.Origin.Row;
        int columnDifference = Math.Abs(move.Target.Column - move.Origin.Column);

        if (rowDifference != direction || columnDifference != 1)
            return false;

        if (board.LastMove is null)
            return false;

        var lastMove = board.LastMove.Value;
        var lastMovedPiece = board.GetPieceAt(lastMove.Target);

        if (lastMovedPiece is null ||
            lastMovedPiece.PieceColor != GetOpponentColor(currentTurn) ||
            lastMovedPiece.PieceType != PieceType.Pawn)
        {
            return false;
        }

        bool lastMoveWasTwoSquarePawnMove =
            Math.Abs(lastMove.Target.Row - lastMove.Origin.Row) == 2;

        if (!lastMoveWasTwoSquarePawnMove)
            return false;

        return lastMove.Target.Row == move.Origin.Row &&
               lastMove.Target.Column == move.Target.Column;
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

    private static bool IsCastlingLegal(ChessBoard board, Move move, PieceColor currentTurn)
    {
        var king = board.GetPieceAt(move.Origin);

        if (king is null || king.PieceType != PieceType.King || king.HasMoved)
            return false;

        int expectedRow = currentTurn == PieceColor.White ? 7 : 0;

        if (move.Origin.Row != expectedRow || move.Origin.Column != 4)
            return false;

        if (move.Target.Row != expectedRow || move.Target.Column is not (2 or 6))
            return false;

        if (!board.IsEmpty(move.Target))
            return false;

        bool isKingSideCastle = move.Target.Column == 6;
        int rookColumn = isKingSideCastle ? 7 : 0;
        var rookPosition = new BoardPosition(expectedRow, rookColumn);
        var rook = board.GetPieceAt(rookPosition);

        if (rook is null ||
            rook.PieceColor != currentTurn ||
            rook.PieceType != PieceType.Rook ||
            rook.HasMoved)
        {
            return false;
        }

        if (!board.IsPathClear(move.Origin, rookPosition))
            return false;

        if (IsKingInCheck(board, currentTurn))
            return false;

        var opponentColor = GetOpponentColor(currentTurn);
        int direction = isKingSideCastle ? 1 : -1;

        var firstKingStep = new BoardPosition(expectedRow, move.Origin.Column + direction);
        var secondKingStep = new BoardPosition(expectedRow, move.Origin.Column + direction * 2);

        if (IsSquareUnderAttack(board, firstKingStep, opponentColor))
            return false;

        if (IsSquareUnderAttack(board, secondKingStep, opponentColor))
            return false;

        return true;
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

    private static Move CreateMove(
        ChessBoard board,
        ChessPiece piece,
        BoardPosition origin,
        BoardPosition target
    )
    {
        bool isPromotion =
            piece.PieceType == PieceType.Pawn &&
            (piece.PieceColor == PieceColor.White ? target.Row == 0 : target.Row == 7);

        bool isEnPassant = IsEnPassantMove(
            board,
            new Move(origin, target),
            piece.PieceColor
        );

        if (isPromotion)
            return new Move(origin, target, PieceType.Queen, isEnPassant);

        return isEnPassant
            ? new Move(origin, target, null, true)
            : new Move(origin, target);
    }

    private static bool IsPawnPromotionMove(ChessPiece piece, BoardPosition targetPosition)
    {
        if (piece.PieceType != PieceType.Pawn)
            return false;

        return piece.PieceColor == PieceColor.White
            ? targetPosition.Row == 0
            : targetPosition.Row == 7;
    }

    private static ChessPiece CreatePromotedPiece(PieceColor pieceColor, PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Queen => new Queen(pieceColor),
            PieceType.Rook => new Rook(pieceColor),
            PieceType.Bishop => new Bishop(pieceColor),
            PieceType.Knight => new Knight(pieceColor),
            _ => throw new ArgumentException("Tipo de peça inválido para promoção.", nameof(pieceType))
        };
    }
}
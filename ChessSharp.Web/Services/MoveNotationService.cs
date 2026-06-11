using ChessSharp.Board;
using ChessSharp.Game;

namespace ChessSharp.Web.Services;

public sealed class MoveNotationService
{
    public string BuildMoveNotation(ChessBoard board, Move move)
    {
        if (ChessRules.IsCastlingMove(board, move, board.GetPieceAt(move.Origin)?.PieceColor ?? default))
            return move.Target.Column == 6 ? "O-O" : "O-O-O";

        bool isEnPassant = board.GetPieceAt(move.Origin) is not null &&
            ChessRules.IsEnPassantMove(
                board,
                move,
                board.GetPieceAt(move.Origin)!.PieceColor);

        string separator = isEnPassant || board.GetPieceAt(move.Target) is not null
            ? " × "
            : " → ";

        string notation = $"{GetBoardNotation(move.Origin)}{separator}{GetBoardNotation(move.Target)}";

        if (move.PromotionPieceType is not null)
            notation += $" = {ChessPresentationService.GetPieceTypeName(move.PromotionPieceType.Value)}";

        return notation;
    }

    private static string GetBoardNotation(BoardPosition position) =>
        $"{(char)('a' + position.Column)}{8 - position.Row}";
}

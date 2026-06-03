using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Pieces;

namespace ChessSharp.AI;

public class ChessBot
{
    private readonly Random _random = new();

    public ChessBot(PieceColor botColor)
    {
        BotColor = botColor;
    }

    public PieceColor BotColor { get; }

    public Move? ChooseMove(ChessBoard board)
    {
        var validMoves = GetAllValidMoves(board);

        if (validMoves.Count == 0)
            return null;

        var captureMoves = validMoves
            .Where(move => board.GetPieceAt(move.Target) is not null)
            .OrderByDescending(move => GetPieceValue(board.GetPieceAt(move.Target)!))
            .ToList();

        if (captureMoves.Count > 0)
            return captureMoves[0];

        int randomIndex = _random.Next(validMoves.Count);
        return validMoves[randomIndex];
    }

    private List<Move> GetAllValidMoves(ChessBoard board)
    {
        var moves = new List<Move>();

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var origin = new BoardPosition(row, column);
                var piece = board.GetPieceAt(origin);

                if (piece is null || piece.PieceColor != BotColor)
                    continue;

                AddValidMovesForPiece(board, origin, piece, moves);
            }
        }

        return moves;
    }

    private void AddValidMovesForPiece(
        ChessBoard board,
        BoardPosition origin,
        ChessPiece piece,
        List<Move> moves
    )
    {
        for (int targetRow = 0; targetRow < 8; targetRow++)
        {
            for (int targetColumn = 0; targetColumn < 8; targetColumn++)
            {
                var target = new BoardPosition(targetRow, targetColumn);

                if (origin == target)
                    continue;

                var targetPiece = board.GetPieceAt(target);

                if (targetPiece is not null && targetPiece.PieceColor == piece.PieceColor)
                    continue;

                if (piece.IsValidMove(origin, target, board))
                    moves.Add(new Move(origin, target));
            }
        }
    }

    private static int GetPieceValue(ChessPiece piece)
    {
        return piece.PieceType switch
        {
            PieceType.Pawn => 1,
            PieceType.Knight => 3,
            PieceType.Bishop => 3,
            PieceType.Rook => 5,
            PieceType.Queen => 9,
            PieceType.King => 100,
            _ => 0
        };
    }
}
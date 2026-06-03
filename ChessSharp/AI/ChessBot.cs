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
        var validMoves = ChessRules.GetLegalMoves(board, BotColor);

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

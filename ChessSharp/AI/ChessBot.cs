using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
namespace ChessSharp.AI;

public class ChessBot
{
    private const int CheckmateScore = 100_000;

    public ChessBot(PieceColor botColor, int searchDepth = 2)
    {
        if (searchDepth < 1)
            throw new ArgumentOutOfRangeException(nameof(searchDepth), "A profundidade de busca deve ser maior que zero.");

        BotColor = botColor;
        SearchDepth = searchDepth;
    }

    public PieceColor BotColor { get; }
    public int SearchDepth { get; }

    public Move? ChooseMove(ChessBoard board)
    {
        var legalMoves = ChessRules.GetLegalMoves(board, BotColor);

        if (legalMoves.Count == 0)
            return null;

        Move? bestMove = null;
        int bestScore = int.MinValue;

        foreach (var move in OrderMoves(board, legalMoves))
        {
            var simulatedBoard = board.Clone();
            ApplyMove(simulatedBoard, move);

            int score = Minimax(
                simulatedBoard,
                SearchDepth - 1,
                ChessRules.GetOpponentColor(BotColor),
                int.MinValue,
                int.MaxValue
            );

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int Minimax(
        ChessBoard board,
        int depth,
        PieceColor currentTurn,
        int alpha,
        int beta
    )
    {
        if (ChessRules.IsCheckmate(board, BotColor))
            return -CheckmateScore - depth;

        if (ChessRules.IsCheckmate(board, ChessRules.GetOpponentColor(BotColor)))
            return CheckmateScore + depth;

        if (depth == 0)
            return BoardEvaluator.Evaluate(board, BotColor);

        var legalMoves = ChessRules.GetLegalMoves(board, currentTurn);

        if (legalMoves.Count == 0)
            return BoardEvaluator.Evaluate(board, BotColor);

        bool isMaximizing = currentTurn == BotColor;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            foreach (var move in OrderMoves(board, legalMoves))
            {
                var simulatedBoard = board.Clone();
                ApplyMove(simulatedBoard, move);

                int score = Minimax(
                    simulatedBoard,
                    depth - 1,
                    ChessRules.GetOpponentColor(currentTurn),
                    alpha,
                    beta
                );

                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, score);

                if (beta <= alpha)
                    break;
            }

            return bestScore;
        }

        int worstScore = int.MaxValue;

        foreach (var move in OrderMoves(board, legalMoves))
        {
            var simulatedBoard = board.Clone();
            ApplyMove(simulatedBoard, move);

            int score = Minimax(
                simulatedBoard,
                depth - 1,
                ChessRules.GetOpponentColor(currentTurn),
                alpha,
                beta
            );

            worstScore = Math.Min(worstScore, score);
            beta = Math.Min(beta, score);

            if (beta <= alpha)
                break;
        }

        return worstScore;
    }

    private static IEnumerable<Move> OrderMoves(ChessBoard board, IEnumerable<Move> moves)
    {
        return moves
            .OrderByDescending(move => GetMovePriority(board, move))
            .ThenBy(move => move.ToString(), StringComparer.Ordinal);
    }

    private static int GetMovePriority(ChessBoard board, Move move)
    {
        var movingPiece = board.GetPieceAt(move.Origin);
        var targetPiece = board.GetPieceAt(move.Target);

        int priority = 0;

        if (targetPiece is not null)
            priority += BoardEvaluator.GetPieceValue(targetPiece) * 10;

        if (movingPiece is not null && move.PromotionPieceType is not null)
            priority += BoardEvaluator.GetPieceValue(
            ChessRules.CreatePromotedPiece(movingPiece.PieceColor, move.PromotionPieceType.Value)
);  
        return priority;
    }

    private static void ApplyMove(ChessBoard board, Move move)
    {
        var piece = board.GetPieceAt(move.Origin);

        if (piece is null)
            throw new InvalidOperationException("Não existe peça na posição de origem.");

        if (ChessRules.IsCastlingMove(board, move, piece.PieceColor))
        {
            ApplyCastlingMove(board, move);
            board.RegisterMove(move);
            return;
        }

        if (ChessRules.IsEnPassantMove(board, move, piece.PieceColor))
        {
            var capturedPawnPosition = board.LastMove!.Value.Target;

            board.SetPieceAt(capturedPawnPosition, null);
            board.MovePiece(move.Origin, move.Target);
            piece.MarkAsMoved();
            board.RegisterMove(move with { IsEnPassant = true });

            return;
        }

        board.MovePiece(move.Origin, move.Target);
        piece.MarkAsMoved();

        if (move.PromotionPieceType is not null)
        {
            var promotedPiece = ChessRules.CreatePromotedPiece(piece.PieceColor, move.PromotionPieceType.Value);
            promotedPiece.MarkAsMoved();

            board.SetPieceAt(move.Target, promotedPiece);
        }

        board.RegisterMove(move);
    }

    private static void ApplyCastlingMove(ChessBoard board, Move move)
    {
        var king = board.GetPieceAt(move.Origin);

        if (king is null)
            throw new InvalidOperationException("Não existe rei na posição de origem do roque.");

        bool isKingSideCastle = move.Target.Column == 6;
        int row = move.Origin.Row;
        int rookOriginColumn = isKingSideCastle ? 7 : 0;
        int rookTargetColumn = isKingSideCastle ? 5 : 3;

        var rookOrigin = new BoardPosition(row, rookOriginColumn);
        var rookTarget = new BoardPosition(row, rookTargetColumn);
        var rook = board.GetPieceAt(rookOrigin);

        if (rook is null)
            throw new InvalidOperationException("Não existe torre na posição de origem do roque.");

        board.MovePiece(move.Origin, move.Target);
        board.MovePiece(rookOrigin, rookTarget);

        king.MarkAsMoved();
        rook.MarkAsMoved();
    }

}

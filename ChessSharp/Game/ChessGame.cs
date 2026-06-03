using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;

namespace ChessSharp.Game;

public class ChessGame
{
    public ChessGame()
    {
        Board = new ChessBoard();
        Board.SetupInitialPosition();
        CurrentTurn = PieceColor.White;
        Status = GameStatus.InProgress;
    }

    public ChessBoard Board { get; }
    public PieceColor CurrentTurn { get; private set; }
    public GameStatus Status { get; private set; }

    public bool IsFinished => Status != GameStatus.InProgress;

    public MoveResult TryMove(string input)
    {
        if (IsFinished)
            return MoveResult.Invalid("A partida já foi encerrada.");

        if (string.IsNullOrWhiteSpace(input))
            return MoveResult.Invalid("Digite um movimento no formato: e2 e4.");

        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is < 2 or > 3)
            return MoveResult.Invalid("Formato inválido. Use o formato: e2 e4 ou e7 e8 q.");

        BoardPosition origin;
        BoardPosition target;

        try
        {
            origin = BoardPosition.FromChessNotation(parts[0]);
            target = BoardPosition.FromChessNotation(parts[1]);
        }
        catch (ArgumentException exception)
        {
            return MoveResult.Invalid(exception.Message);
        }

        PieceType? promotionPieceType = null;

        if (parts.Length == 3)
        {
            if (!TryParsePromotionPiece(parts[2], out promotionPieceType))
            {
                return MoveResult.Invalid(
                    "Peça de promoção inválida. Use q para rainha, r para torre, b para bispo ou n para cavalo."
                );
            }
        }

        return TryMove(new Move(origin, target, promotionPieceType));
    }

    public MoveResult TryMove(Move move)
    {
        if (IsFinished)
            return MoveResult.Invalid("A partida já foi encerrada.");

        if (move.Origin == move.Target)
            return MoveResult.Invalid("A posição de origem e destino não podem ser iguais.");

        var piece = Board.GetPieceAt(move.Origin);

        if (piece is null)
            return MoveResult.Invalid("Não existe peça na posição de origem.");

        if (piece.PieceColor != CurrentTurn)
            return MoveResult.Invalid($"Não é a vez das peças {GetTurnName(CurrentTurn)}.");

        var targetPiece = Board.GetPieceAt(move.Target);

        if (targetPiece is not null && targetPiece.PieceColor == piece.PieceColor)
            return MoveResult.Invalid("Você não pode capturar uma peça da mesma cor.");

        if (targetPiece is not null && targetPiece.PieceType == PieceType.King)
            return MoveResult.Invalid("O rei não pode ser capturado diretamente.");

        if (move.PromotionPieceType is not null && !IsPawnPromotionMove(piece, move.Target))
            return MoveResult.Invalid("Promoção só é permitida quando um peão alcança a última fileira.");

        if (!piece.IsValidMove(move.Origin, move.Target, Board))
            return MoveResult.Invalid("Movimento inválido para essa peça.");

        if (!ChessRules.IsLegalMove(Board, move, CurrentTurn))
            return MoveResult.Invalid("Movimento inválido. O rei ficaria em xeque.");

        string moveMessage;

        if (ChessRules.IsCastlingMove(Board, move, CurrentTurn))
        {
            ExecuteCastlingMove(move);
            moveMessage = move.Target.Column == 6
                ? "Roque pequeno realizado."
                : "Roque grande realizado.";
        }
        else
        {
            Board.MovePiece(move.Origin, move.Target);
            piece.MarkAsMoved();

            moveMessage = targetPiece is null
                ? $"Movimento realizado: {move.Origin} para {move.Target}."
                : $"Movimento realizado: {move.Origin} capturou {targetPiece.PieceType} em {move.Target}.";

            if (IsPawnPromotionMove(piece, move.Target))
            {
                var promotionPieceType = move.PromotionPieceType ?? PieceType.Queen;
                var promotedPiece = CreatePromotedPiece(piece.PieceColor, promotionPieceType);

                Board.SetPieceAt(move.Target, promotedPiece);

                moveMessage += $" Peão promovido para {GetPieceName(promotionPieceType)}.";
            }
        }

        var opponentColor = CurrentTurn == PieceColor.White
            ? PieceColor.Black
            : PieceColor.White;

        if (ChessRules.IsCheckmate(Board, opponentColor))
        {
            Status = CurrentTurn == PieceColor.White
                ? GameStatus.WhiteWins
                : GameStatus.BlackWins;

            moveMessage += " Xeque-mate.";
            return MoveResult.Valid(moveMessage);
        }

        ChangeTurn();

        return MoveResult.Valid(moveMessage);
    }

    public void FinishByPlayerQuit()
    {
        Status = GameStatus.PlayerQuit;
    }

    public void FinishWithWhiteWin()
    {
        Status = GameStatus.WhiteWins;
    }

    private void ExecuteCastlingMove(Move move)
    {
        var king = Board.GetPieceAt(move.Origin);

        if (king is null)
            throw new InvalidOperationException("Não existe rei na posição de origem do roque.");

        bool isKingSideCastle = move.Target.Column == 6;

        int row = move.Origin.Row;
        int rookOriginColumn = isKingSideCastle ? 7 : 0;
        int rookTargetColumn = isKingSideCastle ? 5 : 3;

        var rookOrigin = new BoardPosition(row, rookOriginColumn);
        var rookTarget = new BoardPosition(row, rookTargetColumn);

        var rook = Board.GetPieceAt(rookOrigin);

        if (rook is null)
            throw new InvalidOperationException("Não existe torre na posição de origem do roque.");

        Board.MovePiece(move.Origin, move.Target);
        Board.MovePiece(rookOrigin, rookTarget);

        king.MarkAsMoved();
        rook.MarkAsMoved();
    }

    private static bool IsPawnPromotionMove(ChessPiece piece, BoardPosition targetPosition)
    {
        if (piece.PieceType != PieceType.Pawn)
            return false;

        return piece.PieceColor == PieceColor.White
            ? targetPosition.Row == 0
            : targetPosition.Row == 7;
    }

    private static bool TryParsePromotionPiece(string value, out PieceType? pieceType)
    {
        pieceType = value.ToLowerInvariant() switch
        {
            "q" => PieceType.Queen,
            "r" => PieceType.Rook,
            "b" => PieceType.Bishop,
            "n" => PieceType.Knight,
            _ => null
        };

        return pieceType is not null;
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

    private static string GetPieceName(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Queen => "rainha",
            PieceType.Rook => "torre",
            PieceType.Bishop => "bispo",
            PieceType.Knight => "cavalo",
            _ => "peça"
        };
    }

    private void ChangeTurn()
    {
        CurrentTurn = CurrentTurn == PieceColor.White
            ? PieceColor.Black
            : PieceColor.White;
    }

    public static string GetTurnName(PieceColor color)
    {
        return color == PieceColor.White ? "brancas" : "pretas";
    }

    public static string GetStatusMessage(GameStatus status)
    {
        return status switch
        {
            GameStatus.InProgress => "Partida em andamento.",
            GameStatus.WhiteWins => "Fim de jogo. As peças brancas venceram.",
            GameStatus.BlackWins => "Fim de jogo. As peças pretas venceram.",
            GameStatus.Draw => "Fim de jogo. A partida terminou empatada.",
            GameStatus.PlayerQuit => "Partida encerrada pelo jogador.",
            _ => "Status desconhecido."
        };
    }
}

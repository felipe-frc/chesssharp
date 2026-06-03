using ChessSharp.Board;
using ChessSharp.Enums;

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

        if (parts.Length != 2)
            return MoveResult.Invalid("Formato inválido. Use o formato: e2 e4.");

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

        return TryMove(new Move(origin, target));
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
            return MoveResult.Invalid("O rei não pode ser capturado. A partida deve terminar por xeque-mate.");

        if (!piece.IsValidMove(move.Origin, move.Target, Board))
            return MoveResult.Invalid("Movimento inválido para essa peça.");

        if (!ChessRules.IsLegalMove(Board, move, CurrentTurn))
            return MoveResult.Invalid("Movimento inválido: seu rei ficaria ou permaneceria em xeque.");

        Board.MovePiece(move.Origin, move.Target);
        piece.MarkAsMoved();

        string moveMessage = targetPiece is null
            ? $"Movimento realizado: {move.Origin} para {move.Target}."
            : $"Movimento realizado: {move.Origin} capturou {targetPiece.PieceType} em {move.Target}.";

        var opponentColor = ChessRules.GetOpponentColor(CurrentTurn);

        if (ChessRules.IsCheckmate(Board, opponentColor))
        {
            Status = CurrentTurn == PieceColor.White
                ? GameStatus.WhiteWins
                : GameStatus.BlackWins;

            return MoveResult.Valid($"{moveMessage} Xeque-mate.");
        }

        if (ChessRules.IsKingInCheck(Board, opponentColor))
            moveMessage = $"{moveMessage} Xeque.";

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

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
    }

    public ChessBoard Board { get; }
    public PieceColor CurrentTurn { get; private set; }

    public MoveResult TryMove(string input)
    {
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
        if (move.Origin == move.Target)
            return MoveResult.Invalid("A posição de origem e destino não podem ser iguais.");

        var piece = Board.GetPieceAt(move.Origin);

        if (piece is null)
            return MoveResult.Invalid("Não existe peça na posição de origem.");

        if (piece.PieceColor != CurrentTurn)
            return MoveResult.Invalid($"Não é a vez das peças {GetTurnName(piece.PieceColor)}.");

        var targetPiece = Board.GetPieceAt(move.Target);

        if (targetPiece is not null && targetPiece.PieceColor == piece.PieceColor)
            return MoveResult.Invalid("Você não pode capturar uma peça da mesma cor.");

        if (!piece.IsValidMove(move.Origin, move.Target, Board))
            return MoveResult.Invalid("Movimento inválido para essa peça.");

        Board.MovePiece(move.Origin, move.Target);
        piece.MarkAsMoved();

        string moveMessage = targetPiece is null
            ? $"Movimento realizado: {move.Origin} para {move.Target}."
            : $"Movimento realizado: {move.Origin} capturou peça em {move.Target}.";

        ChangeTurn();

        return MoveResult.Valid(moveMessage);
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
}
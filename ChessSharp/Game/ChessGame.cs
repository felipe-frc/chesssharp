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

        if (origin == target)
            return MoveResult.Invalid("A posição de origem e destino não podem ser iguais.");

        var piece = Board.GetPieceAt(origin);

        if (piece is null)
            return MoveResult.Invalid("Não existe peça na posição de origem.");

        if (piece.PieceColor != CurrentTurn)
            return MoveResult.Invalid($"Não é a vez das peças {GetTurnName(piece.PieceColor)}.");

        var targetPiece = Board.GetPieceAt(target);

        if (targetPiece is not null && targetPiece.PieceColor == piece.PieceColor)
            return MoveResult.Invalid("Você não pode capturar uma peça da mesma cor.");

        if (!piece.IsValidMove(origin, target, Board))
            return MoveResult.Invalid("Movimento inválido para essa peça.");

        Board.MovePiece(origin, target);
        piece.MarkAsMoved();

        string moveMessage = targetPiece is null
            ? $"Movimento realizado: {origin} para {target}."
            : $"Movimento realizado: {origin} capturou peça em {target}.";

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
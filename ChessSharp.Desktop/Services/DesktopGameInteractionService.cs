using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Desktop.Services;

public sealed class DesktopGameInteractionService
{
    private readonly List<BoardPosition> _legalTargetPositions = new();

    public BoardPosition? SelectedPosition { get; private set; }
    public IReadOnlyList<BoardPosition> LegalTargetPositions => _legalTargetPositions;

    public void ClearSelection()
    {
        SelectedPosition = null;
        _legalTargetPositions.Clear();
    }

    public SquareClickResult HandleSquareClick(
        ChessGame game,
        PieceColor playerColor,
        BoardPosition clickedPosition)
    {
        if (game.CurrentTurn != playerColor)
            return SquareClickResult.NoOp();

        if (SelectedPosition is null)
            return SquareClickResult.SelectionChanged();

        if (SelectedPosition.Value == clickedPosition)
        {
            ClearSelection();
            return SquareClickResult.SelectionCleared();
        }

        var clickedPiece = game.Board.GetPieceAt(clickedPosition);

        if (clickedPiece is not null &&
            clickedPiece.PieceColor == playerColor &&
            !_legalTargetPositions.Contains(clickedPosition))
        {
            return SquareClickResult.SelectionChanged();
        }

        if (!_legalTargetPositions.Contains(clickedPosition))
            return SquareClickResult.InvalidTarget();

        return SquareClickResult.MoveRequested(SelectedPosition.Value, clickedPosition);
    }

    public SelectionResult SelectPiece(
        ChessGame game,
        PieceColor playerColor,
        BoardPosition position)
    {
        var piece = game.Board.GetPieceAt(position);

        if (piece is null || piece.PieceColor != playerColor)
        {
            ClearSelection();
            return SelectionResult.InvalidPiece(piece);
        }

        SelectedPosition = position;

        _legalTargetPositions.Clear();
        _legalTargetPositions.AddRange(
            ChessRules.GetLegalMoves(game.Board, playerColor)
                .Where(move => move.Origin == position)
                .Select(move => move.Target)
                .Distinct());

        return _legalTargetPositions.Count == 0
            ? SelectionResult.NoLegalMoves()
            : SelectionResult.Success();
    }

    public MoveExecutionResult TryMoveSelectedPiece(
        ChessGame game,
        BoardPosition targetPosition,
        PieceType? promotionPieceType)
    {
        if (SelectedPosition is null)
            return MoveExecutionResult.MissingSelection();

        var origin = SelectedPosition.Value;
        var moveResult = game.TryMove(new Move(origin, targetPosition, promotionPieceType));

        ClearSelection();

        return moveResult.Success
            ? MoveExecutionResult.Success()
            : MoveExecutionResult.InvalidTarget();
    }
}

public readonly record struct SelectionResult(
    bool IsValidSelection,
    bool HasLegalMoves,
    ChessSharp.Pieces.ChessPiece? ClickedPiece)
{
    public static SelectionResult Success() => new(true, true, null);
    public static SelectionResult NoLegalMoves() => new(true, false, null);
    public static SelectionResult InvalidPiece(ChessSharp.Pieces.ChessPiece? piece) => new(false, false, piece);
}

public readonly record struct SquareClickResult(
    SquareClickAction Action,
    BoardPosition? Origin,
    BoardPosition? Target)
{
    public static SquareClickResult NoOp() => new(SquareClickAction.NoOp, null, null);
    public static SquareClickResult InvalidTarget() => new(SquareClickAction.InvalidTarget, null, null);
    public static SquareClickResult SelectionChanged() => new(SquareClickAction.SelectionChanged, null, null);
    public static SquareClickResult SelectionCleared() => new(SquareClickAction.SelectionCleared, null, null);
    public static SquareClickResult MoveRequested(BoardPosition origin, BoardPosition target) => new(SquareClickAction.MoveRequested, origin, target);
}

public enum SquareClickAction
{
    NoOp,
    InvalidTarget,
    SelectionChanged,
    SelectionCleared,
    MoveRequested
}

public readonly record struct MoveExecutionResult(
    bool MoveSucceeded,
    bool HadSelection)
{
    public static MoveExecutionResult Success() => new(true, true);
    public static MoveExecutionResult InvalidTarget() => new(false, true);
    public static MoveExecutionResult MissingSelection() => new(false, false);
}

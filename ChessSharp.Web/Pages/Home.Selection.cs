using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Web.Pages;

public partial class Home
{
    private async Task OnSquareClickedAsync(BoardPosition clickedPosition)
    {
        if (_showColorSelection || _showPromotionSelection || _isBotThinking || _game.IsFinished)
            return;

        if (_game.CurrentTurn != _playerColor)
            return;

        if (_selectedPosition is null)
        {
            SelectPiece(clickedPosition);
            return;
        }

        if (_selectedPosition.Value == clickedPosition)
        {
            ClearSelection();
            _statusMessage = GetPlayerTurnMessage();
            return;
        }

        var clickedPiece = _game.Board.GetPieceAt(clickedPosition);

        if (clickedPiece is not null &&
            clickedPiece.PieceColor == _playerColor &&
            !_legalTargetPositions.Contains(clickedPosition))
        {
            SelectPiece(clickedPosition);
            return;
        }

        if (!_legalTargetPositions.Contains(clickedPosition))
        {
            _statusMessage = GetInvalidTargetMessage();
            return;
        }

        await TryMoveSelectedPieceAsync(clickedPosition);
    }

    private void SelectPiece(BoardPosition position)
    {
        var piece = _game.Board.GetPieceAt(position);

        if (piece is null || piece.PieceColor != _playerColor)
        {
            ClearSelection();
            _statusMessage = _statusTextService.GetInvalidSelectionMessage(piece is not null);
            return;
        }

        _selectedPosition = position;
        _legalTargetPositions = ChessRules
            .GetLegalMoves(_game.Board, _playerColor)
            .Where(move => move.Origin == position)
            .Select(move => move.Target)
            .Distinct()
            .ToList();

        if (_legalTargetPositions.Count == 0)
        {
            _statusMessage = _statusTextService.GetNoLegalMovesMessage(
                ChessRules.IsKingInCheck(_game.Board, _playerColor));
            return;
        }

        _statusMessage = _statusTextService.GetSelectDestinationMessage();
    }

    private void ClearSelection()
    {
        _selectedPosition = null;
        _legalTargetPositions.Clear();
    }
}

using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Web.Pages;

public partial class Home
{
    private async Task TryMoveSelectedPieceAsync(BoardPosition targetPosition)
    {
        if (_selectedPosition is null)
            return;

        var origin = _selectedPosition.Value;
        var movingPiece = _game.Board.GetPieceAt(origin);

        if (movingPiece is null)
            return;

        if (ChessRules.IsPawnPromotionMove(movingPiece, targetPosition))
        {
            _pendingPromotionOrigin = origin;
            _pendingPromotionTarget = targetPosition;
            _showPromotionSelection = true;
            _statusMessage = _statusTextService.GetChoosePromotionMessage();
            return;
        }

        await ExecutePlayerMoveAsync(new Move(origin, targetPosition));
    }

    private async Task CompletePromotionAsync(PieceType pieceType)
    {
        if (_pendingPromotionOrigin is null || _pendingPromotionTarget is null)
            return;

        _showPromotionSelection = false;

        var move = new Move(
            _pendingPromotionOrigin.Value,
            _pendingPromotionTarget.Value,
            pieceType);

        _pendingPromotionOrigin = null;
        _pendingPromotionTarget = null;

        await ExecutePlayerMoveAsync(move);
    }
}

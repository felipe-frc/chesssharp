using ChessSharp.Game;
using Microsoft.JSInterop;
using ChessSharp.Web.ViewModels;

namespace ChessSharp.Web.Pages;

public partial class Home
{
    private async Task FlushPendingAnimationAsync()
    {
        if (_pendingAnimation is null)
            return;

        var animation = _pendingAnimation;
        _pendingAnimation = null;
        await JS.InvokeVoidAsync("chessSharpUi.animateMove", animation.Origin, animation.Target);
    }

    private void QueueBoardFeedback(Move move, bool isCapture)
    {
        _pendingAnimation = new PendingAnimation(
            GetBoardNotation(move.Origin),
            GetBoardNotation(move.Target));

        bool isCheck = !_game.IsFinished && ChessRules.IsKingInCheck(_game.Board, _game.CurrentTurn);
        _pendingSound = _game.IsFinished
            ? "end"
            : isCheck
                ? "check"
                : isCapture
                    ? "capture"
                    : "move";
    }

    private static Task DelayForBoardFeedbackAsync() => Task.Delay(300);
}

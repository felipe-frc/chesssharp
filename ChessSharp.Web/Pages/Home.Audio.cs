using Microsoft.JSInterop;

namespace ChessSharp.Web.Pages;

public partial class Home
{
    private async Task FlushPendingSoundAsync()
    {
        if (_soundEnabled && !string.IsNullOrWhiteSpace(_pendingSound))
        {
            var sound = _pendingSound;
            _pendingSound = null;
            await JS.InvokeVoidAsync("chessSharpUi.playSound", sound);
        }
        else
        {
            _pendingSound = null;
        }
    }

    private void QueuePendingSound(string sound) => _pendingSound = sound;
}

using System.Windows;
using ChessSharp.Enums;

namespace ChessSharp.Desktop.Services;

public sealed class PromotionDialogService
{
    private TaskCompletionSource<PieceColor>? _colorSelectionSource;
    private TaskCompletionSource<PieceType>? _promotionSelectionSource;

    public bool IsAwaitingColorSelection => _colorSelectionSource is not null;
    public bool IsAwaitingPromotionSelection => _promotionSelectionSource is not null;

    public async Task<PieceColor> RequestPlayerColorAsync(
        FrameworkElement overlay,
        Action onOpen)
    {
        _colorSelectionSource = new TaskCompletionSource<PieceColor>();
        overlay.Visibility = Visibility.Visible;
        onOpen();

        try
        {
            return await _colorSelectionSource.Task;
        }
        finally
        {
            _colorSelectionSource = null;
        }
    }

    public async Task<PieceType?> RequestPromotionPieceAsync(
        FrameworkElement overlay,
        Action onOpen)
    {
        _promotionSelectionSource = new TaskCompletionSource<PieceType>();
        overlay.Visibility = Visibility.Visible;
        onOpen();

        try
        {
            return await _promotionSelectionSource.Task;
        }
        finally
        {
            _promotionSelectionSource = null;
        }
    }

    public void CompleteColorSelection(FrameworkElement overlay, PieceColor selectedColor)
    {
        overlay.Visibility = Visibility.Collapsed;
        _colorSelectionSource?.TrySetResult(selectedColor);
    }

    public void CompletePromotionSelection(FrameworkElement overlay, PieceType selectedPiece)
    {
        overlay.Visibility = Visibility.Collapsed;
        _promotionSelectionSource?.TrySetResult(selectedPiece);
    }

    public void Hide(FrameworkElement overlay)
    {
        overlay.Visibility = Visibility.Collapsed;
    }
}

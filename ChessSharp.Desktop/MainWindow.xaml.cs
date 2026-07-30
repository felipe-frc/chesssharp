using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using ChessSharp.AI;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Desktop.Services;

namespace ChessSharp.Desktop;

public partial class MainWindow : Window
{
    private ChessGame _game = new();
    private ChessBot _bot = new(PieceColor.Black);
    private PieceColor _playerColor = PieceColor.White;

    private readonly PieceImageService _pieceImageService = new();
    private readonly DesktopGameInteractionService _interactionService = new();
    private readonly PromotionDialogService _promotionDialogService = new();
    private readonly GameStatusPresentationService _statusPresentationService = new();
    private readonly BoardRenderService _boardRenderService;

    public MainWindow()
    {
        _boardRenderService = new BoardRenderService(_pieceImageService);

        InitializeComponent();

        Loaded += MainWindow_Loaded;
        ConfigureColorOptions();
        ConfigurePromotionOptions();
        RenderBoard();
        UpdateStatusMessage(_statusPresentationService.GetChooseColorMessage());
    }

    private async void Square_MouseLeftButtonDown(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_game.IsFinished ||
            _promotionDialogService.IsAwaitingPromotionSelection ||
            _promotionDialogService.IsAwaitingColorSelection)
            return;

        if (sender is not Grid square || square.Tag is not BoardPosition clickedPosition)
            return;

        var clickResult = _interactionService.HandleSquareClick(_game, _playerColor, clickedPosition);

        switch (clickResult.Action)
        {
            case SquareClickAction.NoOp:
                return;

            case SquareClickAction.SelectionCleared:
                UpdateStatusMessage(_statusPresentationService.GetPlayerTurnMessage(_game.Board, _playerColor));
                RenderBoard();
                return;

            case SquareClickAction.InvalidTarget:
                UpdateStatusMessage(_statusPresentationService.GetInvalidTargetMessage(_game.Board, _playerColor));
                return;

            case SquareClickAction.MoveRequested:
                await TryMoveSelectedPiece(clickedPosition);
                return;
        }

        UpdateStatusAfterSelection(clickedPosition);
    }

    private async Task TryMoveSelectedPiece(BoardPosition targetPosition)
    {
        var selectedPosition = _interactionService.SelectedPosition;

        if (selectedPosition is null)
            return;

        var origin = selectedPosition.Value;
        var movingPiece = _game.Board.GetPieceAt(origin);

        if (movingPiece is null)
            return;

        PieceType? promotionPieceType = null;

        if (ChessRules.IsPawnPromotionMove(movingPiece, targetPosition))
        {
            promotionPieceType = await RequestPromotionPieceAsync();

            if (promotionPieceType is null)
                return;
        }

        var result = _interactionService.TryMoveSelectedPiece(_game, targetPosition, promotionPieceType);
        RenderBoard();

        if (!result.MoveSucceeded)
        {
            UpdateStatusMessage(_statusPresentationService.GetInvalidTargetMessage(_game.Board, _playerColor));
            return;
        }

        if (_game.IsFinished)
        {
            UpdateStatusMessage(_statusPresentationService.GetFinalMessage(_game.Status, _playerColor));
            return;
        }

        UpdateStatusMessage(_statusPresentationService.GetMoveCompletedMessage());
        await MakeBotMoveAsync();
    }

    private async Task MakeBotMoveAsync()
    {
        if (_game.CurrentTurn != _bot.BotColor)
            return;

        UpdateStatusMessage(_statusPresentationService.GetBotThinkingMessage());
        await Task.Delay(300);

        var botMove = _bot.ChooseMove(_game.Board);

        if (botMove is null)
        {
            _game.FinishByNoLegalMoves(_bot.BotColor);
            UpdateStatusMessage(_statusPresentationService.GetFinalMessage(_game.Status, _playerColor));
            return;
        }

        var botMoveResult = _game.TryMove(botMove.Value);
        RenderBoard();

        if (!botMoveResult.Success)
        {
            UpdateStatusMessage(_statusPresentationService.GetBotStrategyErrorMessage());
            return;
        }

        if (_game.IsFinished)
        {
            UpdateStatusMessage(_statusPresentationService.GetFinalMessage(_game.Status, _playerColor));
            return;
        }

        UpdateStatusMessage(_statusPresentationService.GetPlayerTurnMessage(_game.Board, _playerColor));
    }

    private void NewGameButton_Click(object sender, RoutedEventArgs e)
    {
        _ = PromptForColorAndStartGameAsync();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void PromotionOptionButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not string pieceTypeName ||
            !Enum.TryParse(pieceTypeName, out PieceType selectedPiece))
        {
            return;
        }

        _promotionDialogService.CompletePromotionSelection(PromotionOverlay, selectedPiece);
    }

    private async void ColorOptionButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not string colorName ||
            !Enum.TryParse(colorName, out PieceColor selectedColor))
        {
            return;
        }

        _promotionDialogService.CompleteColorSelection(ColorSelectionOverlay, selectedColor);
        await Task.CompletedTask;
    }

    private void UpdateStatusMessage(string message)
    {
        StatusText.Text = message.ToUpper(CultureInfo.CurrentCulture);
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindow_Loaded;
        await PromptForColorAndStartGameAsync();
    }

    private void ConfigureColorOptions()
    {
        ChooseWhiteButton.Content = _pieceImageService.CreateColorOptionContent(PieceColor.White, "BRANCAS", "VOCÊ COMEÇA");
        ChooseBlackButton.Content = _pieceImageService.CreateColorOptionContent(PieceColor.Black, "PRETAS", "A MÁQUINA COMEÇA");
    }

    private void ConfigurePromotionOptions()
    {
        UpdatePromotionOptionContent();
    }

    private void UpdatePromotionOptionContent()
    {
        PromotionQueenButton.Content = _pieceImageService.CreatePromotionOptionContent(PieceType.Queen, "RAINHA", _playerColor);
        PromotionRookButton.Content = _pieceImageService.CreatePromotionOptionContent(PieceType.Rook, "TORRE", _playerColor);
        PromotionBishopButton.Content = _pieceImageService.CreatePromotionOptionContent(PieceType.Bishop, "BISPO", _playerColor);
        PromotionKnightButton.Content = _pieceImageService.CreatePromotionOptionContent(PieceType.Knight, "CAVALO", _playerColor);
    }

    private async Task PromptForColorAndStartGameAsync()
    {
        var selectedColor = await RequestPlayerColorAsync();
        await StartNewGameAsync(selectedColor);
    }

    private async Task StartNewGameAsync(PieceColor playerColor)
    {
        _playerColor = playerColor;
        _game = new ChessGame();
        _bot = new ChessBot(ChessRules.GetOpponentColor(playerColor));

        _interactionService.ClearSelection();
        HidePromotionOverlay();
        UpdatePromotionOptionContent();
        RenderBoard();

        if (_game.CurrentTurn == _playerColor)
        {
            UpdateStatusMessage(_statusPresentationService.GetPlayerTurnMessage(_game.Board, _playerColor));
            return;
        }

        await MakeBotMoveAsync();
    }

    private async Task<PieceColor> RequestPlayerColorAsync()
    {
        return await _promotionDialogService.RequestPlayerColorAsync(
            ColorSelectionOverlay,
            () => UpdateStatusMessage(_statusPresentationService.GetChooseColorMessage()));
    }

    private async Task<PieceType?> RequestPromotionPieceAsync()
    {
        return await _promotionDialogService.RequestPromotionPieceAsync(
            PromotionOverlay,
            () => UpdateStatusMessage(_statusPresentationService.GetChoosePromotionMessage()));
    }

    private void HidePromotionOverlay()
    {
        _promotionDialogService.Hide(PromotionOverlay);
    }

    private void HideColorSelectionOverlay()
    {
        _promotionDialogService.Hide(ColorSelectionOverlay);
    }

    private void RenderBoard()
    {
        _boardRenderService.RenderBoard(
            BoardGrid,
            _game,
            _playerColor,
            _interactionService.SelectedPosition,
            _interactionService.LegalTargetPositions,
            Square_MouseLeftButtonDown);
    }

    private void UpdateStatusAfterSelection(BoardPosition clickedPosition)
    {
        var selectionResult = _interactionService.SelectPiece(_game, _playerColor, clickedPosition);
        UpdateStatusMessage(_statusPresentationService.GetSelectionMessage(_game.Board, _playerColor, selectionResult));
        RenderBoard();
    }
}

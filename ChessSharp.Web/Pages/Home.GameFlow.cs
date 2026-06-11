using ChessSharp.AI;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Web.Pages;

public partial class Home
{
    private async Task StartNewGameAsync(PieceColor playerColor)
    {
        ResetGameUiState();
        _playerColor = playerColor;
        _game = new ChessGame();
        _bot = new ChessBot(ChessRules.GetOpponentColor(playerColor));
        _moveHistory.Clear();
        ClearSelection();

        _statusMessage = GetPlayerTurnMessage();

        if (_game.CurrentTurn != _playerColor)
            await MakeBotMoveAsync();
    }

    private void PromptForColorSelection()
    {
        _showPromotionSelection = false;
        _showHistoryModal = false;
        _showGameOverModal = false;
        _pendingPromotionOrigin = null;
        _pendingPromotionTarget = null;
        ClearSelection();
        _showColorSelection = true;
        _statusMessage = _statusTextService.GetChooseColorMessage();
    }

    private async Task ExecutePlayerMoveAsync(Move move)
    {
        bool isCapture = IsCaptureMove(move);
        var result = _game.TryMove(move);
        ClearSelection();

        if (!result.Success)
        {
            _statusMessage = GetInvalidTargetMessage();
            return;
        }

        RegisterMoveHistory(move);
        QueueBoardFeedback(move, isCapture);

        if (_game.IsFinished)
        {
            HandleFinishedGame();
            return;
        }

        _statusMessage = _statusTextService.GetBotThinkingMessage();
        await MakeBotMoveAsync();
    }

    private async Task MakeBotMoveAsync()
    {
        if (_game.CurrentTurn != _bot.BotColor)
            return;

        _isBotThinking = true;
        _statusMessage = _statusTextService.GetBotThinkingMessage();

        await DelayForBoardFeedbackAsync();

        try
        {
            var botMove = _bot.ChooseMove(_game.Board);

            if (botMove is null)
            {
                _game.FinishByNoLegalMoves(_bot.BotColor);
                QueuePendingSound("end");
                HandleFinishedGame();
                return;
            }

            bool isCapture = IsCaptureMove(botMove.Value);
            var botMoveResult = _game.TryMove(botMove.Value);

            if (!botMoveResult.Success)
            {
                _statusMessage = _statusTextService.GetInvalidBotMoveMessage();
                return;
            }

            RegisterMoveHistory(botMove.Value);
            QueueBoardFeedback(botMove.Value, isCapture);

            if (_game.IsFinished)
            {
                HandleFinishedGame();
                return;
            }

            _statusMessage = GetPlayerTurnMessage();
        }
        finally
        {
            _isBotThinking = false;
        }
    }

    private void HandleFinishedGame()
    {
        _showGameOverModal = true;
        _statusMessage = GetFinalMessage();
    }

    private bool IsCaptureMove(Move move)
    {
        var movingPiece = _game.Board.GetPieceAt(move.Origin);
        var targetPiece = _game.Board.GetPieceAt(move.Target);

        if (movingPiece is null)
            return false;

        if (targetPiece is not null && targetPiece.PieceColor != movingPiece.PieceColor)
            return true;

        return ChessRules.IsEnPassantMove(_game.Board, move, movingPiece.PieceColor);
    }

    private void ResetGameUiState()
    {
        _showColorSelection = false;
        _showPromotionSelection = false;
        _showHistoryModal = false;
        _showGameOverModal = false;
        _pendingPromotionOrigin = null;
        _pendingPromotionTarget = null;
        _pendingAnimation = null;
        _pendingSound = null;
    }
}

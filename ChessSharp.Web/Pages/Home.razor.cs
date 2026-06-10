using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ChessSharp.AI;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Pieces;
using ChessSharp.Web.Services;
using ChessSharp.Web.ViewModels;

namespace ChessSharp.Web.Pages;

public partial class Home : ComponentBase
{
    private static readonly PieceType[] CapturablePieceOrder =
    [
        PieceType.Queen,
        PieceType.Rook,
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Pawn
    ];

    private ChessGame _game = new();
    private ChessBot _bot = new(PieceColor.Black);
    private PieceColor _playerColor = PieceColor.White;
    private BoardPosition? _selectedPosition;
    private List<BoardPosition> _legalTargetPositions = [];
    private string _statusMessage = "Escolha sua cor para começar.";
    private bool _showColorSelection = true;
    private bool _showPromotionSelection;
    private bool _showHistoryModal;
    private bool _showGameOverModal;
    private bool _isBotThinking;
    private bool _soundEnabled;
    private bool _isSidebarCollapsed;
    private BoardPosition? _pendingPromotionOrigin;
    private BoardPosition? _pendingPromotionTarget;
    private PendingAnimation? _pendingAnimation;
    private string? _pendingSound;
    private readonly List<MoveHistoryEntry> _moveHistory = [];

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private IReadOnlyList<PieceType> PromotionChoices { get; } =
    [
        PieceType.Queen,
        PieceType.Rook,
        PieceType.Bishop,
        PieceType.Knight
    ];

    private IEnumerable<int> DisplayRows =>
        _playerColor == PieceColor.White
            ? Enumerable.Range(0, 8)
            : Enumerable.Range(0, 8).Reverse();

    private IEnumerable<int> DisplayColumns =>
        _playerColor == PieceColor.White
            ? Enumerable.Range(0, 8)
            : Enumerable.Range(0, 8).Reverse();

    private IReadOnlyList<MoveHistoryEntry> RecentHistory =>
        _moveHistory.TakeLast(6).Reverse().ToList();

    private IReadOnlyList<MoveHistoryEntry> FullHistory =>
        _moveHistory.AsEnumerable().Reverse().ToList();

    private IReadOnlyList<CapturedPieceView> WhiteCapturedPieces =>
        GetCapturedPieces(PieceColor.White);

    private IReadOnlyList<CapturedPieceView> BlackCapturedPieces =>
        GetCapturedPieces(PieceColor.Black);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.InvokeVoidAsync("chessSharpUi.init");

        if (_pendingAnimation is not null)
        {
            var animation = _pendingAnimation;
            _pendingAnimation = null;
            await JS.InvokeVoidAsync("chessSharpUi.animateMove", animation.Origin, animation.Target);
        }

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
            _statusMessage = piece is null
                ? "Selecione uma peça."
                : "Selecione uma peça da sua cor.";
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
            _statusMessage = ChessRules.IsKingInCheck(_game.Board, _playerColor)
                ? "Seu rei precisa de defesa."
                : "Essa peça não tem lances válidos.";
            return;
        }

        _statusMessage = "Escolha o destino.";
    }

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
            _statusMessage = "Escolha a promoção.";
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

    private async Task ExecutePlayerMoveAsync(Move move)
    {
        var movingPiece = _game.Board.GetPieceAt(move.Origin);
        var targetPiece = _game.Board.GetPieceAt(move.Target);
        bool isCapture = movingPiece is not null &&
            targetPiece is not null &&
            targetPiece.PieceColor != movingPiece.PieceColor;

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

        _statusMessage = "Bot pensando...";
        await MakeBotMoveAsync();
    }

    private async Task MakeBotMoveAsync()
    {
        if (_game.CurrentTurn != _bot.BotColor)
            return;

        _isBotThinking = true;
        _statusMessage = "Bot pensando...";

        await Task.Delay(300);

        try
        {
            var botMove = _bot.ChooseMove(_game.Board);

            if (botMove is null)
            {
                _statusMessage = "Sem lances disponíveis.";
                return;
            }

            var movingPiece = _game.Board.GetPieceAt(botMove.Value.Origin);
            var targetPiece = _game.Board.GetPieceAt(botMove.Value.Target);
            bool isCapture = movingPiece is not null &&
                targetPiece is not null &&
                targetPiece.PieceColor != movingPiece.PieceColor;

            var botMoveResult = _game.TryMove(botMove.Value);

            if (!botMoveResult.Success)
            {
                _statusMessage = "O bot tentou um lance inválido.";
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

    private async Task StartNewGameAsync(PieceColor playerColor)
    {
        _showColorSelection = false;
        _showPromotionSelection = false;
        _showHistoryModal = false;
        _showGameOverModal = false;
        _pendingPromotionOrigin = null;
        _pendingPromotionTarget = null;
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
        _statusMessage = "Escolha sua cor para começar.";
    }

    private void OpenHistoryModal() => _showHistoryModal = true;

    private void CloseHistoryModal() => _showHistoryModal = false;

    private void CloseGameOverModal() => _showGameOverModal = false;

    private void ToggleSound() => _soundEnabled = !_soundEnabled;

    private void ToggleSidebar() => _isSidebarCollapsed = !_isSidebarCollapsed;

    private string GetSoundButtonLabel() =>
        _soundEnabled ? "🔊 Som ligado" : "🔇 Som desligado";

    private void ClearSelection()
    {
        _selectedPosition = null;
        _legalTargetPositions.Clear();
    }

    private void HandleFinishedGame()
    {
        _showGameOverModal = true;
        _statusMessage = GetFinalMessage();
    }

    private string GetPlayerTurnMessage() =>
        ChessRules.IsKingInCheck(_game.Board, _playerColor)
            ? "Seu rei está em xeque."
            : "Selecione uma peça.";

    private string GetInvalidTargetMessage() =>
        ChessRules.IsKingInCheck(_game.Board, _playerColor)
            ? "Você precisa responder ao xeque."
            : "Escolha uma casa válida.";

    private string GetFinalMessage() =>
        _game.Status switch
        {
            GameStatus.WhiteWins => _playerColor == PieceColor.White
                ? "Vitória das Brancas."
                : "Vitória das Pretas.",
            GameStatus.BlackWins => _playerColor == PieceColor.Black
                ? "Vitória das Pretas."
                : "Vitória das Brancas.",
            GameStatus.Draw => "Empate.",
            GameStatus.PlayerQuit => "Partida encerrada.",
            _ => "Jogo finalizado."
        };

    private string GetGameOverTitle() =>
        _game.Status switch
        {
            GameStatus.WhiteWins => "Vitória das Brancas",
            GameStatus.BlackWins => "Vitória das Pretas",
            GameStatus.Draw => "Empate",
            GameStatus.PlayerQuit => "Partida encerrada",
            _ => "Fim da partida"
        };

    private string GetGameOverReason() =>
        _game.Status switch
        {
            GameStatus.WhiteWins or GameStatus.BlackWins => "Xeque-mate.",
            GameStatus.Draw => "Empate por afogamento.",
            GameStatus.PlayerQuit => "Partida encerrada pelo jogador.",
            _ => "Partida concluída."
        };

    private string GetTurnOwner() =>
        _game.CurrentTurn == _playerColor ? "Você" : "Bot";

    private string GetKingSafetyLabel() =>
        ChessRules.IsKingInCheck(_game.Board, _playerColor) ? "Em xeque" : "Protegido";

    private string GetHeroPanelTitle()
    {
        if (_showColorSelection)
            return "Escolha o lado.";

        if (_game.IsFinished)
            return "Partida encerrada.";

        return _game.CurrentTurn == _playerColor
            ? "Sua vez."
            : "Bot pensando.";
    }

    private string GetPositionSummary()
    {
        if (_showColorSelection)
            return "Escolha sua cor para iniciar a partida.";

        if (_game.IsFinished)
            return "A partida terminou. Revise os últimos lances ou comece uma nova rodada.";

        if (ChessRules.IsKingInCheck(_game.Board, _game.CurrentTurn))
            return _game.CurrentTurn == PieceColor.White
                ? "O rei branco está sob pressão."
                : "O rei preto está sob pressão.";

        return GetPositionEvaluation();
    }

    private string GetPositionEvaluation()
    {
        string phase = GetPhaseLabel();
        int whiteDelta = GetMaterialDelta(PieceColor.White);

        if (phase == "Final")
        {
            if (whiteDelta > 1)
                return "Final favorável às brancas.";

            if (whiteDelta < -1)
                return "Final favorável às pretas.";

            return "Final equilibrado.";
        }

        if (whiteDelta == 0)
            return "Posição equilibrada.";

        return whiteDelta > 0
            ? "Brancas têm vantagem."
            : "Pretas têm vantagem.";
    }

    private string GetSidebarPlayerClass(PieceColor color)
    {
        var classes = new List<string> { "sidebar-player" };

        if (_game.CurrentTurn == color && !_game.IsFinished && !_showColorSelection)
            classes.Add("sidebar-player-active");

        return string.Join(" ", classes);
    }

    private bool IsSelected(BoardPosition position) =>
        _selectedPosition is not null && _selectedPosition.Value == position;

    private bool IsLegalTarget(BoardPosition position) =>
        _legalTargetPositions.Contains(position);

    private bool IsCaptureTarget(BoardPosition position)
    {
        var piece = _game.Board.GetPieceAt(position);
        return piece is not null && piece.PieceColor != _playerColor;
    }

    private bool IsLastMoveSquare(BoardPosition position)
    {
        if (_game.Board.LastMove is null)
            return false;

        var move = _game.Board.LastMove.Value;
        return move.Origin == position || move.Target == position;
    }

    private bool IsCheckedKingSquare(BoardPosition position)
    {
        if (!ChessRules.IsKingInCheck(_game.Board, _game.CurrentTurn))
            return false;

        var kingPosition = _game.Board.FindKingPosition(_game.CurrentTurn);
        return kingPosition is not null && kingPosition.Value == position;
    }

    private string GetSquareClass(BoardPosition position)
    {
        var classes = new List<string> { "board-square" };

        if (IsSelected(position))
            classes.Add("board-square-selected");

        return string.Join(" ", classes);
    }

    private string GetPieceFrameClass(ChessPiece? piece, BoardPosition position)
    {
        if (piece is null)
            return "piece-image";

        var classes = new List<string>
        {
            "piece-image",
            piece.PieceType switch
            {
                PieceType.Pawn => "piece-pawn",
                PieceType.Rook => "piece-rook",
                PieceType.Knight => "piece-knight",
                PieceType.Bishop => "piece-bishop",
                PieceType.Queen => "piece-queen",
                PieceType.King => "piece-king",
                _ => string.Empty
            }
        };

        if (IsBottomDisplayRow(position))
            classes.Add("piece-edge-bottom");
        else if (IsTopDisplayRow(position))
            classes.Add("piece-edge-top");

        return string.Join(" ", classes.Where(static value => !string.IsNullOrWhiteSpace(value)));
    }

    private bool IsTopDisplayRow(BoardPosition position) =>
        _playerColor == PieceColor.White ? position.Row == 0 : position.Row == 7;

    private bool IsBottomDisplayRow(BoardPosition position) =>
        _playerColor == PieceColor.White ? position.Row == 7 : position.Row == 0;

    private string GetStatusOrbClass()
    {
        var classes = new List<string> { "status-orb" };

        if (_showColorSelection || _showPromotionSelection)
            classes.Add("status-orb-attention");
        else if (_isBotThinking)
            classes.Add("status-orb-thinking");
        else if (_game.IsFinished)
            classes.Add("status-orb-finished");
        else if (ChessRules.IsKingInCheck(_game.Board, _game.CurrentTurn))
            classes.Add("status-orb-alert");
        else if (!_showColorSelection)
            classes.Add(_game.CurrentTurn == PieceColor.White
                ? "status-orb-white-turn"
                : "status-orb-black-turn");
        else
            classes.Add("status-orb-ready");

        return string.Join(" ", classes);
    }

    private string? GetAlertBadgeText()
    {
        if (_game.Status is GameStatus.WhiteWins or GameStatus.BlackWins)
            return "XEQUE-MATE";

        if (!_showColorSelection && ChessRules.IsKingInCheck(_game.Board, _game.CurrentTurn))
            return "XEQUE";

        return null;
    }

    private string GetAlertBadgeClass()
    {
        var classes = new List<string> { "status-badge" };

        if (_game.Status is GameStatus.WhiteWins or GameStatus.BlackWins)
            classes.Add("status-badge-mate");
        else
            classes.Add("status-badge-check");

        return string.Join(" ", classes);
    }

    private string GetMoveCounterLabel() => $"Lance {_moveHistory.Count}";

    private string GetPhaseLabel()
    {
        if (_moveHistory.Count < 8)
            return "Abertura";

        if (_moveHistory.Count < 24)
            return "Meio-jogo";

        return "Final";
    }

    private string GetMaterialSummary()
    {
        int whiteDelta = GetMaterialDelta(PieceColor.White);

        if (whiteDelta == 0)
            return "Material equilibrado";

        if (whiteDelta == 1)
            return "Vantagem das brancas";

        if (whiteDelta == -1)
            return "Vantagem das pretas";

        if (whiteDelta > 1)
            return $"Brancas +{whiteDelta}";

        return $"Pretas +{Math.Abs(whiteDelta)}";
    }

    private int GetMaterialDelta(PieceColor perspective)
    {
        int own = GetMaterialScore(perspective);
        int opponent = GetMaterialScore(ChessRules.GetOpponentColor(perspective));
        return own - opponent;
    }

    private int GetMaterialScore(PieceColor color)
    {
        int total = 0;

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var piece = _game.Board.GetPieceAt(new BoardPosition(row, column));

                if (piece?.PieceColor != color)
                    continue;

                total += piece.PieceType switch
                {
                    PieceType.Pawn => 1,
                    PieceType.Knight => 3,
                    PieceType.Bishop => 3,
                    PieceType.Rook => 5,
                    PieceType.Queen => 9,
                    _ => 0
                };
            }
        }

        return total;
    }

    private IReadOnlyList<CapturedPieceView> GetCapturedPieces(PieceColor capturedColor)
    {
        List<CapturedPieceView> pieces = [];

        foreach (var pieceType in CapturablePieceOrder)
        {
            int missingCount = GetInitialPieceCount(pieceType) - GetRemainingPieceCount(capturedColor, pieceType);

            for (int i = 0; i < missingCount; i++)
                pieces.Add(new CapturedPieceView(pieceType, capturedColor));
        }

        return pieces;
    }

    private int GetRemainingPieceCount(PieceColor color, PieceType pieceType)
    {
        int total = 0;

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var piece = _game.Board.GetPieceAt(new BoardPosition(row, column));

                if (piece?.PieceColor == color && piece.PieceType == pieceType)
                    total++;
            }
        }

        return total;
    }

    private static int GetInitialPieceCount(PieceType pieceType) =>
        pieceType switch
        {
            PieceType.Queen => 1,
            PieceType.Rook => 2,
            PieceType.Bishop => 2,
            PieceType.Knight => 2,
            PieceType.Pawn => 8,
            _ => 0
        };

    private static string GetBoardNotation(BoardPosition position) =>
        $"{(char)('a' + position.Column)}{8 - position.Row}";

    private void RegisterMoveHistory(Move move)
    {
        _moveHistory.Add(new MoveHistoryEntry(
            _moveHistory.Count + 1,
            BuildMoveNotation(move)));
    }

    private static string BuildMoveNotation(Move move)
    {
        string notation = $"{GetBoardNotation(move.Origin)} → {GetBoardNotation(move.Target)}";

        if (move.PromotionPieceType is not null)
            notation += $" = {ChessPresentationService.GetPieceTypeName(move.PromotionPieceType.Value)}";

        return notation;
    }
}

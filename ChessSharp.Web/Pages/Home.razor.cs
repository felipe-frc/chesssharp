using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ChessSharp.AI;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Web.Services;
using ChessSharp.Web.ViewModels;

namespace ChessSharp.Web.Pages;

public partial class Home : ComponentBase
{
    private readonly GameStatusTextService _statusTextService = new();
    private readonly BoardCssClassService _boardCssClassService = new();
    private readonly MoveNotationService _moveNotationService = new();

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
        ChessPresentationService.GetCapturedPieces(_game.Board, PieceColor.White);

    private IReadOnlyList<CapturedPieceView> BlackCapturedPieces =>
        ChessPresentationService.GetCapturedPieces(_game.Board, PieceColor.Black);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.InvokeVoidAsync("chessSharpUi.init");

        await FlushPendingAnimationAsync();
        await FlushPendingSoundAsync();
    }

    private void OpenHistoryModal() => _showHistoryModal = true;

    private void CloseHistoryModal() => _showHistoryModal = false;

    private void CloseGameOverModal() => _showGameOverModal = false;

    private void ToggleSound() => _soundEnabled = !_soundEnabled;

    private void ToggleSidebar() => _isSidebarCollapsed = !_isSidebarCollapsed;
}

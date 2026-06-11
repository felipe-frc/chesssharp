using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Pieces;
using ChessSharp.Web.Services;

namespace ChessSharp.Web.Pages;

public partial class Home
{
    private string GetSoundButtonLabel() =>
        _soundEnabled ? "🔊 Som ligado" : "🔇 Som desligado";

    private string GetPlayerTurnMessage() =>
        _statusTextService.GetPlayerTurnMessage(
            ChessRules.IsKingInCheck(_game.Board, _playerColor));

    private string GetInvalidTargetMessage() =>
        _statusTextService.GetInvalidTargetMessage(
            ChessRules.IsKingInCheck(_game.Board, _playerColor));

    private string GetFinalMessage() =>
        _statusTextService.GetFinalMessage(_game.Status, _playerColor);

    private string GetGameOverTitle() =>
        _statusTextService.GetGameOverTitle(_game.Status);

    private string GetGameOverReason() =>
        _statusTextService.GetGameOverReason(_game.Status);

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
        int whiteDelta = ChessPresentationService.GetMaterialDelta(_game.Board, PieceColor.White);

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

        if (piece is not null)
            return piece.PieceColor != _playerColor;

        if (_selectedPosition is null)
            return false;

        var selectedPiece = _game.Board.GetPieceAt(_selectedPosition.Value);

        if (selectedPiece is null || selectedPiece.PieceType != PieceType.Pawn)
            return false;

        return ChessRules.IsEnPassantMove(
            _game.Board,
            new Move(_selectedPosition.Value, position),
            selectedPiece.PieceColor);
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

    private string GetSquareClass(BoardPosition position) =>
        _boardCssClassService.GetSquareClass(IsSelected(position));

    private string GetPieceFrameClass(ChessPiece? piece, BoardPosition position)
    {
        if (piece is null)
            return "piece-image";

        return _boardCssClassService.GetPieceFrameClass(
            piece.PieceType,
            IsTopDisplayRow(position),
            IsBottomDisplayRow(position));
    }

    private bool IsTopDisplayRow(BoardPosition position) =>
        _playerColor == PieceColor.White ? position.Row == 0 : position.Row == 7;

    private bool IsBottomDisplayRow(BoardPosition position) =>
        _playerColor == PieceColor.White ? position.Row == 7 : position.Row == 0;

    private string GetStatusOrbClass() =>
        _boardCssClassService.GetStatusOrbClass(
            _showColorSelection,
            _showPromotionSelection,
            _isBotThinking,
            _game.IsFinished,
            ChessRules.IsKingInCheck(_game.Board, _game.CurrentTurn),
            _game.CurrentTurn);

    private string? GetAlertBadgeText()
    {
        if (_game.Status is GameStatus.WhiteWins or GameStatus.BlackWins)
            return "XEQUE-MATE";

        if (!_showColorSelection && ChessRules.IsKingInCheck(_game.Board, _game.CurrentTurn))
            return "XEQUE";

        return null;
    }

    private string GetAlertBadgeClass() =>
        _boardCssClassService.GetAlertBadgeClass(
            _game.Status is GameStatus.WhiteWins or GameStatus.BlackWins);

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
        int whiteDelta = ChessPresentationService.GetMaterialDelta(_game.Board, PieceColor.White);

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
}

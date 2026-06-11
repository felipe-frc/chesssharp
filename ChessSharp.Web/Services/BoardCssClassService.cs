using ChessSharp.Enums;

namespace ChessSharp.Web.Services;

public sealed class BoardCssClassService
{
    public string GetSquareClass(bool isSelected)
    {
        var classes = new List<string> { "board-square" };

        if (isSelected)
            classes.Add("board-square-selected");

        return string.Join(" ", classes);
    }

    public string GetPieceFrameClass(PieceType pieceType, bool isTopDisplayRow, bool isBottomDisplayRow)
    {
        var classes = new List<string>
        {
            "piece-image",
            pieceType switch
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

        if (isBottomDisplayRow)
            classes.Add("piece-edge-bottom");
        else if (isTopDisplayRow)
            classes.Add("piece-edge-top");

        return string.Join(" ", classes.Where(static value => !string.IsNullOrWhiteSpace(value)));
    }

    public string GetStatusOrbClass(
        bool showColorSelection,
        bool showPromotionSelection,
        bool isBotThinking,
        bool isFinished,
        bool isCheck,
        PieceColor currentTurn)
    {
        var classes = new List<string> { "status-orb" };

        if (showColorSelection || showPromotionSelection)
            classes.Add("status-orb-attention");
        else if (isBotThinking)
            classes.Add("status-orb-thinking");
        else if (isFinished)
            classes.Add("status-orb-finished");
        else if (isCheck)
            classes.Add("status-orb-alert");
        else if (!showColorSelection)
            classes.Add(currentTurn == PieceColor.White
                ? "status-orb-white-turn"
                : "status-orb-black-turn");
        else
            classes.Add("status-orb-ready");

        return string.Join(" ", classes);
    }

    public string GetAlertBadgeClass(bool isCheckmate)
    {
        var classes = new List<string> { "status-badge" };
        classes.Add(isCheckmate ? "status-badge-mate" : "status-badge-check");
        return string.Join(" ", classes);
    }
}

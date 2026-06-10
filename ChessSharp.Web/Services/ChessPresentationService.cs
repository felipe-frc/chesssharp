using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;
using ChessSharp.Web.ViewModels;

namespace ChessSharp.Web.Services;

public static class ChessPresentationService
{
    private static readonly PieceType[] CapturablePieceOrder =
    [
        PieceType.Queen,
        PieceType.Rook,
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Pawn
    ];

    public static string GetColorName(PieceColor color) =>
        color == PieceColor.White ? "Brancas" : "Pretas";

    public static string GetPieceAssetPath(PieceType pieceType, PieceColor color)
    {
        string colorName = color == PieceColor.White ? "white" : "black";

        string pieceName = pieceType switch
        {
            PieceType.King => "king",
            PieceType.Queen => "queen",
            PieceType.Rook => "rook",
            PieceType.Bishop => "bishop",
            PieceType.Knight => "knight",
            PieceType.Pawn => "pawn",
            _ => throw new InvalidOperationException("Tipo de peça inválido.")
        };

        return $"assets/images/pieces/{colorName}-{pieceName}.png";
    }

    public static string GetPieceAltText(ChessPiece piece) =>
        $"{GetPieceTypeName(piece.PieceType)} {(piece.PieceColor == PieceColor.White ? "branco" : "preto")}";

    public static string GetPieceTypeName(PieceType pieceType) =>
        pieceType switch
        {
            PieceType.Queen => "Rainha",
            PieceType.Rook => "Torre",
            PieceType.Bishop => "Bispo",
            PieceType.Knight => "Cavalo",
            PieceType.King => "Rei",
            PieceType.Pawn => "Peão",
            _ => pieceType.ToString()
        };

    public static int GetMaterialDelta(ChessBoard board, PieceColor perspective)
    {
        int own = GetMaterialScore(board, perspective);
        int opponent = GetMaterialScore(board, GetOpponentColor(perspective));
        return own - opponent;
    }

    public static IReadOnlyList<CapturedPieceView> GetCapturedPieces(ChessBoard board, PieceColor capturedColor)
    {
        List<CapturedPieceView> pieces = [];

        foreach (var pieceType in CapturablePieceOrder)
        {
            int missingCount = GetInitialPieceCount(pieceType) - GetRemainingPieceCount(board, capturedColor, pieceType);

            for (int i = 0; i < missingCount; i++)
                pieces.Add(new CapturedPieceView(pieceType, capturedColor));
        }

        return pieces;
    }

    private static int GetMaterialScore(ChessBoard board, PieceColor color)
    {
        int total = 0;

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var piece = board.GetPieceAt(new BoardPosition(row, column));

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

    private static int GetRemainingPieceCount(ChessBoard board, PieceColor color, PieceType pieceType)
    {
        int total = 0;

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var piece = board.GetPieceAt(new BoardPosition(row, column));

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

    private static PieceColor GetOpponentColor(PieceColor color) =>
        color == PieceColor.White ? PieceColor.Black : PieceColor.White;
}

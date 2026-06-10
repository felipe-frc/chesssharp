using ChessSharp.Enums;
using ChessSharp.Pieces;

namespace ChessSharp.Web.Services;

public static class ChessPresentationService
{
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
}

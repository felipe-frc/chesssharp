using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Pieces;

public abstract class ChessPiece
{
    protected ChessPiece(PieceColor pieceColor, PieceType pieceType, string symbol)
    {
        PieceColor = pieceColor;
        PieceType = pieceType;
        Symbol = symbol;
    }

    public PieceColor PieceColor { get; }
    public PieceType PieceType { get; }
    public string Symbol { get; }
    public bool HasMoved { get; private set; }

    public void MarkAsMoved()
    {
        HasMoved = true;
    }

    public abstract bool IsValidMove(
        BoardPosition currentPosition,
        BoardPosition targetPosition,
        ChessBoard board
    );
}
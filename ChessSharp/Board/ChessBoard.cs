using ChessSharp.Pieces;

namespace ChessSharp.Board;

public class ChessBoard
{
    private readonly ChessPiece?[,] _squares = new ChessPiece?[8, 8];

    public ChessPiece? GetPieceAt(BoardPosition position)
    {
        if (!position.IsValid())
            throw new ArgumentOutOfRangeException(nameof(position), "A posição informada está fora do tabuleiro.");

        return _squares[position.Row, position.Column];
    }

    public void SetPieceAt(BoardPosition position, ChessPiece? piece)
    {
        if (!position.IsValid())
            throw new ArgumentOutOfRangeException(nameof(position), "A posição informada está fora do tabuleiro.");

        _squares[position.Row, position.Column] = piece;
    }

    public bool IsEmpty(BoardPosition position)
    {
        return GetPieceAt(position) is null;
    }
}
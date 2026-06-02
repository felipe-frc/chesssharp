using ChessSharp.Enums;
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

    public bool IsPathClear(BoardPosition currentPosition, BoardPosition targetPosition)
    {
        int rowStep = Math.Sign(targetPosition.Row - currentPosition.Row);
        int columnStep = Math.Sign(targetPosition.Column - currentPosition.Column);

        int currentRow = currentPosition.Row + rowStep;
        int currentColumn = currentPosition.Column + columnStep;

        while (currentRow != targetPosition.Row || currentColumn != targetPosition.Column)
        {
            if (_squares[currentRow, currentColumn] is not null)
                return false;

            currentRow += rowStep;
            currentColumn += columnStep;
        }

        return true;
    }

    public void SetupInitialPosition()
    {
        Clear();

        SetPieceAt(new BoardPosition(0, 0), new Rook(PieceColor.Black));
        SetPieceAt(new BoardPosition(0, 1), new Knight(PieceColor.Black));
        SetPieceAt(new BoardPosition(0, 2), new Bishop(PieceColor.Black));
        SetPieceAt(new BoardPosition(0, 3), new Queen(PieceColor.Black));
        SetPieceAt(new BoardPosition(0, 4), new King(PieceColor.Black));
        SetPieceAt(new BoardPosition(0, 5), new Bishop(PieceColor.Black));
        SetPieceAt(new BoardPosition(0, 6), new Knight(PieceColor.Black));
        SetPieceAt(new BoardPosition(0, 7), new Rook(PieceColor.Black));

        for (int column = 0; column < 8; column++)
            SetPieceAt(new BoardPosition(1, column), new Pawn(PieceColor.Black));

        for (int column = 0; column < 8; column++)
            SetPieceAt(new BoardPosition(6, column), new Pawn(PieceColor.White));

        SetPieceAt(new BoardPosition(7, 0), new Rook(PieceColor.White));
        SetPieceAt(new BoardPosition(7, 1), new Knight(PieceColor.White));
        SetPieceAt(new BoardPosition(7, 2), new Bishop(PieceColor.White));
        SetPieceAt(new BoardPosition(7, 3), new Queen(PieceColor.White));
        SetPieceAt(new BoardPosition(7, 4), new King(PieceColor.White));
        SetPieceAt(new BoardPosition(7, 5), new Bishop(PieceColor.White));
        SetPieceAt(new BoardPosition(7, 6), new Knight(PieceColor.White));
        SetPieceAt(new BoardPosition(7, 7), new Rook(PieceColor.White));
    }

    private void Clear()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                _squares[row, column] = null;
            }
        }
    }
}
using ChessSharp.Board;

namespace ChessSharp.Game;

public readonly record struct Move(BoardPosition Origin, BoardPosition Target)
{
    public override string ToString()
    {
        return $"{Origin} {Target}";
    }
}
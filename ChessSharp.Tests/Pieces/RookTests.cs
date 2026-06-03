using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Pieces;

public class RookTests
{
    [Fact]
    public void IsValidMove_ShouldAllowVerticalMove_WhenPathIsClear()
    {
        var board = new ChessBoard();
        var rook = new Rook(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("a1");
        var target = BoardPosition.FromChessNotation("a5");

        board.SetPieceAt(origin, rook);

        bool result = rook.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowHorizontalMove_WhenPathIsClear()
    {
        var board = new ChessBoard();
        var rook = new Rook(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("a1");
        var target = BoardPosition.FromChessNotation("h1");

        board.SetPieceAt(origin, rook);

        bool result = rook.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowDiagonalMove()
    {
        var board = new ChessBoard();
        var rook = new Rook(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("a1");
        var target = BoardPosition.FromChessNotation("b2");

        board.SetPieceAt(origin, rook);

        bool result = rook.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowVerticalMove_WhenPathIsBlocked()
    {
        var board = new ChessBoard();
        var rook = new Rook(PieceColor.White);
        var blockingPiece = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("a1");
        var blocker = BoardPosition.FromChessNotation("a3");
        var target = BoardPosition.FromChessNotation("a5");

        board.SetPieceAt(origin, rook);
        board.SetPieceAt(blocker, blockingPiece);

        bool result = rook.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowHorizontalMove_WhenPathIsBlocked()
    {
        var board = new ChessBoard();
        var rook = new Rook(PieceColor.White);
        var blockingPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("a1");
        var blocker = BoardPosition.FromChessNotation("c1");
        var target = BoardPosition.FromChessNotation("h1");

        board.SetPieceAt(origin, rook);
        board.SetPieceAt(blocker, blockingPiece);

        bool result = rook.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowCapture_WhenTargetHasEnemyPieceAndPathIsClear()
    {
        var board = new ChessBoard();
        var rook = new Rook(PieceColor.White);
        var enemyPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("a1");
        var target = BoardPosition.FromChessNotation("a8");

        board.SetPieceAt(origin, rook);
        board.SetPieceAt(target, enemyPiece);

        bool result = rook.IsValidMove(origin, target, board);

        Assert.True(result);
    }
}
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Pieces;

public class QueenTests
{
    [Fact]
    public void IsValidMove_ShouldAllowVerticalMove_WhenPathIsClear()
    {
        var board = new ChessBoard();
        var queen = new Queen(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("d1");
        var target = BoardPosition.FromChessNotation("d6");

        board.SetPieceAt(origin, queen);

        bool result = queen.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowHorizontalMove_WhenPathIsClear()
    {
        var board = new ChessBoard();
        var queen = new Queen(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("d1");
        var target = BoardPosition.FromChessNotation("h1");

        board.SetPieceAt(origin, queen);

        bool result = queen.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowDiagonalMove_WhenPathIsClear()
    {
        var board = new ChessBoard();
        var queen = new Queen(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("d1");
        var target = BoardPosition.FromChessNotation("h5");

        board.SetPieceAt(origin, queen);

        bool result = queen.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowInvalidMove()
    {
        var board = new ChessBoard();
        var queen = new Queen(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("d1");
        var target = BoardPosition.FromChessNotation("f4");

        board.SetPieceAt(origin, queen);

        bool result = queen.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowVerticalMove_WhenPathIsBlocked()
    {
        var board = new ChessBoard();
        var queen = new Queen(PieceColor.White);
        var blockingPiece = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("d1");
        var blocker = BoardPosition.FromChessNotation("d3");
        var target = BoardPosition.FromChessNotation("d6");

        board.SetPieceAt(origin, queen);
        board.SetPieceAt(blocker, blockingPiece);

        bool result = queen.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowHorizontalMove_WhenPathIsBlocked()
    {
        var board = new ChessBoard();
        var queen = new Queen(PieceColor.White);
        var blockingPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("d1");
        var blocker = BoardPosition.FromChessNotation("f1");
        var target = BoardPosition.FromChessNotation("h1");

        board.SetPieceAt(origin, queen);
        board.SetPieceAt(blocker, blockingPiece);

        bool result = queen.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowDiagonalMove_WhenPathIsBlocked()
    {
        var board = new ChessBoard();
        var queen = new Queen(PieceColor.White);
        var blockingPiece = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("d1");
        var blocker = BoardPosition.FromChessNotation("f3");
        var target = BoardPosition.FromChessNotation("h5");

        board.SetPieceAt(origin, queen);
        board.SetPieceAt(blocker, blockingPiece);

        bool result = queen.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowCapture_WhenTargetHasEnemyPieceAndPathIsClear()
    {
        var board = new ChessBoard();
        var queen = new Queen(PieceColor.White);
        var enemyPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("d1");
        var target = BoardPosition.FromChessNotation("h5");

        board.SetPieceAt(origin, queen);
        board.SetPieceAt(target, enemyPiece);

        bool result = queen.IsValidMove(origin, target, board);

        Assert.True(result);
    }
}
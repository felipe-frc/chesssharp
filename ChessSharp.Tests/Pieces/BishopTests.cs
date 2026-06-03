using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Pieces;

public class BishopTests
{
    [Fact]
    public void IsValidMove_ShouldAllowDiagonalMove_WhenPathIsClear()
    {
        var board = new ChessBoard();
        var bishop = new Bishop(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("c1");
        var target = BoardPosition.FromChessNotation("h6");

        board.SetPieceAt(origin, bishop);

        bool result = bishop.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowDiagonalMoveInOppositeDirection_WhenPathIsClear()
    {
        var board = new ChessBoard();
        var bishop = new Bishop(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("f1");
        var target = BoardPosition.FromChessNotation("a6");

        board.SetPieceAt(origin, bishop);

        bool result = bishop.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowHorizontalMove()
    {
        var board = new ChessBoard();
        var bishop = new Bishop(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("c1");
        var target = BoardPosition.FromChessNotation("h1");

        board.SetPieceAt(origin, bishop);

        bool result = bishop.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowVerticalMove()
    {
        var board = new ChessBoard();
        var bishop = new Bishop(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("c1");
        var target = BoardPosition.FromChessNotation("c5");

        board.SetPieceAt(origin, bishop);

        bool result = bishop.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowDiagonalMove_WhenPathIsBlocked()
    {
        var board = new ChessBoard();
        var bishop = new Bishop(PieceColor.White);
        var blockingPiece = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("c1");
        var blocker = BoardPosition.FromChessNotation("e3");
        var target = BoardPosition.FromChessNotation("h6");

        board.SetPieceAt(origin, bishop);
        board.SetPieceAt(blocker, blockingPiece);

        bool result = bishop.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowCapture_WhenTargetHasEnemyPieceAndPathIsClear()
    {
        var board = new ChessBoard();
        var bishop = new Bishop(PieceColor.White);
        var enemyPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("c1");
        var target = BoardPosition.FromChessNotation("h6");

        board.SetPieceAt(origin, bishop);
        board.SetPieceAt(target, enemyPiece);

        bool result = bishop.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowMoveThatIsNotPerfectDiagonal()
    {
        var board = new ChessBoard();
        var bishop = new Bishop(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("c1");
        var target = BoardPosition.FromChessNotation("e5");

        board.SetPieceAt(origin, bishop);

        bool result = bishop.IsValidMove(origin, target, board);

        Assert.False(result);
    }
}
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Pieces;

public class KnightTests
{
    [Theory]
    [InlineData("d4", "b3")]
    [InlineData("d4", "b5")]
    [InlineData("d4", "c2")]
    [InlineData("d4", "c6")]
    [InlineData("d4", "e2")]
    [InlineData("d4", "e6")]
    [InlineData("d4", "f3")]
    [InlineData("d4", "f5")]
    public void IsValidMove_ShouldAllowLShapedMove(
        string originNotation,
        string targetNotation
    )
    {
        var board = new ChessBoard();
        var knight = new Knight(PieceColor.White);

        var origin = BoardPosition.FromChessNotation(originNotation);
        var target = BoardPosition.FromChessNotation(targetNotation);

        board.SetPieceAt(origin, knight);

        bool result = knight.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Theory]
    [InlineData("d4", "d5")]
    [InlineData("d4", "d6")]
    [InlineData("d4", "e4")]
    [InlineData("d4", "f4")]
    [InlineData("d4", "e5")]
    [InlineData("d4", "f6")]
    public void IsValidMove_ShouldNotAllowNonLShapedMove(
        string originNotation,
        string targetNotation
    )
    {
        var board = new ChessBoard();
        var knight = new Knight(PieceColor.White);

        var origin = BoardPosition.FromChessNotation(originNotation);
        var target = BoardPosition.FromChessNotation(targetNotation);

        board.SetPieceAt(origin, knight);

        bool result = knight.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowMoveEvenWhenPathIsBlocked()
    {
        var board = new ChessBoard();
        var knight = new Knight(PieceColor.White);
        var blockingPiece1 = new Pawn(PieceColor.White);
        var blockingPiece2 = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("g1");
        var blocker1 = BoardPosition.FromChessNotation("g2");
        var blocker2 = BoardPosition.FromChessNotation("f2");
        var target = BoardPosition.FromChessNotation("e2");

        board.SetPieceAt(origin, knight);
        board.SetPieceAt(blocker1, blockingPiece1);
        board.SetPieceAt(blocker2, blockingPiece2);

        bool result = knight.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowCapture_WhenTargetHasEnemyPiece()
    {
        var board = new ChessBoard();
        var knight = new Knight(PieceColor.White);
        var enemyPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("g1");
        var target = BoardPosition.FromChessNotation("e2");

        board.SetPieceAt(origin, knight);
        board.SetPieceAt(target, enemyPiece);

        bool result = knight.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldReturnTrueForOwnPieceTarget_WhenOnlyPieceRuleIsChecked()
    {
        var board = new ChessBoard();
        var knight = new Knight(PieceColor.White);
        var ownPiece = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("g1");
        var target = BoardPosition.FromChessNotation("e2");

        board.SetPieceAt(origin, knight);
        board.SetPieceAt(target, ownPiece);

        bool result = knight.IsValidMove(origin, target, board);

        Assert.True(result);
    }
}
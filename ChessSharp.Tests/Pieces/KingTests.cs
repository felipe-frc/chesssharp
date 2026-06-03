using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Pieces;

public class KingTests
{
    [Theory]
    [InlineData("e4", "e5")]
    [InlineData("e4", "e3")]
    [InlineData("e4", "d4")]
    [InlineData("e4", "f4")]
    [InlineData("e4", "d5")]
    [InlineData("e4", "f5")]
    [InlineData("e4", "d3")]
    [InlineData("e4", "f3")]
    public void IsValidMove_ShouldAllowOneSquareMoveInAnyDirection(
        string originNotation,
        string targetNotation
    )
    {
        var board = new ChessBoard();
        var king = new King(PieceColor.White);

        var origin = BoardPosition.FromChessNotation(originNotation);
        var target = BoardPosition.FromChessNotation(targetNotation);

        board.SetPieceAt(origin, king);

        bool result = king.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Theory]
    [InlineData("e4", "e6")]
    [InlineData("e4", "e2")]
    [InlineData("e4", "c4")]
    [InlineData("e4", "g4")]
    [InlineData("e4", "c6")]
    [InlineData("e4", "g6")]
    [InlineData("e4", "c2")]
    [InlineData("e4", "g2")]
    public void IsValidMove_ShouldNotAllowMoveMoreThanOneSquare(
        string originNotation,
        string targetNotation
    )
    {
        var board = new ChessBoard();
        var king = new King(PieceColor.White);

        var origin = BoardPosition.FromChessNotation(originNotation);
        var target = BoardPosition.FromChessNotation(targetNotation);

        board.SetPieceAt(origin, king);

        bool result = king.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Theory]
    [InlineData("e4", "c5")]
    [InlineData("e4", "c3")]
    [InlineData("e4", "d6")]
    [InlineData("e4", "f6")]
    [InlineData("e4", "g5")]
    [InlineData("e4", "g3")]
    [InlineData("e4", "d2")]
    [InlineData("e4", "f2")]
    public void IsValidMove_ShouldNotAllowKnightLikeMove(
        string originNotation,
        string targetNotation
    )
    {
        var board = new ChessBoard();
        var king = new King(PieceColor.White);

        var origin = BoardPosition.FromChessNotation(originNotation);
        var target = BoardPosition.FromChessNotation(targetNotation);

        board.SetPieceAt(origin, king);

        bool result = king.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowCapture_WhenTargetHasEnemyPiece()
    {
        var board = new ChessBoard();
        var king = new King(PieceColor.White);
        var enemyPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("e4");
        var target = BoardPosition.FromChessNotation("f5");

        board.SetPieceAt(origin, king);
        board.SetPieceAt(target, enemyPiece);

        bool result = king.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldReturnTrueForOwnPieceTarget_WhenOnlyPieceRuleIsChecked()
    {
        var board = new ChessBoard();
        var king = new King(PieceColor.White);
        var ownPiece = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("e4");
        var target = BoardPosition.FromChessNotation("f5");

        board.SetPieceAt(origin, king);
        board.SetPieceAt(target, ownPiece);

        bool result = king.IsValidMove(origin, target, board);

        Assert.True(result);
    }
}
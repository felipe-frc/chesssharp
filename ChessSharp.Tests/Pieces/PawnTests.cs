using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Pieces;

public class PawnTests
{
    [Fact]
    public void IsValidMove_ShouldAllowWhitePawnToMoveOneSquareForward()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("e2");
        var target = BoardPosition.FromChessNotation("e3");

        board.SetPieceAt(origin, pawn);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowBlackPawnToMoveOneSquareForward()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("e7");
        var target = BoardPosition.FromChessNotation("e6");

        board.SetPieceAt(origin, pawn);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowWhitePawnToMoveTwoSquaresFromStartingPosition()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("e2");
        var target = BoardPosition.FromChessNotation("e4");

        board.SetPieceAt(origin, pawn);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowBlackPawnToMoveTwoSquaresFromStartingPosition()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("e7");
        var target = BoardPosition.FromChessNotation("e5");

        board.SetPieceAt(origin, pawn);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowWhitePawnToMoveBackward()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("e4");
        var target = BoardPosition.FromChessNotation("e3");

        board.SetPieceAt(origin, pawn);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowBlackPawnToMoveBackward()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("e5");
        var target = BoardPosition.FromChessNotation("e6");

        board.SetPieceAt(origin, pawn);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowPawnToMoveForwardWhenTargetIsOccupied()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.White);
        var blockingPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("e2");
        var target = BoardPosition.FromChessNotation("e3");

        board.SetPieceAt(origin, pawn);
        board.SetPieceAt(target, blockingPiece);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowPawnToMoveTwoSquaresWhenPathIsBlocked()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.White);
        var blockingPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("e2");
        var intermediate = BoardPosition.FromChessNotation("e3");
        var target = BoardPosition.FromChessNotation("e4");

        board.SetPieceAt(origin, pawn);
        board.SetPieceAt(intermediate, blockingPiece);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowWhitePawnToCaptureDiagonally()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.White);
        var enemyPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("e4");
        var target = BoardPosition.FromChessNotation("d5");

        board.SetPieceAt(origin, pawn);
        board.SetPieceAt(target, enemyPiece);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldAllowBlackPawnToCaptureDiagonally()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.Black);
        var enemyPiece = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("e5");
        var target = BoardPosition.FromChessNotation("d4");

        board.SetPieceAt(origin, pawn);
        board.SetPieceAt(target, enemyPiece);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowPawnToCaptureForward()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.White);
        var enemyPiece = new Pawn(PieceColor.Black);

        var origin = BoardPosition.FromChessNotation("e2");
        var target = BoardPosition.FromChessNotation("e3");

        board.SetPieceAt(origin, pawn);
        board.SetPieceAt(target, enemyPiece);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ShouldNotAllowPawnToCaptureOwnPieceDiagonally()
    {
        var board = new ChessBoard();
        var pawn = new Pawn(PieceColor.White);
        var ownPiece = new Pawn(PieceColor.White);

        var origin = BoardPosition.FromChessNotation("e4");
        var target = BoardPosition.FromChessNotation("d5");

        board.SetPieceAt(origin, pawn);
        board.SetPieceAt(target, ownPiece);

        bool result = pawn.IsValidMove(origin, target, board);

        Assert.False(result);
    }
}
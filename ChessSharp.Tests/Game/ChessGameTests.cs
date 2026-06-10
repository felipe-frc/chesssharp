using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Game;

public class ChessGameTests
{
    [Fact]
    public void Constructor_ShouldStartWithWhiteTurn()
    {
        var game = new ChessGame();

        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void Constructor_ShouldStartWithGameInProgress()
    {
        var game = new ChessGame();

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.False(game.IsFinished);
    }

    [Fact]
    public void TryMove_ShouldMovePieceAndChangeTurn_WhenMoveIsValid()
    {
        var game = new ChessGame();

        var result = game.TryMove("e2 e4");

        Assert.True(result.Success);
        Assert.Equal(PieceColor.Black, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldNotChangeTurn_WhenMoveIsInvalid()
    {
        var game = new ChessGame();

        var result = game.TryMove("e2 e5");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenOriginIsEmpty()
    {
        var game = new ChessGame();

        var result = game.TryMove("e3 e4");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenTryingToMoveOpponentPiece()
    {
        var game = new ChessGame();

        var result = game.TryMove("e7 e5");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenTryingToCaptureOwnPiece()
    {
        var game = new ChessGame();

        var result = game.TryMove("e1 e2");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenInputFormatIsInvalid()
    {
        var game = new ChessGame();

        var result = game.TryMove("e2");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenInputPositionIsInvalid()
    {
        var game = new ChessGame();

        var result = game.TryMove("e9 e4");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenOriginAndTargetAreEqual()
    {
        var game = new ChessGame();

        var result = game.TryMove("e2 e2");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void FinishByPlayerQuit_ShouldSetStatusToPlayerQuit()
    {
        var game = new ChessGame();

        game.FinishByPlayerQuit();

        Assert.Equal(GameStatus.PlayerQuit, game.Status);
        Assert.True(game.IsFinished);
    }

    [Fact]
    public void FinishByNoLegalMoves_ShouldSetStatusToDraw_WhenKingIsNotInCheck()
    {
        var game = new ChessGame();

        game.FinishByNoLegalMoves(PieceColor.White);

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.True(game.IsFinished);
    }

    [Fact]
    public void FinishByNoLegalMoves_ShouldSetOpponentAsWinner_WhenKingIsInCheck()
    {
        var game = new ChessGame();
        game.Board.Clear();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e8"), new King(PieceColor.Black));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e7"), new Queen(PieceColor.Black));

        game.FinishByNoLegalMoves(PieceColor.White);

        Assert.Equal(GameStatus.BlackWins, game.Status);
        Assert.True(game.IsFinished);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenGameIsAlreadyFinished()
    {
        var game = new ChessGame();

        game.FinishByPlayerQuit();

        var result = game.TryMove("e2 e4");

        Assert.False(result.Success);
        Assert.True(game.IsFinished);
        Assert.Equal(GameStatus.PlayerQuit, game.Status);
    }

    [Fact]
    public void TryMove_ShouldNotAllowKingCapture()
    {
        var game = new ChessGame();
        game.Board.Clear();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e8"), new King(PieceColor.Black));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e7"), new Queen(PieceColor.White));

        var result = game.TryMove("e7 e8");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void TryMove_ShouldNotAllowMoveThatLeavesOwnKingInCheck()
    {
        var game = new ChessGame();
        game.Board.Clear();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a8"), new King(PieceColor.Black));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e2"), new Rook(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e8"), new Rook(PieceColor.Black));

        var result = game.TryMove("e2 f2");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void TryMove_ShouldDetectCheckmate_WithFoolsMate()
    {
        var game = new ChessGame();

        Assert.True(game.TryMove("f2 f3").Success);
        Assert.True(game.TryMove("e7 e5").Success);
        Assert.True(game.TryMove("g2 g4").Success);

        var result = game.TryMove("d8 h4");

        Assert.True(result.Success);
        Assert.True(game.IsFinished);
        Assert.Equal(GameStatus.BlackWins, game.Status);
    }

    [Fact]
    public void TryMove_ShouldReportCheckWithoutFinishingGame()
    {
        var game = new ChessGame();

        Assert.True(game.TryMove("e2 e4").Success);
        Assert.True(game.TryMove("d7 d5").Success);

        var result = game.TryMove("f1 b5");

        Assert.True(result.Success);
        Assert.False(game.IsFinished);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(PieceColor.Black, game.CurrentTurn);
    }
}

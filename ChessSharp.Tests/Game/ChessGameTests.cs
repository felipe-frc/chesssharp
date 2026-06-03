using ChessSharp.Enums;
using ChessSharp.Game;
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
        Assert.Contains("Não existe peça", result.Message);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenTryingToMoveOpponentPiece()
    {
        var game = new ChessGame();

        var result = game.TryMove("e7 e5");

        Assert.False(result.Success);
        Assert.Contains("Não é a vez", result.Message);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenTryingToCaptureOwnPiece()
    {
        var game = new ChessGame();

        var result = game.TryMove("e1 e2");

        Assert.False(result.Success);
        Assert.Contains("mesma cor", result.Message);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenInputFormatIsInvalid()
    {
        var game = new ChessGame();

        var result = game.TryMove("e2");

        Assert.False(result.Success);
        Assert.Contains("Formato inválido", result.Message);
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
    public void FinishByPlayerQuit_ShouldSetStatusToPlayerQuit()
    {
        var game = new ChessGame();

        game.FinishByPlayerQuit();

        Assert.Equal(GameStatus.PlayerQuit, game.Status);
        Assert.True(game.IsFinished);
    }

    [Fact]
    public void FinishWithWhiteWin_ShouldSetStatusToWhiteWins()
    {
        var game = new ChessGame();

        game.FinishWithWhiteWin();

        Assert.Equal(GameStatus.WhiteWins, game.Status);
        Assert.True(game.IsFinished);
    }
}
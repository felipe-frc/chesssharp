using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using Xunit;

namespace ChessSharp.Tests.Game;

public class EnPassantTests
{
    [Fact]
    public void TryMove_ShouldCaptureEnPassant_WhenLastMoveEnablesIt()
    {
        var game = new ChessGame();

        Assert.True(game.TryMove("e2 e4").Success);
        Assert.True(game.TryMove("a7 a6").Success);
        Assert.True(game.TryMove("e4 e5").Success);
        Assert.True(game.TryMove("d7 d5").Success);

        var result = game.TryMove("e5 d6");

        Assert.True(result.Success);
        Assert.Null(game.Board.GetPieceAt(BoardPosition.FromChessNotation("d5")));

        var whitePawn = game.Board.GetPieceAt(BoardPosition.FromChessNotation("d6"));
        Assert.NotNull(whitePawn);
        Assert.Equal(PieceType.Pawn, whitePawn!.PieceType);
        Assert.Equal(PieceColor.White, whitePawn.PieceColor);
        Assert.Equal(PieceColor.Black, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldRejectEnPassant_WhenOpportunityHasExpired()
    {
        var game = new ChessGame();

        Assert.True(game.TryMove("e2 e4").Success);
        Assert.True(game.TryMove("a7 a6").Success);
        Assert.True(game.TryMove("e4 e5").Success);
        Assert.True(game.TryMove("d7 d5").Success);
        Assert.True(game.TryMove("h2 h3").Success);
        Assert.True(game.TryMove("a6 a5").Success);

        var result = game.TryMove("e5 d6");

        Assert.False(result.Success);

        var whitePawn = game.Board.GetPieceAt(BoardPosition.FromChessNotation("e5"));
        Assert.NotNull(whitePawn);
        Assert.Equal(PieceType.Pawn, whitePawn!.PieceType);

        var blackPawn = game.Board.GetPieceAt(BoardPosition.FromChessNotation("d5"));
        Assert.NotNull(blackPawn);
        Assert.Equal(PieceType.Pawn, blackPawn!.PieceType);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }
}

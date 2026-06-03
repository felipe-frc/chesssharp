using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Game;

public class PawnPromotionTests
{
    [Fact]
    public void TryMove_ShouldPromoteWhitePawnToQueen_WhenPromotionPieceIsNotProvided()
    {
        var game = CreateEmptyGameWithKings();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e7"), new Pawn(PieceColor.White));

        var result = game.TryMove("e7 e8");

        var promotedPiece = game.Board.GetPieceAt(BoardPosition.FromChessNotation("e8"));

        Assert.True(result.Success);
        Assert.NotNull(promotedPiece);
        Assert.Equal(PieceType.Queen, promotedPiece!.PieceType);
        Assert.Equal(PieceColor.White, promotedPiece.PieceColor);
    }

    [Theory]
    [InlineData("q", PieceType.Queen)]
    [InlineData("r", PieceType.Rook)]
    [InlineData("b", PieceType.Bishop)]
    [InlineData("n", PieceType.Knight)]
    public void TryMove_ShouldPromoteWhitePawnToSelectedPiece(
        string promotionNotation,
        PieceType expectedPieceType
    )
    {
        var game = CreateEmptyGameWithKings();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e7"), new Pawn(PieceColor.White));

        var result = game.TryMove($"e7 e8 {promotionNotation}");

        var promotedPiece = game.Board.GetPieceAt(BoardPosition.FromChessNotation("e8"));

        Assert.True(result.Success);
        Assert.NotNull(promotedPiece);
        Assert.Equal(expectedPieceType, promotedPiece!.PieceType);
        Assert.Equal(PieceColor.White, promotedPiece.PieceColor);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenPromotionPieceIsInvalid()
    {
        var game = CreateEmptyGameWithKings();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e7"), new Pawn(PieceColor.White));

        var result = game.TryMove("e7 e8 k");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldReturnInvalid_WhenPromotionIsProvidedForNonPromotionMove()
    {
        var game = CreateEmptyGameWithKings();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e2"), new Pawn(PieceColor.White));

        var result = game.TryMove("e2 e4 q");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldPromoteBlackPawnToQueen_WhenPromotionPieceIsNotProvided()
    {
        var game = CreateEmptyGameWithKings();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a2"), new Pawn(PieceColor.White));

        var whiteMoveResult = game.TryMove("a2 a3");

        Assert.True(whiteMoveResult.Success);
        Assert.Equal(PieceColor.Black, game.CurrentTurn);

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e2"), new Pawn(PieceColor.Black));

        var result = game.TryMove("e2 e1");

        var promotedPiece = game.Board.GetPieceAt(BoardPosition.FromChessNotation("e1"));

        Assert.True(result.Success);
        Assert.NotNull(promotedPiece);
        Assert.Equal(PieceType.Queen, promotedPiece!.PieceType);
        Assert.Equal(PieceColor.Black, promotedPiece.PieceColor);
    }

    private static ChessGame CreateEmptyGameWithKings()
    {
        var game = new ChessGame();

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                game.Board.SetPieceAt(new BoardPosition(row, column), null);
            }
        }

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h8"), new King(PieceColor.Black));

        return game;
    }
}
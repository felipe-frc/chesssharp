using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.Game;

public class CastlingTests
{
    [Fact]
    public void TryMove_ShouldAllowWhiteKingSideCastling()
    {
        var game = CreateEmptyGame();
        var king = new King(PieceColor.White);
        var rook = new Rook(PieceColor.White);

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), king);
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h1"), rook);
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a8"), new King(PieceColor.Black));

        var result = game.TryMove("e1 g1");

        Assert.True(result.Success);
        Assert.Equal(PieceType.King, game.Board.GetPieceAt(BoardPosition.FromChessNotation("g1"))!.PieceType);
        Assert.Equal(PieceType.Rook, game.Board.GetPieceAt(BoardPosition.FromChessNotation("f1"))!.PieceType);
        Assert.Null(game.Board.GetPieceAt(BoardPosition.FromChessNotation("e1")));
        Assert.Null(game.Board.GetPieceAt(BoardPosition.FromChessNotation("h1")));
        Assert.True(king.HasMoved);
        Assert.True(rook.HasMoved);
        Assert.Equal(PieceColor.Black, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldAllowWhiteQueenSideCastling()
    {
        var game = CreateEmptyGame();
        var king = new King(PieceColor.White);
        var rook = new Rook(PieceColor.White);

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), king);
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a1"), rook);
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a8"), new King(PieceColor.Black));

        var result = game.TryMove("e1 c1");

        Assert.True(result.Success);
        Assert.Equal(PieceType.King, game.Board.GetPieceAt(BoardPosition.FromChessNotation("c1"))!.PieceType);
        Assert.Equal(PieceType.Rook, game.Board.GetPieceAt(BoardPosition.FromChessNotation("d1"))!.PieceType);
        Assert.Null(game.Board.GetPieceAt(BoardPosition.FromChessNotation("e1")));
        Assert.Null(game.Board.GetPieceAt(BoardPosition.FromChessNotation("a1")));
        Assert.True(king.HasMoved);
        Assert.True(rook.HasMoved);
        Assert.Equal(PieceColor.Black, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldAllowBlackKingSideCastling()
    {
        var game = CreateEmptyGame();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a2"), new Pawn(PieceColor.White));

        Assert.True(game.TryMove("a2 a3").Success);

        var king = new King(PieceColor.Black);
        var rook = new Rook(PieceColor.Black);

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e8"), king);
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h8"), rook);

        var result = game.TryMove("e8 g8");

        Assert.True(result.Success);
        Assert.Equal(PieceType.King, game.Board.GetPieceAt(BoardPosition.FromChessNotation("g8"))!.PieceType);
        Assert.Equal(PieceType.Rook, game.Board.GetPieceAt(BoardPosition.FromChessNotation("f8"))!.PieceType);
        Assert.Null(game.Board.GetPieceAt(BoardPosition.FromChessNotation("e8")));
        Assert.Null(game.Board.GetPieceAt(BoardPosition.FromChessNotation("h8")));
        Assert.True(king.HasMoved);
        Assert.True(rook.HasMoved);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldNotAllowCastling_WhenKingHasMoved()
    {
        var game = CreateEmptyGame();

        var king = new King(PieceColor.White);
        king.MarkAsMoved();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), king);
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h1"), new Rook(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a8"), new King(PieceColor.Black));

        var result = game.TryMove("e1 g1");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldNotAllowCastling_WhenRookHasMoved()
    {
        var game = CreateEmptyGame();

        var rook = new Rook(PieceColor.White);
        rook.MarkAsMoved();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h1"), rook);
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a8"), new King(PieceColor.Black));

        var result = game.TryMove("e1 g1");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldNotAllowCastling_WhenPathIsBlocked()
    {
        var game = CreateEmptyGame();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h1"), new Rook(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("f1"), new Bishop(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a8"), new King(PieceColor.Black));

        var result = game.TryMove("e1 g1");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldNotAllowCastling_WhenKingIsInCheck()
    {
        var game = CreateEmptyGame();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h1"), new Rook(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a8"), new King(PieceColor.Black));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e8"), new Rook(PieceColor.Black));

        var result = game.TryMove("e1 g1");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void TryMove_ShouldNotAllowCastling_WhenKingPassesThroughAttackedSquare()
    {
        var game = CreateEmptyGame();

        game.Board.SetPieceAt(BoardPosition.FromChessNotation("e1"), new King(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("h1"), new Rook(PieceColor.White));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("a8"), new King(PieceColor.Black));
        game.Board.SetPieceAt(BoardPosition.FromChessNotation("f8"), new Rook(PieceColor.Black));

        var result = game.TryMove("e1 g1");

        Assert.False(result.Success);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    private static ChessGame CreateEmptyGame()
    {
        var game = new ChessGame();
        game.Board.Clear();

        return game;
    }
}

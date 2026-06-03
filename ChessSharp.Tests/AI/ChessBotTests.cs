using ChessSharp.AI;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;
using ChessSharp.Pieces;
using Xunit;

namespace ChessSharp.Tests.AI;

public class ChessBotTests
{
    [Fact]
    public void ChooseMove_ShouldReturnLegalMove_WhenBotHasAvailableMoves()
    {
        var board = new ChessBoard();
        board.SetupInitialPosition();

        var bot = new ChessBot(PieceColor.Black);

        var move = bot.ChooseMove(board);

        Assert.NotNull(move);
        Assert.True(ChessRules.IsLegalMove(board, move.Value, PieceColor.Black));
    }

    [Fact]
    public void ChooseMove_ShouldReturnNull_WhenBotHasNoLegalMoves()
    {
        var board = new ChessBoard();

        board.SetPieceAt(BoardPosition.FromChessNotation("h8"), new King(PieceColor.Black));
        board.SetPieceAt(BoardPosition.FromChessNotation("f7"), new Queen(PieceColor.White));
        board.SetPieceAt(BoardPosition.FromChessNotation("g6"), new King(PieceColor.White));

        var bot = new ChessBot(PieceColor.Black);

        var move = bot.ChooseMove(board);

        Assert.Null(move);
    }

    [Fact]
    public void ChooseMove_ShouldPreferCapturingHighValuePiece_WhenCaptureIsLegal()
    {
        var board = CreateEmptyBoardWithKings();

        board.SetPieceAt(BoardPosition.FromChessNotation("d5"), new Queen(PieceColor.Black));
        board.SetPieceAt(BoardPosition.FromChessNotation("d2"), new Queen(PieceColor.White));
        board.SetPieceAt(BoardPosition.FromChessNotation("a7"), new Pawn(PieceColor.White));

        var bot = new ChessBot(PieceColor.Black);

        var move = bot.ChooseMove(board);

        Assert.NotNull(move);
        Assert.Equal(BoardPosition.FromChessNotation("d5"), move.Value.Origin);
        Assert.Equal(BoardPosition.FromChessNotation("d2"), move.Value.Target);
    }

    [Fact]
    public void ChooseMove_ShouldNotReturnMoveThatCapturesKing()
    {
        var board = CreateEmptyBoardWithKings();

        board.SetPieceAt(BoardPosition.FromChessNotation("e7"), new Queen(PieceColor.Black));

        var bot = new ChessBot(PieceColor.Black);

        var move = bot.ChooseMove(board);

        Assert.NotNull(move);

        var targetPiece = board.GetPieceAt(move.Value.Target);

        Assert.False(targetPiece is not null && targetPiece.PieceType == PieceType.King);
    }

    [Fact]
    public void ChooseMove_ShouldPromotePawnToQueen_WhenPromotionIsAvailable()
    {
        var board = CreateEmptyBoardWithKings();

        board.SetPieceAt(BoardPosition.FromChessNotation("a2"), new Pawn(PieceColor.Black));

        var bot = new ChessBot(PieceColor.Black);

        var move = bot.ChooseMove(board);

        Assert.NotNull(move);
        Assert.Equal(BoardPosition.FromChessNotation("a2"), move.Value.Origin);
        Assert.Equal(BoardPosition.FromChessNotation("a1"), move.Value.Target);
        Assert.Equal(PieceType.Queen, move.Value.PromotionPieceType);
    }

    private static ChessBoard CreateEmptyBoardWithKings()
    {
        var board = new ChessBoard();

        board.SetPieceAt(BoardPosition.FromChessNotation("h1"), new King(PieceColor.White));
        board.SetPieceAt(BoardPosition.FromChessNotation("h8"), new King(PieceColor.Black));

        return board;
    }
}
using ChessSharp.Board;
using Xunit;

namespace ChessSharp.Tests.Board;

public class BoardPositionTests
{
    [Theory]
    [InlineData("a1", 7, 0)]
    [InlineData("h1", 7, 7)]
    [InlineData("a8", 0, 0)]
    [InlineData("h8", 0, 7)]
    [InlineData("e2", 6, 4)]
    [InlineData("e4", 4, 4)]
    public void FromChessNotation_ShouldConvertValidNotationToBoardPosition(
        string notation,
        int expectedRow,
        int expectedColumn
    )
    {
        var position = BoardPosition.FromChessNotation(notation);

        Assert.Equal(expectedRow, position.Row);
        Assert.Equal(expectedColumn, position.Column);
    }

    [Theory]
    [InlineData("a1")]
    [InlineData("h8")]
    [InlineData("e4")]
    public void ToString_ShouldConvertBoardPositionToChessNotation(string notation)
    {
        var position = BoardPosition.FromChessNotation(notation);

        Assert.Equal(notation, position.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    [InlineData("a9")]
    [InlineData("i1")]
    [InlineData("11")]
    [InlineData("abc")]
    public void FromChessNotation_ShouldThrowException_WhenNotationIsInvalid(string notation)
    {
        Assert.Throws<ArgumentException>(() =>
            BoardPosition.FromChessNotation(notation)
        );
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(7, 7, true)]
    [InlineData(3, 4, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, -1, false)]
    [InlineData(8, 0, false)]
    [InlineData(0, 8, false)]
    public void IsValid_ShouldReturnExpectedResult(
        int row,
        int column,
        bool expectedResult
    )
    {
        var position = new BoardPosition(row, column);

        Assert.Equal(expectedResult, position.IsValid());
    }
}
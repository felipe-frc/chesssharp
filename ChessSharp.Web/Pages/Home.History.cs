using ChessSharp.Board;
using ChessSharp.Game;
using ChessSharp.Web.ViewModels;

namespace ChessSharp.Web.Pages;

public partial class Home
{
    private void RegisterMoveHistory(Move move)
    {
        _moveHistory.Add(new MoveHistoryEntry(
            _moveHistory.Count + 1,
            _moveNotationService.BuildMoveNotation(_game.Board, move)));
    }

    private static string GetBoardNotation(BoardPosition position) =>
        $"{(char)('a' + position.Column)}{8 - position.Row}";
}

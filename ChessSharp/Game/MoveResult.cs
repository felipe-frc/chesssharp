namespace ChessSharp.Game;

public class MoveResult
{
    private MoveResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; }
    public string Message { get; }

    public static MoveResult Valid(string message)
    {
        return new MoveResult(true, message);
    }

    public static MoveResult Invalid(string message)
    {
        return new MoveResult(false, message);
    }
}
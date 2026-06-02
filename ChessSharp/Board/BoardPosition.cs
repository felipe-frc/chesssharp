namespace ChessSharp.Board;

public readonly record struct BoardPosition(int Row, int Column)
{
    public bool IsValid()
    {
        return Row >= 0 && Row < 8 && Column >= 0 && Column < 8;
    }

    public static BoardPosition FromChessNotation(string notation)
    {
        if (string.IsNullOrWhiteSpace(notation) || notation.Length != 2)
            throw new ArgumentException("A posição deve estar no formato padrão do xadrez, como e2 ou a1.");

        char file = char.ToLowerInvariant(notation[0]);
        char rank = notation[1];

        if (file < 'a' || file > 'h' || rank < '1' || rank > '8')
            throw new ArgumentException("A posição informada é inválida.");

        int column = file - 'a';
        int row = 8 - (rank - '0');

        return new BoardPosition(row, column);
    }

    public override string ToString()
    {
        char file = (char)('a' + Column);
        int rank = 8 - Row;

        return $"{file}{rank}";
    }
}
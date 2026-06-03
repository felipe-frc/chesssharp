using ChessSharp.Enums;

namespace ChessSharp.UI;

public static class PlayerColorSelector
{
    public static PieceColor AskPlayerColor()
    {
        while (true)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("♟ ChessSharp");
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Escolha a cor que deseja jogar:");
            Console.WriteLine();
            Console.WriteLine("1 - Brancas");
            Console.WriteLine("2 - Pretas");
            Console.WriteLine();
            Console.Write("Digite sua escolha: ");

            string? input = Console.ReadLine();

            if (input == "1")
                return PieceColor.White;

            if (input == "2")
                return PieceColor.Black;

            Console.WriteLine();
            Console.WriteLine("Opção inválida. Pressione qualquer tecla para tentar novamente...");
            Console.ReadKey();
        }
    }
}
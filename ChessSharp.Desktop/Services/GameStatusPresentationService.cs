using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Desktop.Services;

public sealed class GameStatusPresentationService
{
    public string GetChooseColorMessage() => "ESCOLHA SUA COR.";

    public string GetChoosePromotionMessage() => "ESCOLHA A PEÇA DA PROMOÇÃO.";

    public string GetMoveCompletedMessage() => "MOVIMENTO REALIZADO.";

    public string GetBotThinkingMessage() => "OPONENTE ESTÁ PENSANDO...";

    public string GetBotStrategyErrorMessage() => "ERRO NA ESTRATÉGIA DO OPONENTE.";

    public string GetSelectDestinationMessage() => "ESCOLHA O DESTINO DA PEÇA.";

    public string GetPlayerTurnMessage(ChessBoard board, PieceColor playerColor)
    {
        return ChessRules.IsKingInCheck(board, playerColor)
            ? "REI EM XEQUE."
            : "SUA VEZ DE JOGAR.";
    }

    public string GetInvalidTargetMessage(ChessBoard board, PieceColor playerColor)
    {
        return ChessRules.IsKingInCheck(board, playerColor)
            ? "MOVIMENTO INVÁLIDO. PROTEJA O REI."
            : "ESCOLHA UMA CASA VÁLIDA.";
    }

    public string GetSelectionMessage(
        ChessBoard board,
        PieceColor playerColor,
        SelectionResult selectionResult)
    {
        if (!selectionResult.IsValidSelection)
        {
            return selectionResult.ClickedPiece is null
                ? "ESCOLHA UMA PEÇA CLARA."
                : $"VOCÊ JOGA COM AS PEÇAS {GetColorName(playerColor)}.";
        }

        return selectionResult.HasLegalMoves
            ? GetSelectDestinationMessage()
            : ChessRules.IsKingInCheck(board, playerColor)
                ? "REI EM PERIGO. PROTEJA-O."
                : "ESTA PEÇA NÃO POSSUI MOVIMENTOS.";
    }

    public string GetFinalMessage(GameStatus status, PieceColor playerColor)
    {
        bool playerWon =
            (playerColor == PieceColor.White && status == GameStatus.WhiteWins) ||
            (playerColor == PieceColor.Black && status == GameStatus.BlackWins);

        return status switch
        {
            GameStatus.WhiteWins or GameStatus.BlackWins => playerWon
                ? "VITÓRIA. VOCÊ VENCEU A PARTIDA."
                : "DERROTA. O OPONENTE VENCEU.",
            GameStatus.Draw => "EMPATE.",
            GameStatus.PlayerQuit => "PARTIDA ENCERRADA.",
            _ => "JOGO FINALIZADO."
        };
    }

    private static string GetColorName(PieceColor color)
    {
        return color == PieceColor.White ? "CLARAS" : "ESCURAS";
    }
}

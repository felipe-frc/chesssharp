using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Web.Services;

public sealed class GameStatusTextService
{
    public string GetChooseColorMessage() => "Escolha sua cor para começar.";

    public string GetChoosePromotionMessage() => "Escolha a promoção.";

    public string GetBotThinkingMessage() => "Bot pensando...";

    public string GetInvalidBotMoveMessage() => "O bot tentou um lance inválido.";

    public string GetInvalidSelectionMessage(bool clickedOwnColorWrongly) =>
        clickedOwnColorWrongly
            ? "Selecione uma peça da sua cor."
            : "Selecione uma peça.";

    public string GetNoLegalMovesMessage(bool kingInCheck) =>
        kingInCheck
            ? "Seu rei precisa de defesa."
            : "Essa peça não tem lances válidos.";

    public string GetSelectDestinationMessage() => "Escolha o destino.";

    public string GetPlayerTurnMessage(bool kingInCheck) =>
        kingInCheck
            ? "Seu rei está em xeque."
            : "Selecione uma peça.";

    public string GetInvalidTargetMessage(bool kingInCheck) =>
        kingInCheck
            ? "Você precisa responder ao xeque."
            : "Escolha uma casa válida.";

    public string GetFinalMessage(GameStatus status, PieceColor playerColor) =>
        status switch
        {
            GameStatus.WhiteWins => playerColor == PieceColor.White
                ? "Vitória das Brancas."
                : "Vitória das Pretas.",
            GameStatus.BlackWins => playerColor == PieceColor.Black
                ? "Vitória das Pretas."
                : "Vitória das Brancas.",
            GameStatus.Draw => "Empate.",
            GameStatus.PlayerQuit => "Partida encerrada.",
            _ => "Jogo finalizado."
        };

    public string GetGameOverTitle(GameStatus status) =>
        status switch
        {
            GameStatus.WhiteWins => "Vitória das Brancas",
            GameStatus.BlackWins => "Vitória das Pretas",
            GameStatus.Draw => "Empate",
            GameStatus.PlayerQuit => "Partida encerrada",
            _ => "Fim da partida"
        };

    public string GetGameOverReason(GameStatus status) =>
        status switch
        {
            GameStatus.WhiteWins or GameStatus.BlackWins => "Xeque-mate.",
            GameStatus.Draw => "Empate por afogamento.",
            GameStatus.PlayerQuit => "Partida encerrada pelo jogador.",
            _ => "Partida concluída."
        };
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using ChessSharp.AI;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Desktop;

public partial class MainWindow : Window
{
    private ChessGame _game = new();
    private ChessBot _bot = new(PieceColor.Black);

    private BoardPosition? _selectedPosition;
    private List<BoardPosition> _legalTargetPositions = new();

    private static readonly Brush LightSquareBrush = new SolidColorBrush(Color.FromRgb(239, 216, 156));
    private static readonly Brush DarkSquareBrush = new SolidColorBrush(Color.FromRgb(154, 86, 39));

    public MainWindow()
    {
        InitializeComponent();

        CreateCoordinates();
        RenderBoard();
        UpdateStatusMessage(GetPlayerTurnMessage());
    }

    private void CreateCoordinates()
    {
        TopFilesPanel.Children.Clear();
        BottomFilesPanel.Children.Clear();
        LeftRanksPanel.Children.Clear();
        RightRanksPanel.Children.Clear();

        for (char file = 'a'; file <= 'h'; file++)
        {
            TopFilesPanel.Children.Add(CreateCoordinateText(file.ToString()));
            BottomFilesPanel.Children.Add(CreateCoordinateText(file.ToString()));
        }

        for (int rank = 8; rank >= 1; rank--)
        {
            LeftRanksPanel.Children.Add(CreateCoordinateText(rank.ToString()));
            RightRanksPanel.Children.Add(CreateCoordinateText(rank.ToString()));
        }
    }

    private static TextBlock CreateCoordinateText(string text)
    {
        return new TextBlock
        {
            Text = text,
            Foreground = new SolidColorBrush(Color.FromRgb(247, 213, 139)),
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private void RenderBoard()
    {
        BoardGrid.Children.Clear();
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();

        for (int index = 0; index < 8; index++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition());
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var position = new BoardPosition(row, column);
                var square = CreateSquare(position, row, column);

                Grid.SetRow(square, row);
                Grid.SetColumn(square, column);

                BoardGrid.Children.Add(square);
            }
        }
    }

    private Grid CreateSquare(BoardPosition position, int row, int column)
    {
        var piece = _game.Board.GetPieceAt(position);
        bool isSelected = _selectedPosition is not null && _selectedPosition.Value == position;
        bool isLegalTarget = _legalTargetPositions.Contains(position);

        var square = new Grid
        {
            Background = (row + column) % 2 == 0 ? LightSquareBrush : DarkSquareBrush,
            Tag = position,
            Cursor = System.Windows.Input.Cursors.Hand
        };

        square.MouseLeftButtonDown += Square_MouseLeftButtonDown;

        if (isSelected)
            square.Children.Add(CreateSelectedSquareHighlight());

        if (isLegalTarget)
        {
            bool isCapture = piece is not null && piece.PieceColor == PieceColor.Black;
            square.Children.Add(CreateLegalMoveIndicator(isCapture));
        }

        if (piece is not null)
            square.Children.Add(CreatePieceText(piece.PieceType, piece.PieceColor));

        return square;
    }

    private static Border CreateSelectedSquareHighlight()
    {
        return new Border
        {
            Margin = new Thickness(4),
            CornerRadius = new CornerRadius(8),
            BorderThickness = new Thickness(3),
            BorderBrush = new SolidColorBrush(Color.FromRgb(246, 196, 68)),
            Background = new SolidColorBrush(Color.FromArgb(65, 246, 196, 68)),
            IsHitTestVisible = false
        };
    }

    private static UIElement CreateLegalMoveIndicator(bool isCapture)
    {
        var fillColor = isCapture
            ? Color.FromRgb(190, 82, 60)
            : Color.FromRgb(70, 140, 76);

        var strokeColor = isCapture
            ? Color.FromRgb(255, 210, 120)
            : Color.FromRgb(185, 230, 165);

        return new Polygon
        {
            Points = new PointCollection
            {
                new Point(0, 15),
                new Point(15, 0),
                new Point(30, 15),
                new Point(15, 30)
            },
            Width = 30,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Fill = new SolidColorBrush(Color.FromArgb(210, fillColor.R, fillColor.G, fillColor.B)),
            Stroke = new SolidColorBrush(strokeColor),
            StrokeThickness = 2,
            IsHitTestVisible = false,
            Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 8,
                ShadowDepth = 0,
                Opacity = 0.35
            }
        };
    }

    private static TextBlock CreatePieceText(PieceType pieceType, PieceColor pieceColor)
    {
        return new TextBlock
        {
            Text = GetPieceUnicode(pieceType, pieceColor),
            FontFamily = new FontFamily("Segoe UI Symbol"),
            FontSize = 50,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsHitTestVisible = false,
            Foreground = pieceColor == PieceColor.White
                ? new SolidColorBrush(Color.FromRgb(250, 248, 240))
                : new SolidColorBrush(Color.FromRgb(24, 18, 14)),
            Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 4,
                ShadowDepth = 2,
                Opacity = 0.35
            }
        };
    }

    private async void Square_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_game.IsFinished)
            return;

        if (sender is not Grid square || square.Tag is not BoardPosition clickedPosition)
            return;

        if (_game.CurrentTurn != PieceColor.White)
            return;

        if (_selectedPosition is null)
        {
            SelectPiece(clickedPosition);
            return;
        }

        if (_selectedPosition.Value == clickedPosition)
        {
            ClearSelection();
            UpdateStatusMessage(GetPlayerTurnMessage());
            RenderBoard();
            return;
        }

        var clickedPiece = _game.Board.GetPieceAt(clickedPosition);

        if (clickedPiece is not null &&
            clickedPiece.PieceColor == PieceColor.White &&
            !_legalTargetPositions.Contains(clickedPosition))
        {
            SelectPiece(clickedPosition);
            return;
        }

        if (!_legalTargetPositions.Contains(clickedPosition))
        {
            UpdateStatusMessage(GetInvalidTargetMessage());
            return;
        }

        await TryMoveSelectedPiece(clickedPosition);
    }

    private void SelectPiece(BoardPosition position)
    {
        var piece = _game.Board.GetPieceAt(position);

        if (piece is null)
        {
            ClearSelection();
            UpdateStatusMessage("Escolha uma peça branca.");
            RenderBoard();
            return;
        }

        if (piece.PieceColor != PieceColor.White)
        {
            ClearSelection();
            UpdateStatusMessage("Você joga com as brancas.");
            RenderBoard();
            return;
        }

        _selectedPosition = position;

        _legalTargetPositions = ChessRules
            .GetLegalMoves(_game.Board, PieceColor.White)
            .Where(move => move.Origin == position)
            .Select(move => move.Target)
            .Distinct()
            .ToList();

        if (_legalTargetPositions.Count == 0)
        {
            UpdateStatusMessage(
                ChessRules.IsKingInCheck(_game.Board, PieceColor.White)
                    ? "Essa peça não resolve o xeque."
                    : "Essa peça não possui movimentos legais."
            );

            RenderBoard();
            return;
        }

        UpdateStatusMessage(
            ChessRules.IsKingInCheck(_game.Board, PieceColor.White)
                ? "Você está em xeque. Escolha um losango."
                : "Escolha um losango."
        );

        RenderBoard();
    }

    private async Task TryMoveSelectedPiece(BoardPosition targetPosition)
    {
        if (_selectedPosition is null)
            return;

        var origin = _selectedPosition.Value;
        string moveText = $"{ToChessNotation(origin)} {ToChessNotation(targetPosition)}";

        var result = _game.TryMove(moveText);

        ClearSelection();
        RenderBoard();

        if (!result.Success)
        {
            UpdateStatusMessage(GetInvalidTargetMessage());
            return;
        }

        if (_game.IsFinished)
        {
            UpdateStatusMessage(GetFinalMessage());
            return;
        }

        UpdateStatusMessage("Movimento realizado.");

        await MakeBotMoveAsync();
    }

    private async Task MakeBotMoveAsync()
    {
        if (_game.CurrentTurn != PieceColor.Black)
            return;

        UpdateStatusMessage("Máquina pensando...");
        await Task.Delay(500);

        var botMove = _bot.ChooseMove(_game.Board);

        if (botMove is null)
        {
            UpdateStatusMessage("Máquina sem movimentos.");
            return;
        }

        var botMoveResult = _game.TryMove(botMove.Value);

        RenderBoard();

        if (!botMoveResult.Success)
        {
            UpdateStatusMessage("Erro no movimento da máquina.");
            return;
        }

        if (_game.IsFinished)
        {
            UpdateStatusMessage(GetFinalMessage());
            return;
        }

        UpdateStatusMessage(GetPlayerTurnMessage());
    }

    private void NewGameButton_Click(object sender, RoutedEventArgs e)
    {
        _game = new ChessGame();
        _bot = new ChessBot(PieceColor.Black);

        ClearSelection();
        RenderBoard();
        UpdateStatusMessage(GetPlayerTurnMessage());
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ClearSelection()
    {
        _selectedPosition = null;
        _legalTargetPositions.Clear();
    }

    private string GetPlayerTurnMessage()
    {
        return ChessRules.IsKingInCheck(_game.Board, PieceColor.White)
            ? "Você está em xeque."
            : "Sua vez.";
    }

    private string GetInvalidTargetMessage()
    {
        return ChessRules.IsKingInCheck(_game.Board, PieceColor.White)
            ? "Esse movimento não resolve o xeque."
            : "Escolha uma casa marcada.";
    }

    private string GetFinalMessage()
    {
        return _game.Status switch
        {
            GameStatus.WhiteWins => "Você venceu.",
            GameStatus.BlackWins => "A máquina venceu.",
            GameStatus.Draw => "Empate.",
            GameStatus.PlayerQuit => "Partida encerrada.",
            _ => "Jogo encerrado."
        };
    }

    private void UpdateStatusMessage(string message)
    {
        StatusTextBlock.Text = message;
    }

    private static string ToChessNotation(BoardPosition position)
    {
        char file = (char)('a' + position.Column);
        int rank = 8 - position.Row;

        return $"{file}{rank}";
    }

    private static string GetPieceUnicode(PieceType type, PieceColor color)
    {
        return (type, color) switch
        {
            (PieceType.King, PieceColor.White) => "♔",
            (PieceType.Queen, PieceColor.White) => "♕",
            (PieceType.Rook, PieceColor.White) => "♖",
            (PieceType.Bishop, PieceColor.White) => "♗",
            (PieceType.Knight, PieceColor.White) => "♘",
            (PieceType.Pawn, PieceColor.White) => "♙",

            (PieceType.King, PieceColor.Black) => "♚",
            (PieceType.Queen, PieceColor.Black) => "♛",
            (PieceType.Rook, PieceColor.Black) => "♜",
            (PieceType.Bishop, PieceColor.Black) => "♝",
            (PieceType.Knight, PieceColor.Black) => "♞",
            (PieceType.Pawn, PieceColor.Black) => "♟",

            _ => "?"
        };
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
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

    private static readonly Brush LightSquareBrush = new SolidColorBrush(Color.FromRgb(232, 201, 143));
    private static readonly Brush DarkSquareBrush = new SolidColorBrush(Color.FromRgb(76, 42, 31));

    public MainWindow()
    {
        InitializeComponent();

        RenderBoard();
        UpdateStatusMessage(GetPlayerTurnMessage());
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
            Cursor = System.Windows.Input.Cursors.Hand,
            ClipToBounds = true
        };

        square.MouseLeftButtonDown += Square_MouseLeftButtonDown;

        square.Children.Add(new Border
        {
            BorderBrush = new SolidColorBrush(Color.FromArgb(26, 255, 245, 220)),
            BorderThickness = new Thickness(0.5),
            IsHitTestVisible = false
        });

        if (isSelected)
            square.Children.Add(CreateSelectedSquareHighlight());

        if (isLegalTarget)
        {
            bool isCapture = piece is not null && piece.PieceColor == PieceColor.Black;
            square.Children.Add(CreateLegalMoveIndicator(isCapture));
        }

        if (piece is not null)
            square.Children.Add(CreatePieceImage(piece.PieceType, piece.PieceColor));

        return square;
    }

    private static Border CreateSelectedSquareHighlight()
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(70, 214, 162, 63)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(214, 162, 63)),
            BorderThickness = new Thickness(2),
            IsHitTestVisible = false
        };
    }

    private static UIElement CreateLegalMoveIndicator(bool isCapture)
    {
        return new Ellipse
        {
            Width = 18,
            Height = 18,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Fill = isCapture
                ? new SolidColorBrush(Color.FromArgb(135, 176, 68, 52))
                : new SolidColorBrush(Color.FromArgb(95, 243, 231, 210)),
            Stroke = isCapture
                ? new SolidColorBrush(Color.FromArgb(160, 236, 184, 94))
                : new SolidColorBrush(Color.FromArgb(120, 255, 248, 230)),
            StrokeThickness = 1,
            IsHitTestVisible = false
        };
    }

    private static Image CreatePieceImage(PieceType pieceType, PieceColor pieceColor)
    {
        string imagePath = GetPieceImagePath(pieceType, pieceColor);

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(imagePath, UriKind.Relative);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        bitmap.Freeze();

        return new Image
        {
            Source = bitmap,
            Width = 72,
            Height = 72,
            Stretch = Stretch.Uniform,
            SnapsToDevicePixels = true,
            UseLayoutRounding = true,
            IsHitTestVisible = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            RenderTransformOrigin = new Point(0.5, 0.5),
            Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 8,
                ShadowDepth = 2,
                Opacity = 0.45
            }
        };
    }

    private static string GetPieceImagePath(PieceType type, PieceColor color)
    {
        string colorName = color == PieceColor.White ? "white" : "black";

        string pieceName = type switch
        {
            PieceType.King => "king",
            PieceType.Queen => "queen",
            PieceType.Rook => "rook",
            PieceType.Bishop => "bishop",
            PieceType.Knight => "knight",
            PieceType.Pawn => "pawn",
            _ => throw new InvalidOperationException("Tipo de peça inválido.")
        };

        return $"Assets/Images/Pieces/{colorName}-{pieceName}.png";
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

        if (piece is null || piece.PieceColor != PieceColor.White)
        {
            ClearSelection();
            UpdateStatusMessage(piece is null ? "ESCOLHA UMA PEÇA CLARA." : "VOCÊ JOGA COM AS PEÇAS CLARAS.");
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
                    ? "REI EM PERIGO. PROTEJA-O."
                    : "ESTA PEÇA NÃO POSSUI MOVIMENTOS."
            );
            RenderBoard();
            return;
        }

        UpdateStatusMessage("ESCOLHA O DESTINO DA PEÇA.");
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

        UpdateStatusMessage("MOVIMENTO REALIZADO.");
        await MakeBotMoveAsync();
    }

    private async Task MakeBotMoveAsync()
    {
        if (_game.CurrentTurn != PieceColor.Black)
            return;

        UpdateStatusMessage("OPONENTE ESTÁ PENSANDO...");
        await Task.Delay(800);

        var botMove = _bot.ChooseMove(_game.Board);

        if (botMove is null)
        {
            UpdateStatusMessage("OPONENTE SEM MOVIMENTOS.");
            return;
        }

        var botMoveResult = _game.TryMove(botMove.Value);
        RenderBoard();

        if (!botMoveResult.Success)
        {
            UpdateStatusMessage("ERRO NA ESTRATÉGIA DO OPONENTE.");
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
            ? "REI EM XEQUE."
            : "SUA VEZ DE JOGAR.";
    }

    private string GetInvalidTargetMessage()
    {
        return ChessRules.IsKingInCheck(_game.Board, PieceColor.White)
            ? "MOVIMENTO INVÁLIDO. PROTEJA O REI."
            : "ESCOLHA UMA CASA VÁLIDA.";
    }

    private string GetFinalMessage()
    {
        return _game.Status switch
        {
            GameStatus.WhiteWins => "VITÓRIA. VOCÊ VENCEU A PARTIDA.",
            GameStatus.BlackWins => "DERROTA. O OPONENTE VENCEU.",
            GameStatus.Draw => "EMPATE.",
            GameStatus.PlayerQuit => "PARTIDA ENCERRADA.",
            _ => "JOGO FINALIZADO."
        };
    }

    private void UpdateStatusMessage(string message)
    {
        StatusText.Text = message.ToUpper();
    }

    private static string ToChessNotation(BoardPosition position)
    {
        char file = (char)('a' + position.Column);
        int rank = 8 - position.Row;
        return $"{file}{rank}";
    }
}
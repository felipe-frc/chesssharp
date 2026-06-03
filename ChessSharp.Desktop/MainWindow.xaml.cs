using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChessSharp.AI;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Desktop;

public partial class MainWindow : Window
{
    private readonly ChessGame _game = new();
    private readonly ChessBot _bot = new(PieceColor.Black);

    private BoardPosition? _selectedPosition;

    private static readonly Brush LightSquareBrush = new SolidColorBrush(Color.FromRgb(239, 216, 156));
    private static readonly Brush DarkSquareBrush = new SolidColorBrush(Color.FromRgb(154, 86, 39));
    private static readonly Brush SelectedSquareBrush = new SolidColorBrush(Color.FromRgb(245, 191, 66));

    public MainWindow()
    {
        InitializeComponent();

        CreateCoordinates();
        RenderBoard();
        UpdateStatusMessage("Sua vez. Clique em uma peça branca e depois na casa de destino.");
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
            FontSize = 18,
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

        RenderSquares();
        RenderPieces();
    }

    private void RenderSquares()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var position = new BoardPosition(row, column);
                bool isSelected = _selectedPosition is not null && _selectedPosition.Value == position;

                var square = new Border
                {
                    Background = isSelected
                        ? SelectedSquareBrush
                        : (row + column) % 2 == 0
                            ? LightSquareBrush
                            : DarkSquareBrush,
                    BorderBrush = isSelected
                        ? new SolidColorBrush(Color.FromRgb(255, 236, 153))
                        : Brushes.Transparent,
                    BorderThickness = isSelected ? new Thickness(4) : new Thickness(0),
                    Tag = position,
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                square.MouseLeftButtonDown += Square_MouseLeftButtonDown;

                Grid.SetRow(square, row);
                Grid.SetColumn(square, column);

                BoardGrid.Children.Add(square);
            }
        }
    }

    private void RenderPieces()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var position = new BoardPosition(row, column);
                var piece = _game.Board.GetPieceAt(position);

                if (piece is null)
                    continue;

                var pieceText = new TextBlock
                {
                    Text = GetPieceUnicode(piece.PieceType, piece.PieceColor),
                    FontFamily = new FontFamily("Segoe UI Symbol"),
                    FontSize = 58,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsHitTestVisible = false,
                    Foreground = piece.PieceColor == PieceColor.White
                        ? new SolidColorBrush(Color.FromRgb(250, 248, 240))
                        : new SolidColorBrush(Color.FromRgb(24, 18, 14)),
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Colors.Black,
                        BlurRadius = 4,
                        ShadowDepth = 2,
                        Opacity = 0.35
                    }
                };

                Grid.SetRow(pieceText, row);
                Grid.SetColumn(pieceText, column);

                BoardGrid.Children.Add(pieceText);
            }
        }
    }

    private async void Square_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_game.IsFinished)
            return;

        if (sender is not Border square || square.Tag is not BoardPosition clickedPosition)
            return;

        if (_game.CurrentTurn != PieceColor.White)
            return;

        if (_selectedPosition is null)
        {
            SelectPiece(clickedPosition);
            return;
        }

        await TryMoveSelectedPiece(clickedPosition);
    }

    private void SelectPiece(BoardPosition position)
    {
        var piece = _game.Board.GetPieceAt(position);

        if (piece is null)
        {
            UpdateStatusMessage("Selecione uma peça branca para mover.");
            return;
        }

        if (piece.PieceColor != PieceColor.White)
        {
            UpdateStatusMessage("Você joga com as peças brancas. Selecione uma peça branca.");
            return;
        }

        _selectedPosition = position;
        UpdateStatusMessage($"Peça selecionada em {ToChessNotation(position)}. Clique na casa de destino.");

        RenderBoard();
    }

    private async Task TryMoveSelectedPiece(BoardPosition targetPosition)
    {
        if (_selectedPosition is null)
            return;

        var origin = _selectedPosition.Value;
        string moveText = $"{ToChessNotation(origin)} {ToChessNotation(targetPosition)}";

        var result = _game.TryMove(moveText);

        _selectedPosition = null;
        RenderBoard();

        if (!result.Success)
        {
            UpdateStatusMessage(result.Message);
            return;
        }

        if (_game.IsFinished)
        {
            UpdateStatusMessage($"{result.Message} {ChessGame.GetStatusMessage(_game.Status)}");
            return;
        }

        UpdateStatusMessage(result.Message);

        await MakeBotMoveAsync();
    }

    private async Task MakeBotMoveAsync()
    {
        if (_game.CurrentTurn != PieceColor.Black)
            return;

        UpdateStatusMessage("A máquina está pensando...");
        await Task.Delay(650);

        var botMove = _bot.ChooseMove(_game.Board);

        if (botMove is null)
        {
            UpdateStatusMessage("A máquina não possui movimentos legais.");
            return;
        }

        var botMoveResult = _game.TryMove(botMove.Value);

        RenderBoard();

        if (!botMoveResult.Success)
        {
            UpdateStatusMessage($"A máquina tentou um movimento inválido: {botMoveResult.Message}");
            return;
        }

        if (_game.IsFinished)
        {
            UpdateStatusMessage($"Máquina jogou: {botMove.Value}. {botMoveResult.Message} {ChessGame.GetStatusMessage(_game.Status)}");
            return;
        }

        UpdateStatusMessage($"Máquina jogou: {botMove.Value}. {botMoveResult.Message} Sua vez. Clique em uma peça branca e depois na casa de destino.");
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
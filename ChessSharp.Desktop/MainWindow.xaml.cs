using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Desktop;

public partial class MainWindow : Window
{
    private readonly ChessGame _game = new();

    private static readonly Brush LightSquareBrush = new SolidColorBrush(Color.FromRgb(239, 216, 156));
    private static readonly Brush DarkSquareBrush = new SolidColorBrush(Color.FromRgb(154, 86, 39));

    public MainWindow()
    {
        InitializeComponent();

        CreateCoordinates();
        CreateBoard();
        RenderPieces();
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

    private void CreateBoard()
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
                var square = new Border
                {
                    Background = (row + column) % 2 == 0
                        ? LightSquareBrush
                        : DarkSquareBrush
                };

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
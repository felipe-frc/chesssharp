using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ChessSharp.Board;
using ChessSharp.Enums;

namespace ChessSharp.Desktop.Services;

public sealed class BoardRenderService
{
    private readonly PieceImageService _pieceImageService;

    public BoardRenderService(PieceImageService pieceImageService)
    {
        _pieceImageService = pieceImageService;
    }

    public void RenderBoard(
        Grid boardGrid,
        ChessSharp.Game.ChessGame game,
        PieceColor playerColor,
        BoardPosition? selectedPosition,
        IReadOnlyCollection<BoardPosition> legalTargetPositions,
        MouseButtonEventHandler squareClickHandler)
    {
        boardGrid.Children.Clear();
        boardGrid.RowDefinitions.Clear();
        boardGrid.ColumnDefinitions.Clear();

        for (int index = 0; index < 8; index++)
        {
            boardGrid.RowDefinitions.Add(new RowDefinition());
            boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                var position = new BoardPosition(row, column);
                var square = CreateSquare(
                    game,
                    playerColor,
                    position,
                    selectedPosition,
                    legalTargetPositions,
                    squareClickHandler);

                Grid.SetRow(square, row);
                Grid.SetColumn(square, column);

                boardGrid.Children.Add(square);
            }
        }
    }

    private Grid CreateSquare(
        ChessSharp.Game.ChessGame game,
        PieceColor playerColor,
        BoardPosition position,
        BoardPosition? selectedPosition,
        IReadOnlyCollection<BoardPosition> legalTargetPositions,
        MouseButtonEventHandler squareClickHandler)
    {
        var piece = game.Board.GetPieceAt(position);
        bool isSelected = selectedPosition is not null && selectedPosition.Value == position;
        bool isLegalTarget = legalTargetPositions.Contains(position);

        var square = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255)),
            Tag = position,
            Cursor = Cursors.Hand,
            ClipToBounds = true
        };

        square.MouseLeftButtonDown += squareClickHandler;

        if (isSelected)
            square.Children.Add(CreateSelectedSquareHighlight());

        if (isLegalTarget)
        {
            bool isCapture = piece is not null && piece.PieceColor != playerColor;
            square.Children.Add(CreateLegalMoveIndicator(isCapture));
        }

        if (piece is not null)
            square.Children.Add(_pieceImageService.CreatePieceImage(piece.PieceType, piece.PieceColor, isSelected));

        return square;
    }

    private static Border CreateSelectedSquareHighlight()
    {
        return new Border
        {
            Margin = new Thickness(3),
            CornerRadius = new CornerRadius(4),
            Background = new SolidColorBrush(Color.FromArgb(36, 255, 238, 160)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 240, 175)),
            BorderThickness = new Thickness(2.5),
            IsHitTestVisible = false,
            Effect = new DropShadowEffect
            {
                Color = Color.FromRgb(255, 230, 150),
                BlurRadius = 24,
                ShadowDepth = 0,
                Opacity = 0.95
            }
        };
    }

    private static UIElement CreateLegalMoveIndicator(bool isCapture)
    {
        if (isCapture)
        {
            return new Border
            {
                Margin = new Thickness(4),
                CornerRadius = new CornerRadius(4),
                Background = new SolidColorBrush(Color.FromArgb(34, 176, 68, 52)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 228, 150)),
                BorderThickness = new Thickness(2.8),
                IsHitTestVisible = false,
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(255, 210, 110),
                    BlurRadius = 24,
                    ShadowDepth = 0,
                    Opacity = 0.9
                }
            };
        }

        return new Border
        {
            Margin = new Thickness(5),
            CornerRadius = new CornerRadius(4),
            Background = new SolidColorBrush(Color.FromArgb(22, 255, 245, 190)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(235, 255, 238, 165)),
            BorderThickness = new Thickness(2.4),
            IsHitTestVisible = false,
            Effect = new DropShadowEffect
            {
                Color = Color.FromRgb(255, 228, 140),
                BlurRadius = 22,
                ShadowDepth = 0,
                Opacity = 0.82
            }
        };
    }
}

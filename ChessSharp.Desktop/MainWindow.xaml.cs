using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ChessSharp.AI;
using ChessSharp.Board;
using ChessSharp.Enums;
using ChessSharp.Game;

namespace ChessSharp.Desktop;

public partial class MainWindow : Window
{
    private ChessGame _game = new();
    private ChessBot _bot = new(PieceColor.Black);
    private PieceColor _playerColor = PieceColor.White;

    private BoardPosition? _selectedPosition;
    private List<BoardPosition> _legalTargetPositions = new();
    private TaskCompletionSource<PieceColor>? _colorSelectionSource;
    private TaskCompletionSource<PieceType>? _promotionSelectionSource;

    private static readonly Dictionary<string, BitmapSource> PieceBitmapCache = new();

    public MainWindow()
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;
        ConfigureColorOptions();
        ConfigurePromotionOptions();
        RenderBoard();
        UpdateStatusMessage("ESCOLHA SUA COR.");
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
                var square = CreateSquare(position);

                Grid.SetRow(square, row);
                Grid.SetColumn(square, column);

                BoardGrid.Children.Add(square);
            }
        }
    }

    private Grid CreateSquare(BoardPosition position)
    {
        var piece = _game.Board.GetPieceAt(position);

        bool isSelected = _selectedPosition is not null &&
                          _selectedPosition.Value == position;

        bool isLegalTarget = _legalTargetPositions.Contains(position);

        var square = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255)),
            Tag = position,
            Cursor = System.Windows.Input.Cursors.Hand,
            ClipToBounds = true
        };

        square.MouseLeftButtonDown += Square_MouseLeftButtonDown;

        if (isSelected)
            square.Children.Add(CreateSelectedSquareHighlight());

        if (isLegalTarget)
        {
            bool isCapture = piece is not null &&
                             piece.PieceColor != _playerColor;

            square.Children.Add(CreateLegalMoveIndicator(isCapture));
        }

        if (piece is not null)
            square.Children.Add(CreatePieceImage(piece.PieceType, piece.PieceColor, isSelected));

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

    private static UIElement CreatePieceImage(
        PieceType pieceType,
        PieceColor pieceColor,
        bool isSelected = false)
    {
        string imagePath = GetPieceImagePath(pieceType, pieceColor);
        var profile = GetPieceRenderProfile(pieceType);

        var image = new Image
        {
            Source = GetOrCreateNormalizedPieceBitmap(imagePath),
            Height = profile.Height,
            MaxWidth = profile.MaxWidth,
            Stretch = Stretch.Uniform,
            SnapsToDevicePixels = true,
            UseLayoutRounding = true,
            IsHitTestVisible = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 0, 0, profile.BottomMargin),
            Effect = isSelected
                ? CreateSelectedPieceGlow()
                : CreateDefaultPieceShadow()
        };

        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
        RenderOptions.SetEdgeMode(image, EdgeMode.Unspecified);

        return new Grid
        {
            IsHitTestVisible = false,
            ClipToBounds = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Children =
            {
                image
            }
        };
    }

    private static PieceRenderProfile GetPieceRenderProfile(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Pawn => new PieceRenderProfile(
                Height: 54,
                MaxWidth: 38,
                BottomMargin: 6),

            PieceType.Rook => new PieceRenderProfile(
                Height: 61,
                MaxWidth: 50,
                BottomMargin: 5),

            PieceType.Knight => new PieceRenderProfile(
                Height: 62,
                MaxWidth: 53,
                BottomMargin: 5),

            PieceType.Bishop => new PieceRenderProfile(
                Height: 66,
                MaxWidth: 49,
                BottomMargin: 5),

            PieceType.Queen => new PieceRenderProfile(
                Height: 66,
                MaxWidth: 50,
                BottomMargin: 5),

            PieceType.King => new PieceRenderProfile(
                Height: 65,
                MaxWidth: 52,
                BottomMargin: 5),

            _ => throw new InvalidOperationException("Tipo de peça inválido.")
        };
    }

    private static BitmapSource GetOrCreateNormalizedPieceBitmap(string imagePath)
    {
        if (PieceBitmapCache.TryGetValue(imagePath, out var cachedBitmap))
            return cachedBitmap;

        var normalizedBitmap = CreateNormalizedPieceBitmap(imagePath);
        PieceBitmapCache[imagePath] = normalizedBitmap;

        return normalizedBitmap;
    }

    private static BitmapSource CreateNormalizedPieceBitmap(string imagePath)
    {
        var originalBitmap = new BitmapImage();

        originalBitmap.BeginInit();
        originalBitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
        originalBitmap.CacheOption = BitmapCacheOption.OnLoad;
        originalBitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        originalBitmap.EndInit();
        originalBitmap.Freeze();

        var convertedBitmap = new FormatConvertedBitmap(
            originalBitmap,
            PixelFormats.Bgra32,
            null,
            0);

        var writableBitmap = new WriteableBitmap(convertedBitmap);

        int width = writableBitmap.PixelWidth;
        int height = writableBitmap.PixelHeight;
        int stride = width * 4;

        byte[] pixels = new byte[height * stride];
        writableBitmap.CopyPixels(pixels, stride, 0);

        RemoveWhiteBackgroundConnectedToEdges(pixels, width, height, stride);

        var contentBounds = FindOpaqueContentBounds(pixels, width, height, stride);

        writableBitmap.WritePixels(
            new Int32Rect(0, 0, width, height),
            pixels,
            stride,
            0);

        writableBitmap.Freeze();

        if (contentBounds.IsEmpty)
            return writableBitmap;

        var croppedBitmap = new CroppedBitmap(writableBitmap, contentBounds);
        croppedBitmap.Freeze();

        return croppedBitmap;
    }

    private static void RemoveWhiteBackgroundConnectedToEdges(
        byte[] pixels,
        int width,
        int height,
        int stride)
    {
        bool[] visited = new bool[width * height];
        var queue = new Queue<int>();

        void TryEnqueue(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            int index = y * width + x;

            if (visited[index])
                return;

            int pixelIndex = y * stride + x * 4;

            if (!IsBackgroundPixel(pixels, pixelIndex))
                return;

            visited[index] = true;
            queue.Enqueue(index);
        }

        for (int x = 0; x < width; x++)
        {
            TryEnqueue(x, 0);
            TryEnqueue(x, height - 1);
        }

        for (int y = 0; y < height; y++)
        {
            TryEnqueue(0, y);
            TryEnqueue(width - 1, y);
        }

        while (queue.Count > 0)
        {
            int index = queue.Dequeue();

            int x = index % width;
            int y = index / width;

            TryEnqueue(x + 1, y);
            TryEnqueue(x - 1, y);
            TryEnqueue(x, y + 1);
            TryEnqueue(x, y - 1);
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                if (!visited[index])
                    continue;

                int pixelIndex = y * stride + x * 4;

                pixels[pixelIndex + 0] = 0;
                pixels[pixelIndex + 1] = 0;
                pixels[pixelIndex + 2] = 0;
                pixels[pixelIndex + 3] = 0;
            }
        }
    }

    private static bool IsBackgroundPixel(byte[] pixels, int pixelIndex)
    {
        byte blue = pixels[pixelIndex + 0];
        byte green = pixels[pixelIndex + 1];
        byte red = pixels[pixelIndex + 2];
        byte alpha = pixels[pixelIndex + 3];

        if (alpha <= 10)
            return true;

        int max = Math.Max(red, Math.Max(green, blue));
        int min = Math.Min(red, Math.Min(green, blue));
        int saturation = max - min;
        int brightness = (red + green + blue) / 3;

        bool isWhiteOrLightGray = brightness >= 222 && saturation <= 32;
        bool isAlmostPureWhite = red >= 235 && green >= 235 && blue >= 235;

        return isWhiteOrLightGray || isAlmostPureWhite;
    }

    private static Int32Rect FindOpaqueContentBounds(
        byte[] pixels,
        int width,
        int height,
        int stride)
    {
        const byte alphaThreshold = 12;

        int left = width;
        int top = height;
        int right = -1;
        int bottom = -1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int pixelIndex = y * stride + x * 4;
                byte alpha = pixels[pixelIndex + 3];

                if (alpha <= alphaThreshold)
                    continue;

                left = Math.Min(left, x);
                top = Math.Min(top, y);
                right = Math.Max(right, x);
                bottom = Math.Max(bottom, y);
            }
        }

        if (right < left || bottom < top)
            return Int32Rect.Empty;

        return new Int32Rect(
            left,
            top,
            right - left + 1,
            bottom - top + 1);
    }

    private static Effect CreateSelectedPieceGlow()
    {
        return new DropShadowEffect
        {
            Color = Color.FromRgb(255, 235, 150),
            BlurRadius = 26,
            ShadowDepth = 0,
            Opacity = 1
        };
    }

    private static Effect CreateDefaultPieceShadow()
    {
        return new DropShadowEffect
        {
            Color = Colors.Black,
            BlurRadius = 9,
            ShadowDepth = 2,
            Opacity = 0.5
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

        return $"pack://application:,,,/Assets/Images/Pieces/{colorName}-{pieceName}.png";
    }

    private async void Square_MouseLeftButtonDown(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_game.IsFinished ||
            _promotionSelectionSource is not null ||
            _colorSelectionSource is not null)
            return;

        if (sender is not Grid square || square.Tag is not BoardPosition clickedPosition)
            return;

        if (_game.CurrentTurn != _playerColor)
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

        if (piece is null || piece.PieceColor != _playerColor)
        {
            ClearSelection();

            UpdateStatusMessage(
                piece is null
                    ? "ESCOLHA UMA PEÇA CLARA."
                    : $"VOCÊ JOGA COM AS PEÇAS {GetColorName(_playerColor)}.");

            RenderBoard();
            return;
        }

        _selectedPosition = position;

        _legalTargetPositions = ChessRules
            .GetLegalMoves(_game.Board, _playerColor)
            .Where(move => move.Origin == position)
            .Select(move => move.Target)
            .Distinct()
            .ToList();

        if (_legalTargetPositions.Count == 0)
        {
            UpdateStatusMessage(
                ChessRules.IsKingInCheck(_game.Board, _playerColor)
                    ? "REI EM PERIGO. PROTEJA-O."
                    : "ESTA PEÇA NÃO POSSUI MOVIMENTOS.");

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
        var movingPiece = _game.Board.GetPieceAt(origin);

        if (movingPiece is null)
            return;

        PieceType? promotionPieceType = null;

        if (ChessRules.IsPawnPromotionMove(movingPiece, targetPosition))
        {
            promotionPieceType = await RequestPromotionPieceAsync();

            if (promotionPieceType is null)
                return;
        }

        var result = _game.TryMove(new Move(origin, targetPosition, promotionPieceType));

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
        if (_game.CurrentTurn != _bot.BotColor)
            return;

        UpdateStatusMessage("OPONENTE ESTÁ PENSANDO...");
        await Task.Delay(300);

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
        _ = PromptForColorAndStartGameAsync();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void PromotionOptionButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not string pieceTypeName ||
            !Enum.TryParse(pieceTypeName, out PieceType selectedPiece))
        {
            return;
        }

        HidePromotionOverlay();
        _promotionSelectionSource?.TrySetResult(selectedPiece);
    }

    private async void ColorOptionButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not string colorName ||
            !Enum.TryParse(colorName, out PieceColor selectedColor))
        {
            return;
        }

        HideColorSelectionOverlay();
        _colorSelectionSource?.TrySetResult(selectedColor);
        await Task.CompletedTask;
    }

    private void ClearSelection()
    {
        _selectedPosition = null;
        _legalTargetPositions.Clear();
    }

    private string GetPlayerTurnMessage()
    {
        return ChessRules.IsKingInCheck(_game.Board, _playerColor)
            ? "REI EM XEQUE."
            : "SUA VEZ DE JOGAR.";
    }

    private string GetInvalidTargetMessage()
    {
        return ChessRules.IsKingInCheck(_game.Board, _playerColor)
            ? "MOVIMENTO INVÁLIDO. PROTEJA O REI."
            : "ESCOLHA UMA CASA VÁLIDA.";
    }

    private string GetFinalMessage()
    {
        bool playerWon =
            (_playerColor == PieceColor.White && _game.Status == GameStatus.WhiteWins) ||
            (_playerColor == PieceColor.Black && _game.Status == GameStatus.BlackWins);

        return _game.Status switch
        {
            GameStatus.WhiteWins or GameStatus.BlackWins => playerWon
                ? "VITÓRIA. VOCÊ VENCEU A PARTIDA."
                : "DERROTA. O OPONENTE VENCEU.",
            GameStatus.Draw => "EMPATE.",
            GameStatus.PlayerQuit => "PARTIDA ENCERRADA.",
            _ => "JOGO FINALIZADO."
        };
    }

    private void UpdateStatusMessage(string message)
    {
        StatusText.Text = message.ToUpper();
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindow_Loaded;
        await PromptForColorAndStartGameAsync();
    }

    private void ConfigureColorOptions()
    {
        ChooseWhiteButton.Content = CreateColorOptionContent(PieceColor.White, "BRANCAS", "VOCÊ COMEÇA");
        ChooseBlackButton.Content = CreateColorOptionContent(PieceColor.Black, "PRETAS", "A MÁQUINA COMEÇA");
    }

    private void ConfigurePromotionOptions()
    {
        UpdatePromotionOptionContent();
    }

    private void UpdatePromotionOptionContent()
    {
        PromotionQueenButton.Content = CreatePromotionOptionContent(PieceType.Queen, "RAINHA", _playerColor);
        PromotionRookButton.Content = CreatePromotionOptionContent(PieceType.Rook, "TORRE", _playerColor);
        PromotionBishopButton.Content = CreatePromotionOptionContent(PieceType.Bishop, "BISPO", _playerColor);
        PromotionKnightButton.Content = CreatePromotionOptionContent(PieceType.Knight, "CAVALO", _playerColor);
    }

    private static UIElement CreateColorOptionContent(
        PieceColor pieceColor,
        string label,
        string subtitle)
    {
        var image = new Image
        {
            Source = GetOrCreateNormalizedPieceBitmap(GetPieceImagePath(PieceType.King, pieceColor)),
            Height = 88,
            Stretch = Stretch.Uniform,
            IsHitTestVisible = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Effect = CreateSelectedPieceGlow()
        };

        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

        return new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                image,
                new TextBlock
                {
                    Text = label,
                    FontSize = 14,
                    FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                },
                new TextBlock
                {
                    Text = subtitle,
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Color.FromRgb(215, 197, 162)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 0)
                }
            }
        };
    }

    private static UIElement CreatePromotionOptionContent(
        PieceType pieceType,
        string label,
        PieceColor pieceColor)
    {
        var image = new Image
        {
            Source = GetOrCreateNormalizedPieceBitmap(GetPieceImagePath(pieceType, pieceColor)),
            Height = 72,
            Stretch = Stretch.Uniform,
            IsHitTestVisible = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Effect = CreateSelectedPieceGlow()
        };

        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

        return new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                image,
                new TextBlock
                {
                    Text = label,
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                }
            }
        };
    }

    private async Task PromptForColorAndStartGameAsync()
    {
        var selectedColor = await RequestPlayerColorAsync();
        await StartNewGameAsync(selectedColor);
    }

    private async Task StartNewGameAsync(PieceColor playerColor)
    {
        _playerColor = playerColor;
        _game = new ChessGame();
        _bot = new ChessBot(ChessRules.GetOpponentColor(playerColor));

        ClearSelection();
        HidePromotionOverlay();
        UpdatePromotionOptionContent();
        RenderBoard();

        if (_game.CurrentTurn == _playerColor)
        {
            UpdateStatusMessage(GetPlayerTurnMessage());
            return;
        }

        await MakeBotMoveAsync();
    }

    private async Task<PieceColor> RequestPlayerColorAsync()
    {
        _colorSelectionSource = new TaskCompletionSource<PieceColor>();
        ColorSelectionOverlay.Visibility = Visibility.Visible;
        UpdateStatusMessage("ESCOLHA SUA COR.");

        try
        {
            return await _colorSelectionSource.Task;
        }
        finally
        {
            _colorSelectionSource = null;
        }
    }

    private async Task<PieceType?> RequestPromotionPieceAsync()
    {
        _promotionSelectionSource = new TaskCompletionSource<PieceType>();
        PromotionOverlay.Visibility = Visibility.Visible;
        UpdateStatusMessage("ESCOLHA A PEÇA DA PROMOÇÃO.");

        try
        {
            return await _promotionSelectionSource.Task;
        }
        finally
        {
            _promotionSelectionSource = null;
        }
    }

    private void HidePromotionOverlay()
    {
        PromotionOverlay.Visibility = Visibility.Collapsed;
    }

    private void HideColorSelectionOverlay()
    {
        ColorSelectionOverlay.Visibility = Visibility.Collapsed;
    }

    private static string GetColorName(PieceColor color)
    {
        return color == PieceColor.White ? "CLARAS" : "ESCURAS";
    }

    private static string ToChessNotation(BoardPosition position)
    {
        char file = (char)('a' + position.Column);
        int rank = 8 - position.Row;
        return $"{file}{rank}";
    }

    private readonly record struct PieceRenderProfile(
        double Height,
        double MaxWidth,
        double BottomMargin);
}

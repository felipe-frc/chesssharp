using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ChessSharp.Enums;

namespace ChessSharp.Desktop.Services;

public sealed class PieceImageService
{
    private static readonly Dictionary<string, BitmapSource> PieceBitmapCache = new();

    public UIElement CreatePieceImage(
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

    public UIElement CreateColorOptionContent(
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

    public UIElement CreatePromotionOptionContent(
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

    private static PieceRenderProfile GetPieceRenderProfile(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Pawn => new PieceRenderProfile(54, 38, 6),
            PieceType.Rook => new PieceRenderProfile(61, 50, 5),
            PieceType.Knight => new PieceRenderProfile(62, 53, 5),
            PieceType.Bishop => new PieceRenderProfile(66, 49, 5),
            PieceType.Queen => new PieceRenderProfile(66, 50, 5),
            PieceType.King => new PieceRenderProfile(65, 52, 5),
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

    private readonly record struct PieceRenderProfile(
        double Height,
        double MaxWidth,
        double BottomMargin);
}

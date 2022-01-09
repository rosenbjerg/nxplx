using Blurhash.ImageSharp;
using NxPlx.Application.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace NxPlx.Integrations.ImageSharp;

public class ImageSharpImageService : IImageService
{
    private readonly Encoder _blurhashEncoder;

    public ImageSharpImageService()
    {
        _blurhashEncoder = new Encoder();
    }
    
    public async Task<string> GenerateBlurhash(string inputFile)
    {
        using var image = await Resized(inputFile, 80, 80);
        using var rgb24Clone = image.CloneAs<Rgb24>();
        var isPortrait = image.Height > image.Width;
        return isPortrait 
            ? _blurhashEncoder.Encode(rgb24Clone, 3, 4) 
            : _blurhashEncoder.Encode(rgb24Clone, 4, 3);
    }
        
    public async Task ResizeImage(string imagePath, int width, int height, string outputPath)
    {
        using var image = await Resized(imagePath, width, height);
        
        var imageExtension = Path.GetExtension(outputPath);
        if (imageExtension is ".png")
        {
            await image.SaveAsPngAsync(outputPath, new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.DefaultCompression
            });
        }
        else if (imageExtension is ".jpg" or ".jpeg")
        {
            await image.SaveAsJpegAsync(outputPath, new JpegEncoder()
            {
                Quality = 80,
                Subsample = JpegSubsample.Ratio420
            });
        }
    }

    private static async Task<Image> Resized(string imagePath, int width, int height)
    {
        var image = await Image.LoadAsync(imagePath);

        image.Mutate(i => i.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(width, height)
        }));

        return image;
    }
}
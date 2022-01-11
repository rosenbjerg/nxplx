using System.Threading.Tasks;

namespace NxPlx.Application.Services;

public interface IImageService
{
    Task<string> GenerateBlurHash(string inputFile);
    Task ResizeImage(string imagePath, int width, int height, string outputPath);
}
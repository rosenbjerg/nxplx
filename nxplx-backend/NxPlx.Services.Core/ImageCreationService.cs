using System.Drawing;
using System.Drawing.Blurhash;
using System.IO;
using System.Threading.Tasks;
using FFMpegCore;
using Instances;
using NxPlx.Application.Core.Options;
using NxPlx.Models;

namespace NxPlx.Core.Services
{
    public class ImageCreationService
    {
        private readonly TempFileService _tempFileService;
        private readonly string _imageFolder;

        public ImageCreationService(FolderOptions folderOptions, TempFileService tempFileService)
        {
            _tempFileService = tempFileService;
            _imageFolder = folderOptions.Images;
        }
        
        public async Task SetPoster(IPosterImageOwner imageOwner, string inputFilePath, string outputFileName)
        {
            await CreateSizedCopy(inputFilePath, 190, 380, outputFileName);
            await CreateSizedCopy(inputFilePath, 270, 540, outputFileName);
            if (imageOwner.PosterPath != outputFileName)
                DeleteOldImages(imageOwner.PosterPath, 190, 270);
            imageOwner.PosterPath = outputFileName;
            imageOwner.PosterBlurHash = await GenerateBlurhashFromThumbnail(inputFilePath);
        }
        public async Task SetBackdrop(IBackdropImageOwner imageOwner, string inputFilePath, string outputFileName)
        {
            await CreateSizedCopy(inputFilePath, 1280, 1280, outputFileName);
            if (imageOwner.BackdropPath != outputFileName)
                DeleteOldImages(imageOwner.BackdropPath, 1280);
            imageOwner.BackdropPath = outputFileName;
            imageOwner.BackdropBlurHash = await GenerateBlurhashFromThumbnail(inputFilePath);
        }
        public async Task SetLogo(ILogoImageOwner imageOwner, string inputFilePath, string outputFileName)
        {
            await CreateSizedCopy(inputFilePath, 120, 120, outputFileName);
            if (imageOwner.LogoPath != outputFileName)
                DeleteOldImages(imageOwner.LogoPath, 120);
            imageOwner.LogoPath = outputFileName;
            imageOwner.LogoBlurHash = await GenerateBlurhashFromThumbnail(inputFilePath);
        }
        public async Task SetStill(IStillImageOwner imageOwner, string inputFilePath, string outputFileName)
        {
            await CreateSizedCopy(inputFilePath, 260, 200, outputFileName);
            if (imageOwner.StillPath != outputFileName)
                DeleteOldImages(imageOwner.StillPath, 260);
            imageOwner.StillPath = outputFileName;
            imageOwner.StillBlurHash = await GenerateBlurhashFromThumbnail(inputFilePath);
        }

        private void DeleteOldImages(string oldFilePath, params int[] sizes)
        {
            if (string.IsNullOrEmpty(oldFilePath)) return;
            foreach (var size in sizes)
            {
                var oldImagePath = Path.Combine(_imageFolder, $"w{size}", oldFilePath);
                if (File.Exists(oldImagePath))
                    File.Delete(oldImagePath);
            }
        }

        public async Task<string> CreateSnapshot(string inputVideoFilePath, double seek, int width, int height)
        {
            var tempFile = _tempFileService.GetFilename("generated", ".png");
            var analysis = await FFProbe.AnalyseAsync(inputVideoFilePath);
            await FFMpeg.SnapshotAsync(analysis, tempFile, new Size(width, height), analysis.Duration * seek);
            return tempFile;
        }

        private async Task CreateSizedCopy(string tempFile, int width, int height, string filename)
        {
            var directory = Path.Combine(_imageFolder, $"w{width}");
            Directory.CreateDirectory(directory);
            await ResizeImage(tempFile, width, height, Path.Combine(directory, filename));
        }

        private async Task<string> GenerateBlurhashFromThumbnail(string tempFile)
        {
            var tempMiniature = _tempFileService.GetFilename("thumb", ".jpg");
            await ResizeImage(tempFile, 90, 90, tempMiniature);
            
            var encoder = new Encoder();
            await using var imageStream = File.OpenRead(tempMiniature);
            using var image = Image.FromStream(imageStream);
            if (image.Width > image.Height)
                return encoder.Encode(image, 4, 3);
            else
                return encoder.Encode(image, 3, 4);
        }
        
        private static async Task ResizeImage(string imagePath, int width, int height, string outputPath)
        {
            var imageExtension = Path.GetExtension(outputPath);
            var arg = $"\"{imagePath}\" -resize {height}x{width} -strip -trim -quality 80";
            if (imageExtension == ".png")
                arg += "-format PNG24";
            else if (imageExtension == ".jpg" || imageExtension == ".jpeg") 
                arg += " -interlace plane -format JPG";

            arg += $" \"{outputPath}\"";
            await Instance.FinishAsync("magick", arg);
        }
    }
}
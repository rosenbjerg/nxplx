using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders.Physical;

namespace NxPlx.ApplicationHost.Api
{
    public static class StaticFileExtensions
    {
        private static readonly Regex HashRegex = new("\\.[0-9a-f]{5}\\.", RegexOptions.Compiled); 
        private static readonly FileExtensionContentTypeProvider FileExtensionContentTypeProvider = new();

        public static IApplicationBuilder UseStaticFileHandler(this IApplicationBuilder applicationBuilder, string directory)
        {
            return applicationBuilder.Use(async (context, next) =>
            {
                var path = context.Request.Path.ToString().TrimStart('/');
                var file = Path.Combine(directory, path);
                var fileInfo = File.Exists(file)
                    ? new FileInfo(file)
                    : new FileInfo(Path.Combine(directory, "index.html"));
                if (!context.Response.HasStarted)
                {
                    if (HashRegex.IsMatch(path))
                        context.Response.Headers.Add("Cache-Control", "max-age=2592000");
                
                    if(!FileExtensionContentTypeProvider.TryGetContentType(fileInfo.Name, out var contentType))
                        contentType = "application/octet-stream";
                    context.Response.ContentType = contentType;
                    context.Response.StatusCode = 200;
                }
                await context.Response.SendFileAsync(new PhysicalFileInfo(fileInfo));
                await context.Response.CompleteAsync();
            });
        }
    }
}
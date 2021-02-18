using System;
using System.Collections.Generic;
using System.IO;
using NxPlx.Application.Core.Options;

namespace NxPlx.Application.Services
{
    public class TempFileService : IDisposable
    {
        private readonly List<string> _tempFiles = new();
        private readonly string _tempFileFolder;

        public TempFileService(FolderOptions folderOptions)
        {
            _tempFileFolder = folderOptions.TempFiles ?? Path.GetTempPath();
        }
        
        public string GetFilename(string type, string extension)
        {
            var tempFile = Path.Combine(_tempFileFolder, $"temp_{type}_{Guid.NewGuid()}{extension}");
            _tempFiles.Add(tempFile);
            return tempFile;
        }
        
        
        ~TempFileService()
        {
            ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            foreach (var tempFile in _tempFiles)
            {
                try
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                }
            }
            _tempFiles.Clear();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}
using System;

namespace NxPlx.Models.File
{
    public abstract class PhysicalFileBase
    {
        public int Id { get; set; }
        public long FileSizeBytes { get; set; }
        
        public string Path { get; set; }

        public DateTime LastWrite { get; set; }
    }
}
using System;

namespace NxPlx.Domain.Models.File
{
    public abstract class PhysicalFileBase : EntityBase
    {
        public long FileSizeBytes { get; set; }
        
        public string Path { get; set; }

        public DateTime LastWrite { get; set; }
    }
}
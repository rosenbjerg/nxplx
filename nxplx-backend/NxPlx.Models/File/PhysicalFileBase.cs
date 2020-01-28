using System;
using NxPlx.Models.Database;

namespace NxPlx.Models.File
{
    public abstract class PhysicalFileBase : IAdded
    {
        public int Id { get; set; }
        public long FileSizeBytes { get; set; }
        
        public string Path { get; set; }
        
        public DateTime Added { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime LastWrite { get; set; }
    }
}
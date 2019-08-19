using System;

namespace NxPlx.Models.File
{
    public abstract class PhysicalFile : Entity
    {
        public long FileSizeBytes { get; set; }
        
        public string Path { get; set; }
        
        public int PathHash { get; set; }
        
        public DateTime Added { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime LastWrite { get; set; }
    }
}
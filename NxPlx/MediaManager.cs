using System.Collections.Generic;
using NxPlx.Models;

namespace NxPlx
{
    class MediaManager
    {
        public List<Library<MediaFileBase>> Libraries { get; } = new List<Library<MediaFileBase>>();
    }
}
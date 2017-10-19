namespace NxPlx.Models
{
    abstract class MediaFileBase
    {
        public string FilePath { get; set; }
        public string SymLink { get; set; }

        public void CreateSymlink(string link)
        {
            SymbolicLinker.CreateSymlink(FilePath, link);
        }
    }
}
namespace NxPlx.Application.Core
{
    public static class JobQueueNames
    {
        public const string GenreIndexing = "a_genre_indexing";
        public const string FileIndexing = "b_file_indexing";
        public const string ImageProcessing = "c_image-processing";
        public const string FileAnalysis = "d_file_analysis";

        public static string[] All = {
            GenreIndexing,
            FileIndexing,
            ImageProcessing,
            FileAnalysis
        };
    }
}
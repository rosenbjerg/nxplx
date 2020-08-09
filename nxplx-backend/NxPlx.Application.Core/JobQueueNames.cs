namespace NxPlx.Application.Core
{
    public static class JobQueueNames
    {
        public const string FileIndexing = "a_file_indexing";
        public const string ImageProcessing = "b_image-processing";
        public const string FileAnalysis = "c_file_analysis";

        public static string[] All = {
            FileIndexing,
            ImageProcessing,
            FileAnalysis
        };
    }
}
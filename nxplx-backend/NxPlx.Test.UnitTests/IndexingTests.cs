using System.Linq;
using NUnit.Framework;
using NxPlx.Services.Index;

namespace NxPlx.Test.UnitTests
{
    public class IndexingTests
    {
        [TestCase("Blade Runner 2049 (2017) 2160p.mp4", "Blade Runner 2049", 2017)]
        [TestCase("blade.runner.2049.2017.1080p.mp4", "Blade Runner 2049", 2017)]
        [TestCase("2001 - A Space Odyssey (1968) - 1080p.mp4", "2001 A Space Odyssey", 1968)]
        [TestCase("2001.a.space.odyssey.1968.1080p.mp4", "2001 A Space Odyssey", 1968)]
        public void IndexFilmWithResolutionInFilename(string filename, string expectedTitle, int expectedYear)
        {
            var indexed = FileIndexer.IndexFilmFiles(new [] { filename }, 0).Single();
            
            Assert.AreEqual(expectedTitle, indexed.Title, "Title is not the same");
            Assert.AreEqual(expectedYear, indexed.Year, "Year is not the same");
        }
        
        [TestCase("Blade Runner 2049 (2017).mp4", "Blade Runner 2049", 2017)]
        [TestCase("blade.runner.2049.2017.mp4", "Blade Runner 2049", 2017)]
        [TestCase("2001 - A Space Odyssey (1968).mp4", "2001 A Space Odyssey", 1968)]
        [TestCase("2001.a.space.odyssey.1968.mp4", "2001 A Space Odyssey", 1968)]
        public void IndexFilmWithYearInTitle(string filename, string expectedTitle, int expectedYear)
        {
            var indexed = FileIndexer.IndexFilmFiles(new [] { filename }, 0).Single();
            
            Assert.AreEqual(expectedTitle, indexed.Title, "Title is not the same");
            Assert.AreEqual(expectedYear, indexed.Year, "Year is not the same");
        }
    }
}
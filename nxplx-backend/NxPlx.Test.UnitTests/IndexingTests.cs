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
        [TestCase("Futurama - The Beast With A Billion Backs (2008).mp4", "Futurama The Beast With A Billion Backs", 2008)]
        public void IndexFilmWithYearInTitle(string filename, string expectedTitle, int expectedYear)
        {
            var indexed = FileIndexer.IndexFilmFiles(new [] { filename }, 0).Single();
            
            Assert.AreEqual(expectedTitle, indexed.Title, "Title is not the same");
            Assert.AreEqual(expectedYear, indexed.Year, "Year is not the same");
        }
        
        [TestCase("Great 88 - S03E03.mp4", "Great 88", 3, 3)]
        [TestCase("The Simpsons - S03E03.mp4", "The Simpsons", 3, 3)]
        [TestCase("The Simpsons - 7x06 - Treehouse of Horror VI.mp4", "The Simpsons", 7, 6)]
        [TestCase("Jeremy Clarkson'S Motorworld - S02E06 Dubai.mp4", "Jeremy Clarkson's Motorworld", 2, 6)]
        [TestCase("Its Always Sunny in Philadelphia - S03E05.mp4", "Its Always Sunny In Philadelphia", 3, 5)]
        public void IndexEpisodes(string filename, string expectedName, int expectedSeason, int expectedEpisode)
        {
            var indexed = FileIndexer.IndexEpisodeFiles(new [] { filename }, 0).Single();
            
            Assert.AreEqual(expectedName, indexed.Name);
            Assert.AreEqual(expectedSeason, indexed.SeasonNumber);
            Assert.AreEqual(expectedEpisode, indexed.EpisodeNumber);
        }
    }
}
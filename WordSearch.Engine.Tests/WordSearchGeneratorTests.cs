using WordSearch.Engine.Logic;
using WordSearch.Engine.Models;
using Xunit;

namespace WordSearch.Engine.Tests
{
    public class WordSearchGeneratorTests
    {
        [Fact]
        public void Constructor_WithValidGrid_CreatesInstance()
        {
            var grid = new WordSearchGrid(10);

            var generator = new WordSearchGenerator(grid);

            Assert.NotNull(generator);
        }


    }
}

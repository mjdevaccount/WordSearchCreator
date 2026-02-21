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

        [Fact]
        public void AddWord_ValidWord_ReturnsSuccess()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("hello");

            Assert.True(result.Success);
        }

        [Fact]
        public void AddWord_WordTooLong_ReturnsFailure()
        {
            var grid = new WordSearchGrid(5);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("dictionary");

            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
        }
    }
}

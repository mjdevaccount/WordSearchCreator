using WordSearch.Engine.Models;
using Xunit;

namespace WordSearch.Engine.Tests
{
    public class SmokeTest
    {
        [Fact]
        public void CanRun()
        {
            Assert.True(true);
        }

        [Fact]
        public void CanCreateGrid()
        {
            var grid = new WordSearchGrid(10);

            Assert.Equal(10, grid.Size);
            Assert.Equal(10, grid.Grid.GetLength(0));
            Assert.Equal(10, grid.Grid.GetLength(1));
            Assert.Empty(grid.IncludedWords);
        }
    }
}

using System;
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

        [Fact]
        public void AddWord_EmptyString_ReturnsFailure()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("");

            Assert.False(result.Success);
        }

        [Fact]
        public void AddWord_ContainsNumbers_ReturnsFailure()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("hello123");

            Assert.False(result.Success);
        }

        [Fact]
        public void AddWord_ContainsSpaces_ReturnsFailure()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("hello world");

            Assert.False(result.Success);
        }

        [Fact]
        public void AddWord_DuplicateWord_ReturnsFailure()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("hello");
            var result = generator.AddWord("hello");

            Assert.False(result.Success);
        }

        [Fact]
        public void AddWord_DuplicateWordDifferentCase_ReturnsFailure()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("Hello");
            var result = generator.AddWord("HELLO");

            Assert.False(result.Success);
        }

        [Fact]
        public void AddWord_ValidWord_AddsToIncludedWords()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("hello");

            Assert.Single(grid.IncludedWords);
            Assert.Equal("HELLO", grid.IncludedWords[0].Word);
        }

        [Fact]
        public void AddWord_ValidWord_GridContainsLetters()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("HELLO");

            int filledCells = 0;
            for (int row = 0; row < grid.Size; row++)
            {
                for (int col = 0; col < grid.Size; col++)
                {
                    if (grid.Grid[row, col] != WordSearchGrid.Empty)
                        filledCells++;
                }
            }

            Assert.Equal(5, filledCells);
        }

        [Fact]
        public void AddWord_Horizontal_PlacesWordLeftToRight()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("HELLO", EDirection.Horizonal);

            var word = grid.IncludedWords[0];
            Assert.Equal(EDirection.Horizonal, word.Direction);
            Assert.Equal('H', grid.Grid[word.StartPosition.Row, word.StartPosition.Col]);
            Assert.Equal('O', grid.Grid[word.StartPosition.Row, word.StartPosition.Col + 4]);
        }

        [Fact]
        public void AddWord_Vertical_PlacesWordTopToBottom()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("HELLO", EDirection.Vertical);

            var word = grid.IncludedWords[0];
            Assert.Equal(EDirection.Vertical, word.Direction);
            Assert.Equal('H', grid.Grid[word.StartPosition.Row, word.StartPosition.Col]);
            Assert.Equal('O', grid.Grid[word.StartPosition.Row + 4, word.StartPosition.Col]);
        }

        [Fact]
        public void AddWord_WithSeededRandom_DiscoverValues()
        {
            var grid = new WordSearchGrid(10);
            var random = new Random(12345);
            var generator = new WordSearchGenerator(grid, random);

            generator.AddWord("HELLO");

            var word = grid.IncludedWords[0];

            Assert.Equal((4, 2), word.StartPosition);
            Assert.Equal(EDirection.Vertical, word.Direction);
        }
        
        
    }
}

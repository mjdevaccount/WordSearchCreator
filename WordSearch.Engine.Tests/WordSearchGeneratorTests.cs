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

            Assert.Equal((0, 1), word.StartPosition);
            Assert.Equal(EDirection.Vertical, word.Direction);
        }

        [Fact]
        public void AddWord_Horizontal_WordFitsExactlyAtEndOfRow()
        {
            var grid = new WordSearchGrid(5);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("HELLO", EDirection.Horizonal);

            Assert.True(result.Success);
            var word = grid.IncludedWords[0];
            Assert.Equal(0, word.StartPosition.Col);
        }

        [Fact]
        public void AddWord_Vertical_WordFitsExactlyAtEndOfColumn()
        {
            var grid = new WordSearchGrid(5);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("HELLO", EDirection.Vertical);

            Assert.True(result.Success);
            var word = grid.IncludedWords[0];
            Assert.Equal(0, word.StartPosition.Row);
        }

        [Fact]
        public void AddWord_Horizontal_MaxLengthWord()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("ABCDEFGHIJ", EDirection.Horizonal);

            Assert.True(result.Success);
            Assert.Equal(0, grid.IncludedWords[0].StartPosition.Col);
        }

        [Fact]
        public void AddWord_Vertical_MaxLengthWord()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("ABCDEFGHIJ", EDirection.Vertical);

            Assert.True(result.Success);
            Assert.Equal(0, grid.IncludedWords[0].StartPosition.Row);
        }

        [Fact]
        public void AddWord_GridSize1_SingleLetterWord_Succeeds()
        {
            var grid = new WordSearchGrid(1);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("A");

            Assert.True(result.Success);
            Assert.Equal('A', grid.Grid[0, 0]);
        }

        [Fact]
        public void AddWord_WordsCanOverlapWithMatchingLetters()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            var helloSuccess = generator.AddWord("HELLO", EDirection.Horizonal);
            var llamaSuccess = generator.AddWord("LLAMA", EDirection.Vertical);

            Assert.True(helloSuccess.Success && llamaSuccess.Success);
        }

        [Fact]
        public void AddWord_WordsCannotOverlapWithConflictingLetters()
        {
            var grid = new WordSearchGrid(5);
            var generator = new WordSearchGenerator(grid);

            var firstWordSuccess = generator.AddWord("AAAAA", EDirection.Horizonal);
            Assert.True(firstWordSuccess.Success);
            
            var secondWordFail = generator.AddWord("BBBBB", EDirection.Vertical);
            Assert.False(secondWordFail.Success);
        }

        [Fact]
        public void AddWord_3x3Grid_DOGandGIL_ShareG_Succeeds()
        {
            var grid = new WordSearchGrid(3);
            var seededRandom = new Random(0);
            var generator = new WordSearchGenerator(grid, seededRandom);

            var dogResult = generator.AddWord("DOG", EDirection.Horizonal);
            var gilResult = generator.AddWord("GIL", EDirection.Vertical);

            Assert.True(dogResult.Success);
            Assert.True(gilResult.Success);
        }

        [Fact]
        public void AddWord_3x3Grid_DOGandBAG_Conflict_Fails()
        {
            var grid = new WordSearchGrid(3);
            var seededRandom = new Random(0);
            var generator = new WordSearchGenerator(grid, seededRandom);

            var dogResult = generator.AddWord("DOG", EDirection.Horizonal);
            var bagResult = generator.AddWord("BAG", EDirection.Vertical);

            Assert.True(dogResult.Success);
            Assert.False(bagResult.Success);
        }
    }
}

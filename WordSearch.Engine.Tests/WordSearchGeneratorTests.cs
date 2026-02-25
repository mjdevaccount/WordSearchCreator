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

            generator.AddWord("HELLO", EDirection.Horizontal);

            var word = grid.IncludedWords[0];
            Assert.Equal(EDirection.Horizontal, word.Direction);
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
        public void AddWord_WithSeededRandom_IsReproducible()
        {
            var grid1 = new WordSearchGrid(10);
            var grid2 = new WordSearchGrid(10);
            var gen1 = new WordSearchGenerator(grid1, new Random(12345));
            var gen2 = new WordSearchGenerator(grid2, new Random(12345));

            gen1.AddWord("HELLO");
            gen2.AddWord("HELLO");

            Assert.Equal(grid1.IncludedWords[0].StartPosition, grid2.IncludedWords[0].StartPosition);
            Assert.Equal(grid1.IncludedWords[0].Direction, grid2.IncludedWords[0].Direction);
        }

        [Fact]
        public void AddWord_Horizontal_WordFitsExactlyAtEndOfRow()
        {
            var grid = new WordSearchGrid(5);
            var generator = new WordSearchGenerator(grid);

            var result = generator.AddWord("HELLO", EDirection.Horizontal);

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

            var result = generator.AddWord("ABCDEFGHIJ", EDirection.Horizontal);

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

            var helloSuccess = generator.AddWord("HELLO", EDirection.Horizontal);
            var llamaSuccess = generator.AddWord("LLAMA", EDirection.Vertical);

            Assert.True(helloSuccess.Success && llamaSuccess.Success);
        }

        [Fact]
        public void AddWord_WordsCannotOverlapWithConflictingLetters()
        {
            // In a size=1 grid, only one cell exists. 'A' fills it. 'B' cannot be placed.
            var grid = new WordSearchGrid(1);
            var generator = new WordSearchGenerator(grid);

            var resultA = generator.AddWord("A");
            var resultB = generator.AddWord("B");

            Assert.True(resultA.Success);
            Assert.False(resultB.Success);
        }

        [Fact]
        public void AddWord_3x3Grid_DOGandGIL_ShareG_Succeeds()
        {
            var grid = new WordSearchGrid(3);
            var seededRandom = new Random(0);
            var generator = new WordSearchGenerator(grid, seededRandom);

            var dogResult = generator.AddWord("DOG", EDirection.Horizontal);
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

            var dogResult = generator.AddWord("DOG", EDirection.Horizontal);
            var bagResult = generator.AddWord("BAG", EDirection.Vertical);

            Assert.True(dogResult.Success);
            Assert.False(bagResult.Success);
        }

        [Fact]
        public void FillEmptySpaces_FillsAllEmptyCells()
        {
            var grid = new WordSearchGrid(5);
            var generator = new WordSearchGenerator(grid);
            generator.AddWord("CAT");

            generator.FillEmptySpaces();

            for (int row = 0; row < grid.Size; row++)
            {
                for (int col = 0; col < grid.Size; col++)
                {
                    Assert.NotEqual(WordSearchGrid.Empty, grid.Grid[row, col]);
                }
            }
        }

        [Fact]
        public void FillEmptySpaces_OnlyFillsEmptyCells_DoesNotOverwriteWords()
        {
            var grid = new WordSearchGrid(5);
            var generator = new WordSearchGenerator(grid);
            generator.AddWord("CAT", EDirection.Horizontal);
            var word = grid.IncludedWords[0];
            var startPos = word.StartPosition;

            generator.FillEmptySpaces();

            Assert.Equal('C', grid.Grid[startPos.Row, startPos.Col]);
            Assert.Equal('A', grid.Grid[startPos.Row, startPos.Col + 1]);
            Assert.Equal('T', grid.Grid[startPos.Row, startPos.Col + 2]);
        }

        [Fact]
        public void Rebuild_WithExistingWords_ReplacesWordsInNewPositions()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid, new Random(12345));
            generator.AddWord("CAT");
            generator.AddWord("DOG");
            bool result = generator.Rebuild();
            Assert.True(result);
            Assert.Equal(2, grid.IncludedWords.Count);
        }

        [Fact]
        public void AddWord_DiagonalDownRight_PlacesWordCorrectly()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("HELLO", EDirection.DiagonalDownRight);

            var word = grid.IncludedWords[0];
            Assert.Equal(EDirection.DiagonalDownRight, word.Direction);
            Assert.Equal('H', grid.Grid[word.StartPosition.Row, word.StartPosition.Col]);
            Assert.Equal('O', grid.Grid[word.StartPosition.Row + 4, word.StartPosition.Col + 4]);
        }

        [Fact]
        public void AddWord_HorizontalReverse_PlacesWordRightToLeft()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("HELLO", EDirection.HorizontalReverse);

            var word = grid.IncludedWords[0];
            Assert.Equal(EDirection.HorizontalReverse, word.Direction);
            Assert.Equal('H', grid.Grid[word.StartPosition.Row, word.StartPosition.Col]);
            Assert.Equal('O', grid.Grid[word.StartPosition.Row, word.StartPosition.Col - 4]);
        }

        [Fact]
        public void AddWord_EndPosition_IsCorrectlyCalculated()
        {
            var grid = new WordSearchGrid(10);
            var generator = new WordSearchGenerator(grid);

            generator.AddWord("HELLO", EDirection.Horizontal);

            var word = grid.IncludedWords[0];
            Assert.Equal(word.StartPosition.Col + 4, word.EndPosition.Col);
            Assert.Equal(word.StartPosition.Row, word.EndPosition.Row);
        }
    }
}

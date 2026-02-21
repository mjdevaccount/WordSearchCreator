using System.Collections.Generic;
using System.Linq;
using WordSearch.Engine.Interface;
using WordSearch.Engine.Models;

namespace WordSearch.Engine.Logic
{
    public class WordSearchGenerator : IWordSearchGenerator
    {
        public WordSearchGrid Grid { get; }

        public WordSearchGenerator(WordSearchGrid grid)
        {
            Grid = grid;
        }

        public AddWordResult AddWord(string word)
        {
            return AddWord(word, null);
        }

        public AddWordResult AddWord(string word, EDirection? forcedDirection)
        {
            if (string.IsNullOrWhiteSpace(word))
                return AddWordResult.Failed("Word cannot be empty");

            word = word.ToUpper();

            if (!word.All(char.IsLetter))
                return AddWordResult.Failed("Word must contain only letters");

            if (word.Length > Grid.Size)
                return AddWordResult.Failed("Word is too long for the grid");

            IncludedWord typedWord = new IncludedWord(word);

            if (Grid.IncludedWords.Contains(typedWord))
                return AddWordResult.Failed("Word has already been added");

            if (!TryPlaceWord(typedWord, forcedDirection))
            {
                return AddWordResult.Failed("Could not fit word in grid");
            }
            
            Grid.IncludedWords.Add(typedWord);

            return AddWordResult.Ok();
        }

        public void FillEmptySpaces()
        {
            
        }

        public void Rebuild()
        {
            
        }

        private bool TryPlaceWord(IncludedWord word, EDirection? forcedDirection)
        {
            EDirection[] directions = forcedDirection.HasValue
                ? new[] { forcedDirection.Value }
                : GetShuffledDirections();

            foreach (var direction in directions)
            {
                var validStarts = GetValidStartingPositions(word.Word, direction);

                foreach (var start in validStarts)
                {
                    if (!CanPlace(word.Word, start, direction)) 
                        continue;
                    
                    Place(word.Word, start, direction);
                    word.StartPosition = start;
                    word.Direction = direction;
                    return true;
                }
            }

            return false;
        }

        private bool CanPlace(string word, (int Row, int Col) start, EDirection direction)
        {
            (int rowShift, int colShift) = GetShifts(direction);

            for (int i = 0; i < word.Length; i++)
            {
                int row = start.Row + (i * rowShift);
                int col = start.Col + (i * colShift);

                char existing = Grid.Grid[row, col];

                if (existing != WordSearchGrid.Empty && existing != word[i])
                    return false;
            }

            return true;
        }


        private EDirection[] GetShuffledDirections()
        {
            return new[]
            {
                EDirection.Horizonal,
                EDirection.Vertical
            };
        }

        private (int RowShift, int ColShift) GetShifts(EDirection direction)
        {
            switch (direction)
            {
                case EDirection.Horizonal:
                    return (0, 1);
                case EDirection.Vertical:
                    return (1, 0);
                default:
                    return (0, 1);
            }
        }

        private void Place(string word, (int Row, int Col) start, EDirection direction)
        {
            var (rowShift, colShift) = GetShifts(direction);

            for (int i = 0; i < word.Length; i++)
            {
                int row = start.Row + i * rowShift;
                int col = start.Col + i * colShift;

                Grid.Grid[row, col] = word[i];
            }
        }

        private List<(int Row, int Col)> GetValidStartingPositions(string word, EDirection direction)
        {
            var positions = new List<(int Row, int Col)>();
            var (rowShift, colShift) = GetShifts(direction);

            int minRow = 0;
            int maxRow = Grid.Size - word.Length * rowShift;
            int maxCol = Grid.Size - word.Length * colShift;

            for (int row = minRow; row <= maxRow; row++)
            {
                for (int col = 0; col <= maxCol; col++)
                {
                    positions.Add((row, col));
                }
            }

            return positions;
        }
    }
}

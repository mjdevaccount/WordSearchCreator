using System;
using System.Collections.Generic;
using System.Linq;
using WordSearch.Engine.Interface;
using WordSearch.Engine.Models;

namespace WordSearch.Engine.Logic
{
    public class WordSearchGenerator : IWordSearchGenerator
    {
        private readonly Random _random;
        public WordSearchGrid Grid { get; }

        public WordSearchGenerator(WordSearchGrid grid) 
            : this(grid, new Random())
        {
            
        }

        public WordSearchGenerator(WordSearchGrid grid, Random random)
        {
            Grid = grid;
            _random = random;
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
            for (int row = 0; row < Grid.Size; row++)
            {
                for (int col = 0; col < Grid.Size; col++)
                {
                    if (Grid.Grid[row, col] == WordSearchGrid.Empty)
                    {
                        Grid.Grid[row, col] = WordSearchGrid.Letters[_random.Next(WordSearchGrid.Letters.Length)];
                    }
                }
            }
        }

        public bool Rebuild()
        {
            List<string> words = Grid.IncludedWords.Select(w => w.Word).ToList();
            Grid.IncludedWords.Clear();
            ClearGrid();

            bool allPlaced = words.All(word => AddWord(word).Success);

            if (allPlaced)
            {
                FillEmptySpaces();
            }

            return allPlaced;
        }

        private void ClearGrid()
        {
            for (int row = 0; row < Grid.Size; row++)
            {
                for (int col = 0; col < Grid.Size; col++)
                {
                    Grid.Grid[row, col] = WordSearchGrid.Empty;
                }
            }
        }

        private bool TryPlaceWord(IncludedWord word, EDirection? forcedDirection)
        {
            List<EDirection> directions = forcedDirection.HasValue
                ? new List<EDirection> { forcedDirection.Value }
                : GetShuffledDirections();

            foreach (EDirection direction in directions)
            {
                List<(int Row, int Col)> validStarts = GetValidStartingPositions(word.Word, direction);
                Shuffle(validStarts);
                
                foreach ((int Row, int Col) start in validStarts
                             .Where(start => CanPlace(word.Word, start, direction)))
                {
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

        private List<EDirection> GetShuffledDirections()
        {
            var baseList = new List<EDirection>
            {
                EDirection.Horizonal,
                EDirection.Vertical
            };
            
            Shuffle(baseList);

            return baseList;
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
            int maxRow = rowShift == 0
                ? Grid.Size
                : Grid.Size - word.Length + 1;

            int maxCol = colShift == 0
                ? Grid.Size
                : Grid.Size - word.Length + 1;

            for (int row = minRow; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    positions.Add((row, col));
                }
            }

            return positions;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}

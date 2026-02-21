using System.Collections.Generic;

namespace WordSearch.Engine.Models
{
    public class WordSearchGrid
    {
        public int Size { get; }
        public char[,] Grid { get; set; }
        public List<IncludedWord> IncludedWords { get; set; }

        public WordSearchGrid(int size)
        {
            Size = size;
            Grid = new char[size, size];
            IncludedWords = new List<IncludedWord>();
        }
    }
}

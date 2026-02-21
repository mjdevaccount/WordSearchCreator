using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (word.Length > Grid.Size)
                return AddWordResult.Failed("Word is too long for the grid");

            // placement logic...

            return AddWordResult.Ok();
        }

        public void FillEmptySpaces()
        {
            
        }

        public void Rebuild()
        {
            
        }
    }
}

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

        public void AddWord(string word)
        {
            
        }

        public void FillEmptySpaces()
        {
            
        }

        public void Rebuild()
        {
            
        }
    }
}

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
            if (string.IsNullOrWhiteSpace(word))
                return AddWordResult.Failed("Word cannot be empty");

            word = word.ToUpper();

            if (!word.All(char.IsLetter))
                return AddWordResult.Failed("Word must contain only letters");

            if (word.Length > Grid.Size)
                return AddWordResult.Failed("Word is too long for the grid");

            
            
            
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

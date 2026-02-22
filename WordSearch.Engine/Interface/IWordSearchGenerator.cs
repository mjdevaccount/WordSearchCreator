using WordSearch.Engine.Models;

namespace WordSearch.Engine.Interface
{
    public interface IWordSearchGenerator
    {
        AddWordResult AddWord(string word);

        void FillEmptySpaces();

        bool Rebuild();
    }
}

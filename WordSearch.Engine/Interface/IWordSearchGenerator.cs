using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordSearch.Engine.Interface
{
    public interface IWordSearchGenerator
    {
        void AddWord(string word);

        void FillEmptySpaces();

        void Rebuild();
    }
}

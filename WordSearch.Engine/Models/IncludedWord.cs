using System;

namespace WordSearch.Engine.Models
{
    public class IncludedWord : IEquatable<IncludedWord>
    {
        public string Word { get; }

        public (int Row, int Col) StartPosition { get; set; }
        public (int Row, int Col) EndPosition { get; set; }
        public EDirection Direction { get; set; }
        public bool IsAdded { get; set; }

        public IncludedWord(string word)
        {
            Word = word.ToUpper();
        }

        public bool Equals(IncludedWord other)
        {
            return !(other is null) && string.Equals(Word, other.Word, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IncludedWord);
        }

        public override int GetHashCode()
        {
            return Word?.ToUpper().GetHashCode() ?? 0;
        }
    }
}

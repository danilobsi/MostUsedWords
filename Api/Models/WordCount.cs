namespace MyMostUsedWords.Models
{
    public class WordCount
    {
        public string Description { get; protected set; }
        public string Translation { get; protected set; }
        public int Count { get; private set; }

        public WordCount(string description, string translation)
        {
            Description = description;
            Translation = translation;
            Count = 1;
        }

        public void Increment()
        {
            Count++;
        }

        public override string ToString()
        {
            return $"{Description} ({Count}): {Translation}";
        }
    }
}

namespace MyMostUsedWords.Models
{
    public class WordCount
    {
        public string Word { get; private set; }
        public string Translation { get; set; }
        public int Count { get; set; }

        public WordCount(string word, string translation, int count)
        {
            Word = word;
            Translation = translation;
            Count = count;
        }
    }
}

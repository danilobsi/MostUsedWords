using System.Threading.Tasks;

namespace MyMostUsedWords.Models
{
    public class WordCount
    {
        Task<string> _translationTask;
        public string Translation => _translationTask.Result;
        public string Description { get; protected set; }
        public int Count { get; private set; }

        public WordCount(string description, Task<string> translationTask)
        {
            Description = description;
            _translationTask = translationTask;
            Count = 1;
        }

        public void Increment()
        {
            Count++;
        }

        public override string ToString()
        {
            return $"{Description} ({Translation})";
        }
    }
}

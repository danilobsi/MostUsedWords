using System.Threading.Tasks;

namespace MyMostUsedWords.Infrastructure
{
    public interface ITranslator
    {
        Task<string> Translate(string word, string sourceLang, string targetLang);
    }
}

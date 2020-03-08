using MyMostUsedWords.Infrastructure;
using System.Threading.Tasks;

namespace Benchmarks.Fakes
{
    public class GoogleTranslatorServiceFake : GoogleTranslatorService
    {
        public override Task<string> Translate(string word, string sourceLang, string targetLang)
        {
            return Task.FromResult(word);
        }
    }
}

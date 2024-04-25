using Microsoft.Extensions.Logging;
using Moq;
using MyMostUsedWords.Infrastructure;
using System.Threading.Tasks;

namespace Benchmarks.Fakes
{
    public class GoogleTranslatorServiceFake : GoogleTranslator
    {
        public GoogleTranslatorServiceFake() : base(new Mock<ILogger<GoogleTranslator>>().Object)
        {

        }

        public override Task<string> Translate(string word, string sourceLang, string targetLang)
        {
            return Task.FromResult(word);
        }
    }
}

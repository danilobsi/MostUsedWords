using Moq;
using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Services;
using Shouldly;
using System.Linq;
using Xunit;

namespace Tests
{
    public class MostUsedWordsTests
    {
        MostUsedWordsService sut;

        public MostUsedWordsTests()
        {
            var tranlatorService = new OfflineTranslatorService(new Mock<GoogleTranslatorService>().Object);

            sut = new MostUsedWordsService(tranlatorService);
        }

        [Fact]
        public void Get()
        {
            var text = "My text to test. It will repeat the text word";

            var result = sut.Get(text);

            result.Count.ShouldBe(9);
            var firstWord = result.FirstOrDefault();
            firstWord.Description.ShouldBe("text");
            firstWord.Count.ShouldBe(2);
        }
    }
}

using Moq;
using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Services;
using Shouldly;
using System.IO;
using System.Linq;
using System.Text;
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

            // convert string to stream
            var byteArray = Encoding.UTF8.GetBytes(text);
            var stream = new MemoryStream(byteArray);

            // convert stream to string
            var textReader = new StreamReader(stream);
            var result = sut.Get(textReader);

            result.Count().ShouldBe(9);
            var firstWord = result.FirstOrDefault();
            firstWord.Value.Description.ShouldBe("text");
            firstWord.Value.Count.ShouldBe(2);
        }
    }
}

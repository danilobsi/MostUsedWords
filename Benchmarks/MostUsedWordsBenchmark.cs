using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Benchmarks.Fakes;
using Benchmarks.PoCs;
using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Models;
using MyMostUsedWords.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class MostUsedWordsBenchmark
    {
        MostUsedWordsService sut;
        MostUsedWordsServicePoC comparisonSut;
        string text;
        StreamReader textReader;
        MemoryStream stream;

        public MostUsedWordsBenchmark()
        {
            var tranlatorService = new OfflineTranslatorService(new GoogleTranslatorServiceFake());
            for (int i = 0; i < 100; i++)
                text += "My text to test. It will repeat the text word";
            sut = new MostUsedWordsService(tranlatorService);
            comparisonSut = new MostUsedWordsServicePoC(tranlatorService);

            // convert string to stream
            var byteArray = Encoding.UTF8.GetBytes(text);
            stream = new MemoryStream(byteArray);
            
            // convert stream to string
            textReader = new StreamReader(stream);
        }

        [Benchmark]
        public List<KeyValuePair<string, WordCount>> Get()
        {
            return sut.Get(textReader).ToList();
        }

        [Benchmark]
        public List<KeyValuePair<string, WordCount>> GetDictionaryComparison()
        {
            return comparisonSut.Get(text).ToList();
        }
    }
}
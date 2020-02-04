using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Moq;
using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Models;
using MyMostUsedWords.Services;
using System.Collections.Generic;

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

        public MostUsedWordsBenchmark()
        {
            var tranlatorService = new OfflineTranslatorService(new Mock<GoogleTranslatorService>().Object);
            sut = new MostUsedWordsService(tranlatorService);
        }

        [Benchmark]
        public List<WordCount> Get()
        {
            var text = "My text to test. It will repeat the text word";
            return sut.Get(text);
        }
    }
}
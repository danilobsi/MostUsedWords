using BenchmarkDotNet.Running;
using System;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MostUsedWordsBenchmark>();
            BenchmarkRunner.Run<StringManipulationBenchmarks>();
            //var t = new MostUsedWordsBenchmark();
            //t.JsonDictionary();
            //t.CustomFormatDictionary();
            //t.CustomFormatSpanDictionary();
            //t.CustomFormatIndexDictionary();
            //t.CustomFormatReadSpanDictionary();
        }
    }
}

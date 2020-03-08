using BenchmarkDotNet.Running;
using System;
using System.Diagnostics;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            BenchmarkRunner.Run<StringManipulationBenchmarks>();

            var t = new MostUsedWordsBenchmark();

            var watch = new Stopwatch();
            watch.Start();
            t.GetDictionaryComparison();
            watch.Stop();
            Console.WriteLine($"GetDictionaryComparison: {watch.ElapsedMilliseconds}");

            watch = new Stopwatch();
            watch.Start();
            t.Get();
            watch.Stop();
            Console.WriteLine($"Get: {watch.ElapsedMilliseconds}");
#else
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#endif
        }
    }
}

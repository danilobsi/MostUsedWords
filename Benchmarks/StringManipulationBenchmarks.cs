using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MyMostUsedWords.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using Utf8Json;

namespace Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class StringManipulationBenchmarks
    {
        readonly string jsonString;
        readonly string customString;

        const int Size = 10000;

        public StringManipulationBenchmarks()
        {
            jsonString = "{";
            for (var i = 0; i < Size; i++)
            {
                customString += $"{i}:{i}\n";
                jsonString += $"\"{i}\":\"{i}\",";
            }
            customString = customString.TrimEnd('\n');
            jsonString = jsonString.TrimEnd(',');
            jsonString += "}";
        }

        [Benchmark]
        public Dictionary<string, string> JsonDictionary()
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
        }

        [Benchmark]
        public Dictionary<string, string> CustomFormatDictionary()
        {
            var lines = customString.Split('\n');

            var dictionary = new Dictionary<string, string>(lines.Length);
            foreach (var line in lines)
            {
                var words = line.Split(':');
                dictionary.Add(words[0], words[1]);
            }

            return dictionary;
        }

        [Benchmark]
        public Dictionary<string, string> CustomFormatIndexDictionary()
        {
            var lines = customString.Split('\n');

            var dictionary = new Dictionary<string, string>(lines.Length);
            foreach (var line in lines)
            {
                int index = line.IndexOf(':');
                dictionary.Add(line.Substring(0, index), line.Substring(index + 1));
            }

            return dictionary;
        }

        [Benchmark]
        public Dictionary<string, string> CustomFormatSpanDictionary()
        {
            var lines = customString.AsSpan();
            var initIndex = 0;
            var dictionary = new Dictionary<string, string>();

            ReadOnlySpan<char> key = ReadOnlySpan<char>.Empty;
            ReadOnlySpan<char> value = ReadOnlySpan<char>.Empty;

            for (var index = 0; index < customString.Length; index++)
            {
                switch (lines[index])
                {
                    case ':':
                        key = lines.Slice(initIndex, index - initIndex);
                        initIndex = index + 1;
                        break;
                    case '\n':
                        value = lines.Slice(initIndex, index - initIndex);
                        initIndex = index + 1;
                        dictionary.Add(key.ToString(), value.ToString());
                        break;                    
                }
            }

            return dictionary;
        }

        [Benchmark]
        public Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>> CustomFormatReadSpanDictionary()
        {
            var lines = customString.AsMemory();
            var initIndex = 0;
            var dictionary = new Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>();

            var key = ReadOnlyMemory<char>.Empty;
            var value = ReadOnlyMemory<char>.Empty;

            for (var index = 0; index < customString.Length; index++)
            {
                switch (lines.Span[index])
                {
                    case ':':
                        key = lines.Slice(initIndex, index - initIndex);
                        initIndex = index + 1;
                        break;
                    case '\n':
                        value = lines.Slice(initIndex, index - initIndex);
                        initIndex = index + 1;
                        dictionary.Add(key, value);
                        break;                    
                }
            }

            return dictionary;
        }

        [Benchmark]
        public void JsonFile()
        {
            using (var writer = File.CreateText("json.json"))
            {
                writer.Write(JsonSerializer.ToJsonString(LanguageDictionary.FromText(jsonString)));
            }
        }

        [Benchmark]
        public void CustomFile()
        {
            var dictionary = LanguageDictionary.FromText(jsonString);

            using (var writer = File.CreateText("custom"))
            {
                foreach(var key in dictionary.Keys)
                    writer.WriteLine($"{key}:{dictionary[key]}");
            }
        }
    }
}

using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utf8Json;

namespace MyMostUsedWords.Infrastructure
{
    public class LanguageDictionary : Dictionary<string, string>
    {
        string Filename { get; set; }

        LanguageDictionary(int capacity) : base(capacity) { }

        public static Result<LanguageDictionary> FromFile(string fileName)
        {
            try
            {
                LanguageDictionary dictionary;
                var jsonIndex = fileName.IndexOf(".json");
                var slashIndex = fileName.LastIndexOf('/') + 1;
                if (jsonIndex != -1)
                {
                    dictionary = FromText(File.ReadAllText(fileName));
                    dictionary.Filename = fileName.Substring(0, jsonIndex);
                }
                else
                {
                    dictionary = JsonSerializer.Deserialize<LanguageDictionary>(File.ReadAllText(fileName));
                    dictionary.Filename = fileName;
                }

                return Result.Ok(dictionary);
            }
            catch (Exception e)
            {
                return Result.Failure<LanguageDictionary>(e.Message);
            }
        }

        public static LanguageDictionary FromText(string text)
        {
            return GetDictionary(text);
        }

        private static LanguageDictionary GetDictionary(string text)
        {
            var lines = text.Split('\n');

            var dictionary = new LanguageDictionary(lines.Length);
            foreach (var line in lines)
            {
                int index = line.IndexOf(':');
                dictionary.Add(line.Substring(0, index), line.Substring(index + 1));
            }

            return dictionary;
        }

        public static LanguageDictionary New(string fileName)
        {
            var dictionary = new LanguageDictionary(1);
            dictionary.Filename = fileName;

            return dictionary;
        }

        public Task<bool> Save()
        {
            try
            {
                using (var writer = File.CreateText(Filename))
                {
                    foreach (var keyName in Keys)
                        writer.WriteLine($"{keyName}:{this[keyName]}");
                }
            }
            catch
            {
                Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}

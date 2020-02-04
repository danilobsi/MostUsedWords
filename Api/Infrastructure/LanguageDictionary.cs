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
            var dictionary = FromText(File.ReadAllText(fileName));

            if (dictionary.IsFailure)
                return dictionary;

            dictionary.Value.Filename = fileName;

            return dictionary;
        }

        public static Result<LanguageDictionary> FromText(string text)
        {
            try
            {
                var dictionary = GetDictionary(text);

                return Result.Ok(dictionary);
            }
            catch (Exception e)
            {
                return Result.Failure<LanguageDictionary>(e.Message);
            }
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

        public Task Save()
        {
            using (var writer = File.CreateText("custom"))
            {
                foreach (var keyName in Keys)
                    writer.WriteLine($"{keyName}:{this[keyName]}");
            }

            return Task.CompletedTask;
        }
    }
}

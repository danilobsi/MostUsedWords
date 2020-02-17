using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyMostUsedWords.Infrastructure
{
    public class LanguageDictionary : Dictionary<string, string>
    {
        string Filename { get; set; }
        public string Language { get; private set; }

        LanguageDictionary(int capacity) : base(capacity) { }

        public static Result<LanguageDictionary> FromFile(string fileName)
        {
            try
            {
                LanguageDictionary dictionary;
                var slashIndex = fileName.LastIndexOf('/') + 1;
                
                dictionary = FromText(File.ReadAllLines(fileName));
                dictionary.Filename = fileName;
                dictionary.Language = fileName.Substring(slashIndex);

                return Result.Ok(dictionary);
            }
            catch (Exception e)
            {
                return Result.Failure<LanguageDictionary>(e.Message);
            }
        }

        private static LanguageDictionary FromText(string[] lines)
        {
            var dictionary = new LanguageDictionary(lines.Length);
            foreach (var line in lines)
            {
                int index = line.IndexOf(':');
                dictionary.TryAdd(line.Substring(0, index), line.Substring(index + 1));
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

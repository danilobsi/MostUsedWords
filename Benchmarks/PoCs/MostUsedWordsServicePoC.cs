using MyMostUsedWords.Buffers;
using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Models;
using MyMostUsedWords.Services;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Benchmarks.PoCs
{
    public class MostUsedWordsServicePoC : MostUsedWordsService
    {
        public MostUsedWordsServicePoC(ITranslator translatorService) : base(translatorService)
        { }

        public virtual IEnumerable<KeyValuePair<string, WordCount>> Get(string text, string sourceLang = "en", string targetLang = "nl")
        {
            var wordsCountList = new Dictionary<string, WordCount>();
            var wordsInText = text.Split(' ', '\r', '\n');

            foreach (var word in wordsInText)
            {
                if (string.IsNullOrWhiteSpace(word))
                    continue;

                var wordCount = wordsCountList.ContainsKey(word);
                if (wordsCountList.ContainsKey(word))
                    wordsCountList[word].Increment();
                else
                {
                    var translation = _translatorService.Translate(word, sourceLang, targetLang);
                    wordsCountList.Add(word, new WordCount(word, translation));
                }
            }

            return wordsCountList.OrderByDescending(w => w.Value.Count);
        }
    }
}

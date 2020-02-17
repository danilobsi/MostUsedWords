using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyMostUsedWords.Services
{
    public class MostUsedWordsService
    {
        ITranslator _translatorService;

        public MostUsedWordsService(ITranslator translatorService)
        {
            _translatorService = translatorService;
        }

        public List<WordCount> Get(string text, string sourceLang = "en", string targetLang = "nl")
        {
            var wordsCountList = new List<WordCount>();
            var wordsInText = text.Split(' ', '\r', '\n');

            foreach (var word in wordsInText)
            {
                if (string.IsNullOrWhiteSpace(word))
                    continue;

                var wordCount = wordsCountList.FirstOrDefault(w => w.Description == word);
                if (wordCount == null)
                {
                    var translation = _translatorService.Translate(word, sourceLang, targetLang).Result;
                    wordsCountList.Add(new WordCount(word, translation));
                }
                else
                    wordCount.Increment();
            }

            return wordsCountList.OrderByDescending(w => w.Count).ToList();
        }

    }
}

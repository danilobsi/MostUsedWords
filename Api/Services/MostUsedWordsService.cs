using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyMostUsedWords.Services
{
    public class MostUsedWordsService
    {
        const string sourceLang = "en";
        const string targetLang = "pt";

        ITranslator _translatorService;

        public MostUsedWordsService(ITranslator translatorService)
        {
            _translatorService = translatorService;
        }

        public List<WordCount> Get(string text)
        {
            var wordsCountList = new List<WordCount>();
            var wordsInText = text.Split(' ');

            foreach (var word in wordsInText)
            {
                var wordCount = wordsCountList.FirstOrDefault(w => w.Description == word);
                if (wordCount == null)
                {
                    var translation = _translatorService.Translate(word, sourceLang, targetLang).Result;
                    wordsCountList.Add(new WordCount(word, translation));
                }
                else
                {
                    wordCount.Increment();
                }
            }

            return wordsCountList.OrderByDescending(w => w.Count).ToList();
        }

    }
}

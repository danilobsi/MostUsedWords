using MyMostUsedWords.Buffers;
using MyMostUsedWords.Helpers;
using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyMostUsedWords.Services
{
    public class MostUsedWordsService
    {
        protected ITranslator _translatorService;

        public MostUsedWordsService(ITranslator translatorService)
        {
            _translatorService = translatorService;
        }

        public IEnumerable<KeyValuePair<string, WordCount>> Get(StreamReader textReader, string sourceLang = "en", string targetLang = "nl")
        {
            var wordsDictionary = new Dictionary<string, WordCount>();

            using (var word = new ArrayBuffer<char>(200))
            {
                bool ignore = false;
                while (textReader.Peek() >= 0)
                {
                    var ch = textReader.Read();

                    if (ch == '<')
                        ignore = true;
                    if (ch == '>')
                        ignore = false;

                    if (ignore)
                        continue;

                    if (!ch.IsEndOfWordCharacter())
                    {
                        word.Add((char)ch);
                        continue;
                    }

                    AddWordToDictionary(wordsDictionary, word.GetWord(), sourceLang, targetLang);
                }
                AddWordToDictionary(wordsDictionary, word.GetWord(), sourceLang, targetLang);
            }

            return wordsDictionary.OrderByDescending(w => w.Value.Count);
        }

        private void AddWordToDictionary(Dictionary<string, WordCount> wordsCountList, string word, string sourceLang, string targetLang)
        {
            if (string.IsNullOrWhiteSpace(word))
                return;

            if (wordsCountList.ContainsKey(word))
                wordsCountList[word].Increment();
            else
            {
                var translation = _translatorService.Translate(word, sourceLang, targetLang);
                wordsCountList.Add(word, new WordCount(word, translation));
            }
        }
    }
}

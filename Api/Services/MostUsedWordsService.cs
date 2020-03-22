using MyMostUsedWords.Buffers;
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
            var wordsCountList = new Dictionary<string, WordCount>();

            using (var word = new ArrayBuffer<char>(200))
            {
                while (textReader.Peek() >= 0)
                {
                    var ch = textReader.Read();

                    if (IsEndOfWordCharacter(ch))
                        AddWordToDictionary(wordsCountList, word, sourceLang, targetLang);
                    else
                        word.Add((char)ch);
                }
                AddWordToDictionary(wordsCountList, word, sourceLang, targetLang);
            }

            return wordsCountList.OrderByDescending(w => w.Value.Count);
        }

        private static bool IsEndOfWordCharacter(int ch)
        {
            return ch >= 32 && ch <= 47;
        }

        private void AddWordToDictionary(Dictionary<string, WordCount> wordsCountList, ArrayBuffer<char> wordBuffer, string sourceLang, string targetLang)
        {
            var word = new string(wordBuffer.ToArray());
            word = word.ToLower();

            if (string.IsNullOrWhiteSpace(word))
                return;

            if (wordsCountList.ContainsKey(word))
                wordsCountList[word].Increment();
            else
            {
                var translation = _translatorService.Translate(word, sourceLang, targetLang).Result;
                wordsCountList.Add(word, new WordCount(word, translation));
            }

            wordBuffer.Clear();
        }
    }
}

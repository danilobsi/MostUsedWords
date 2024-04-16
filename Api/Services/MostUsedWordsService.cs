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

            using (var wordBuffer = new ArrayBuffer<char>(200))
            {
                bool ignore = false;
                bool htmlTag = false;
                int lastCharacter = 0;
                while (textReader.Peek() >= 0)
                {
                    var ch = textReader.Read();

                    if (ch == '<')
                        htmlTag = true;

                    if (!ch.IsEndOfWordCharacter())
                    {
                        wordBuffer.Add((char)ch);
                        continue;
                    }

                    var word = wordBuffer.GetWord();
                    if (htmlTag && (word == "style" || word == "script"))
                    {
                        ignore = lastCharacter != '/';
                    }

                    if (!(ignore || htmlTag))
                        AddWordToDictionary(wordsDictionary, word, sourceLang, targetLang);
 
                    if (ch == '>')
                        htmlTag = false;

                    lastCharacter = ch;
                }
                AddWordToDictionary(wordsDictionary, wordBuffer.GetWord(), sourceLang, targetLang);
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

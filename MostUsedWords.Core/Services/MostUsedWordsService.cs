using MyMostUsedWords.Buffers;
using MyMostUsedWords.Helpers;
using MyMostUsedWords.Infrastructure;
using MyMostUsedWords.Models;

namespace MyMostUsedWords.Services
{
    public class MostUsedWordsService
    {
        static string[] IgnorableHtmlTags = { "style", "script" };
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
                bool ignoreWord = false;
                bool isHtmlTag = false;
                char lastCharacter = '\0';
                char currentCharacter = '\0';

                while (textReader.Peek() >= 0)
                {
                    currentCharacter = (char)textReader.Read();

                    if (currentCharacter.IsValidLetter())
                    {
                        wordBuffer.Add(currentCharacter);
                        continue;
                    }

                    var word = wordBuffer.GetWord();
                    if (isHtmlTag && IgnorableHtmlTags.Contains(word))
                    {
                        ignoreWord = lastCharacter != '/';
                    }

                    if (!(ignoreWord || isHtmlTag))
                    {
                        AddWordToDictionary(wordsDictionary, word, sourceLang, targetLang);
                    }

                    isHtmlTag = IsHtmlTag(isHtmlTag, currentCharacter);

                    lastCharacter = currentCharacter;
                }
                AddWordToDictionary(wordsDictionary, wordBuffer.GetWord(), sourceLang, targetLang);
            }

            return wordsDictionary.OrderByDescending(w => w.Value.Count);
        }

        private static bool IsHtmlTag(bool isHtmlTag, char currentCharacter)
        {
            if (currentCharacter == '<')
            {
                return true;
            }
            if (currentCharacter == '>')
            {
                return false;
            }

            return isHtmlTag;
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

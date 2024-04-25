using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyMostUsedWords.Infrastructure
{
    public class OfflineTranslator : ITranslator, IDisposable
    {
        const string _dictionariesPath = "Infrastructure/Dictionaries/";

        IGoogleTranslator _googleTranslatorService;
        Dictionary<string, LanguageDictionary> _dictionaries;

        public OfflineTranslator(IGoogleTranslator googleTranslatorService)
        {
            _googleTranslatorService = googleTranslatorService;

            if (!Directory.Exists(_dictionariesPath))
            {
                _dictionaries = new Dictionary<string, LanguageDictionary>();
                return;
            }

            var fileNames = Directory.GetFiles(_dictionariesPath);
            _dictionaries = new Dictionary<string, LanguageDictionary>(fileNames.Length);

            foreach (var file in fileNames)
            {
                var dictionary = LanguageDictionary.FromFile(file);

                if (dictionary.IsSuccess)
                    _dictionaries.TryAdd(dictionary.Value.Language.ToLower(), dictionary.Value);
            }
        }

        public void Dispose()
        {
            foreach (var dictionary in _dictionaries.Values)
            {
                dictionary.Save();
            }
        }

        public Task<string> Translate(string word, string sourceLang, string targetLang)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return Task.FromResult(string.Empty);
            }

            word = word.ToLower();
            sourceLang = sourceLang.ToLower();
            targetLang = targetLang.ToLower();

            var dictionaryName = sourceLang + targetLang;

            if (!_dictionaries.TryGetValue(dictionaryName, out var dictionary))
            {
                dictionary = LanguageDictionary.New(_dictionariesPath + dictionaryName);
                _dictionaries.Add(dictionaryName, dictionary);
            }

            if (dictionary.TryGetValue(word, out var translationTask))
            {
                return translationTask;
            }

            translationTask = _googleTranslatorService.Translate(word, sourceLang, targetLang);
            dictionary.Add(word, translationTask);

            return translationTask;
        }
    }
}

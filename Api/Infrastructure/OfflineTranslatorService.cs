using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyMostUsedWords.Infrastructure
{
    public class OfflineTranslatorService : ITranslator
    {
        const string _dictionariesPath = "Infrastructure/Dictionaries/";

        GoogleTranslatorService _googleTranslatorService;
        Dictionary<string, LanguageDictionary> _dictionaries;

        public OfflineTranslatorService(GoogleTranslatorService googleTranslatorService)
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
                var language = file.AsSpan().Slice(file.LastIndexOf('/') + 1);
                var dictionary = LanguageDictionary.FromFile(file);

                if (dictionary.IsSuccess)
                    _dictionaries.TryAdd(language.ToString(), dictionary.Value);
            }
        }

        public async Task<string> Translate(string word, string sourceLang, string targetLang)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return string.Empty;
            }

            var dictionaryName = sourceLang + targetLang;

            if (!_dictionaries.TryGetValue(dictionaryName, out var dictionary))
            {
                dictionary = LanguageDictionary.New(_dictionariesPath + dictionaryName);
                _dictionaries.Add(dictionaryName, dictionary);
            }

            if (dictionary.TryGetValue(word, out var translation))
            {
                return translation;
            }

            translation = await _googleTranslatorService.Translate(word, sourceLang, targetLang);
            if (string.IsNullOrEmpty(translation))
            {
                return string.Empty;
            }

            dictionary.Add(word, translation);
            dictionary.Save();

            _dictionaries[dictionaryName] = dictionary;

            return translation;
        }
    }
}

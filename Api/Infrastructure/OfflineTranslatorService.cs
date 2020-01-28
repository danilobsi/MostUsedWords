using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utf8Json;

namespace MyMostUsedWords.Infrastructure
{
    public class OfflineTranslatorService : ITranslator
    {
        const string _dictionariesPath = "Infrastructure/Dictionaries/";

        GoogleTranslatorService _googleTranslatorService;
        Dictionary<string, Dictionary<string, string>> _dictionaries;

        public OfflineTranslatorService(GoogleTranslatorService googleTranslatorService)
        {
            _googleTranslatorService = googleTranslatorService;

            if (!Directory.Exists(_dictionariesPath))
            {
                _dictionaries = new Dictionary<string, Dictionary<string, string>>();
                return;
            }

            var fileNames = Directory.GetFiles(_dictionariesPath);
            _dictionaries = new Dictionary<string, Dictionary<string, string>>(fileNames.Length);

            foreach (var file in fileNames)
            {
                var language = file.AsSpan().Slice(file.LastIndexOf('/') + 1);
                var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file));
                _dictionaries.Add(language.ToString(), dictionary);
            }
        }

        public async Task<string> Translate(string word, string sourceLang, string targetLang)
        {
            if (string.IsNullOrEmpty(word))
            {
                return string.Empty;
            }

            string translation;
            Dictionary<string, string> dictionary;
            var dictionaryName = sourceLang + targetLang;

            if (!_dictionaries.TryGetValue(dictionaryName, out dictionary))
            {
                dictionary = new Dictionary<string, string>(1);
                _dictionaries.Add(dictionaryName, dictionary);
            }

            if (dictionary.TryGetValue(word, out translation))
            {
                return translation;
            }

            translation = await _googleTranslatorService.Translate(word, sourceLang, targetLang);
            if (string.IsNullOrEmpty(translation))
            {
                return string.Empty;
            }

            UpdateDictionary(word, translation, dictionary, dictionaryName);

            _dictionaries[dictionaryName] = dictionary;

            return translation;
        }

        private static void UpdateDictionary(string word, string translation, Dictionary<string, string> dictionary, string dictionaryName)
        {
            dictionary.Add(word, translation);
            using (var writer = File.CreateText(_dictionariesPath + dictionaryName))
            {
                writer.Write(JsonSerializer.ToJsonString(dictionary));
            }
        }
    }
}

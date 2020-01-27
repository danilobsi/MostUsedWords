using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMostUsedWords.Infrastructure
{
    public class OfflineTranslatorService : ITranslator
    {
        GoogleTranslatorService _googleTranslatorService;

        public OfflineTranslatorService(GoogleTranslatorService googleTranslatorService)
        {
            _googleTranslatorService = googleTranslatorService;
        }

        public Task<string> Translate(string word, string sourceLang, string targetLang)
        {
            return _googleTranslatorService.Translate(word, sourceLang, targetLang);
        }
    }
}

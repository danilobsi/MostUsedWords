using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyMostUsedWords.Infrastructure
{
    /// <summary>
    /// Examples on how to use Google Translate API:
    /// https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + encodeURI(sourceText)
    /// https://translate.googleapis.com/translate_a/single?client=gtx&sl=pt&tl=en&dt=t&q=Olá
    /// </summary>
    public class GoogleTranslatorService : ITranslator
    {
        HttpClient client;

        public GoogleTranslatorService()
        {
            client = new HttpClient();
        }

        public virtual async Task<string> Translate(string word, string sourceLang, string targetLang)
        {
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={word}";
            var response = await client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var translation = await response.Content.ReadAsStringAsync();
                translation = translation.Substring(translation.IndexOf("\"") + 1);
                translation = translation.Substring(0, translation.IndexOf("\""));

                return translation;
            }
            return string.Empty;
        }
    }
}

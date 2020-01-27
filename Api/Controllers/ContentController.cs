using Microsoft.AspNetCore.Mvc;
using MyMostUsedWords.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MyMostUsedWords.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController: Controller
    {
        const string sourceLang = "pt";
        const string targetLang = "en";
        
        HttpClient client;

        public ContentController()
        {
            client = new HttpClient();
        }

        [HttpPost]
        //public async Task<IEnumerable<WordCount>> Post()
        public async Task<string> Post()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var text = await reader.ReadToEndAsync();

            var wordsCountList = new List<WordCount>();
            var wordsInText = text.Split(' ');

            foreach (var word in wordsInText)
            {
                var wordCount = wordsCountList.FirstOrDefault(w => w.Word == word);
                if (wordCount == null)
                {
                    var translation = "";// await Translate(word);
                    wordsCountList.Add(new WordCount(word, translation, 1));
                }
                else
                {
                    wordCount.Count++;
                }
            }

            return GetResponse(wordsCountList.OrderByDescending(w => w.Count));

            //return wordsCountList.OrderByDescending(w => w.Count);
        }

        public async Task<string> Translate(string word)
        {
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={word}";
            var response = await client.GetAsync(url);
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var translation = await response.Content.ReadAsStringAsync();
                translation = translation.Substring(translation.IndexOf("\""));
                translation = translation.Substring(0, translation.IndexOf("\""));

                return translation;
            }
            return string.Empty;
        }

        public string GetResponse(IEnumerable<WordCount> words)
        {
            var result = "";
            foreach(var word in words)
            {
                result += $"{word.Word},{word.Count}\n";
            }
            return result;
        }
    }    
}

//https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + encodeURI(sourceText)
//https://translate.googleapis.com/translate_a/single?client=gtx&sl=pt&tl=en&dt=t&q=Olá
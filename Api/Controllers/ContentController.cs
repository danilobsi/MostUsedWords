using Microsoft.AspNetCore.Mvc;
using MyMostUsedWords.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using MyMostUsedWords.Services;
using System.Text;

namespace MyMostUsedWords.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController: Controller
    {
        MostUsedWordsService _mostUsedWordsService;

        public ContentController(MostUsedWordsService mostUsedWordsService)
        {
            _mostUsedWordsService = mostUsedWordsService;
        }

        [HttpPost]
        //public async Task<IEnumerable<WordCount>> Post()
        public async Task<string> Post()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var text = await reader.ReadToEndAsync();
            
            return GetResponse(_mostUsedWordsService.Get(text));

            //return wordsCountList.OrderByDescending(w => w.Count);
        }

        public string GetResponse(IList<WordCount> words)
        {
            var result = new StringBuilder();
            for(var i = 1; i <= words.Count; i++)
            {   
                result.Append($"{i}. {words[i].ToString()}\n");
            }
            return result.ToString();
        }
    }    
}
using Microsoft.AspNetCore.Mvc;
using MyMostUsedWords.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using MyMostUsedWords.Services;
using System.Text;

namespace MyMostUsedWords.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopController: Controller
    {
        MostUsedWordsService _mostUsedWordsService;

        public TopController(MostUsedWordsService mostUsedWordsService)
        {
            _mostUsedWordsService = mostUsedWordsService;
        }

        [HttpPost]
        public async Task<string> Post()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var text = await reader.ReadToEndAsync();
            
            return GetResponse(_mostUsedWordsService.Get(text));
        }

        [HttpPost("{src}/{target}")]
        public async Task<string> Post(string src, string target)
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var text = await reader.ReadToEndAsync();

            return GetResponse(_mostUsedWordsService.Get(text, src, target));
        }

        public string GetResponse(IList<WordCount> words)
        {
            var result = new StringBuilder();
            for(var i = 0; i < words.Count; i++)
            {   
                result.Append($"{words[i].ToString()}\n");
            }
            return result.ToString();
        }
    }    
}
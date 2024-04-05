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

        [HttpPost("{src}/{target}")]
        public async Task<string> Post(string src, string target)
        {
            using var reader = new StreamReader(HttpContext.Request.Body);

            return GetResponse(_mostUsedWordsService.Get(reader, src, target));
        }

        public string GetResponse(IEnumerable<KeyValuePair<string, WordCount>> words)
        {
            var result = new StringBuilder();
            foreach (var word in words)
            {
                result.Append($"{word.Value.ToString()}:{word.Value.Count}\n");
            }
            return result.ToString();
        }
    }    
}
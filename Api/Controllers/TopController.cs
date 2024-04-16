using Microsoft.AspNetCore.Mvc;
using MyMostUsedWords.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using MyMostUsedWords.Services;
using System.Text;
using System.Net.Http;
using System;
using Microsoft.Extensions.Primitives;

namespace MyMostUsedWords.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopController : Controller
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

        [HttpPost("website/{src}/{target}")]
        public async Task<string> Get(string src, string target)
        {
            using var websiteReader = new StreamReader(HttpContext.Request.Body);
            var website = await websiteReader.ReadToEndAsync();

            HttpClient client = new()
            {
                BaseAddress = new Uri($"https://{website}"),
            };
            using var reader = new StreamReader(await client.GetStreamAsync(string.Empty));

            return GetResponse(_mostUsedWordsService.Get(reader, src, target));
        }

        public string GetResponse(IEnumerable<KeyValuePair<string, WordCount>> words)
        {
            var result = new StringBuilder();
            int wordCount = 0;
            foreach (var word in words)
            {
                result.AppendLine($"{word.Value.ToString()}: {word.Value.Count}");
                wordCount += word.Value.Count;
            }
            result.Append($"Total Word count: {wordCount}");
            return result.ToString();
        }
    }
}
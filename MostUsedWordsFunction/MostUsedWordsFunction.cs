using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyMostUsedWords.Models;
using MyMostUsedWords.Services;
using System.Text;

namespace MostUsedWordsFunction
{
    public class MostUsedWordsFunction
    {
        readonly ILogger<MostUsedWordsFunction> _logger;
        MostUsedWordsService _mostUsedWordsService;

        public MostUsedWordsFunction(ILogger<MostUsedWordsFunction> logger, MostUsedWordsService mostUsedWordsService)
        {
            _logger = logger;
            _mostUsedWordsService = mostUsedWordsService;
        }

        [Function("MostUsedWordsFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            using var reader = new StreamReader(req.Body);
            req.Query.TryGetValue("source", out var source);
            req.Query.TryGetValue("target", out var target);
            
            return new OkObjectResult(GetResponse(_mostUsedWordsService.Get(reader, source, target)));
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

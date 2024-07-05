using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MyMostUsedWords.Models;
using MyMostUsedWords.Services;
using System.Text;

namespace MostUsedWordsFunction
{
    public class MostUsedWordsFunction(ILogger<MostUsedWordsFunction> logger, MostUsedWordsService mostUsedWordsService)
    {
        readonly ILogger<MostUsedWordsFunction> _logger = logger;
        readonly MostUsedWordsService _mostUsedWordsService = mostUsedWordsService;

        [Function("MostUsedWordsFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            using var reader = new StreamReader(req.Body);
            req.Query.TryGetValue("source", out var source);
            req.Query.TryGetValue("target", out var target);
            
            return new OkObjectResult(GetResponse(_mostUsedWordsService.Get(reader, source, target)));
        }

        [Function("MostUsedWordsFunctionWebSite")]
        public async Task<IActionResult> RunWebSite([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            try
            {
                using var reader = new StreamReader(req.Body);
                req.Query.TryGetValue("source", out var source);
                req.Query.TryGetValue("target", out var target);

                var website = await reader.ReadToEndAsync();
                _logger.LogInformation($"website: {website}");

                HttpClient client = new()
                {
                    BaseAddress = new Uri($"https://{website}"),
                };
                using var bodyReader = new StreamReader(await client.GetStreamAsync(string.Empty));

                return new OkObjectResult(GetResponse(_mostUsedWordsService.Get(bodyReader, source, target)));
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Something went wrong." + e.Message);
                return new BadRequestObjectResult(e.Message);
           }
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

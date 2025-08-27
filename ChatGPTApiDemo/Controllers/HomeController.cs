using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPrompt(List<IFormFile> files, string prompt)
        {
            StringBuilder fileContent = new StringBuilder();

            // Read uploaded files
            foreach (var file in files)
            {
                using var reader = new StreamReader(file.OpenReadStream());
                fileContent.AppendLine(await reader.ReadToEndAsync());
            }

            // Combine file content with prompt
            string fullPrompt = prompt;
            if (fileContent.Length > 0)
            {
                fullPrompt += "\n\nHere are the uploaded file contents:\n" + fileContent.ToString();
            }

            string responseText = await CallChatGPT(fullPrompt);
            ViewBag.ChatGPTResponse = responseText;

            return View("Index");
        }

        private async Task<string> CallChatGPT(string prompt)
        {

            var apiKey = _config["OpenAI:ApiKey"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestData = new
            {
                model = "gpt-4o-mini", // You can use gpt-4o, gpt-4o-mini, or gpt-3.5-turbo
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = prompt }
                }
            };


            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseBody);

            return result.choices[0].message.content.ToString();
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ChatGPTApiDemo.Models
{
    public class ChatGptRequest
    {
        public List<IFormFile> Files { get; set; }
        public IFormFile FileTemplate { get; set; }
        public string Prompt { get; set; }
        public string Response { get; set; }
    }
}

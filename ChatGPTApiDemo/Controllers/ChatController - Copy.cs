//using Azure.Core;
//using ChatGPTApiDemo.Models;
//using DocumentFormat.OpenXml.Packaging;
//using Microsoft.AspNetCore.Mvc;
//using OpenAI;
//using OpenAI.Chat;
//using OpenAI.Files;
//using System.Data;
//using System.Text;

//namespace ChatGPTApiDemo.Controllers
//{
//    public class ChatController : Controller
//    {
//        private readonly ChatClient _chatClient;
//        private readonly OpenAIFileClient _fileClient;
//        private readonly IWebHostEnvironment _env;
//        private readonly OpenAIClient _client;
//        public ChatController(ChatClient chatClient, OpenAIFileClient fileClient, IWebHostEnvironment env)
//        {
//            _chatClient = chatClient;
//            _fileClient = fileClient;
//            _env = env;
//        }





//        [HttpGet]
//        public IActionResult Index()
//        {
//            return View(new ChatGptRequest());
//        }

//        [HttpPost]
//        public async Task<IActionResult> Index(ChatGptRequest model)
//        {

//            // 1. Handle file uploads and read content
//            var allFileContents = new StringBuilder();

//            if (model.Files != null && model.Files.Count > 0)
//            {
//                foreach (var file in model.Files)
//                {
//                    var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
//                    Directory.CreateDirectory(uploadPath);

//                    var filePath = Path.Combine(uploadPath, file.FileName);

//                    using (var fs = new FileStream(filePath, FileMode.Create))
//                    {
//                        await file.CopyToAsync(fs);
//                    }

//                    // Read the content (text extraction for txt, docx, pdf etc.)
//                    string fileContent = "";
//                    if (Path.GetExtension(file.FileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
//                    {
//                        fileContent = await System.IO.File.ReadAllTextAsync(filePath);
//                    }
//                    //else if (Path.GetExtension(file.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase))
//                    //{
//                    //    using var doc = new DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(filePath, false);
//                    //    fileContent = doc.MainDocumentPart.Document.Body.InnerText;
//                    //}
//                    else if (Path.GetExtension(file.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase))
//                    {
//                        using (var doc = WordprocessingDocument.Open(filePath, false))
//                        {
//                            fileContent = doc.MainDocumentPart.Document.Body.InnerText;
//                        }
//                    }
//                    else if (Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
//                    {
//                        // Use iTextSharp or PdfPig or IronPDF to extract text
//                        // Example using PdfPig:
//                        using (var pdf = UglyToad.PdfPig.PdfDocument.Open(filePath))
//                        {
//                            foreach (var page in pdf.GetPages())
//                            {
//                                fileContent += page.Text;
//                            }
//                        }
//                    }

//                    allFileContents.AppendLine($"[File: {file.FileName}]");
//                    allFileContents.AppendLine(fileContent);
//                    allFileContents.AppendLine("--------------------------------------------------");
//                }
//            }

//            // 2. Combine file content with user prompt
//            string combinedPrompt = $"You are a helpful assistant. Analyze the following files and answer the user query.\n\n" +
//                                    $"Files Content:\n{allFileContents}\n\n" +
//                                    $"User Question:\n{model.Prompt}";

//            // 3. Call OpenAI Chat API
//            var response = await _chatClient.CompleteChatAsync(new[]
//            {
//                    ChatMessage.CreateUserMessage(combinedPrompt)
//                });

//            // 4. Return response
//            model.Response = response.Value.Content[0].Text;

//            return Ok(model.Response);


//            //// 1. Handle file uploads (save locally + upload to OpenAI if needed)
//            //if (model.Files != null && model.Files.Count > 0)
//            //{
//            //    foreach (var file in model.Files)
//            //    {
//            //        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
//            //        Directory.CreateDirectory(uploadPath);

//            //        var filePath = Path.Combine(uploadPath, file.FileName);

//            //        using (var fs = new FileStream(filePath, FileMode.Create))
//            //        {
//            //            await file.CopyToAsync(fs);
//            //        }

//            //        // Optional: upload to OpenAI (for future vector store / file search)
//            //        using var fsOpen = System.IO.File.OpenRead(filePath);
//            //        await _fileClient.UploadFileAsync(fsOpen, file.FileName, "assistants");
//            //    }
//            //}

//            //////// 2. Call OpenAI Chat API
//            //var response = await _chatClient.CompleteChatAsync(new[]
//            //{
//            //    //ChatMessage.cre("You are a helpful assistant."),
//            //    ChatMessage.CreateUserMessage(model.Prompt ?? string.Empty)                

//            //});



//            //////// 3. Set the response in the model
//            //model.Response = response.Value.Content[0].Text;

//            //return Ok(model);

//        }



//    }
//}

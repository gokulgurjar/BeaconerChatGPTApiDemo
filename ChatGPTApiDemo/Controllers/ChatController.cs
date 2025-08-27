using Azure.Core;
using ChatGPTApiDemo.Models;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Files;
using System.Data;
using System.Text;

namespace ChatGPTApiDemo.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatClient _chatClient;
        private readonly OpenAIFileClient _fileClient;
        private readonly IWebHostEnvironment _env;
        private readonly OpenAIClient _client;
        public ChatController(ChatClient chatClient, OpenAIFileClient fileClient, IWebHostEnvironment env)
        {
            _chatClient = chatClient;
            _fileClient = fileClient;
            _env = env;
        }





        [HttpGet]
        public IActionResult Index()
        {
            return View(new ChatGptRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ChatGptRequest model)
        {

            // 1. Handle file uploads and read content
            var allFileContents = new StringBuilder();

            if (model.Files != null && model.Files.Count > 0)
            {
                foreach (var file in model.Files)
                {
                    var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, file.FileName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }

                    // Read the content (text extraction for txt, docx, pdf etc.)
                    string fileContent = "";
                    if (Path.GetExtension(file.FileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        fileContent = await System.IO.File.ReadAllTextAsync(filePath);
                    }
                    if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var workbook = new ClosedXML.Excel.XLWorkbook(filePath))
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (var worksheet in workbook.Worksheets)
                            {
                                sb.AppendLine($"--- Sheet: {worksheet.Name} ---");

                                var range = worksheet.RangeUsed();
                                if (range != null)
                                {
                                    foreach (var row in range.Rows())
                                    {
                                        foreach (var cell in row.Cells())
                                        {
                                            sb.Append(cell.Value.ToString() + "\t"); // Tab-separated
                                        }
                                        sb.AppendLine();
                                    }
                                }

                                sb.AppendLine(); // Add space between sheets
                            }

                            fileContent = sb.ToString();
                        }
                    }                    
                    else if (Path.GetExtension(file.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var doc = WordprocessingDocument.Open(filePath, false))
                        {
                            fileContent = doc.MainDocumentPart.Document.Body.InnerText;
                        }
                    }
                    else if (Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        // Use iTextSharp or PdfPig or IronPDF to extract text
                        // Example using PdfPig:
                        using (var pdf = UglyToad.PdfPig.PdfDocument.Open(filePath))
                        {
                            foreach (var page in pdf.GetPages())
                            {
                                fileContent += page.Text;
                            }
                        }
                    }

                    allFileContents.AppendLine($"[File: {file.FileName}]");
                    allFileContents.AppendLine(fileContent);
                    allFileContents.AppendLine("--------------------------------------------------");
                }
            }

            // 2. Combine file content with user prompt
            string combinedPrompt = $"You are a helpful assistant. Analyze the following files and answer the user query.\n\n" +
                                    $"Files Content:\n{allFileContents}\n\n" +
                                    $"User Question:\n{model.Prompt}";

            // 3. Call OpenAI Chat API
            var response = await _chatClient.CompleteChatAsync(new[]
            {
                    ChatMessage.CreateUserMessage(combinedPrompt)
                });

            // 4. Return response
            model.Response = response.Value.Content[0].Text;

            //return Ok(model.Response);


            // 5. Save response in a formatted Word document
            string docsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "responses");
            Directory.CreateDirectory(docsPath);

            string docFileName = $"GPT_Response_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
            string docFilePath = Path.Combine(docsPath, docFileName);

            using (var wordDoc = WordprocessingDocument.Create(docFilePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                // Add main document part
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                var body = new DocumentFormat.OpenXml.Wordprocessing.Body();

                // Add Title (Heading 1)
                var titleParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                        new DocumentFormat.OpenXml.Wordprocessing.Text("ChatGPT Response")
                    )
                );
                titleParagraph.ParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties(
                    new DocumentFormat.OpenXml.Wordprocessing.ParagraphStyleId() { Val = "Heading1" }
                );
                body.Append(titleParagraph);

                // Split response into lines
                var lines = model.Response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("-") || line.Trim().StartsWith("*"))
                    {
                        // Add bullet point using "•"
                        var bulletText = "• " + line.TrimStart('-', '*', ' ');
                        var bulletPara = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text(bulletText)
                            )
                        );
                        body.Append(bulletPara);
                    }
                    else
                    {
                        // Normal paragraph
                        var para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text(line)
                            )
                        );
                        body.Append(para);
                    }
                }

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            // Return the file as a download
            var fileBytes = await System.IO.File.ReadAllBytesAsync(docFilePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", docFileName);

        }



    }
}

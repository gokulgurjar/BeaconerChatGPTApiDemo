//using Azure.Core;
//using ChatGPTApiDemo.Models;
//using DocumentFormat.OpenXml.InkML;
//using DocumentFormat.OpenXml.Packaging;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Routing.Template;
//using Newtonsoft.Json;
//using OpenAI;
//using OpenAI.Chat;
//using OpenAI.Files;
//using System.Data;
//using System.Text;
//using System.Text.RegularExpressions;

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
//        //public async Task<IActionResult> Index1(ChatGptRequest model)
//        //{

//        //    // 1. Handle file uploads and read content
//        //    var allFileContents = new StringBuilder();

//        //    if (model.Files != null && model.Files.Count > 0)
//        //    {
//        //        foreach (var file in model.Files)
//        //        {
//        //            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
//        //            Directory.CreateDirectory(uploadPath);

//        //            var filePath = Path.Combine(uploadPath, file.FileName);

//        //            using (var fs = new FileStream(filePath, FileMode.Create))
//        //            {
//        //                await file.CopyToAsync(fs);
//        //            }

//        //            // Read the content (text extraction for txt, docx, pdf etc.)
//        //            string fileContent = "";
//        //            if (Path.GetExtension(file.FileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
//        //            {
//        //                fileContent = await System.IO.File.ReadAllTextAsync(filePath);
//        //            }
//        //            if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
//        //            {
//        //                using (var workbook = new ClosedXML.Excel.XLWorkbook(filePath))
//        //                {
//        //                    StringBuilder sb = new StringBuilder();

//        //                    foreach (var worksheet in workbook.Worksheets)
//        //                    {
//        //                        sb.AppendLine($"--- Sheet: {worksheet.Name} ---");

//        //                        var range = worksheet.RangeUsed();
//        //                        if (range != null)
//        //                        {
//        //                            foreach (var row in range.Rows())
//        //                            {
//        //                                foreach (var cell in row.Cells())
//        //                                {
//        //                                    sb.Append(cell.Value.ToString() + "\t"); // Tab-separated
//        //                                }
//        //                                sb.AppendLine();
//        //                            }
//        //                        }

//        //                        sb.AppendLine(); // Add space between sheets
//        //                    }

//        //                    fileContent = sb.ToString();
//        //                }
//        //            }
//        //            else if (Path.GetExtension(file.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase))
//        //            {
//        //                using (var doc = WordprocessingDocument.Open(filePath, false))
//        //                {
//        //                    fileContent = doc.MainDocumentPart.Document.Body.InnerText;
//        //                }
//        //            }
//        //            else if (Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
//        //            {
//        //                // Use iTextSharp or PdfPig or IronPDF to extract text
//        //                // Example using PdfPig:
//        //                using (var pdf = UglyToad.PdfPig.PdfDocument.Open(filePath))
//        //                {
//        //                    foreach (var page in pdf.GetPages())
//        //                    {
//        //                        fileContent += page.Text;
//        //                    }
//        //                }
//        //            }

//        //            allFileContents.AppendLine($"[File: {file.FileName}]");
//        //            allFileContents.AppendLine(fileContent);
//        //            allFileContents.AppendLine("--------------------------------------------------");
//        //        }
//        //    }

//        //    // 2. Combine file content with user prompt
//        //    string combinedPrompt = $"You are a helpful assistant. Analyze the following files and answer the user query.\n\n" +
//        //                            $"Files Content:\n{allFileContents}\n\n" +
//        //                            $"User Question:\n{model.Prompt}";

//        //    // 3. Call OpenAI Chat API
//        //    var response = await _chatClient.CompleteChatAsync(new[]
//        //    {
//        //            ChatMessage.CreateUserMessage(combinedPrompt)
//        //        });

//        //    // 4. Return response
//        //    model.Response = response.Value.Content[0].Text;

//        //    //return Ok(model.Response);


//        //    // 5. Save response in a formatted Word document
//        //    string docsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "responses");
//        //    Directory.CreateDirectory(docsPath);

//        //    string docFileName = $"GPT_Response_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
//        //    string docFilePath = Path.Combine(docsPath, docFileName);

//        //    using (var wordDoc = WordprocessingDocument.Create(docFilePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
//        //    {
//        //        // Add main document part
//        //        var mainPart = wordDoc.AddMainDocumentPart();
//        //        mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
//        //        var body = new DocumentFormat.OpenXml.Wordprocessing.Body();

//        //        // Add Title (Heading 1)
//        //        var titleParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
//        //            new DocumentFormat.OpenXml.Wordprocessing.Run(
//        //                new DocumentFormat.OpenXml.Wordprocessing.Text("ChatGPT Response")
//        //            )
//        //        );
//        //        titleParagraph.ParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties(
//        //            new DocumentFormat.OpenXml.Wordprocessing.ParagraphStyleId() { Val = "Heading1" }
//        //        );
//        //        body.Append(titleParagraph);

//        //        // Split response into lines
//        //        var lines = model.Response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

//        //        foreach (var line in lines)
//        //        {
//        //            if (line.Trim().StartsWith("-") || line.Trim().StartsWith("*"))
//        //            {
//        //                // Add bullet point using "•"
//        //                var bulletText = "• " + line.TrimStart('-', '*', ' ');
//        //                var bulletPara = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
//        //                    new DocumentFormat.OpenXml.Wordprocessing.Run(
//        //                        new DocumentFormat.OpenXml.Wordprocessing.Text(bulletText)
//        //                    )
//        //                );
//        //                body.Append(bulletPara);
//        //            }
//        //            else
//        //            {
//        //                // Normal paragraph
//        //                var para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
//        //                    new DocumentFormat.OpenXml.Wordprocessing.Run(
//        //                        new DocumentFormat.OpenXml.Wordprocessing.Text(line)
//        //                    )
//        //                );
//        //                body.Append(para);
//        //            }
//        //        }

//        //        mainPart.Document.Append(body);
//        //        mainPart.Document.Save();
//        //    }



//        //    //Replace Template file
//        //    //======================================================
//        //    //======================================================


//        //    string filePathT = @"C:\Users\GURJAR\Downloads\Beaconer Report Template for Chat GPT.docx";

//        //    // ✅ Extract placeholders from the file
//        //    var placeholders = GetPlaceholders(filePathT);

//        //    // ✅ Your content to send to ChatGPT
//        //    string content1 = "Executive summary\r\n• Overall rating: Satisfactory to proceed. Inherent risk is High (confidential data, external access, fourth‑party use), but controls are Mostly Effective (92% overall, 19/231 in‑scope fails; 3/108 critical). Residual risk: Medium.\r\n• Key assurances:\r\n• Independent attestations: SOC 2 Type 2 for Microsoft Azure (Security, Availability, Confidentiality) and Logicworks Managed Cloud Operations (Security, Availability, Confidentiality) current through 2023.\r\n• Documented IS program aligned to NIST SP 800‑30/39; annual risk assessments; encryption TLS 1.2+ in transit and AES‑256 at rest; MFA and SSO; logging/SIEM; quarterly vuln scans and annual pen tests; BCP/DR with annual testing; background checks.\r\n• Password standard uses strong passphrases, 14+ characters, 6‑month user rotations (Password Guideline).\r\nWhat drives the High inherent risk\r\n• Data is Confidential; external and fourth‑party access are in scope; SaaS delivery; customer data processed.\r\nStrengths observed (sample)\r\n• Access control: MFA, least privilege, periodic access reviews, unique IDs, SSO supported (93%).\r\n• Application security: formal program, WAF, SAST/DAST, pre‑prod security testing, API security with logging and standard gateway (95%).\r\n• Data security: classification, DLP, encryption in transit and at rest with centralized key mgmt, phishing controls, removable media controls, secondary use prohibited (95%).\r\n• BCP/DR: documented and tested annually; offsite backups; retention and deletion defined (95%).\r\n• Network security: deny‑by‑default, VPN/MFA for remote, segmentation, hardening, anti‑malware, logging (95%).\r\n• Risk, TPRM, VM, Incident Mgmt domains all ≥95% by the model.\r\nNotable control gaps and how to treat them\r\nTreat within 90 days for critical and 180 days for medium unless otherwise noted.\r\n• Password rotation/user authentication (Medium)\r\n• Current: 6‑month user rotation. Risk: brute‑force/credential stuffing if MFA is not universal.\r\n• Action: Enforce universal MFA for all interactive and admin access (preferable to shortening rotation per NIST 800‑63‑3). If any scope cannot use MFA, reduce rotation to 90 days for those accounts and enable lockout, risk‑based auth, and anomaly detection.\r\n• Risk treatment program (Critical if still applicable)\r\n• Security questionnaire shows “No” for “program to manage the treatment of risks identified.” Action: Stand up a documented risk treatment workflow (owners, timelines, acceptance criteria, exception tracking) and link it to the risk register and management reporting.\r\n• Session/logoff control (Medium)\r\n• “System policy requires logoff when session is finished” = No. Action: Implement enforced inactivity timeouts/auto‑lock and re‑auth for endpoints and critical apps; document the control.\r\n• Password handling (Medium)\r\n• “Prohibit keeping unencrypted record of passwords” = No in questionnaire (contradicts your guideline). Action: Update policy to explicitly prohibit any plaintext password storage; require company‑approved password managers; verify via audit.\r\n• Asset lifecycle (Medium)\r\n• “Process to verify return of assets on termination” = No. Action: Implement a formal leaver asset return/reconciliation workflow (HR–IT–manager sign‑off) and an asset disposal record with certificates of destruction.\r\n• Wireless/endpoint & BYOD (Medium)\r\n• Wireless controls = No; BYOD MDM controls = No. Action: Enforce WPA2‑Enterprise/802.1X, disable WPS, segment guest SSIDs. For mobile/BYOD, require MDM with device encryption, PIN/biometric, remote wipe, and app/container controls or block corporate access on unmanaged devices.\r\n• Network device hardening standards (Medium)\r\n• “Security and hardening standards for network devices” = No. Action: Adopt CIS benchmarks, implement configuration baselines, change control, and periodic config compliance scans.\r\n• Third‑party governance (Medium)\r\n• “Third‑party inventory” = No; “Remediation reporting for subcontractor issues” = No. Action: Maintain a complete vendor inventory with data flows; formalize corrective‑action tracking for supplier findings; keep current SOC/ISO attestations and SLAs (breach notice, RTA, audit rights).\r\n• Software composition analysis (Medium)\r\n• SCA = No. Action: Add SCA/SBOM generation to the CI/CD pipeline; set policies for vulnerable/unknown components.\r\n• Data retention/destruction across third parties (Medium)\r\n• Ensure retention/destruction requirements explicitly cover live media, backups, and subcontractors (questionnaire showed a gap).\r\nAzure platform observations to monitor\r\n• Azure SOC 2 noted exceptions historically for network device password rotation, secret rotation, and quota configuration; Microsoft remediated. Require your Azure teams/partners to:\r\n• Enforce privileged password rotation cadence for network devices.\r\n• Track and evidence key/secret rotations against policy.\r\n• Ensure break‑glass account alerting is active and monitored.\r\nPrivacy assessment\r\n• Privacy score 92%; residual risk Low; 1 failed control, 0 critical. Controls include DPIA, DSAR, consent, encryption, data residency, masking, PI destruction, fourth‑party assessments, and logging. Maintain annual re‑testing and ensure any AI use excludes PI/SPI for automated decisioning without human review (already in place).\r\nConditions to proceed and ongoing monitoring\r\n• Proceed with onboarding subject to a corrective action plan addressing the items above, with due dates and control owners.\r\n• Contractual: include breach notification ≤72 hours, right‑to‑audit, RTA/RPO/RTO clarity, data deletion SLAs (including backups), encryption/MFA requirements, and supplier oversight.\r\n• Evidence cadence: annual SOC 2 Type 2 (Azure + hosting partner), updated SIG/questionnaire, penetration test summary, BCP/DR test report, and key risk register with treatment status.\r\nBottom line\r\n• With strong attestations, mature policies, and high control coverage, the vendor is satisfactory. Address the noted medium/critical gaps (especially universal MFA, risk treatment workflow, and endpoint/wireless/MDM controls) to reduce residual risk from Medium toward Low.\r\n";
//        //    string prompt = $"Content:\n{content1}\nPlaceholders:\n{placeholders}\nReturn all the placeholders key-value JSON format from given content.";

//        //    // ✅ Call OpenAI API
//        //    var responseT = await _chatClient.CompleteChatAsync(new[]
//        //    {
//        //        ChatMessage.CreateUserMessage(prompt)
//        //    });

//        //    string jsonData = responseT.Value.Content[0].Text.Trim();

//        //    // ✅ Validate
//        //    if (string.IsNullOrWhiteSpace(jsonData))
//        //        return BadRequest("JSON data is empty.");

//        //    // ✅ Use original docx file (not text-based fake)
//        //    using var ms = new MemoryStream(System.IO.File.ReadAllBytes(filePathT));

//        //    // ✅ Replace placeholders
//        //    var replacements = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

//        //    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, true))
//        //    {
//        //        var body = wordDoc.MainDocumentPart.Document.Body;

//        //        foreach (var kvp in replacements)
//        //        {
//        //            string placeholder = $"{{{{{kvp.Key}}}}}";
//        //            foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
//        //            {
//        //                if (text.Text.Contains(placeholder))
//        //                {
//        //                    text.Text = text.Text.Replace(placeholder, kvp.Value ?? string.Empty);
//        //                }
//        //            }
//        //        }
//        //        wordDoc.MainDocumentPart.Document.Save();
//        //    }
//        //    // Return the file as a download
//        //    var fileBytes = await System.IO.File.ReadAllBytesAsync(docFilePath);

//        //    //return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", docFileName);

//        //    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", docFileName);
//        //}


//        public string GetPlaceholders(string filePath)
//        {
//            string documentText;
//            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
//            {
//                documentText = wordDoc.MainDocumentPart.Document.Body.InnerText;
//            }

//            Regex regex = new Regex(@"\{\{(.*?)\}\}");
//            var matches = regex.Matches(documentText);

//            StringBuilder sb = new StringBuilder();
//            foreach (Match match in matches)
//            {
//                sb.AppendLine("{" + match.Groups[1].Value.Trim() + "}");
//            }

//            return sb.ToString();
//        }

//        //public async Task<IActionResult> Index(ChatGptRequest model)
     
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
//                    if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
//                    {
//                        using (var workbook = new ClosedXML.Excel.XLWorkbook(filePath))
//                        {
//                            StringBuilder sb = new StringBuilder();

//                            foreach (var worksheet in workbook.Worksheets)
//                            {
//                                sb.AppendLine($"--- Sheet: {worksheet.Name} ---");

//                                var range = worksheet.RangeUsed();
//                                if (range != null)
//                                {
//                                    foreach (var row in range.Rows())
//                                    {
//                                        foreach (var cell in row.Cells())
//                                        {
//                                            sb.Append(cell.Value.ToString() + "\t"); // Tab-separated
//                                        }
//                                        sb.AppendLine();
//                                    }
//                                }

//                                sb.AppendLine(); // Add space between sheets
//                            }

//                            fileContent = sb.ToString();
//                        }
//                    }
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
//            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(15));
//            var response = await _chatClient.CompleteChatAsync(new[]
//            {
//               ChatMessage.CreateUserMessage(combinedPrompt)
//            }, cancellationToken: cts.Token);

//            // 4. Return response
//            model.Response = response.Value.Content[0].Text;

//            //return Ok(model.Response);


//            string filePathT = string.Empty;

//            if (model.FileTemplate != null && model.FileTemplate.Length > 0)
//            {
//                var templateName = Path.GetFileName(model.FileTemplate.FileName);
//                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", templateName);
//                Directory.CreateDirectory(Path.GetDirectoryName(templatePath));
//                using (var stream = new FileStream(templatePath, FileMode.Create))
//                {
//                    await model.FileTemplate.CopyToAsync(stream);
//                }
//                filePathT = templatePath;
//            }
//            if (string.IsNullOrWhiteSpace(filePathT))
//            {
//                return BadRequest("Template file is required.");
//            }

//            // ✅ Extract placeholders from the file
//            var placeholders = GetPlaceholders(filePathT);

//            string content1 = model.Response; // "Executive summary\r\n• Overall rating: Satisfactory to proceed. Inherent risk is High (confidential data, external access, fourth‑party use), but controls are Mostly Effective (92% overall, 19/231 in‑scope fails; 3/108 critical). Residual risk: Medium.\r\n• Key assurances:\r\n• Independent attestations: SOC 2 Type 2 for Microsoft Azure (Security, Availability, Confidentiality) and Logicworks Managed Cloud Operations (Security, Availability, Confidentiality) current through 2023.\r\n• Documented IS program aligned to NIST SP 800‑30/39; annual risk assessments; encryption TLS 1.2+ in transit and AES‑256 at rest; MFA and SSO; logging/SIEM; quarterly vuln scans and annual pen tests; BCP/DR with annual testing; background checks.\r\n• Password standard uses strong passphrases, 14+ characters, 6‑month user rotations (Password Guideline).\r\nWhat drives the High inherent risk\r\n• Data is Confidential; external and fourth‑party access are in scope; SaaS delivery; customer data processed.\r\nStrengths observed (sample)\r\n• Access control: MFA, least privilege, periodic access reviews, unique IDs, SSO supported (93%).\r\n• Application security: formal program, WAF, SAST/DAST, pre‑prod security testing, API security with logging and standard gateway (95%).\r\n• Data security: classification, DLP, encryption in transit and at rest with centralized key mgmt, phishing controls, removable media controls, secondary use prohibited (95%).\r\n• BCP/DR: documented and tested annually; offsite backups; retention and deletion defined (95%).\r\n• Network security: deny‑by‑default, VPN/MFA for remote, segmentation, hardening, anti‑malware, logging (95%).\r\n• Risk, TPRM, VM, Incident Mgmt domains all ≥95% by the model.\r\nNotable control gaps and how to treat them\r\nTreat within 90 days for critical and 180 days for medium unless otherwise noted.\r\n• Password rotation/user authentication (Medium)\r\n• Current: 6‑month user rotation. Risk: brute‑force/credential stuffing if MFA is not universal.\r\n• Action: Enforce universal MFA for all interactive and admin access (preferable to shortening rotation per NIST 800‑63‑3). If any scope cannot use MFA, reduce rotation to 90 days for those accounts and enable lockout, risk‑based auth, and anomaly detection.\r\n• Risk treatment program (Critical if still applicable)\r\n• Security questionnaire shows “No” for “program to manage the treatment of risks identified.” Action: Stand up a documented risk treatment workflow (owners, timelines, acceptance criteria, exception tracking) and link it to the risk register and management reporting.\r\n• Session/logoff control (Medium)\r\n• “System policy requires logoff when session is finished” = No. Action: Implement enforced inactivity timeouts/auto‑lock and re‑auth for endpoints and critical apps; document the control.\r\n• Password handling (Medium)\r\n• “Prohibit keeping unencrypted record of passwords” = No in questionnaire (contradicts your guideline). Action: Update policy to explicitly prohibit any plaintext password storage; require company‑approved password managers; verify via audit.\r\n• Asset lifecycle (Medium)\r\n• “Process to verify return of assets on termination” = No. Action: Implement a formal leaver asset return/reconciliation workflow (HR–IT–manager sign‑off) and an asset disposal record with certificates of destruction.\r\n• Wireless/endpoint & BYOD (Medium)\r\n• Wireless controls = No; BYOD MDM controls = No. Action: Enforce WPA2‑Enterprise/802.1X, disable WPS, segment guest SSIDs. For mobile/BYOD, require MDM with device encryption, PIN/biometric, remote wipe, and app/container controls or block corporate access on unmanaged devices.\r\n• Network device hardening standards (Medium)\r\n• “Security and hardening standards for network devices” = No. Action: Adopt CIS benchmarks, implement configuration baselines, change control, and periodic config compliance scans.\r\n• Third‑party governance (Medium)\r\n• “Third‑party inventory” = No; “Remediation reporting for subcontractor issues” = No. Action: Maintain a complete vendor inventory with data flows; formalize corrective‑action tracking for supplier findings; keep current SOC/ISO attestations and SLAs (breach notice, RTA, audit rights).\r\n• Software composition analysis (Medium)\r\n• SCA = No. Action: Add SCA/SBOM generation to the CI/CD pipeline; set policies for vulnerable/unknown components.\r\n• Data retention/destruction across third parties (Medium)\r\n• Ensure retention/destruction requirements explicitly cover live media, backups, and subcontractors (questionnaire showed a gap).\r\nAzure platform observations to monitor\r\n• Azure SOC 2 noted exceptions historically for network device password rotation, secret rotation, and quota configuration; Microsoft remediated. Require your Azure teams/partners to:\r\n• Enforce privileged password rotation cadence for network devices.\r\n• Track and evidence key/secret rotations against policy.\r\n• Ensure break‑glass account alerting is active and monitored.\r\nPrivacy assessment\r\n• Privacy score 92%; residual risk Low; 1 failed control, 0 critical. Controls include DPIA, DSAR, consent, encryption, data residency, masking, PI destruction, fourth‑party assessments, and logging. Maintain annual re‑testing and ensure any AI use excludes PI/SPI for automated decisioning without human review (already in place).\r\nConditions to proceed and ongoing monitoring\r\n• Proceed with onboarding subject to a corrective action plan addressing the items above, with due dates and control owners.\r\n• Contractual: include breach notification ≤72 hours, right‑to‑audit, RTA/RPO/RTO clarity, data deletion SLAs (including backups), encryption/MFA requirements, and supplier oversight.\r\n• Evidence cadence: annual SOC 2 Type 2 (Azure + hosting partner), updated SIG/questionnaire, penetration test summary, BCP/DR test report, and key risk register with treatment status.\r\nBottom line\r\n• With strong attestations, mature policies, and high control coverage, the vendor is satisfactory. Address the noted medium/critical gaps (especially universal MFA, risk treatment workflow, and endpoint/wireless/MDM controls) to reduce residual risk from Medium toward Low.\r\n";
//            // ✅ Prepare the prompt for ChatGPT
//            string prompt = $@"
//                        Content:
//                        {content1}

//                        Placeholders:
//                        {string.Join(", ", placeholders)}

//                        Task:
//                        Return all placeholders as key-value pairs in valid JSON format based on the content.
//                        If value is not found, keep it as an empty string.
//                        If Yes or No placeholder value blank consider it No
//                        ";


//            var responseT = await _chatClient.CompleteChatAsync(new[]
//            {
//                ChatMessage.CreateUserMessage(prompt)
//            }, cancellationToken: cts.Token);

//            //var responseT = await _chatClient.CompleteChatAsync(new[]
//            //{
//            //ChatMessage.CreateUserMessage(prompt)
//            //});
//            //
//            string jsonData = responseT.Value.Content[0].Text.Trim();

//            if (string.IsNullOrWhiteSpace(jsonData))
//                return BadRequest("JSON data is empty.");

//            // ✅ Convert JSON into Dictionary
//            var replacements = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

//            // ✅ Open the template file in a MemoryStream for modification
//            byte[] fileBytes = System.IO.File.ReadAllBytes(filePathT);

//            // ✅ Use original docx file (not text-based fake)
//            using var ms = new MemoryStream(System.IO.File.ReadAllBytes(filePathT));

//            // ✅ Apply replacements
//            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, true))
//            {
//                var body = wordDoc.MainDocumentPart.Document.Body;

//                foreach (var kvp in replacements)
//                {

//                    string placeholder = "{{" + kvp.Key.Replace("{", "").Replace("}", "") + "}}";  //$"{{{{kvp.Key}}}}"; // Example: {{Name}}
//                    foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
//                    {
//                        if (text.Text.Contains(placeholder))
//                        {
//                            text.Text = text.Text.Replace(placeholder, kvp.Value ?? string.Empty);
//                        }
//                    }
//                }

//                // ✅ Save updated document
//                wordDoc.MainDocumentPart.Document.Save();
//            }
//            ms.Position = 0;
//            // ✅ Return updated file as download
//            return File(ms.ToArray(),
//                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//                "Beaconer_Report_Updated.docx");
//        }




//    }
//}

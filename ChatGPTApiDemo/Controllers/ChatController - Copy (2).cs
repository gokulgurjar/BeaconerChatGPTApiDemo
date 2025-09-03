//using ChatGPTApiDemo.Models;
//using DocumentFormat.OpenXml.Packaging;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using OpenAI;
//using OpenAI.Chat;
//using OpenAI.Files;
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

//        // ✅ Extract placeholders from a docx file
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

//        [HttpPost]
//        public async Task<IActionResult> Index(ChatGptRequest model)
//        {








//            //================================
//            //================================
//            string filePath = @"C:\Users\GURJAR\Downloads\Beaconer Report Template for Chat GPT.docx";

//            // ✅ Extract placeholders from the file
//            var placeholders = GetPlaceholders(filePath);

//            // ✅ Your content to send to ChatGPT
//            string content1 = "Executive summary\r\n• Overall rating: Satisfactory to proceed. Inherent risk is High (confidential data, external access, fourth‑party use), but controls are Mostly Effective (92% overall, 19/231 in‑scope fails; 3/108 critical). Residual risk: Medium.\r\n• Key assurances:\r\n• Independent attestations: SOC 2 Type 2 for Microsoft Azure (Security, Availability, Confidentiality) and Logicworks Managed Cloud Operations (Security, Availability, Confidentiality) current through 2023.\r\n• Documented IS program aligned to NIST SP 800‑30/39; annual risk assessments; encryption TLS 1.2+ in transit and AES‑256 at rest; MFA and SSO; logging/SIEM; quarterly vuln scans and annual pen tests; BCP/DR with annual testing; background checks.\r\n• Password standard uses strong passphrases, 14+ characters, 6‑month user rotations (Password Guideline).\r\nWhat drives the High inherent risk\r\n• Data is Confidential; external and fourth‑party access are in scope; SaaS delivery; customer data processed.\r\nStrengths observed (sample)\r\n• Access control: MFA, least privilege, periodic access reviews, unique IDs, SSO supported (93%).\r\n• Application security: formal program, WAF, SAST/DAST, pre‑prod security testing, API security with logging and standard gateway (95%).\r\n• Data security: classification, DLP, encryption in transit and at rest with centralized key mgmt, phishing controls, removable media controls, secondary use prohibited (95%).\r\n• BCP/DR: documented and tested annually; offsite backups; retention and deletion defined (95%).\r\n• Network security: deny‑by‑default, VPN/MFA for remote, segmentation, hardening, anti‑malware, logging (95%).\r\n• Risk, TPRM, VM, Incident Mgmt domains all ≥95% by the model.\r\nNotable control gaps and how to treat them\r\nTreat within 90 days for critical and 180 days for medium unless otherwise noted.\r\n• Password rotation/user authentication (Medium)\r\n• Current: 6‑month user rotation. Risk: brute‑force/credential stuffing if MFA is not universal.\r\n• Action: Enforce universal MFA for all interactive and admin access (preferable to shortening rotation per NIST 800‑63‑3). If any scope cannot use MFA, reduce rotation to 90 days for those accounts and enable lockout, risk‑based auth, and anomaly detection.\r\n• Risk treatment program (Critical if still applicable)\r\n• Security questionnaire shows “No” for “program to manage the treatment of risks identified.” Action: Stand up a documented risk treatment workflow (owners, timelines, acceptance criteria, exception tracking) and link it to the risk register and management reporting.\r\n• Session/logoff control (Medium)\r\n• “System policy requires logoff when session is finished” = No. Action: Implement enforced inactivity timeouts/auto‑lock and re‑auth for endpoints and critical apps; document the control.\r\n• Password handling (Medium)\r\n• “Prohibit keeping unencrypted record of passwords” = No in questionnaire (contradicts your guideline). Action: Update policy to explicitly prohibit any plaintext password storage; require company‑approved password managers; verify via audit.\r\n• Asset lifecycle (Medium)\r\n• “Process to verify return of assets on termination” = No. Action: Implement a formal leaver asset return/reconciliation workflow (HR–IT–manager sign‑off) and an asset disposal record with certificates of destruction.\r\n• Wireless/endpoint & BYOD (Medium)\r\n• Wireless controls = No; BYOD MDM controls = No. Action: Enforce WPA2‑Enterprise/802.1X, disable WPS, segment guest SSIDs. For mobile/BYOD, require MDM with device encryption, PIN/biometric, remote wipe, and app/container controls or block corporate access on unmanaged devices.\r\n• Network device hardening standards (Medium)\r\n• “Security and hardening standards for network devices” = No. Action: Adopt CIS benchmarks, implement configuration baselines, change control, and periodic config compliance scans.\r\n• Third‑party governance (Medium)\r\n• “Third‑party inventory” = No; “Remediation reporting for subcontractor issues” = No. Action: Maintain a complete vendor inventory with data flows; formalize corrective‑action tracking for supplier findings; keep current SOC/ISO attestations and SLAs (breach notice, RTA, audit rights).\r\n• Software composition analysis (Medium)\r\n• SCA = No. Action: Add SCA/SBOM generation to the CI/CD pipeline; set policies for vulnerable/unknown components.\r\n• Data retention/destruction across third parties (Medium)\r\n• Ensure retention/destruction requirements explicitly cover live media, backups, and subcontractors (questionnaire showed a gap).\r\nAzure platform observations to monitor\r\n• Azure SOC 2 noted exceptions historically for network device password rotation, secret rotation, and quota configuration; Microsoft remediated. Require your Azure teams/partners to:\r\n• Enforce privileged password rotation cadence for network devices.\r\n• Track and evidence key/secret rotations against policy.\r\n• Ensure break‑glass account alerting is active and monitored.\r\nPrivacy assessment\r\n• Privacy score 92%; residual risk Low; 1 failed control, 0 critical. Controls include DPIA, DSAR, consent, encryption, data residency, masking, PI destruction, fourth‑party assessments, and logging. Maintain annual re‑testing and ensure any AI use excludes PI/SPI for automated decisioning without human review (already in place).\r\nConditions to proceed and ongoing monitoring\r\n• Proceed with onboarding subject to a corrective action plan addressing the items above, with due dates and control owners.\r\n• Contractual: include breach notification ≤72 hours, right‑to‑audit, RTA/RPO/RTO clarity, data deletion SLAs (including backups), encryption/MFA requirements, and supplier oversight.\r\n• Evidence cadence: annual SOC 2 Type 2 (Azure + hosting partner), updated SIG/questionnaire, penetration test summary, BCP/DR test report, and key risk register with treatment status.\r\nBottom line\r\n• With strong attestations, mature policies, and high control coverage, the vendor is satisfactory. Address the noted medium/critical gaps (especially universal MFA, risk treatment workflow, and endpoint/wireless/MDM controls) to reduce residual risk from Medium toward Low.\r\n";
//            string prompt = $"Content:\n{content1}\nPlaceholders:\n{placeholders}\nReturn all the placeholders key-value JSON format from given content.";

//            // ✅ Call OpenAI API
//            var response = await _chatClient.CompleteChatAsync(new[]
//            {
//                ChatMessage.CreateUserMessage(prompt)
//            });

//            string jsonData = response.Value.Content[0].Text.Trim();

//            // ✅ Validate
//            if (string.IsNullOrWhiteSpace(jsonData))
//                return BadRequest("JSON data is empty.");

//            // ✅ Use original docx file (not text-based fake)
//            using var ms = new MemoryStream(System.IO.File.ReadAllBytes(filePath));

//            // ✅ Replace placeholders
//            var replacements = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

//            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, true))
//            {
//                var body = wordDoc.MainDocumentPart.Document.Body;

//                foreach (var kvp in replacements)
//                {
//                    string placeholder = $"{{{{{kvp.Key}}}}}";
//                    foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
//                    {
//                        if (text.Text.Contains(placeholder))
//                        {
//                            text.Text = text.Text.Replace(placeholder, kvp.Value ?? string.Empty);
//                        }
//                    }
//                }
//                wordDoc.MainDocumentPart.Document.Save();
//            }

//            return File(ms.ToArray(),
//                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//                        "UpdatedDocument.docx");
//        }

//        // ✅ Independent API endpoint for manual upload
//        [HttpPost("replace-placeholders")]
//        public async Task<IActionResult> ReplacePlaceholdersAsync([FromForm] IFormFile file, [FromForm] string jsonData)
//        {
//            if (file == null || file.Length == 0)
//                return BadRequest("File is required.");
//            if (string.IsNullOrWhiteSpace(jsonData))
//                return BadRequest("JSON data is required.");

//            var placeholderValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

//            using var ms = new MemoryStream();
//            await file.CopyToAsync(ms);
//            ms.Seek(0, SeekOrigin.Begin);

//            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, true))
//            {
//                var body = wordDoc.MainDocumentPart.Document.Body;

//                foreach (var kvp in placeholderValues)
//                {
//                    string placeholder = $"{{{{{kvp.Key}}}}}";
//                    foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
//                    {
//                        if (text.Text.Contains(placeholder))
//                        {
//                            text.Text = text.Text.Replace(placeholder, kvp.Value ?? string.Empty);
//                        }
//                    }
//                }
//                wordDoc.MainDocumentPart.Document.Save();
//            }

//            return File(ms.ToArray(),
//                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//                        "UpdatedDocument.docx");
//        }
//    }
//}

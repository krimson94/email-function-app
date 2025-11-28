using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using EmailFunctionApp.Models; 

namespace EmailFunctionApp.Functions
{
    public class EmailSetup
    {
        private readonly ILogger<EmailSetup> _logger;

        
        public EmailSetup(ILogger<EmailSetup> logger)
        {
            _logger = logger;
        }

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [Function("EmailSetup")]
        [OpenApiOperation(operationId: "EmailSetup", tags: ["Email"])]
        [OpenApiRequestBody("application/json", typeof(EmailRequest), Description = "Templates and Tokens")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(EmailResponse), Description = "The processed email")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid request")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processing Email Template.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            EmailRequest? request;
            try
            {
                request = JsonSerializer.Deserialize<EmailRequest>(requestBody, _jsonSerializerOptions);
            }
            catch
            {
                return new BadRequestObjectResult("Invalid JSON format.");
            }

            if (request == null || request.Templates == null)
            {
                return new BadRequestObjectResult("Payload must contain 'templates'.");
            }

            // Process Recipients
            _logger.LogInformation("Setting recipient tokens.");
            var emailTo = SetRecipientTokens(request.Templates.To);
            var emailCc = SetRecipientTokens(request.Templates.Cc);

            // Perform replacements
            _logger.LogInformation("Replacing tokens in email template.");
            var response = new EmailResponse
            {
                To = ReplaceTokens(emailTo, request.RecipientTokens),
                Cc = ReplaceTokens(emailCc, request.RecipientTokens),
                Subject = ReplaceTokens(request.Templates.Subject, request.Tokens),
                Body = ReplaceTokens(request.Templates.Body, request.Tokens)
            };

            return new OkObjectResult(response);
        }

        private static string ReplaceTokens(string content, Dictionary<string, string>? tokens)
        {
            if (string.IsNullOrEmpty(content) || tokens == null) return content ?? "";

            foreach (var token in tokens)
            {
                string pattern = $@"\[{Regex.Escape(token.Key)}\]";
                content = Regex.Replace(content, pattern, token.Value, RegexOptions.IgnoreCase);
            }

            return content;

        }
        private static string SetRecipientTokens(List<string>? recipients)
        {
            if (recipients?.Count == 0 || recipients == null) return "";
            var updatedRecipientList = new List<string>();
            
            foreach (var recipient in recipients)
            {
                var updatedRecipient = $"[{recipient}]";
                updatedRecipientList.Add(updatedRecipient);
            }

            return string.Join(";", updatedRecipientList);
        }
    }
}
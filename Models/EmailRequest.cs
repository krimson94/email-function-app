using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace EmailFunctionApp.Models
{
    public class EmailRequest
    {
        [OpenApiProperty(Description = "The email templates containing [Tags] (in []).")]
        public required EmailFields Templates { get; set; }

        [OpenApiProperty(Description = "Key-value pairs for the email body where Key = Tag Name and Value = Replacement Text.")]
        public required Dictionary<string, string> Tokens { get; set; }
        
        [OpenApiProperty(Description = "Key-value for the email recipients pairs where Key = Tag Name and Value = Replacement Text.")]
        public required Dictionary<string, string> RecipientTokens { get; set; }
    }

    public class EmailFields
    {
        [OpenApiProperty(Description = "Email Recipients To field.")]
        public required List<string>  To { get; set; }
        [OpenApiProperty(Description = "Email Recipients CC field.")]
        public List<string>? Cc { get; set; }
        [OpenApiProperty(Description = "Email Subject field.")]
        public required string Subject { get; set; }
        [OpenApiProperty(Description = "Email Body field.")]
        public required string Body { get; set; }
    }

    public class EmailResponse
    {
        public required string  To { get; set; }
        public string? Cc { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}
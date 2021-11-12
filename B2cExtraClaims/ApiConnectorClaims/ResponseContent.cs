namespace ApiConnectorClaims
{
    public class ResponseContent
    {
        public const string ApiVersion = "1.0.0";

        public ResponseContent()
        {
            Version = ApiVersion;
            Action = "Continue";
        }

        public ResponseContent(string action, string userMessage)
        {
            Version = ApiVersion;
            Action = action;
            UserMessage = userMessage;
            if (action == "ValidationError")
            {
                Status = "400";
            }
        }

        public string Version { get; }
        public string Action { get; set; }

        public string? UserMessage { get; set; }

        public string? Status { get; set; }

        public string Extension_CustomClaim { get; set; }
    }
}

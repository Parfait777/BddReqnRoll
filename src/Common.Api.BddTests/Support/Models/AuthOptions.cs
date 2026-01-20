namespace Common.Api.BddTests.Support.Models
{
    public sealed record AuthOptions
    {
        // "Bearer" or "ApiKey"
        public string Mode { get; set; } = "Bearer";

        // Bearer token flow (client_credentials)
        public string TokenUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}

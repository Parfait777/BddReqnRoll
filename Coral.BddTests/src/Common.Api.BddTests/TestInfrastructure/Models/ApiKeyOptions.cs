namespace Common.Api.BddTests.TestInfrastructure.Models
{
    public sealed record ApiKeyOptions
    {
        public string HeaderName { get; set; } = "X-API-Key";
        public string Value { get; set; } = string.Empty;
    }
}

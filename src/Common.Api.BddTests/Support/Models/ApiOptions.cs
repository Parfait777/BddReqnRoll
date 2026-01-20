namespace Common.Api.BddTests.Support.Models
{
    public sealed record ApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public int RetryCount { get; set; } = 5;
        public TimeSpan? BaseDelay { get; set; } = TimeSpan.FromMilliseconds(200);
        public int MaxJitter { get; set; } = 150;
    }
}

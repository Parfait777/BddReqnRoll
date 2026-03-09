namespace Coral.Clean.API.Models
{
    public sealed record SubmissionRequestContract
    {
        public int EntityId { get; set; }

        public ReportingPeriodContract ReportingPeriod { get; set; } = null!;
    }
}

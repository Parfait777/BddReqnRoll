namespace Coral.Clean.API.Models
{
    public sealed record ReportingPeriodContract
    {
        public DateOnly ReportingPeriodFrom { get; set; }

        public DateOnly ReportingPeriodTo { get; set; }
    }
}

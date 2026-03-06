using Common.TestHelpers.Interfaces;

namespace Common.TestHelpers.Impl
{
    public sealed class DefaultDateTimeWrapper : IDateTimeWrapper
    {
        public DateTime GetNow() => DateTime.Now;
        public DateTime GetUtcNow() => DateTime.UtcNow;
    }
}

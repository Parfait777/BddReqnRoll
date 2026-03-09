namespace Coral.Clean.API.Models.Responses
{
    public sealed record ApiResponseContract<TResponse>
    {
        public TResponse Data { get; init; }
        public string Message { get; init; }
        public bool Success { get; init; }
    }
}

namespace Common.Api.BddTests.Support.Services.Interfaces
{
    public interface IApiSender
    {
        Task SendAsync(HttpMethod method, string path, string? jsonBody);
    }
}

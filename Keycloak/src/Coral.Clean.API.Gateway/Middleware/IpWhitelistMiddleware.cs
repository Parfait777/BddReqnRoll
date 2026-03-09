using Coral.Clean.API.Gateway.Configurations;
using Coral.Clean.API.Gateway.Helpers;
using Microsoft.Extensions.Options;
using System.Net;

namespace Coral.Clean.API.Gateway.Middleware
{
    public sealed class IpWhitelistMiddleware : IMiddleware
    {
        private readonly GatewayOptions options;

        public IpWhitelistMiddleware(IOptions<GatewayOptions> options) =>
            this.options = options.Value;

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.StartsWithSegments("/health") || context.Request.Path.StartsWithSegments("/swagger"))
            {
                return next(context);
            }


            IPAddress? remoteIp = GetClientIp(context);

            if (remoteIp is null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return context.Response.WriteAsync("Forbidden: client IP not available.");
            }

            if (!IsAllowed(remoteIp, this.options.IpWhitelist))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return context.Response.WriteAsync("Forbidden: IP not allowed.");
            }

            return next(context);
        }

        private static IPAddress? GetClientIp(HttpContext ctx)
        {
            IPAddress? ip = ctx.Connection.RemoteIpAddress;
            if (ip is null)
            {
                return null;
            }

            // Allow localhost in dev/test scenarios
            if (IPAddress.IsLoopback(ip))
            {
                return ip;
            }

            // If you get IPv4 addresses represented as IPv6 (e.g. ::ffff:10.0.0.1)
            return ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip;
        }

        private static bool IsAllowed(IPAddress ip, IEnumerable<string> cidrs)
        {
            foreach (string cidr in cidrs)
            {
                if (CidrMatcher.Contains(ip, cidr))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

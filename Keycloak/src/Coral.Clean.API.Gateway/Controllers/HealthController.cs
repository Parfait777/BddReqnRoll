using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Coral.Clean.API.Gateway.Controllers
{
    /// <summary>
    /// A health check controller providing liveness and readiness endpoints.
    /// </summary>
    /// <param name="healthCheckService">The health check service.</param>
    [Route("health")]
    public sealed class HealthController(HealthCheckService healthCheckService)
    {
    }
}
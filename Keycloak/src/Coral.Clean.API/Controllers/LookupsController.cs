using Coral.Clean.API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coral.Clean.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class LookupsController : ControllerBase
    {
        [Authorize]
        [HttpGet("reporting-periods")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get reporting periods",
            Description = "Retrieves a list of available reporting periods for filings.",
            OperationId = nameof(GetReportingPeriods)
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "List of reporting periods retrieved successfully", typeof(ApiResponseContract<IEnumerable<LookupContract>>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ProblemDetails))]

        public IActionResult GetReportingPeriods()
        {
            // In a real implementation, this would likely come from a database or configuration.
            var reportingPeriods = new[]
            {
                new { Id = 1, Name = "Q1 2024" },
                new { Id = 2, Name = "Q2 2024" },
                new { Id = 3, Name = "Q3 2024" },
                new { Id = 4, Name = "Q4 2024" }
            };
            return Ok(new ApiResponseContract<IEnumerable<LookupContract>>
            {
                Data = reportingPeriods.Select(rp => new LookupContract
                {
                    AllowedValue = rp.Name
                }),
                Success = true,
                Message = "List of reporting periods retrieved successfully."
            });
        }
    }
}

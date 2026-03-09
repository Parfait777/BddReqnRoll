using Coral.Clean.API.Models;
using Coral.Clean.API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coral.Clean.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class FilingsController : ControllerBase
    {
        [Authorize]
        [HttpPost("FAR")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Submit a FAR filing",
            Description = "Submits a FAR filing to the system. The request body should contain the necessary information for processing the filing.",
            OperationId = nameof(SubmitMutualFund)
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "FAR filing submitted successfully", typeof(ApiResponseContract<SubmissionResponseContract>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ProblemDetails))]
        public IActionResult SubmitMutualFund([FromBody] SubmissionRequestContract request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "The request body is missing or invalid."
                });
            }

            // Process the request here

            return Ok(new ApiResponseContract<SubmissionResponseContract>
            {
                Data = new SubmissionResponseContract
                {
                    FilingId = Guid.NewGuid().ToString()
                },
                Success = true,
                Message = "FAR filing submitted successfully."
            });
        }

        [Authorize]
        [HttpPost("PFR")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Submit a PFR filing",
            Description = "Submits a PFR filing to the system. The request body should contain the necessary information for processing the filing.",
            OperationId = nameof(SubmitPrivateFund)
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "PFR filing submitted successfully", typeof(ApiResponseContract<SubmissionResponseContract>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ProblemDetails))]
        public IActionResult SubmitPrivateFund([FromBody] SubmissionRequestContract request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "The request body is missing or invalid."
                });
            }

            // Process the request here 

            return Ok(new ApiResponseContract<SubmissionResponseContract>
            {
                Data = new SubmissionResponseContract
                {
                    FilingId = Guid.NewGuid().ToString()
                },
                Success = true,
                Message = "PFR filing submitted successfully."
            });
        }
    }
}

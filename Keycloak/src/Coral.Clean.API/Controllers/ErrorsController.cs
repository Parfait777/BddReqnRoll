using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coral.Clean.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ErrorsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get error information",
            Description = "Retrieves detailed information about errors that have occurred in the system.",
            OperationId = nameof(GetErrors)
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Error information retrieved successfully", typeof(IEnumerable<ProblemDetails>))]
        public IActionResult GetErrors()
        {
            // In a real implementation, this would likely come from a logging system or database.
            var errors = new[]
            {
                new ProblemDetails
                {
                    Title = "Sample error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "This is a sample error for demonstration purposes."
                },
                new ProblemDetails
                {
                    Title = "Another error",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "This is another sample error for demonstration purposes."
                },
                new ProblemDetails
                {
                    Title = "Unauthorized access",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "This error indicates that the user is not authorized to access the requested resource."
                },
                new ProblemDetails
                {
                    Title = "Forbidden access",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = "This error indicates that the user is authenticated but does not have permission to access the requested resource."
                },
                new ProblemDetails
                {
                    Title = "Resource not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = "This error indicates that the requested resource could not be found."
                }
            };
            return Ok(errors);
        }
    }
}

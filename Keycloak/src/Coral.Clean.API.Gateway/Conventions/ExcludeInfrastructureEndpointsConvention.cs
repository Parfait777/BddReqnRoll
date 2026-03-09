using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Coral.Clean.API.Gateway.Conventions
{
    /// <summary>
    /// Excludes infrastructure-related endpoints from appearing in API documentation.
    /// These endpoints are automatically added by the framework and are not intended for public consumption.
    /// </summary>
    public sealed class ExcludeInfrastructureEndpointsConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (ControllerModel controller in application.Controllers)
            {
                string name = controller.ControllerName;

                if (name.Contains("FileConfiguration", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("OutputCache", StringComparison.OrdinalIgnoreCase))
                {
                    controller.ApiExplorer.IsVisible = false;
                }
            }
        }
    }
}

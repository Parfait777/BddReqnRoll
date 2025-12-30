using Common.Api.BddTests.TestInfrastructure.Services;
using Reqnroll;

namespace Common.Api.BddTests.Specs.Hooks
{
    [Binding]
    public static class ScenarioHooks
    {
        [BeforeTestRun(Order = 0)]
        
        public static void BeforeTest()
        {
            Setup.CreateServices();
        }
    }
}

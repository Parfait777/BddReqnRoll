using Common.Api.BddTests.Support.Services;
using Reqnroll;

namespace Common.Api.BddTests.Hooks
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

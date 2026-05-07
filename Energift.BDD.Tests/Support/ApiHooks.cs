namespace Energift.BDD.Tests.Support;

[Binding]
public class ApiHooks
{
    private readonly ScenarioContext _scenarioContext;
    private TestWebApplicationFactory? _factory;

    public ApiHooks(ScenarioContext scenarioContext) => _scenarioContext = scenarioContext;

    [BeforeScenario]
    public void BeforeScenario()
    {
        _factory = new TestWebApplicationFactory();
        _scenarioContext.Set(_factory.CreateClient(), "HttpClient");
        _scenarioContext.Set(_factory, "Factory");
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _factory?.Dispose();
    }
}

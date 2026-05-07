using System.Net;
using System.Text.Json;
using Energift.Tests.Support;
using Xunit;

namespace Energift.Tests.ApiTests;

public class HealthApiTests
{
    [Fact]
    public async Task HealthEndpoint_ReturnsOkWithStatusField()
    {
        using var factory = new TestWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.Equal("ok", doc.RootElement.GetProperty("status").GetString());
    }
}

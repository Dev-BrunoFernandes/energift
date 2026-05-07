using System.Net;
using System.Text.Json;
using Energift.Fiap.Application.Dtos.Ranking;
using Energift.Fiap.Application.Services.Interfaces;
using Energift.Tests.Support;
using Moq;
using NJsonSchema;
using Xunit;

namespace Energift.Tests.ApiTests;

public class RankingApiTests
{
    // ─── GET /api/ranking ────────────────────────────────────────────────────

    [Fact]
    public async Task GetRanking_DefaultPeriod_Returns200WithRankingSchema()
    {
        using var factory = new TestWebApplicationFactory();
        factory.RankingServiceMock
            .Setup(s => s.GetRankingAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<RankingResponse>
            {
                new() { UsuarioId = 1, TotalSavedKwh = 50 },
                new() { UsuarioId = 2, TotalSavedKwh = 30 }
            });

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/ranking");
        var content = await response.Content.ReadAsStringAsync();

        // Validação de status code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Validação do corpo JSON
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.Equal(2, doc.RootElement.GetArrayLength());

        var first = doc.RootElement[0];
        Assert.True(first.TryGetProperty("usuarioId", out _), "Campo 'usuarioId' ausente.");
        Assert.True(first.TryGetProperty("totalSavedKwh", out _), "Campo 'totalSavedKwh' ausente.");

        // Validação de contrato (JSON Schema)
        var schemaJson = await File.ReadAllTextAsync("Schemas/ranking-response.schema.json");
        var schema = await JsonSchema.FromJsonAsync(schemaJson);
        var errors = schema.Validate(content);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task GetRanking_MonthlyPeriod_Returns200WithCorrectPeriod()
    {
        using var factory = new TestWebApplicationFactory();
        factory.RankingServiceMock
            .Setup(s => s.GetRankingAsync("monthly"))
            .ReturnsAsync(new List<RankingResponse>
            {
                new() { UsuarioId = 1, TotalSavedKwh = 75 }
            });

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/ranking?period=monthly");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.Equal(1, doc.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task GetRanking_NoUsers_Returns200WithEmptyArray()
    {
        using var factory = new TestWebApplicationFactory();
        factory.RankingServiceMock
            .Setup(s => s.GetRankingAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<RankingResponse>());

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/ranking");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.Equal(0, doc.RootElement.GetArrayLength());
    }
}

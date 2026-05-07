using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Energift.Fiap.Application.Dtos.Goal;
using Energift.Fiap.Application.Services.Interfaces;
using Energift.Tests.Support;
using Moq;
using NJsonSchema;
using Xunit;

namespace Energift.Tests.ApiTests;

public class GoalApiTests
{
    // ─── POST /api/goal ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateGoal_ValidRequest_Returns200WithGoalSchema()
    {
        using var factory = new TestWebApplicationFactory();
        var expected = new GoalResponse
        {
            Id = Guid.NewGuid(), UsuarioId = 1, ImovelId = 1,
            TargetPercentReduction = 10, StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(6), Achieved = false
        };
        factory.GoalServiceMock
            .Setup(s => s.CreateGoalAsync(It.IsAny<GoalRequest>()))
            .ReturnsAsync(expected);

        var client = factory.CreateClient();
        var body = new GoalRequest
        {
            UsuarioId = 1, ImovelId = 1, TargetPercentReduction = 10,
            StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(6)
        };

        var response = await client.PostAsJsonAsync("/api/goal", body);
        var content = await response.Content.ReadAsStringAsync();

        // Validação de status code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Validação do corpo JSON
        using var doc = JsonDocument.Parse(content);
        Assert.True(doc.RootElement.TryGetProperty("id", out _), "Campo 'id' ausente.");
        Assert.True(doc.RootElement.TryGetProperty("achieved", out _), "Campo 'achieved' ausente.");
        Assert.True(doc.RootElement.TryGetProperty("targetPercentReduction", out var pct));
        Assert.Equal(10, pct.GetDecimal());

        // Validação de contrato (JSON Schema)
        var schemaJson = await File.ReadAllTextAsync("Schemas/goal-response.schema.json");
        var schema = await JsonSchema.FromJsonAsync(schemaJson);
        var errors = schema.Validate(content);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task CreateGoal_InvalidPercent_Returns500WithErrorBody()
    {
        using var factory = new TestWebApplicationFactory();
        factory.GoalServiceMock
            .Setup(s => s.CreateGoalAsync(It.IsAny<GoalRequest>()))
            .ThrowsAsync(new ArgumentException("TargetPercentReduction deve ser entre 1 e 100."));

        var client = factory.CreateClient();
        var body = new GoalRequest
        {
            UsuarioId = 1, ImovelId = 1, TargetPercentReduction = 0,
            StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(6)
        };

        var response = await client.PostAsJsonAsync("/api/goal", body);
        var content = await response.Content.ReadAsStringAsync();

        // Validação de status code (ExceptionHandler retorna 500)
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        // Validação do corpo JSON de erro
        using var doc = JsonDocument.Parse(content);
        Assert.True(doc.RootElement.TryGetProperty("error", out _), "Campo 'error' ausente na resposta de erro.");
    }

    [Fact]
    public async Task CreateGoal_EndDateBeforeStartDate_Returns500WithErrorBody()
    {
        using var factory = new TestWebApplicationFactory();
        factory.GoalServiceMock
            .Setup(s => s.CreateGoalAsync(It.IsAny<GoalRequest>()))
            .ThrowsAsync(new ArgumentException("EndDate deve ser posterior ao StartDate."));

        var client = factory.CreateClient();
        var body = new GoalRequest
        {
            UsuarioId = 1, ImovelId = 1, TargetPercentReduction = 10,
            StartDate = DateTime.UtcNow.AddMonths(6), EndDate = DateTime.UtcNow
        };

        var response = await client.PostAsJsonAsync("/api/goal", body);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.True(doc.RootElement.TryGetProperty("error", out _));
    }
}

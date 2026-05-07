using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Energift.Fiap.Application.Dtos.CalculateCoins;
using Energift.Fiap.Application.Dtos.Consumo;
using Energift.Fiap.Application.Services.Interfaces;
using Energift.Tests.Support;
using Moq;
using NJsonSchema;
using Xunit;

namespace Energift.Tests.ApiTests;

public class ConsumoApiTests
{
    // ─── POST /api/consumo ───────────────────────────────────────────────────

    [Fact]
    public async Task CreateConsumo_ValidRequest_Returns200WithConsumoSchema()
    {
        using var factory = new TestWebApplicationFactory();
        var expected = new ConsumoResponse
        {
            Id = Guid.NewGuid(), UsuarioId = 1, ImovelId = 1,
            Referencia = DateTime.UtcNow, Kwh = 200, Valor = 150
        };
        factory.ConsumoServiceMock
            .Setup(s => s.CreateConsumptionAsync(It.IsAny<ConsumoRequest>()))
            .ReturnsAsync(expected);

        var client = factory.CreateClient();
        var body = new ConsumoRequest
            { UsuarioId = 1, ImovelId = 1, Referencia = DateTime.UtcNow, Kwh = 200, Valor = 150 };

        var response = await client.PostAsJsonAsync("/api/consumo", body);
        var content = await response.Content.ReadAsStringAsync();

        // Validação de status code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Validação do corpo JSON
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);
        Assert.True(doc.RootElement.TryGetProperty("id", out _), "Campo 'id' ausente.");
        Assert.True(doc.RootElement.TryGetProperty("usuarioId", out _), "Campo 'usuarioId' ausente.");
        Assert.True(doc.RootElement.TryGetProperty("kwh", out _), "Campo 'kwh' ausente.");

        // Validação de contrato (JSON Schema)
        var schemaJson = await File.ReadAllTextAsync("Schemas/consumo-response.schema.json");
        var schema = await JsonSchema.FromJsonAsync(schemaJson);
        var errors = schema.Validate(content);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task CreateConsumo_ServiceThrows_Returns500()
    {
        using var factory = new TestWebApplicationFactory();
        factory.ConsumoServiceMock
            .Setup(s => s.CreateConsumptionAsync(It.IsAny<ConsumoRequest>()))
            .ThrowsAsync(new Exception("Erro interno simulado"));

        var client = factory.CreateClient();
        var body = new ConsumoRequest
            { UsuarioId = 0, ImovelId = 0, Referencia = DateTime.UtcNow, Kwh = 0, Valor = 0 };

        var response = await client.PostAsJsonAsync("/api/consumo", body);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.True(doc.RootElement.TryGetProperty("error", out _), "Campo 'error' ausente na resposta de erro.");
    }

    // ─── GET /api/consumo ────────────────────────────────────────────────────

    [Fact]
    public async Task GetConsumo_ValidUsuarioId_Returns200WithPagedSchema()
    {
        using var factory = new TestWebApplicationFactory();
        var items = new List<ConsumoResponse>
        {
            new() { Id = Guid.NewGuid(), UsuarioId = 1, ImovelId = 1,
                    Referencia = DateTime.UtcNow, Kwh = 200, Valor = 150 }
        };
        var page = new { Items = items, Page = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        factory.ConsumoServiceMock
            .Setup(s => s.GetPagedAsync(1, It.IsAny<int?>(), It.IsAny<DateTime?>(),
                                        It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(((object)page, 1));

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/consumo?usuarioId=1");
        var content = await response.Content.ReadAsStringAsync();

        // Validação de status code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Validação do corpo JSON
        using var doc = JsonDocument.Parse(content);
        Assert.True(doc.RootElement.TryGetProperty("items", out _), "Campo 'items' ausente.");
        Assert.True(doc.RootElement.TryGetProperty("totalItems", out _), "Campo 'totalItems' ausente.");
        Assert.True(doc.RootElement.TryGetProperty("totalPages", out _), "Campo 'totalPages' ausente.");

        // Validação de contrato (JSON Schema)
        var schemaJson = await File.ReadAllTextAsync("Schemas/consumo-paged-response.schema.json");
        var schema = await JsonSchema.FromJsonAsync(schemaJson);
        var errors = schema.Validate(content);
        Assert.Empty(errors);
    }

    // ─── POST /api/consumo/calculate-coins ──────────────────────────────────

    [Fact]
    public async Task CalculateCoins_WithReduction_Returns200WithAwardedCoins()
    {
        using var factory = new TestWebApplicationFactory();
        factory.ConsumoServiceMock
            .Setup(s => s.CalculateAndRegisterWattCoinsAsync(It.IsAny<CalculateCoinsRequest>()))
            .ReturnsAsync(100);

        var client = factory.CreateClient();
        var body = new CalculateCoinsRequest
        {
            UsuarioId = 1, ImovelId = 1, Referencia = DateTime.UtcNow,
            Kwh = 180, Valor = 120, PreviousKwh = 200
        };

        var response = await client.PostAsJsonAsync("/api/consumo/calculate-coins", body);
        var content = await response.Content.ReadAsStringAsync();

        // Validação de status code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Validação do corpo JSON
        using var doc = JsonDocument.Parse(content);
        Assert.True(doc.RootElement.TryGetProperty("awarded", out var awarded), "Campo 'awarded' ausente.");
        Assert.True(awarded.GetInt32() >= 0, "awarded deve ser >= 0.");
    }

    [Fact]
    public async Task CalculateCoins_NoReduction_Returns200WithZeroCoins()
    {
        using var factory = new TestWebApplicationFactory();
        factory.ConsumoServiceMock
            .Setup(s => s.CalculateAndRegisterWattCoinsAsync(It.IsAny<CalculateCoinsRequest>()))
            .ReturnsAsync(0);

        var client = factory.CreateClient();
        var body = new CalculateCoinsRequest
        {
            UsuarioId = 1, ImovelId = 1, Referencia = DateTime.UtcNow,
            Kwh = 250, Valor = 180, PreviousKwh = 200
        };

        var response = await client.PostAsJsonAsync("/api/consumo/calculate-coins", body);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = JsonDocument.Parse(content);
        Assert.True(doc.RootElement.TryGetProperty("awarded", out var awarded));
        Assert.Equal(0, awarded.GetInt32());
    }
}

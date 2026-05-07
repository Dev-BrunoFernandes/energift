namespace Energift.BDD.Tests.StepDefinitions;

[Binding]
public class ApiSteps
{
    private readonly ScenarioContext _context;
    private HttpResponseMessage? _response;
    private string _responseContent = string.Empty;

    public ApiSteps(ScenarioContext context) => _context = context;

    private HttpClient Client => _context.Get<HttpClient>("HttpClient");
    private TestWebApplicationFactory Factory => _context.Get<TestWebApplicationFactory>("Factory");

    // ─── CONSUMO ────────────────────────────────────────────────────────────

    [Given("que preparo um registro de consumo com {int} kWh para o usuário {int}")]
    public void PrepararRegistroConsumo(int kwh, int usuarioId)
    {
        var consumoResponse = new ConsumoResponse
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            ImovelId = 1,
            Referencia = DateTime.UtcNow,
            Kwh = kwh,
            Valor = 150.00m
        };

        Factory.ConsumoServiceMock
            .Setup(s => s.CreateConsumptionAsync(It.IsAny<ConsumoRequest>()))
            .ReturnsAsync(consumoResponse);

        _context["requestBody"] = new ConsumoRequest
        {
            UsuarioId = usuarioId,
            ImovelId = 1,
            Referencia = DateTime.UtcNow,
            Kwh = kwh,
            Valor = 150.00m
        };
    }

    [Given("que o serviço retorna uma página de consumos para o usuário {int}")]
    public void ConfigurarPaginaConsumos(int usuarioId)
    {
        var items = new List<ConsumoResponse>
        {
            new() { Id = Guid.NewGuid(), UsuarioId = usuarioId, ImovelId = 1,
                    Referencia = DateTime.UtcNow, Kwh = 200, Valor = 150 }
        };
        var page = new { Items = items, Page = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        Factory.ConsumoServiceMock
            .Setup(s => s.GetPagedAsync(usuarioId, It.IsAny<int?>(), It.IsAny<DateTime?>(),
                                        It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(((object)page, 1));
    }

    [Given("que preparo um cálculo de WattCoins com {int} kWh para o usuário {int}")]
    public void PrepararCalculoWattCoins(int kwh, int usuarioId)
    {
        Factory.ConsumoServiceMock
            .Setup(s => s.CalculateAndRegisterWattCoinsAsync(It.IsAny<CalculateCoinsRequest>()))
            .ReturnsAsync(100);

        _context["requestBody"] = new CalculateCoinsRequest
        {
            UsuarioId = usuarioId,
            ImovelId = 1,
            Referencia = DateTime.UtcNow,
            Kwh = kwh,
            Valor = 120.00m,
            PreviousKwh = 200m
        };
    }

    // ─── GOAL ───────────────────────────────────────────────────────────────

    [Given("que preparo uma meta com {int} porcento de redução para o usuário {int}")]
    public void PrepararMeta(int percent, int usuarioId)
    {
        if (percent <= 0)
        {
            Factory.GoalServiceMock
                .Setup(s => s.CreateGoalAsync(It.IsAny<GoalRequest>()))
                .ThrowsAsync(new ArgumentException("TargetPercentReduction deve ser entre 1 e 100."));
        }
        else
        {
            var goalResponse = new GoalResponse
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                ImovelId = 1,
                TargetPercentReduction = percent,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(6),
                Achieved = false
            };

            Factory.GoalServiceMock
                .Setup(s => s.CreateGoalAsync(It.IsAny<GoalRequest>()))
                .ReturnsAsync(goalResponse);
        }

        _context["requestBody"] = new GoalRequest
        {
            UsuarioId = usuarioId,
            ImovelId = 1,
            TargetPercentReduction = percent,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(6)
        };
    }

    // ─── RANKING ────────────────────────────────────────────────────────────

    [Given("que o sistema possui usuários com consumo registrado")]
    public void ConfigurarUsuariosComConsumo()
    {
        var ranking = new List<RankingResponse>
        {
            new() { UsuarioId = 1, TotalSavedKwh = 50 },
            new() { UsuarioId = 2, TotalSavedKwh = 30 }
        };

        Factory.RankingServiceMock
            .Setup(s => s.GetRankingAsync(It.IsAny<string>()))
            .ReturnsAsync(ranking);
    }

    // ─── WHEN ───────────────────────────────────────────────────────────────

    [When("envio uma requisição POST para {string}")]
    public async Task EnviarPost(string url)
    {
        var body = _context.ContainsKey("requestBody") ? _context["requestBody"] : new { };
        _response = await Client.PostAsJsonAsync(url, body);
        _responseContent = await _response.Content.ReadAsStringAsync();
    }

    [When("envio uma requisição GET para {string}")]
    public async Task EnviarGet(string url)
    {
        _response = await Client.GetAsync(url);
        _responseContent = await _response.Content.ReadAsStringAsync();
    }

    // ─── THEN ───────────────────────────────────────────────────────────────

    [Then("devo receber o status HTTP {int}")]
    public void VerificarStatusHttp(int statusCode)
    {
        Assert.NotNull(_response);
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }

    [Then("a resposta deve conter o campo {string}")]
    public void VerificarCampoNaResposta(string field)
    {
        Assert.False(string.IsNullOrWhiteSpace(_responseContent),
            "Corpo da resposta está vazio.");

        using var doc = JsonDocument.Parse(_responseContent);
        var found = doc.RootElement.EnumerateObject()
            .Any(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));

        Assert.True(found, $"Campo '{field}' não encontrado. Resposta: {_responseContent}");
    }

    [Then("a resposta deve ser uma lista")]
    public void VerificarRespostaEhLista()
    {
        Assert.False(string.IsNullOrWhiteSpace(_responseContent),
            "Corpo da resposta está vazio.");

        using var doc = JsonDocument.Parse(_responseContent);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
    }
}

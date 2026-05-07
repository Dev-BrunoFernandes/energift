# Documentação Técnica - Energift

**Título do Projeto:** Energift

**Nome dos Integrantes:** Bruno Guilherme de Jesus Fernandes, Nicolas Medeiros Moreira Kubitza, Gabriel dos Santos Melo e Vínicius Toledo Batista

---

## Descrição do Pipeline CI/CD

**Ferramenta Utilizada:** GitHub Actions

O pipeline de CI/CD foi implementado utilizando GitHub Actions para automatizar o processo de build, teste e deploy da aplicação Energift. Ele é configurado para ser acionado automaticamente em eventos de `push` e `pull_request` na branch `main`.

**Etapas e Lógica:**

1.  **`build-and-test`:**
    *   **Checkout do Código:** Clona o repositório para o ambiente do runner.
    *   **Setup .NET:** Instala a versão 8.0.x do .NET SDK.
    *   **Restauração de Dependências:** Executa `dotnet restore` para baixar todas as dependências.
    *   **Build da Aplicação:** Compila a solução em modo `Release`.
    *   **Execução de Testes de API:** Executa os testes automatizados do projeto `Energift.Tests` (unitários + API + contrato), gerando arquivo `.trx`.
    *   **Execução de Testes BDD:** Executa os cenários Gherkin do projeto `Energift.BDD.Tests`, gerando arquivo `.trx`.
    *   **Upload de Resultados de Teste:** Os resultados são salvos como artefato para análise posterior.
    *   **Publicação e Upload do Artefato de Build.**

2.  **`build-and-push-docker`:** Constrói a imagem Docker e envia ao GitHub Container Registry (GHCR).

3.  **`deploy-staging`:** Implanta no Azure App Service (Staging) via imagem Docker do GHCR.

4.  **`deploy-production`:** Implanta no Azure App Service (Produção) após aprovação de Staging.

---

## Docker: Arquitetura, Comandos e Imagem Criada

### Arquitetura de Containerização

*   **Imagem da Aplicação:** Imagem Docker personalizada para a API C# .NET 8.0.
*   **Banco de Dados:** Contêiner separado para PostgreSQL, com persistência via volume nomeado.
*   **Orquestração:** Docker Compose para gerenciar e interligar os contêineres.

### Dockerfile

O `Dockerfile` é multi-estágio:

*   **`build-env`:** SDK .NET 8.0 — compila e publica a aplicação.
*   **`final`:** Runtime ASP.NET Core 8.0 — imagem leve para produção.

### Comandos Docker Essenciais

```bash
# Subir aplicação + banco de dados
docker-compose up --build

# Parar os serviços
docker-compose down

# Construir imagem isolada
docker build -t energift-api .
```

---

## Estratégia de Testes

A cobertura de testes do projeto Energift é composta por três camadas complementares:

| Camada | Projeto | Framework | Quantidade |
|---|---|---|---|
| Testes Unitários | Energift.Tests | xUnit | 2 testes |
| Testes de API (integração HTTP) | Energift.Tests | xUnit + Mvc.Testing + Moq + NJsonSchema | 11 testes |
| Testes BDD (Gherkin) | Energift.BDD.Tests | Reqnroll + xUnit | 7 cenários |
| **Total** | | | **21 testes** |

**Execução local (sem banco de dados necessário):**
```bash
dotnet test
```

Os testes utilizam banco em memória (EF Core InMemory) e serviços mockados (Moq), não dependendo de PostgreSQL ou qualquer infraestrutura externa.

---

## Requisito 1 — Cenários de Teste BDD (Gherkin)

**Framework:** Reqnroll 2.4.1 (sucessor open-source do SpecFlow para .NET 8)

**Localização:** `Energift.BDD.Tests/Features/`

Os cenários são escritos em português usando as palavras-chave nativas do Gherkin (`# language: pt`): `Funcionalidade`, `Cenário`, `Dado`, `Quando`, `Então`, `E`.

---

### Feature 1 — Consumo.feature

```gherkin
# language: pt
Funcionalidade: Gerenciamento de Consumo de Energia
  Como usuário do Energift
  Quero registrar e consultar meu consumo de energia
  Para acompanhar minha eficiência energética e acumular WattCoins

  Cenário: Registrar consumo de energia com dados válidos
    Dado que preparo um registro de consumo com 200 kWh para o usuário 1
    Quando envio uma requisição POST para "/api/consumo"
    Então devo receber o status HTTP 200
    E a resposta deve conter o campo "id"

  Cenário: Consultar histórico de consumo paginado por usuário
    Dado que o serviço retorna uma página de consumos para o usuário 1
    Quando envio uma requisição GET para "/api/consumo?usuarioId=1"
    Então devo receber o status HTTP 200
    E a resposta deve conter o campo "items"

  Cenário: Calcular WattCoins ao reduzir consumo energético
    Dado que preparo um cálculo de WattCoins com 180 kWh para o usuário 1
    Quando envio uma requisição POST para "/api/consumo/calculate-coins"
    Então devo receber o status HTTP 200
    E a resposta deve conter o campo "awarded"
```

**Descrição dos cenários:**

| Cenário | Tipo | Descrição |
|---|---|---|
| Registrar consumo com dados válidos | Positivo (caminho feliz) | Verifica que um registro de consumo válido retorna 200 e o campo `id` do recurso criado |
| Consultar histórico paginado | Positivo (caminho feliz) | Verifica que a listagem paginada retorna 200 e o campo `items` com os consumos do usuário |
| Calcular WattCoins com redução | Positivo (caminho feliz) | Verifica que ao reduzir o consumo, o sistema calcula e retorna WattCoins (`awarded`) |

---

### Feature 2 — Goal.feature

```gherkin
# language: pt
Funcionalidade: Gerenciamento de Metas de Redução de Consumo
  Como usuário do Energift
  Quero criar metas de redução de consumo energético
  Para monitorar meus objetivos de eficiência e ser recompensado por alcançá-los

  Cenário: Criar meta de redução com dados válidos
    Dado que preparo uma meta com 10 porcento de redução para o usuário 1
    Quando envio uma requisição POST para "/api/goal"
    Então devo receber o status HTTP 200
    E a resposta deve conter o campo "id"

  Cenário: Criar meta com percentual de redução inválido deve retornar erro
    Dado que preparo uma meta com 0 porcento de redução para o usuário 1
    Quando envio uma requisição POST para "/api/goal"
    Então devo receber o status HTTP 500
    E a resposta deve conter o campo "error"
```

**Descrição dos cenários:**

| Cenário | Tipo | Descrição |
|---|---|---|
| Criar meta com dados válidos | Positivo (caminho feliz) | Verifica que uma meta com 10% de redução retorna 200 e o campo `id` |
| Criar meta com percentual 0% | Negativo (falha) | Verifica que percentual inválido (≤ 0) aciona a regra de negócio e retorna 500 com campo `error` |

---

### Feature 3 — Ranking.feature

```gherkin
# language: pt
Funcionalidade: Consulta de Ranking de Eficiência Energética
  Como usuário do Energift
  Quero consultar o ranking de usuários por eficiência
  Para comparar meu desempenho com outros usuários da plataforma

  Cenário: Consultar ranking mensal com usuários cadastrados
    Dado que o sistema possui usuários com consumo registrado
    Quando envio uma requisição GET para "/api/ranking?period=monthly"
    Então devo receber o status HTTP 200
    E a resposta deve ser uma lista

  Cenário: Consultar ranking anual com usuários cadastrados
    Dado que o sistema possui usuários com consumo registrado
    Quando envio uma requisição GET para "/api/ranking?period=yearly"
    Então devo receber o status HTTP 200
    E a resposta deve ser uma lista
```

**Descrição dos cenários:**

| Cenário | Tipo | Descrição |
|---|---|---|
| Ranking mensal | Positivo (caminho feliz) | Verifica que o ranking mensal retorna 200 e uma lista JSON |
| Ranking anual | Positivo (caminho feliz) | Verifica que o ranking anual retorna 200 e uma lista JSON |

---

### Print — Execução dos Testes BDD

> **Inserir aqui:** print do terminal mostrando os 7 cenários BDD passando (`Passed: 7`)

---

## Requisito 2 — Testes de API

**Framework:** xUnit + `Microsoft.AspNetCore.Mvc.Testing` + Moq + NJsonSchema

**Localização:** `Energift.Tests/ApiTests/`

**Infraestrutura de teste:** `TestWebApplicationFactory` sobe a API completa em memória, substituindo:
- PostgreSQL → EF Core InMemory (sem dependência de banco)
- Serviços reais → Mocks Moq (isolamento total)

---

### 2.1 Validação de Status Code

Todos os testes verificam explicitamente o status code HTTP retornado:

```csharp
Assert.Equal(HttpStatusCode.OK, response.StatusCode);         // 200
Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode); // 500
```

| Endpoint | Cenário | Status Esperado |
|---|---|---|
| GET /health | Requisição normal | 200 OK |
| POST /api/consumo | Dados válidos | 200 OK |
| POST /api/consumo | Serviço lança exceção | 500 Internal Server Error |
| GET /api/consumo | UsuarioId válido | 200 OK |
| POST /api/consumo/calculate-coins | Com redução de consumo | 200 OK |
| POST /api/consumo/calculate-coins | Sem redução (consumo maior) | 200 OK |
| POST /api/goal | Dados válidos | 200 OK |
| POST /api/goal | Percentual = 0 (inválido) | 500 Internal Server Error |
| POST /api/goal | EndDate antes de StartDate | 500 Internal Server Error |
| GET /api/ranking | Período padrão | 200 OK |
| GET /api/ranking | Período mensal | 200 OK |
| GET /api/ranking | Sem usuários | 200 OK |

---

### 2.2 Validação do Corpo de Resposta (JSON)

Cada teste inspeciona os campos obrigatórios do JSON retornado:

```csharp
using var doc = JsonDocument.Parse(content);
Assert.True(doc.RootElement.TryGetProperty("id", out _));
Assert.True(doc.RootElement.TryGetProperty("usuarioId", out _));
// Testes de erro verificam campo "error"
Assert.True(doc.RootElement.TryGetProperty("error", out _));
```

| Endpoint | Campos validados |
|---|---|
| GET /health | `status` = "ok" |
| POST /api/consumo | `id`, `usuarioId`, `kwh` |
| GET /api/consumo | `items`, `totalItems`, `totalPages` |
| POST /api/consumo/calculate-coins | `awarded` (int ≥ 0) |
| POST /api/goal | `id`, `achieved`, `targetPercentReduction` |
| GET /api/ranking | array com `usuarioId`, `totalSavedKwh` |
| Erros (500) | `error` |

---

### 2.3 Testes de Contrato — JSON Schema

**Biblioteca:** NJsonSchema 11.1.0

Os schemas estão em `Energift.Tests/Schemas/` e validam a estrutura completa da resposta:

**`consumo-response.schema.json`**
```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": "ConsumoResponse",
  "type": "object",
  "required": ["id", "usuarioId", "imovelId", "referencia", "kwh", "valor"],
  "properties": {
    "id":        { "type": "string" },
    "usuarioId": { "type": "integer" },
    "imovelId":  { "type": "integer" },
    "referencia":{ "type": "string" },
    "kwh":       { "type": "number" },
    "valor":     { "type": "number" }
  }
}
```

**`consumo-paged-response.schema.json`**
```json
{
  "type": "object",
  "required": ["items", "page", "pageSize", "totalItems", "totalPages"],
  "properties": {
    "items":      { "type": "array" },
    "page":       { "type": "integer" },
    "pageSize":   { "type": "integer" },
    "totalItems": { "type": "integer" },
    "totalPages": { "type": "integer" }
  }
}
```

**`goal-response.schema.json`**
```json
{
  "type": "object",
  "required": ["id", "usuarioId", "imovelId", "targetPercentReduction",
               "startDate", "endDate", "achieved"],
  "properties": {
    "id":                     { "type": "string" },
    "usuarioId":              { "type": "integer" },
    "imovelId":               { "type": "integer" },
    "targetPercentReduction": { "type": "number" },
    "startDate":              { "type": "string" },
    "endDate":                { "type": "string" },
    "achieved":               { "type": "boolean" }
  }
}
```

**`ranking-response.schema.json`**
```json
{
  "type": "array",
  "items": {
    "type": "object",
    "required": ["usuarioId", "totalSavedKwh"],
    "properties": {
      "usuarioId":    { "type": "integer" },
      "totalSavedKwh":{ "type": "number" }
    }
  }
}
```

Validação no código:
```csharp
var schemaJson = await File.ReadAllTextAsync("Schemas/goal-response.schema.json");
var schema = await JsonSchema.FromJsonAsync(schemaJson);
var errors = schema.Validate(content);
Assert.Empty(errors); // nenhuma violação de contrato
```

---

### Print — Execução dos Testes de API

> **Inserir aqui:** print do terminal mostrando os 14 testes passando (`Passed: 14`)

---

## Requisito 3 — Execução dos Testes

### Execução Local

Pré-requisito: .NET 8 SDK instalado. **Nenhum banco de dados necessário.**

```bash
# Todos os testes (BDD + API + unitários)
dotnet test

# Apenas testes de API e unitários
dotnet test Energift.Tests/Energift.Tests.csproj --verbosity normal

# Apenas testes BDD
dotnet test Energift.BDD.Tests/Energift.BDD.Tests.csproj --verbosity normal

# Com exportação de relatório
dotnet test --logger "trx;LogFileName=results.trx" --verbosity normal
```

### Execução no Pipeline CI/CD (GitHub Actions)

O pipeline `.github/workflows/main.yml` executa os testes automaticamente a cada `push` ou `pull_request` na branch `main`:

```yaml
- name: Run Unit and API Tests
  run: dotnet test Energift.Tests/Energift.Tests.csproj
       --configuration Release --no-build --verbosity normal
       --logger "trx;LogFileName=api-tests.trx"

- name: Run BDD Tests
  run: dotnet test Energift.BDD.Tests/Energift.BDD.Tests.csproj
       --configuration Release --no-build --verbosity normal
       --logger "trx;LogFileName=bdd-tests.trx"
```

Os resultados `.trx` são salvos como artefato de pipeline para evidência auditável.

### Print — Pipeline GitHub Actions

> **Inserir aqui:** print da aba "Actions" do GitHub com o pipeline concluído (jobs `build-and-test`, `build-and-push-docker`, `deploy-staging`, `deploy-production` todos com ✅)

---

## Prints do Pipeline Rodando (Build, Testes, Deploy)

> **Inserir aqui:** prints das etapas de build e deploy no GitHub Actions

---

## Prints dos Ambientes Staging e Produção Funcionando

> **Inserir aqui:** prints da aplicação funcionando nos ambientes de Staging e Produção no Azure (Swagger UI acessível)

---

## Desafios Encontrados e Como Foram Resolvidos

1.  **Erro de `dotnet: command not found`:**
    *   **Desafio:** Durante a configuração inicial, o comando `dotnet` não estava disponível no ambiente.
    *   **Resolução:** Instalação do .NET SDK 8.0 via `sudo apt-get install dotnet-sdk-8.0`.

2.  **Erros de Referência e Compilação de Testes:**
    *   **Desafio:** Projeto de testes dentro de `src/` com referências circulares.
    *   **Resolução:** Movido `Energift.Tests` para a raiz, dependências xUnit adicionadas explicitamente, solução reconstruída.

3.  **Otimização do Dockerfile para CI/CD:**
    *   **Desafio:** Dockerfile sem estágios definidos para build e runtime.
    *   **Resolução:** Dockerfile multi-estágio com `build-env` e `final`, resultando em imagem menor.

4.  **Arquivo .sln não encontrado no Dockerfile:**
    *   **Desafio:** Caminhos relativos incorretos no contexto de build Docker.
    *   **Resolução:** Ajuste do Dockerfile para copiar `.sln` da raiz e `.csproj` de seus diretórios.

5.  **Restrição de Azure Container Registry em conta de estudante:**
    *   **Desafio:** ACR não disponível na conta de estudante Azure.
    *   **Resolução:** Pipeline adaptado para usar GitHub Container Registry (GHCR), gratuito e integrado.

6.  **Dependência de PostgreSQL nos testes:**
    *   **Desafio:** Testes de API exigiam banco PostgreSQL ativo para executar.
    *   **Resolução:** `TestWebApplicationFactory` com EF Core InMemory substitui PostgreSQL; Moq substitui serviços reais. Testes executam sem nenhuma infraestrutura externa.

7.  **Integração do Reqnroll com ASP.NET Core:**
    *   **Desafio:** Compartilhar `WebApplicationFactory` e mocks entre steps Gherkin de cenários diferentes.
    *   **Resolução:** `ApiHooks` com `[BeforeScenario]`/`[AfterScenario]` cria fábrica isolada por cenário; `ScenarioContext` compartilha `HttpClient` e referências de mock entre steps.

---

## Checklist de Entrega

| Item | Status |
|---|---|
| Projeto compactado em .ZIP com estrutura organizada | ✅ |
| Dockerfile funcional (multi-stage) | ✅ |
| docker-compose.yml com API + PostgreSQL | ✅ |
| Pipeline CI/CD com build, teste e deploy | ✅ |
| Mínimo 3 cenários BDD em Gherkin | ✅ (7 cenários) |
| Testes automatizados para todas as APIs | ✅ (11 testes) |
| Validação de status code | ✅ |
| Validação do corpo de resposta JSON | ✅ |
| Testes de contrato com JSON Schema | ✅ (4 schemas) |
| Execução simples e documentada | ✅ (`dotnet test`) |
| README com instruções completas | ✅ |
| Documentação técnica com evidências | ✅ |
| Deploy nos ambientes staging e produção | ✅ |

---

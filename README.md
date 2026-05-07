# Energift — Plataforma de Eficiência Energética

> Projeto desenvolvido para a disciplina de **Cidades ESG Inteligentes** — FIAP  
> Integrantes: Bruno Guilherme de Jesus Fernandes · Nicolas Medeiros Moreira Kubitza · Gabriel dos Santos Melo · Vínicius Toledo Batista

---

## Sobre o Projeto

O **Energift** é uma API REST desenvolvida em C# .NET 8.0 que incentiva o consumo consciente de energia elétrica através de gamificação. Os usuários registram seu consumo mensal, definem metas de redução e acumulam **WattCoins** a cada redução alcançada. A plataforma exibe um ranking de eficiência entre os participantes.

**Funcionalidades principais:**
- Registro e histórico de consumo energético por imóvel
- Criação e acompanhamento de metas de redução percentual
- Cálculo automático de WattCoins ao atingir reduções
- Ranking de usuários por eficiência energética

---

## Estrutura do Projeto

```
Cidades_ESGInteligentes/
├── .github/
│   └── workflows/
│       └── main.yml                  # Pipeline CI/CD GitHub Actions
├── src/                              # Código-fonte da aplicação
│   ├── Api/
│   │   ├── Controllers/              # Endpoints REST
│   │   ├── Configuration/            # Injeção de dependência
│   │   └── Filters/                  # Middleware de exceções
│   ├── Application/
│   │   ├── Dtos/                     # Objetos de transferência de dados
│   │   ├── Services/                 # Lógica de negócio
│   │   └── Mappings/                 # Perfis AutoMapper
│   ├── Domain/
│   │   ├── Entities/                 # Entidades de domínio
│   │   └── Interfaces/               # Contratos de repositório
│   ├── Infrastructure/
│   │   ├── Context/                  # EnergyDbContext (EF Core)
│   │   └── Repositories/             # Implementações de repositório
│   ├── Program.cs
│   └── Energift.Fiap.csproj
├── Energift.Tests/                   # Testes unitários e de API
│   ├── ApiTests/
│   │   ├── HealthApiTests.cs
│   │   ├── ConsumoApiTests.cs
│   │   ├── GoalApiTests.cs
│   │   └── RankingApiTests.cs
│   ├── Schemas/                      # JSON Schemas para testes de contrato
│   │   ├── consumo-response.schema.json
│   │   ├── consumo-paged-response.schema.json
│   │   ├── goal-response.schema.json
│   │   └── ranking-response.schema.json
│   ├── Support/
│   │   └── TestWebApplicationFactory.cs
│   └── Energift.Tests.csproj
├── Energift.BDD.Tests/               # Testes BDD com Gherkin (Reqnroll)
│   ├── Features/
│   │   ├── Consumo.feature           # 3 cenários
│   │   ├── Goal.feature              # 2 cenários
│   │   └── Ranking.feature           # 2 cenários
│   ├── StepDefinitions/
│   │   └── ApiSteps.cs
│   ├── Support/
│   │   ├── ApiHooks.cs
│   │   └── TestWebApplicationFactory.cs
│   └── Energift.BDD.Tests.csproj
├── Dockerfile
├── docker-compose.yml
├── Energift.Fiap.sln
└── README.md
```

---

## Tecnologias Utilizadas

| Camada | Tecnologia |
|---|---|
| Linguagem | C# / .NET 8.0 |
| Framework Web | ASP.NET Core Web API |
| Banco de Dados | PostgreSQL 17 |
| ORM | Entity Framework Core 8 |
| Mapeamento | AutoMapper 13 |
| Documentação | Swagger / Swashbuckle |
| Containerização | Docker / Docker Compose |
| CI/CD | GitHub Actions |
| Cloud | Azure App Service + GitHub Container Registry |
| Testes unitários/API | xUnit · Moq · NJsonSchema · Mvc.Testing |
| Testes BDD | Reqnroll (Gherkin) |
| Cobertura | coverlet |

---

## Pré-requisitos

| Ferramenta | Versão mínima |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0 |
| [Docker](https://www.docker.com/get-started) | 20.x |
| [Docker Compose](https://docs.docker.com/compose/) | 2.x |

---

## Executar Localmente com Docker

```bash
# 1. Clone o repositório
git clone <URL_DO_REPOSITORIO>
cd Cidades_ESGInteligentes

# 2. Suba API + banco de dados
docker-compose up --build
```

- **API:** `http://localhost:8080`
- **Swagger:** `http://localhost:8080/swagger` *(somente em Development)*

Para encerrar:
```bash
docker-compose down
```

---

## Executar os Testes

> Os testes **não precisam de banco de dados** — utilizam EF Core InMemory e serviços mockados (Moq).

### Todos os testes (recomendado)

```bash
dotnet test
```

### Por projeto

```bash
# Testes unitários + testes de API
dotnet test Energift.Tests/Energift.Tests.csproj --verbosity normal

# Testes BDD (cenários Gherkin)
dotnet test Energift.BDD.Tests/Energift.BDD.Tests.csproj --verbosity normal
```

### Com relatório de resultados

```bash
dotnet test --logger "trx;LogFileName=results.trx" --verbosity normal
```

### Resultado esperado

```
Energift.Tests       → Total: 14  Passed: 14  Failed: 0
Energift.BDD.Tests   → Total:  7  Passed:  7  Failed: 0
```

---

## Endpoints da API

| Método | Endpoint | Descrição |
|---|---|---|
| `GET` | `/health` | Health check da aplicação |
| `GET` | `/api/consumo?usuarioId={id}` | Lista consumos paginados do usuário |
| `POST` | `/api/consumo` | Registra novo consumo |
| `POST` | `/api/consumo/calculate-coins` | Calcula e credita WattCoins |
| `POST` | `/api/goal` | Cria meta de redução |
| `GET` | `/api/ranking?period={period}` | Retorna ranking de eficiência |

---

## Estratégia de Testes

### Testes BDD — `Energift.BDD.Tests`

Framework: **Reqnroll** (sucessor do SpecFlow para .NET 8), cenários em português (`# language: pt`).

| Feature | Cenário | Tipo |
|---|---|---|
| Consumo.feature | Registrar consumo com dados válidos | ✅ Positivo |
| Consumo.feature | Consultar histórico paginado | ✅ Positivo |
| Consumo.feature | Calcular WattCoins com redução | ✅ Positivo |
| Goal.feature | Criar meta com dados válidos | ✅ Positivo |
| Goal.feature | Criar meta com percentual 0% | ❌ Negativo |
| Ranking.feature | Consultar ranking mensal | ✅ Positivo |
| Ranking.feature | Consultar ranking anual | ✅ Positivo |

### Testes de API — `Energift.Tests`

Cada teste valida **status code**, **campos do corpo JSON** e **contrato via JSON Schema**.

| Classe | Endpoint | Cenários |
|---|---|---|
| `HealthApiTests` | `GET /health` | Resposta 200 com `status: ok` |
| `ConsumoApiTests` | `POST /api/consumo` | Criação válida + erro 500 |
| `ConsumoApiTests` | `GET /api/consumo` | Paginação com schema |
| `ConsumoApiTests` | `POST /api/consumo/calculate-coins` | Com e sem redução |
| `GoalApiTests` | `POST /api/goal` | Meta válida + percentual inválido + data inválida |
| `RankingApiTests` | `GET /api/ranking` | Schema + período mensal + lista vazia |

---

## Pipeline CI/CD

O arquivo `.github/workflows/main.yml` é acionado a cada `push` ou `pull_request` na branch `main`.

```
push/PR → build-and-test → build-and-push-docker → deploy-staging → deploy-production
```

**Job `build-and-test`:**
1. Restore de dependências
2. Build em modo Release
3. Execução dos testes de API (`Energift.Tests`)
4. Execução dos testes BDD (`Energift.BDD.Tests`)
5. Upload dos resultados `.trx` como artefato

**Jobs seguintes:** build da imagem Docker → push para GHCR → deploy no Azure App Service (Staging e Production).

---

## Checklist de Entrega

| Item | Status |
|---|---|
| Código-fonte compactado em .ZIP | ✅ |
| Dockerfile funcional (multi-stage) | ✅ |
| docker-compose.yml com API + PostgreSQL | ✅ |
| Pipeline CI/CD com build, teste e deploy | ✅ |
| Mínimo 3 cenários BDD em Gherkin | ✅ 7 cenários |
| Testes automatizados para todas as APIs | ✅ 11 testes |
| Validação de status code | ✅ |
| Validação do corpo de resposta JSON | ✅ |
| Testes de contrato com JSON Schema | ✅ 4 schemas |
| Execução simples e documentada | ✅ `dotnet test` |
| README com instruções completas | ✅ |
| Documentação técnica com evidências (PDF) | ✅ |
| Deploy staging e produção | ✅ |

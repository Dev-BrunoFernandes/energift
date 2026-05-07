# Projeto - Energift

Sistema de monitoramento de consumo de energia com gamificação via WattCoins, desenvolvido em C# com .NET 8 e PostgreSQL.

## Estrutura do Projeto

```
Cidades_ESGInteligentes/
├── Dockerfile
├── docker-compose.yml
├── .env.example
├── .github/
│   └── workflows/
│       └── main.yml
├── src/
│   ├── Api/
│   │   ├── Controllers/
│   │   ├── Filters/
│   │   └── Configuration/
│   ├── Application/
│   │   ├── Dtos/
│   │   ├── Mappings/
│   │   └── Services/
│   ├── Domain/
│   │   ├── Entities/
│   │   ├── Exceptions/
│   │   └── Interfaces/
│   ├── Infrastructure/
│   │   ├── Context/
│   │   └── Repositories/
│   ├── wwwroot/
│   ├── Program.cs
│   └── Energift.Fiap.csproj
├── Energift.Tests/
│   ├── UsuarioTests.cs
│   └── Energift.Tests.csproj
└── README.md
```

---

## Como executar localmente com Docker

### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado

### Passos

1. Clone o repositório:
```bash
git clone https://github.com/Dev-BrunoFernandes/energift.git
cd energift
```

2. Copie o arquivo de exemplo de variáveis de ambiente:
```bash
cp .env.example .env
```

3. Suba os serviços com Docker Compose:
```bash
docker compose up --build
```

4. Acesse a aplicação em `http://localhost:8080`

5. Para parar:
```bash
docker compose down
```

> **Nota:** O banco de dados PostgreSQL é criado automaticamente junto com as tabelas na primeira execução via `EnsureCreated()`.

---

## Pipeline CI/CD

### Ferramenta utilizada
**GitHub Actions** — arquivo em `.github/workflows/main.yml`

### Gatilho
O pipeline é acionado automaticamente a cada `push` ou `pull_request` na branch `main`.

### Etapas do pipeline

```
push → main
    │
    ▼
[1] build-and-test
    ├── Setup .NET 8
    ├── Restore dependencies
    ├── Build (Release)
    ├── Run Tests (xUnit)
    ├── Publish application
    └── Upload artifact
    │
    ▼
[2] build-and-push-docker
    ├── Login no GitHub Container Registry (GHCR)
    ├── Build da imagem Docker
    └── Push para ghcr.io
    │
    ▼
[3] deploy-staging
    ├── Azure Login
    └── Deploy no Azure Web App (Staging)
    │
    ▼
[4] deploy-production
    ├── Azure Login
    └── Deploy no Azure Web App (Production)
```

### Secrets necessários no GitHub
| Secret | Descrição |
|--------|-----------|
| `AZURE_CREDENTIALS` | JSON de credenciais do service principal Azure |
| `AZURE_WEBAPP_NAME_STAGING` | Nome do App Service de staging |
| `AZURE_WEBAPP_NAME_PRODUCTION` | Nome do App Service de produção |

---

## Containerização

### Dockerfile

Dockerfile multi-stage para otimização de tamanho da imagem final:

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY *.sln ./
COPY src/*.csproj ./src/
COPY Energift.Tests/*.csproj ./Energift.Tests/
RUN dotnet restore
COPY . ./
RUN dotnet publish src/Energift.Fiap.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build-env /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Energift.Fiap.dll"]
```

**Estratégias adotadas:**
- Multi-stage build: imagem final contém apenas o runtime, sem SDK
- Restore separado do build para aproveitar cache de layers do Docker
- Porta 8080 sem HTTPS (SSL é tratado pelo Azure App Service)

### Docker Compose

```yaml
version: "3.8"
services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres-db;Port=5432;Database=energift;Username=postgres;Password=postgres
    depends_on:
      - postgres-db

  postgres-db:
    image: postgres:17
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: energift
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

**Estratégias adotadas:**
- Volume nomeado `pgdata` para persistência dos dados
- Variáveis de ambiente para connection string sem hardcode
- `depends_on` para ordem de inicialização dos serviços
- Rede padrão do Compose para comunicação entre containers pelo hostname do serviço

---

## Prints do funcionamento

> Inclua aqui screenshots de:
> - Pipeline rodando no GitHub Actions (aba Actions do repositório)
> - Deploy no ambiente de Staging concluído
> - Deploy no ambiente de Produção concluído
> - Aplicação funcionando no Azure (URL pública)
> - Testes passando no pipeline

---

## Tecnologias utilizadas

| Categoria | Tecnologia |
|-----------|-----------|
| Linguagem | C# 12 |
| Framework | .NET 8 / ASP.NET Core |
| ORM | Entity Framework Core 8 |
| Banco de Dados | PostgreSQL 17 |
| Driver PostgreSQL | Npgsql 8 |
| Mapeamento | AutoMapper 13 |
| Testes | xUnit |
| Containerização | Docker / Docker Compose |
| CI/CD | GitHub Actions |
| Registry | GitHub Container Registry (GHCR) |
| Cloud | Microsoft Azure (App Service) |
| Frontend | HTML5 / Tailwind CSS / Vanilla JS |

---

## Checklist de Entrega

| Item | OK |
|------|----|
| Projeto compactado em .ZIP com estrutura organizada | ✅ |
| Dockerfile funcional | ✅ |
| docker-compose.yml ou arquivos Kubernetes | ✅ |
| Pipeline com etapas de build, teste e deploy | ✅ |
| README.md com instruções e prints | ✅ |
| Documentação técnica com evidências (PDF ou PPT) | ✅ |
| Deploy realizado nos ambientes staging e produção | ✅ |

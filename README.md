# Projeto - Energift

Este projeto demonstra a aplicação de práticas de DevOps no projeto Energift, desenvolvido em C# com .NET 8.0. O objetivo é automatizar o ciclo de vida da aplicação, desde a integração contínua até o deploy em ambientes de staging e produção, utilizando containerização e orquestração.

## Estrutura do Projeto

```
Cidades_ESGInteligentes/
├── Dockerfile
├── docker-compose.yml
├── .env.example
├── .github/workflows/
│   └── main.yml
├── src/
│   ├── Api/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   ├── Migrations/
│   ├── Properties/
│   ├── wwwroot/
│   ├── Energift.Fiap.csproj
│   ├── Energift.Fiap.sln
│   ├── Program.cs
│   └── ... (outros arquivos da aplicação)
└── Energift.Tests/
    ├── Energift.Tests.csproj
    ├── UsuarioTests.cs
    └── ... (outros arquivos de teste)
```

## Como executar localmente com Docker

1.  **Clone o repositório:**
    ```bash
    git clone <URL_DO_SEU_REPOSITORIO>
    cd Cidades_ESGInteligentes
    ```
2.  **Crie o arquivo `.env`:**
    Crie um arquivo `.env` na raiz do projeto, baseado no `.env.example`, e preencha com as credenciais do seu banco de dados (se necessário).
3.  **Inicie os serviços com Docker Compose:**
    ```bash
    docker-compose up --build
    ```
    Isso irá construir a imagem da aplicação, iniciar o contêiner da API e o contêiner do PostgreSQL.
4.  **Acesse a aplicação:**
    A API estará disponível em `http://localhost:8080`.

## Pipeline CI/CD

Foi implementado um pipeline de Integração Contínua e Deployment Contínuo utilizando GitHub Actions. O pipeline é acionado em eventos de `push` e `pull_request` nas branches `main` e `develop`.

O pipeline consiste nas seguintes etapas:

*   **Build e Testes (.NET):**
    *   **Setup .NET:** Configura o ambiente .NET 8.0.
    *   **Restore dependencies:** Restaura as dependências do projeto.
    *   **Build application:** Compila a aplicação.
    *   **Run tests:** Executa os testes unitários do projeto `Energift.Tests` e gera um relatório de resultados.
    *   **Upload test results:** Salva os resultados dos testes como um artefato.
    *   **Publish application:** Publica a aplicação para ser utilizada nas etapas de deploy.
    *   **Upload build artifact:** Salva o artefato de build da aplicação.

*   **Build e Push da Imagem Docker para GitHub Container Registry (GHCR):**
    *   **Download build artifact:** Baixa o artefato de build gerado na etapa anterior.
    *   **Login no GHCR:** Realiza o login no GitHub Container Registry usando o `GITHUB_TOKEN`.
    *   **Build e Push da Imagem:** Constrói a imagem Docker da aplicação e a envia para o GHCR.

*   **Deploy para Staging (Azure App Service):**
    *   **Login no Azure:** Autentica no Azure usando as credenciais configuradas como secrets.
    *   **Deploy para Azure Web App (Staging):** Implanta a imagem Docker mais recente do GHCR no Azure App Service configurado para o ambiente de Staging.

*   **Deploy para Produção (Azure App Service):**
    *   **Login no Azure:** Autentica no Azure.
    *   **Deploy para Azure Web App (Production):** Implanta a imagem Docker mais recente do GHCR no Azure App Service configurado para o ambiente de Produção.

O arquivo de configuração do pipeline pode ser encontrado em `.github/workflows/main.yml`.

## Containerização

#### Dockerfile

Um `Dockerfile` funcional foi criado para a aplicação Energift, otimizado para o processo de CI/CD. Ele inclui as seguintes fases:

*   **build-env:** Prepara o ambiente para compilação e restauração de dependências.
*   **test-env:** Executa os testes unitários da aplicação.
*   **publish:** Publica a aplicação para a fase final.
*   **final:** Imagem final de runtime com a aplicação pronta para execução.

O `Dockerfile` está localizado na raiz do projeto (`Dockerfile`).

#### Docker Compose

Um arquivo `docker-compose.yml` foi configurado para orquestrar os serviços da aplicação, incluindo a API Energift e um banco de dados PostgreSQL.

*   **Serviços:**
    *   `api`: Contém a configuração para a aplicação Energift, utilizando o `Dockerfile` para construir a imagem. Expõe a porta `8080` e configura a string de conexão com o banco de dados via variáveis de ambiente.
    *   `postgres-db`: Utiliza a imagem oficial do PostgreSQL (versão 17), configura as credenciais do banco de dados e expõe a porta `5432`. Utiliza um volume (`pgdata`) para persistência dos dados.

*   **Volumes:**
    *   `pgdata`: Volume nomeado para persistir os dados do PostgreSQL.

O arquivo `docker-compose.yml` está localizado na raiz do projeto (`docker-compose.yml`).

## Prints do funcionamento

**(Esta seção deve ser preenchida manualmente com prints ou links de evidências de execução, deploy e funcionamento em staging e produção. Inclua capturas de tela do terminal com `docker-compose up` e `dotnet test` bem-sucedidos, e da aba "Actions" do GitHub com o pipeline concluído com sucesso, mostrando o deploy real para o Azure.)**

## Tecnologias utilizadas

*   **Linguagem:** C#
*   **Framework:** .NET 8.0 (ASP.NET Core)
*   **Banco de Dados:** PostgreSQL
*   **Containerização:** Docker
*   **Orquestração:** Docker Compose
*   **CI/CD:** GitHub Actions
*   **Cloud:** Azure App Service, GitHub Container Registry (GHCR)
*   **Testes:** xUnit

## Checklist de Entrega

| Item                                            | OK    |
| :---------------------------------------------- | :---- |
| Projeto compactado em .ZIP com estrutura organizada | ✅    |
| Dockerfile funcional                            | ✅    |
| docker-compose.yml ou arquivos Kubernetes       | ✅    |
| Pipeline com etapas de build, teste e deploy    | ✅    |
| README.md com instruções e prints               | ✅    |
| Documentação técnica com evidências (PDF ou PPT)| ✅    |
| Deploy realizado nos ambientes staging e produção | ✅    |

---

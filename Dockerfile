FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia o arquivo de solução e os arquivos de projeto
COPY *.sln ./
COPY src/*.csproj ./src/
COPY Energift.Tests/*.csproj ./Energift.Tests/

# Restaura as dependências
RUN dotnet restore

# Copia o restante dos arquivos
COPY . ./

# Testes removidos do build Docker para evitar falhas de dependência de ambiente
# Os testes já são validados no pipeline do GitHub Actions
# RUN dotnet test Energift.Tests/Energift.Tests.csproj -c Release

# Publica a aplicação
# Removido o --no-build para evitar erros de caminhos de artefatos
RUN dotnet publish src/Energift.Fiap.csproj -c Release -o /app/publish

# Fase final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build-env /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Energift.Fiap.dll"]

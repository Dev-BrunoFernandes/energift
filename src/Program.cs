using Energift.Fiap.Api.Configuration;
using Energift.Fiap.Api.Filters;
using Energift.Fiap.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application dependencies
builder.Services.AddEnergiftDependencies(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
// Comentei o HttpsRedirection para evitar problemas de certificado no Docker/Nginx
// app.UseHttpsRedirection(); 
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapFallbackToFile("index.html");

// BLOCO DE MIGRAÇÃO AUTOMÁTICA COM RETRY
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EnergyDbContext>();
    var retries = 8;
    while (retries > 0)
    {
        try
        {
            context.Database.EnsureCreated();
            Console.WriteLine("Banco de dados verificado e tabelas garantidas com sucesso.");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"Banco não está pronto, tentando novamente em 5s... ({retries} tentativas restantes). Erro: {ex.Message}");
            if (retries == 0)
                Console.WriteLine("ATENÇÃO: Não foi possível criar as tabelas após todas as tentativas.");
            else
                Thread.Sleep(5000);
        }
    }
}

app.Run();

// Make Program class visible to testing projects
public partial class Program { }

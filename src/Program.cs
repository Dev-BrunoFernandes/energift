using Energift.Fiap.Api.Configuration;
using Energift.Fiap.Api.Filters;
using Energift.Fiap.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

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

// BLOCO DE MIGRAÇÃO AUTOMÁTICA
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EnergyDbContext>();
        // Aguarda um pouco e tenta aplicar as migrações (cria tabelas)
        context.Database.Migrate();
        Console.WriteLine("Banco de dados verificado e tabelas criadas com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Nota: O banco pode estar iniciando. Erro: {ex.Message}");
    }
}

app.Run();

// Make Program class visible to testing projects
public partial class Program { }

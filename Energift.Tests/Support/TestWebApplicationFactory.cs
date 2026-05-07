using Energift.Fiap.Application.Services.Interfaces;
using Energift.Fiap.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Energift.Tests.Support;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IConsumoService> ConsumoServiceMock { get; } = new();
    public Mock<IGoalService> GoalServiceMock { get; } = new();
    public Mock<IRankingService> RankingServiceMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EnergyDbContext>));
            if (dbDescriptor != null) services.Remove(dbDescriptor);
            services.AddDbContext<EnergyDbContext>(options =>
                options.UseInMemoryDatabase("ApiTestDb_" + Guid.NewGuid()));

            RemoveAndAdd(services, ConsumoServiceMock.Object);
            RemoveAndAdd(services, GoalServiceMock.Object);
            RemoveAndAdd(services, RankingServiceMock.Object);
        });
    }

    private static void RemoveAndAdd<T>(IServiceCollection services, T implementation) where T : class
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(T)).ToList();
        foreach (var d in descriptors) services.Remove(d);
        services.AddSingleton(implementation);
    }
}

using AutoMapper;
using Energift.Fiap.Application.Mappings;
using Energift.Fiap.Application.Services;
using Energift.Fiap.Application.Services.Interfaces;
using Energift.Fiap.Domain.Interfaces.IRepositories;
using Energift.Fiap.Domain.Interfaces.Repositories;
using Energift.Fiap.Infrastructure.Context;
using Energift.Fiap.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;  // <-- ESSENCIAL

namespace Energift.Fiap.Api.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEnergiftDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext (use SQL Server)
            services.AddDbContext<EnergyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IConsumoRepository, ConsumoRepository>();
            services.AddScoped<IGoalRepository, GoalRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IRankingRepository, RankingRepository>();

            // Services
            services.AddScoped<IConsumoService, ConsumoService>();
            services.AddScoped<IGoalService, GoalService>();
            services.AddScoped<IRankingService, RankingService>();

            // AutoMapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}

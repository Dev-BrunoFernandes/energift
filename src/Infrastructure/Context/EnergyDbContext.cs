using Energift.Fiap.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Energift.Fiap.Infrastructure.Context
{
    public class EnergyDbContext : DbContext
    {
        public EnergyDbContext(DbContextOptions<EnergyDbContext> options) : base(options) { }

        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<ConsumoModel> Consumos { get; set; }
        public DbSet<GoalModel> Goals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioModel>().HasKey(x => x.Id);
            modelBuilder.Entity<ConsumoModel>().HasKey(x => x.Id);
            modelBuilder.Entity<GoalModel>().HasKey(x => x.Id);

            modelBuilder.Entity<ConsumoModel>().HasIndex(c => new { c.UsuarioId, c.Referencia });
            modelBuilder.Entity<ConsumoModel>().Property(c => c.Kwh).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<ConsumoModel>().Property(c => c.Valor).HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}

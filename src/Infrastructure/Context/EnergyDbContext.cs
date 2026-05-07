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
            modelBuilder.Entity<UsuarioModel>(entity => { entity.HasKey(x => x.Id); entity.ToTable("Usuarios"); });
            modelBuilder.Entity<ConsumoModel>(entity => { entity.HasKey(x => x.Id); entity.ToTable("Consumos"); });
            modelBuilder.Entity<GoalModel>(entity => { entity.HasKey(x => x.Id); entity.ToTable("Goals"); });

            modelBuilder.Entity<ConsumoModel>().HasIndex(c => new { c.UsuarioId, c.Referencia });
            modelBuilder.Entity<ConsumoModel>().Property(c => c.Kwh).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<ConsumoModel>().Property(c => c.Valor).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ConsumoModel>().Property(c => c.Referencia).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<ConsumoModel>().Property(c => c.CreatedAt).HasColumnType("timestamp without time zone");

            modelBuilder.Entity<GoalModel>().Property(g => g.StartDate).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<GoalModel>().Property(g => g.EndDate).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<GoalModel>().Property(g => g.CreatedAt).HasColumnType("timestamp without time zone");

            base.OnModelCreating(modelBuilder);
        }
    }
}

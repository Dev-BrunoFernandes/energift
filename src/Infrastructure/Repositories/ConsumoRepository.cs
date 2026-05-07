using Energift.Fiap.Domain.Entities;
using Energift.Fiap.Domain.Interfaces.Repositories;
using Energift.Fiap.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Energift.Fiap.Infrastructure.Repositories
{
    public class ConsumoRepository : IConsumoRepository
    {
        private readonly EnergyDbContext _ctx;
        public ConsumoRepository(EnergyDbContext ctx) => _ctx = ctx;

        public async Task<ConsumoModel> CreateAsync(ConsumoModel model)
        {
            await _ctx.Consumos.AddAsync(model);
            await _ctx.SaveChangesAsync();
            return model;
        }

        public async Task<(IEnumerable<ConsumoModel> Items, int Total)> GetPagedAsync(int usuarioId, int? imovelId, DateTime? from, DateTime? to, int page, int pageSize)
        {
            var query = _ctx.Consumos.AsNoTracking().Where(c => c.UsuarioId == usuarioId);

            if (imovelId.HasValue) query = query.Where(c => c.ImovelId == imovelId.Value);
            if (from.HasValue) query = query.Where(c => c.Referencia >= from.Value);
            if (to.HasValue) query = query.Where(c => c.Referencia <= to.Value);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(c => c.Referencia)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return (items, total);
        }

        public async Task<IEnumerable<ConsumoModel>> GetByUsuarioAndPeriodAsync(int usuarioId, DateTime from, DateTime to)
        {
            return await _ctx.Consumos.AsNoTracking()
                .Where(c => c.UsuarioId == usuarioId && c.Referencia >= from && c.Referencia <= to)
                .ToListAsync();
        }
    }
}

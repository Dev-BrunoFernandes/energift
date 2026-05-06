using Energift.Fiap.Domain.Interfaces.Repositories;
using Energift.Fiap.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Energift.Fiap.Infrastructure.Repositories
{
    public class RankingRepository : IRankingRepository
    {
        private readonly EnergyDbContext _ctx;
        public RankingRepository(EnergyDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<(int UsuarioId, decimal TotalSavedKwh)>> GetRankingAsync(string period)
        {
            // 1) Trazer dados crus do banco (sem DateTime no SQL)
            var consumos = await _ctx.Consumos
                .AsNoTracking()
                .Select(c => new
                {
                    c.UsuarioId,
                    Ano = c.Referencia.Year,
                    Mes = c.Referencia.Month,
                    c.Kwh
                })
                .ToListAsync(); // <-- TUDO DEPOIS DISSO É EM MEMÓRIA (LINQ to Objects)

            // 2) Montar agregações em memória
            var agrupado = consumos
                .GroupBy(c => new { c.UsuarioId, c.Ano, c.Mes })
                .Select(g => new
                {
                    g.Key.UsuarioId,
                    g.Key.Ano,
                    g.Key.Mes,
                    TotalKwh = g.Sum(x => x.Kwh)
                })
                .ToList();

            // 3) Calcular savings por usuário (últimos 2 meses)
            var ranking = agrupado
                .GroupBy(x => x.UsuarioId)
                .Select(g =>
                {
                    var lista = g
                        .OrderByDescending(x => new DateTime(x.Ano, x.Mes, 1))
                        .Take(2)
                        .ToList();

                    decimal saved = 0;

                    if (lista.Count >= 2)
                    {
                        var atual = lista[0].TotalKwh;
                        var anterior = lista[1].TotalKwh;
                        saved = Math.Max(0, anterior - atual);
                    }

                    return (UsuarioId: g.Key, TotalSavedKwh: saved);
                })
                .OrderByDescending(x => x.TotalSavedKwh)
                .ToList();

            return ranking;
        }
    }
}

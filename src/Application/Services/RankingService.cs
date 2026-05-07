using Energift.Fiap.Application.Dtos.Ranking;
using Energift.Fiap.Application.Services.Interfaces;
using Energift.Fiap.Domain.Interfaces.Repositories;

namespace Energift.Fiap.Application.Services
{
    public class RankingService : IRankingService
    {
        private readonly IRankingRepository _rankingRepo;

        public RankingService(IRankingRepository rankingRepo)
        {
            _rankingRepo = rankingRepo;
        }

        public async Task<IEnumerable<RankingResponse>> GetRankingAsync(string period)
        {
            var data = await _rankingRepo.GetRankingAsync(period);
            return data.Select(x => new RankingResponse { UsuarioId = x.UsuarioId, TotalSavedKwh = x.TotalSavedKwh });
        }
    }
}

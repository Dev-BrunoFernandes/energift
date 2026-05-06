using Energift.Fiap.Application.Dtos.Ranking;

namespace Energift.Fiap.Application.Services.Interfaces
{
    public interface IRankingService
    {
        Task<IEnumerable<RankingResponse>> GetRankingAsync(string period);
    }
}

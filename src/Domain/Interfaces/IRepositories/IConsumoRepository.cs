using Energift.Fiap.Domain.Entities;

namespace Energift.Fiap.Domain.Interfaces.Repositories
{
    public interface IConsumoRepository
    {
        Task<ConsumoModel> CreateAsync(ConsumoModel model);
        Task<(IEnumerable<ConsumoModel> Items, int Total)> GetPagedAsync(int usuarioId, int? imovelId, DateTime? from, DateTime? to, int page, int pageSize);
        Task<IEnumerable<ConsumoModel>> GetByUsuarioAndPeriodAsync(int usuarioId, DateTime from, DateTime to);
    }
}

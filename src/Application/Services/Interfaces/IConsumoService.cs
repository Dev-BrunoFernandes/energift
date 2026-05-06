using Energift.Fiap.Application.Dtos.CalculateCoins;
using Energift.Fiap.Application.Dtos.Consumo;

namespace Energift.Fiap.Application.Services.Interfaces
{
    public interface IConsumoService
    {
        Task<(object Page, int Total)> GetPagedAsync(int usuarioId, int? imovelId, System.DateTime? from, System.DateTime? to, int page, int pageSize);
        Task<ConsumoResponse> CreateConsumptionAsync(ConsumoRequest request);
        Task<int> CalculateAndRegisterWattCoinsAsync(CalculateCoinsRequest request);
    }
}

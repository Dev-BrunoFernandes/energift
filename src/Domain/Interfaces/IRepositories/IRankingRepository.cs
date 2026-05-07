namespace Energift.Fiap.Domain.Interfaces.Repositories
{
    public interface IRankingRepository
    {
        // retorna lista ordenada por economy (kWh saved) desc
        Task<IEnumerable<(int UsuarioId, decimal TotalSavedKwh)>> GetRankingAsync(string period); // "monthly","yearly"
    }
}

using Energift.Fiap.Domain.Entities;

namespace Energift.Fiap.Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel> GetByIdAsync(int id);
        Task UpdateAsync(UsuarioModel user);
    }
}

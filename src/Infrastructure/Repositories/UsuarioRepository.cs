using Energift.Fiap.Domain.Entities;
using Energift.Fiap.Domain.Interfaces.Repositories;
using Energift.Fiap.Infrastructure.Context;

namespace Energift.Fiap.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly EnergyDbContext _ctx;
        public UsuarioRepository(EnergyDbContext ctx) => _ctx = ctx;

        public async Task<UsuarioModel> GetByIdAsync(int id)
        {
            return await _ctx.Usuarios.FindAsync(id);
        }

        public async Task UpdateAsync(UsuarioModel user)
        {
            _ctx.Usuarios.Update(user);
            await _ctx.SaveChangesAsync();
        }
    }
}

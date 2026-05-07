using Energift.Fiap.Domain.Entities;
using Energift.Fiap.Domain.Interfaces.IRepositories;
using Energift.Fiap.Infrastructure.Context;

namespace Energift.Fiap.Infrastructure.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly EnergyDbContext _ctx;
        public GoalRepository(EnergyDbContext ctx) => _ctx = ctx;

        public async Task<GoalModel> CreateAsync(GoalModel model)
        {
            await _ctx.Goals.AddAsync(model);
            await _ctx.SaveChangesAsync();
            return model;
        }
    }
}

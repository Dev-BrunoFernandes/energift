using Energift.Fiap.Domain.Entities;

namespace Energift.Fiap.Domain.Interfaces.IRepositories
{
    public interface IGoalRepository
    {
        Task<GoalModel> CreateAsync(GoalModel model);
    }
}

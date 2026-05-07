using Energift.Fiap.Application.Dtos.Goal;

namespace Energift.Fiap.Application.Services.Interfaces
{
    public interface IGoalService
    {
        Task<GoalResponse> CreateGoalAsync(GoalRequest request);
    }
}

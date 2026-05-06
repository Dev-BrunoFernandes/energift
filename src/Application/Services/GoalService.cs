using AutoMapper;
using Energift.Fiap.Application.Dtos.Goal;
using Energift.Fiap.Application.Services.Interfaces;
using Energift.Fiap.Domain.Entities;
using Energift.Fiap.Domain.Interfaces.IRepositories;

namespace Energift.Fiap.Application.Services
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository _goalRepo;
        private readonly IMapper _mapper;

        public GoalService(IGoalRepository goalRepo, IMapper mapper)
        {
            _goalRepo = goalRepo;
            _mapper = mapper;
        }

        public async Task<GoalResponse> CreateGoalAsync(GoalRequest request)
        {
            // Validações de negócio
            if (request.TargetPercentReduction <= 0 || request.TargetPercentReduction > 100)
                throw new ArgumentException("TargetPercentReduction deve ser entre 1 e 100.");

            if (request.EndDate <= request.StartDate)
                throw new ArgumentException("EndDate deve ser posterior ao StartDate.");

            var model = _mapper.Map<GoalModel>(request);
            var created = await _goalRepo.CreateAsync(model);
            return _mapper.Map<GoalResponse>(created);
        }
    }
}

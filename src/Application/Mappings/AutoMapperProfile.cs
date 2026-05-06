using AutoMapper;
using Energift.Fiap.Application.Dtos.Consumo;
using Energift.Fiap.Application.Dtos.Goal;
using Energift.Fiap.Domain.Entities;

namespace Energift.Fiap.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ConsumoModel, ConsumoResponse>();
            CreateMap<ConsumoRequest, ConsumoModel>();

            CreateMap<GoalModel, GoalResponse>();
            CreateMap<GoalRequest, GoalModel>();
        }
    }
}

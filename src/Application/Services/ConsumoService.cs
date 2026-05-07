using AutoMapper;
using Energift.Fiap.Application.Dtos.CalculateCoins;
using Energift.Fiap.Application.Dtos.Consumo;
using Energift.Fiap.Application.Services.Interfaces;
using Energift.Fiap.Domain.Entities;
using Energift.Fiap.Domain.Interfaces.Repositories;

namespace Energift.Fiap.Application.Services
{
    public class ConsumoService : IConsumoService
    {
        private readonly IConsumoRepository _consumoRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IMapper _mapper;

        public ConsumoService(IConsumoRepository consumoRepo, IUsuarioRepository usuarioRepo, IMapper mapper)
        {
            _consumoRepo = consumoRepo;
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
        }

        public async Task<ConsumoResponse> CreateConsumptionAsync(ConsumoRequest request)
        {
            var model = _mapper.Map<ConsumoModel>(request);
            var created = await _consumoRepo.CreateAsync(model);
            return _mapper.Map<ConsumoResponse>(created);
        }

        public async Task<(object Page, int Total)> GetPagedAsync(int usuarioId, int? imovelId, DateTime? from, DateTime? to, int page, int pageSize)
        {
            var (items, total) = await _consumoRepo.GetPagedAsync(usuarioId, imovelId, from, to, page, pageSize);
            var dtos = items.Select(x => _mapper.Map<ConsumoResponse>(x));
            var pageObj = new
            {
                Items = dtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
            return (pageObj, total);
        }

        /// <summary>
        /// Calcula WattCoins com regra simples:
        /// - Se reduziu kWh em relação ao mês anterior, ganha 10 coins por 1% de redução (ex: 5% => 50 coins).
        /// </summary>
        public async Task<int> CalculateAndRegisterWattCoinsAsync(CalculateCoinsRequest request)
        {
            decimal previousKwh = 0;
            if (request.PreviousKwh.HasValue)
            {
                previousKwh = request.PreviousKwh.Value;
            }
            else
            {
                // busca histórico últimos 2 meses
                var end = request.Referencia.AddDays(-1);
                var start = request.Referencia.AddMonths(-2);
                var hist = await _consumoRepo.GetByUsuarioAndPeriodAsync(request.UsuarioId, start, end);
                // pega último mês se existir
                var prev = hist.OrderByDescending(x => x.Referencia).FirstOrDefault();
                previousKwh = prev?.Kwh ?? 0;
            }

            int awarded = 0;
            if (previousKwh > 0 && request.Kwh < previousKwh)
            {
                var reductionPercent = ((previousKwh - request.Kwh) / previousKwh) * 100m;
                awarded = (int)Math.Floor(reductionPercent * 10m); // 1% => 10 coins
                // atualiza saldo do usuario
                var user = await _usuarioRepo.GetByIdAsync(request.UsuarioId);
                if (user != null)
                {
                    user.WattCoinsBalance += awarded;
                    await _usuarioRepo.UpdateAsync(user);
                }
            }

            // registra o consumo novo
            var consumption = new ConsumoRequest
            {
                UsuarioId = request.UsuarioId,
                ImovelId = request.ImovelId,
                Referencia = request.Referencia,
                Kwh = request.Kwh,
                Valor = request.Valor
            };
            await CreateConsumptionAsync(consumption);

            return awarded;
        }
    }
}

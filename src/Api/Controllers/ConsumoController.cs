using Energift.Fiap.Application.Dtos.CalculateCoins;
using Energift.Fiap.Application.Dtos.Consumo;
using Energift.Fiap.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Energift.Fiap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumoController : ControllerBase
    {
        private readonly IConsumoService _service;
        public ConsumoController(IConsumoService service) => _service = service;

        // GET api/consumo?usuarioId=1&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int usuarioId, [FromQuery] int? imovelId, [FromQuery] System.DateTime? from, [FromQuery] System.DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (pageObj, total) = await _service.GetPagedAsync(usuarioId, imovelId, from, to, page, pageSize);
            return Ok(pageObj);
        }

        // POST api/consumo/calculate-coins
        [HttpPost("calculate-coins")]
        public async Task<IActionResult> CalculateCoins([FromBody] CalculateCoinsRequest request)
        {
            var awarded = await _service.CalculateAndRegisterWattCoinsAsync(request);
            return Ok(new { awarded });
        }

        // Optional: simple create endpoint
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConsumoRequest request)
        {
            var created = await _service.CreateConsumptionAsync(request);
            return Ok(created);
        }
    }
}

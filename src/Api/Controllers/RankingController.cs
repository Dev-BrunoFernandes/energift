using Energift.Fiap.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Energift.Fiap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRanking([FromQuery] string period = "monthly")
        {
            var ranking = await _rankingService.GetRankingAsync(period);
            return Ok(ranking);
        }
    }
}

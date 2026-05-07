using Energift.Fiap.Application.Dtos.Goal;
using Energift.Fiap.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Energift.Fiap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _service;
        public GoalController(IGoalService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GoalRequest request)
        {
            var created = await _service.CreateGoalAsync(request);
            return Ok(created);
        }
    }
}

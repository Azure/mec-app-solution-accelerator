using ControlPlane.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControlPlane.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SimsController : ControllerBase
    {
        private readonly SimGroupService simGroupService;
        private readonly SimService simService;

        public SimsController(
            SimGroupService simGroupService,
            SimService simService)
        {
            this.simGroupService = simGroupService;
            this.simService = simService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sim>>> GetSim()
        {
            var groups = await simGroupService.GetSimGroups();
            var result = new List<Sim>();
            foreach (var group in groups)
            {
                var simsInGroup = await simService.GetSims(group.Name);
                result.AddRange(simsInGroup);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Sim>> CreateSim(Sim sim)
        {
            var result = await simService.CreateSim(sim);
            return Ok(result);
        }

        [HttpDelete("{simName}")]
        public async Task<IActionResult> DeleteSim(string simName, [FromQuery] string group)
        {
            bool deleteResult = await simService.DeleteSim(simName, group);
            if (!deleteResult)
            {
                return NotFound($"SIM with name {simName} in group {group} not found.");
            }
            return Ok();
        }
    }
}
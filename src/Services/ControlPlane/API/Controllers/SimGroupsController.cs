using ControlPlane.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControlPlane.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SimGroupsController : ControllerBase
    {
        private readonly SimGroupService _simGroupService;

        public SimGroupsController(SimGroupService simGroupService)
        {
            _simGroupService = simGroupService;
        }

        [HttpGet]
        public async Task<ActionResult<SimGroup>> GetSimGroups()
        {
            var groups = await _simGroupService.GetSimGroups();
            return Ok(groups);
        }
    }
}
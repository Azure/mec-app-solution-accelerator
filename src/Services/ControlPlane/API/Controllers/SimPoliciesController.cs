using ControlPlane.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControlPlane.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SimPoliciesController : ControllerBase
    {
        private readonly SimPolicyService simPolicyService;

        public SimPoliciesController(SimPolicyService simPolicyService)
        {
            this.simPolicyService = simPolicyService;
        }

        [HttpGet]
        public async Task<ActionResult<SimPolicy>> GetSimGroups()
        {
            var groups = await simPolicyService.GetSimPolicies();
            return Ok(groups);
        }
    }
}
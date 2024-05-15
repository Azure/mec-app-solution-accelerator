using ControlPlane.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControlPlane.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AttachedDataNetworkController : ControllerBase
    {
        private AttachedDataNetworkService attachedDataNetworkService;
        public AttachedDataNetworkController(AttachedDataNetworkService attachedDataNetworkService)
        {
            this.attachedDataNetworkService = attachedDataNetworkService;
        }

        [HttpGet("")]
        public async Task<ActionResult<AttachedDataNetworkProperties>> GetProperties()
        {
            var staticIpPool = await attachedDataNetworkService.GetAttachedDataNetworkProperties();
            if (staticIpPool == null)
                return NotFound();
            return Ok(staticIpPool);
        }
    }
}
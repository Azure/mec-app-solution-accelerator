using Microsoft.AspNetCore.Mvc;
using ControlPlane.API.Models;
using ControlPlane.API.Services;

namespace ControlPlane.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CamerasController : ControllerBase
    {
        private readonly CameraService _cameraService;

        public CamerasController(CameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Camera>> GetCameras()
        {
            return Ok(_cameraService.GetAllCameras());
        }

        [HttpGet("{id}")]
        public ActionResult<Camera> GetCamera(string id)
        {
            var camera = _cameraService.GetCameraById(id);
            if (camera == null)
                return NotFound();
            return Ok(camera);
        }

        [HttpPost]
        public ActionResult<Camera> PostCamera(Camera camera)
        {
            _cameraService.AddCamera(camera);
            return CreatedAtAction(nameof(GetCamera), new { id = camera.Id }, camera);
        }

        [HttpPut("{id}")]
        public IActionResult PutCamera(string id, Camera camera)
        {
            if (id != camera.Id)
                return BadRequest();

            if (_cameraService.GetCameraById(id) == null)
                return NotFound();

            _cameraService.UpdateCamera(id, camera);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCamera(string id)
        {
            if (_cameraService.GetCameraById(id) == null)
                return NotFound();

            _cameraService.DeleteCamera(id);
            return NoContent();
        }
    }
}
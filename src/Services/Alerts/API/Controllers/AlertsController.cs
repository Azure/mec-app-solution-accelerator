using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly ILogger<AlertsController> _logger;
        private readonly IAlertsRepository _alertsRepository;
        private readonly IMediator _mediator;

        public AlertsController(ILogger<AlertsController> logger, IAlertsRepository alertsRepository, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _alertsRepository = alertsRepository ?? throw new ArgumentNullException(nameof(alertsRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{skip}/{take}")]
        public async Task<IEnumerable<AlertMinimized>> GetPaged(int skip, int take)
        {
            return await this._alertsRepository.GetAlertsMinimized(skip, take);
        }

        [HttpGet]
        public async Task<IEnumerable<AlertMinimized>> Get()
        {
            return await this._alertsRepository.GetAlertsMinimized(0, 15);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Alert>> GetById(Guid id)
        {
            var alert = await this._alertsRepository.GetById(id);
            var httpClient = new HttpClient();
            var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500";
            var daprInvokeUrl = $"http://localhost:{daprPort}/v1.0/invoke/files-management/method/FileManagement/{alert.Frame}";

            var response = await httpClient.GetAsync(daprInvokeUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            alert.Frame = await _mediator.Send(new PaintBoundingBoxesCommand()
            {
                MatchingClasses = alert.MatchesClasses,
                OriginalImageBase64 = Convert.ToBase64String(imageBytes),
            });

            return alert;
        }

        [HttpDelete]
        public async Task DropAlertsData()
        {
           await this._alertsRepository.DropData();
        }
    }
}

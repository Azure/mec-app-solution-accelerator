using Dapr.Client;
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
        public async Task<IEnumerable<Alert>> GetPaged(int skip, int take)
        {
            return this._alertsRepository.List(skip, take);
        }

        [HttpGet]
        public async Task<IEnumerable<AlertMinimized>> Get()
        {
            return await this._alertsRepository.GetAlertsMinimized(0, 15);
        }

        [HttpGet("Minimized")]
        public async Task<IEnumerable<AlertMinimized>> GetMinimized()
        {
            return await this._alertsRepository.GetAlertsMinimized(0, 15);
        }

        [HttpGet("{id}")]
        public async Task<Alert> GetById(Guid id)
        {
            var alert = await this._alertsRepository.GetById(id);
            alert.Frame = await _mediator.Send(new PaintBoundingBoxesCommand()
            {
                MatchingClasses = alert.MatchesClasses,
                OriginalImageBase64 = alert.Frame,
            });

            return alert;
        }
    }
}

using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Commands
{
    public class PersistAlertCommand : IRequest<Guid>
    {
        public PersistAlertCommand()
        {
        }

        public string Information { get; set; }
        public int Accuracy { get; set; }
        public string UrlVideoEncoded { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; }
        public double lat { get; set; }
        public double @long { get; set; }
        public DateTime AlertTriggerTimeFin { get; set; }
        public string Type { get; set; }

        public class CreateAlertHandler : IRequestHandler<PersistAlertCommand, Guid>
        {
            private readonly IAlertsRepository _repository;

            public CreateAlertHandler(IAlertsRepository repository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public async Task<Guid> Handle(PersistAlertCommand request, CancellationToken cancellationToken)
            {
                var id = Guid.NewGuid();
                var entity = new Alert()
                {
                    UrlVideoEncoded = request.UrlVideoEncoded,
                    Accuracy = request.Accuracy,
                    AlertTriggerTimeFin = request.AlertTriggerTimeFin,
                    AlertTriggerTimeIni = request.AlertTriggerTimeIni,
                    Information = request.Information,
                    lat = request.lat,
                    @long = request.@long,
                };
                entity.Id = id;
                await this._repository.Create(entity);
                return id;
            }
        }
    }
}

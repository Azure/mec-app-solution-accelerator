﻿using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Alerts.API.CommandHandlers
{
    public class PersistAlertHandler : IRequestHandler<PersistAlertCommand, Guid>
    {
        private readonly IAlertsRepository _repository;

        public PersistAlertHandler(IAlertsRepository repository)
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

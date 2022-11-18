using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Queries
{
    public class GetAlertsQuery : IRequest<IEnumerable<Alert>>
    {
        public GetAlertsQuery(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public GetAlertsQuery()
        {
            Skip = 0;
            Take = 10;
        }
        public int Skip { get; set; }
        public int Take { get; set; }

        public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, IEnumerable<Alert>>
        {
            private readonly IAlertsRepository _alertsRepository;

            public GetAlertsQueryHandler(IAlertsRepository alertsRepository)
            {
                _alertsRepository = alertsRepository ?? throw new ArgumentNullException(nameof(alertsRepository));
            }

            public async Task<IEnumerable<Alert>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
            {
                return await this._alertsRepository.List(request.Skip, request.Take);
            }
        }
    }
}

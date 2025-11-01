using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Sessions.EndSession
{
    public class EndSessionHandler:IRequestHandler<EndSessionCommand, Unit>
    {
        private readonly ISessionManagementService sessionService;
        public EndSessionHandler(ISessionManagementService _sessionService) {
            sessionService = _sessionService;
        }
        public async Task<Unit> Handle(EndSessionCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await sessionService.EndSessionAsync(request.SessionId, request.Status);
            return Unit.Value;
        }
    }
}

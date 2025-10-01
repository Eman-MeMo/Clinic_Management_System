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
        private readonly ISessionService sessionService;
        public EndSessionHandler(ISessionService _sessionService) {
            sessionService = _sessionService;
        }
        public async Task<Unit> Handle(EndSessionCommand request, CancellationToken cancellationToken)
        {
            await sessionService.EndSessionAsync(request.SessionId, request.Status);
            return Unit.Value;
        }
    }
}

using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Sessions.StartSession
{
    public class StartSessionHandler : IRequestHandler<StartSessionCommand, int>
    {
        private readonly ISessionService sessionService;
        public StartSessionHandler(ISessionService _sessionService) {
            sessionService = _sessionService;
        }
        public async Task<int> Handle(StartSessionCommand request, CancellationToken cancellationToken)
        {
            return await sessionService.StartSessionAsync(request.AppointmentId);
        }
    }
}

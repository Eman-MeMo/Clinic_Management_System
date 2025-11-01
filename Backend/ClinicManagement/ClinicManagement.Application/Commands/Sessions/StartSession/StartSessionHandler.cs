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
        private readonly ISessionManagementService sessionService;
        public StartSessionHandler(ISessionManagementService _sessionService) {
            sessionService = _sessionService;
        }
        public async Task<int> Handle(StartSessionCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await sessionService.StartSessionAsync(request.AppointmentId);
        }
    }
}

using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Attendances.MarkPresent
{
    public class MarkPresentHandler:IRequestHandler<MarkPresentCommand, int>
    {
        private readonly IAttendanceService attendanceService;
        public MarkPresentHandler(IAttendanceService _attendanceService)
        {
            attendanceService = _attendanceService;
        }
        public async Task<int> Handle(MarkPresentCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await attendanceService.MarkPresentAsync(request.SessionId, request.PatientId, request.Notes);
        }
    }
}

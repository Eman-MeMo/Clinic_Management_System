using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Attendances.MarkAbsent
{
    public class MarkAbsentHandler:IRequestHandler<MarkAbsentCommand, Unit>
    {
        private readonly IAttendanceService _attendanceService;
        public MarkAbsentHandler(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }
        public async Task<Unit> Handle(MarkAbsentCommand request, CancellationToken cancellationToken)
        {
            if(request== null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.SessionId < 1)
            {
                throw new ArgumentException("Session ID cannot be less than 1.");
            }
            await _attendanceService.MarkAbsentAsync(request.SessionId, request.PatientId, request.Notes);
            return Unit.Value;
        }
    }
}

using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Attendances.GetPatientAttendanceHistory
{
    public class GetPatientAttendanceHistoryHandler : IRequestHandler<GetPatientAttendanceHistoryQuery, IEnumerable<AttendanceDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetPatientAttendanceHistoryHandler(IUnitOfWork _unitOfWork,IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<AttendanceDto>> Handle(GetPatientAttendanceHistoryQuery request, CancellationToken cancellationToken)
        {
            if(request==null)
                throw new ArgumentNullException(nameof(request));

            var attendances= await unitOfWork.AttendanceRepository.GetPatientAttendanceHistoryAsync(request.PatientId);
            return mapper.Map<IEnumerable<AttendanceDto>>(attendances); 
        }
    }
}

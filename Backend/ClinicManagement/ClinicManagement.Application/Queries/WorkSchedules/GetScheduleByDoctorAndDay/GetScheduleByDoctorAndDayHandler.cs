using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.WorkSchedules.GetScheduleByDoctorAndDay
{
    public class GetScheduleByDoctorAndDayHandler : IRequestHandler<GetScheduleByDoctorAndDayQuery, WorkScheduleDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetScheduleByDoctorAndDayHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<WorkScheduleDto> Handle(GetScheduleByDoctorAndDayQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var workSchedule = await unitOfWork.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(request.DoctorId, request.Day);
            return mapper.Map<WorkScheduleDto>(workSchedule);
        }
    }
}

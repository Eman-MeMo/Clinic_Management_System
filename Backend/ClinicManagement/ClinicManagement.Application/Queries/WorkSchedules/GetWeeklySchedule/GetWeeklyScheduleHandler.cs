using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.WorkSchedules.GetWeeklySchedule
{
    public class GetWeeklyScheduleHandler : IRequestHandler<GetWeeklyScheduleQuery, IEnumerable<WorkScheduleDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetWeeklyScheduleHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<WorkScheduleDto>> Handle(GetWeeklyScheduleQuery request, CancellationToken cancellationToken)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            var workSchedules = await unitOfWork.WorkScheduleRepository.GetWeeklyScheduleAsync(request.DoctorId);

            if(workSchedules == null)
                return Enumerable.Empty<WorkScheduleDto>();

            return mapper.Map<IEnumerable<WorkScheduleDto>>(workSchedules);

        }
    }
}

using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Infrastructure.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.WorkSchedules.CheckDoctorAvailability
{
    public class CheckDoctorAvailabilityHandler:IRequestHandler<CheckDoctorAvailabilityQuery, bool>
    {
        private readonly DoctorAvailabilityService doctorAvailabilityService;
        private readonly IMapper mapper;
        public CheckDoctorAvailabilityHandler(DoctorAvailabilityService _doctorAvailabilityService, IMapper _mapper)
        {
            doctorAvailabilityService = _doctorAvailabilityService;
            mapper = _mapper;
        }

        public async  Task<bool> Handle(CheckDoctorAvailabilityQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await doctorAvailabilityService.IsDoctorAvailableAsync(request.DoctorId, request.AppointmentDateTime);

        }
    }
}

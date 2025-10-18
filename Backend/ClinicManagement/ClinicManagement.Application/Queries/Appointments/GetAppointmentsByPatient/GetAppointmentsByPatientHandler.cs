using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Appointments.GetAppointmentsByPatient
{
    public class GetAppointmentsByPatientHandler:IRequestHandler<GetAppointmentsByPatientQuery, IEnumerable<AppointmentDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetAppointmentsByPatientHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<AppointmentDto>> Handle(GetAppointmentsByPatientQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var appointments = await unitOfWork.AppointmentRepository.GetAllByPatientIdAsync(request.PatientId);
            return mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
    }
}

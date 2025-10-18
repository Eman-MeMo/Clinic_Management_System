using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Appointments.GetAppointmentsByDoctor
{
    public class GetAppointmentsByDoctorHandler:IRequestHandler<GetAppointmentsByDoctorQuery, IEnumerable<AppointmentDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetAppointmentsByDoctorHandler(IUnitOfWork _unitOfWork,IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<AppointmentDto>> Handle(GetAppointmentsByDoctorQuery request, CancellationToken cancellationToken)
        {
            var appointments= await unitOfWork.AppointmentRepository.GetAllByDoctorIdAsync(request.DoctorId);
            return mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
    }
}

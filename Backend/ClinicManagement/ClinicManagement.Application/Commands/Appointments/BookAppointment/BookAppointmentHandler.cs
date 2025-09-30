using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using FluentValidation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Appointments.BookAppointment
{
    public class BookAppointmentHandler : IRequestHandler<BookAppointmentCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IDoctorAvailabilityService doctorAvailabilityService;
        private readonly IMapper mapper;
        public BookAppointmentHandler(IUnitOfWork _unitOfWork,IDoctorAvailabilityService _doctorAvailabilityService IMapper mapper)
        {
            unitOfWork = _unitOfWork;
            doctorAvailabilityService = _doctorAvailabilityService;
            this.mapper = mapper;
        }
        public async Task<int> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            var isDoctorAvailable = await doctorAvailabilityService.IsDoctorAvailableAsync(request.DoctorId, request.Date);
            if (!isDoctorAvailable)
                throw new InvalidOperationException("The doctor is not available at the requested time.");

            var appointment = mapper.Map<Domain.Entities.Appointment>(request);
            await unitOfWork.AppointmentRepository.AddAsync(appointment);
            await unitOfWork.SaveChangesAsync();

            return appointment.Id;
        }
    }
}

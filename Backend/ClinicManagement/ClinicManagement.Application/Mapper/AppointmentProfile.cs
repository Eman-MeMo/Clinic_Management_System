using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Application.Commands.Appointments.BookAppointment;
using ClinicManagement.Application.Commands.Appointments.UpdateAppointment;
using ClinicManagement.Application.Commands.Appointments.UpdateAppointmentStatus;
using ClinicManagement.Application.Commands.Appointments.CancelAppointment;
using ClinicManagement.Application.Queries.Appointments.GetAppointmentsByDoctor;
using ClinicManagement.Application.Queries.Appointments.GetAppointmentsByPatient;

namespace ClinicManagement.Application.Mapper
{
    public class AppointmentProfile:Profile
    {
        public AppointmentProfile()
        {
            CreateMap<AppointmentDto, Appointment>()
                .ReverseMap();

            CreateMap<BookAppointmentCommand,Appointment>()
                .ReverseMap();

            CreateMap<Appointment, UpdateAppointmentCommand>()
                .ReverseMap();

            CreateMap<UpdateAppointmentStatusCommand, Appointment>()
                .ReverseMap();

            CreateMap<Appointment, CancelAppointmentCommand>()
                .ReverseMap();

            CreateMap<Appointment, GetAppointmentsByDoctorQuery>()
                .ReverseMap();

            CreateMap<Appointment, GetAppointmentsByPatientQuery>()
                .ReverseMap();
        }
    }
}

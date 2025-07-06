using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Mapper
{
    public class AppointmentProfile:Profile
    {
        public AppointmentProfile()
        {
            CreateMap<AppointmentDto, Appointment>()
                .ReverseMap();

            CreateMap<CreateAppointmentDto, Appointment>()
                .ReverseMap();
        }
    }
}

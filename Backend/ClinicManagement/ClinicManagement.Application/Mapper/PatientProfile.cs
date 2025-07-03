using AutoMapper;
using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientRegisterDto>().ReverseMap();
        }
    }
}

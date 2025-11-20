using AutoMapper;
using ClinicManagement.Domain.DTOs.PatientDTOs;
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
            CreateMap<PatientDto, Patient>().AfterMap((src, dest) =>
            {
                dest.UserName = src.FirstName + src.LastName;
            }).ReverseMap();
        }
    }
}

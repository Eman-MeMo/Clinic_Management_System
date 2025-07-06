using AutoMapper;
using ClinicManagement.Domain.DTOs.AccountDTOs;
using ClinicManagement.Domain.DTOs.DoctorDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class DoctorProfile:Profile
    {
        public DoctorProfile()
        {

            CreateMap<DoctorRegisterDto, Doctor>()
                .AfterMap((src, dest) =>
                {
                    dest.UserName = src.FirstName + src.LastName;
                }).ReverseMap();

            CreateMap<EditDoctorDto, Doctor>()
                .AfterMap((src, dest) =>
                {
                    dest.UserName = src.FirstName + src.LastName;
                }).ReverseMap();

            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization.Name));

        }
    }
}

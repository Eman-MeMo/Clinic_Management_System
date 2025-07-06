using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicManagement.Domain.DTOs.SpecializationDTOs;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Mapper
{
    public class SpecializationProfile:Profile
    {
        public SpecializationProfile()
        {
            CreateMap<SpecializationDto, Specialization>()
                .ReverseMap();
            CreateMap<CreateSpecializationDto, Specialization>()
                .ReverseMap();
        }
    }
}

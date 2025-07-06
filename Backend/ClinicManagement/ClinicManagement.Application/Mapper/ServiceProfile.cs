using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClinicManagement.Domain.DTOs.ServiceDTOs;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Mapper
{
    public class ServiceProfile:Profile
    {
        public ServiceProfile() { 

            CreateMap<ServiceDto, Service>()
                .ReverseMap();

            CreateMap<CreateServiceDto, Service>()
                .ReverseMap();
        }
    }
}

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

            CreateMap<Service, ServiceDto>()
                           .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => (int)src.Duration.TotalMinutes));

            CreateMap<ServiceDto, Service>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.Duration)))
                .ForMember(dest => dest.SessionServices, opt => opt.Ignore());

            CreateMap<Service, CreateServiceDto>()
               .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => (int)src.Duration.TotalMinutes));

            CreateMap<CreateServiceDto, Service>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.Duration)))
                .ForMember(dest => dest.SessionServices, opt => opt.Ignore());

        }
    }
}

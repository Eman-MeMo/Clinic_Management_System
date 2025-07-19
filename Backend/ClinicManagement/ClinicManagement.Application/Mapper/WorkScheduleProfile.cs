using AutoMapper;
using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class WorkScheduleProfile:Profile
    {
        public WorkScheduleProfile() { 

            CreateMap<WorkSchedule,WorkScheduleDto>()
                .ReverseMap();

            CreateMap<CreateWorkScheduleDto, WorkSchedule>()
                .ReverseMap();
        }
    }
}

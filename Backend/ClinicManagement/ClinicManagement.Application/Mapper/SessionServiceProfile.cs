using AutoMapper;
using ClinicManagement.Domain.DTOs.SessionServiceDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class SessionServiceProfile:Profile
    {
        public SessionServiceProfile()
        {
            CreateMap<SessionService, SessionServiceDto>().ReverseMap();
        }
    }
}

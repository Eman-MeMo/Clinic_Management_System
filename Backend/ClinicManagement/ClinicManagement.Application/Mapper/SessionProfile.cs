using AutoMapper;
using ClinicManagement.Domain.DTOs.SessionDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class SessionProfile:Profile
    {
        public SessionProfile()
        {
            CreateMap<Session,SessionDto>().ReverseMap();
        }
    }
}

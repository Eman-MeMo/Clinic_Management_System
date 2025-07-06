using AutoMapper;
using ClinicManagement.Domain.DTOs.AccountDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class AdminProfile:Profile
    {
        public AdminProfile() {
            CreateMap<AdminRegisterDto, Admin>()
             .AfterMap((src, dest) => {
                 dest.UserName = src.FirstName + src.LastName;
             });

        }

    }
}

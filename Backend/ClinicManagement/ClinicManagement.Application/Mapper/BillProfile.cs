using AutoMapper;
using ClinicManagement.Domain.DTOs.BillDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class BillProfile:Profile
    {
        public BillProfile() {
            CreateMap<Bill, BillDto>()
             .ReverseMap();

            CreateMap<Bill, CraeteBillDto>()
                .ReverseMap();
        }
    }
}

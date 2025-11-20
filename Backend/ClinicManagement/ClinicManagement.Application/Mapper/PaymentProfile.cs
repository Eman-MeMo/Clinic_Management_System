using AutoMapper;
using ClinicManagement.Domain.DTOs.PaymentDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class PaymentProfile:Profile
    {
        public PaymentProfile() {
            CreateMap<Payment, PaymentDto>()
            .ReverseMap();
        }
    }
}

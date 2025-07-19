using AutoMapper;
using ClinicManagement.Domain.DTOs.PrescriptionDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class PrescriptionProfile:Profile
    {
        public PrescriptionProfile() { 
            CreateMap<Prescription,PrescriptionDto>()
                .ReverseMap();

            CreateMap<Prescription, CreatePrescriptionDto>()
                .ReverseMap();

        }
    }
}

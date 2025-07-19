using AutoMapper;
using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class MedicalRecordProfile:Profile
    {
        public MedicalRecordProfile()
        {
            CreateMap<MedicalRecord, MedicalRecordDto>()
                .ReverseMap();

            CreateMap<MedicalRecord, CreateMedicalRecordDto>()
                .ReverseMap();
        }
    }
}

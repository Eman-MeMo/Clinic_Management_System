using AutoMapper;
using ClinicManagement.Domain.DTOs.AccountDTOs;
using ClinicManagement.Domain.DTOs.AdminDTOs;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Mapper
{
    public class AttendanceProfile:Profile
    {
        public AttendanceProfile() {
            CreateMap<AttendanceDto, Attendance>()
             .ReverseMap();

            CreateMap<CreateAttendaceDto, Admin>()
                .ReverseMap();

            CreateMap<AttendanceSummaryDto, Admin>()
                .ReverseMap();
        }
    }
}

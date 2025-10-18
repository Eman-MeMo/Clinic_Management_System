using ClinicManagement.Domain.DTOs.DoctorDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Users.Doctor.GetDoctorBySpecilation
{
    public class GetDoctorBySpecilationQuery:IRequest<IEnumerable<DoctorDto>>
    {
        public int SpecializationId { get; set; }
    }
}

using ClinicManagement.Domain.DTOs.PatientDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Users.Patient.GetPatientByNationalId
{
    public class GetPatientByNationalIdQuery:IRequest<PatientDto>
    {
        public string NationalId { get; set; }
    }
}

using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.MedicalRecords.GetMedicalRecordsByPatientId
{
    public class GetMedicalRecordsByPatientIdQuery:IRequest<IEnumerable<MedicalRecordDto>>
    {
        public string PatientId { get; set; }
    }
}

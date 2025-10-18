using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.MedicalRecords.GetLatestMedicalRecordByPatientId
{
    public class GetLatestMedicalRecordByPatientIdQuery:IRequest<MedicalRecordDto>
    {
        public string PatientId { get; set; }
    }
}

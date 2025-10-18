using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.MedicalRecords.GetMedicalRecordsByDate
{
    public class GetMedicalRecordsByDateQuery:IRequest<IEnumerable<MedicalRecordDto>>
    {
        public DateTime date {  get; set; }
    }
}

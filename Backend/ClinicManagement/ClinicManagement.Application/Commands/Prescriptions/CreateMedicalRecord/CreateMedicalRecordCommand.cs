using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Prescriptions.CreateMedicalRecord
{
    public class CreateMedicalRecordCommand:IRequest<int>
    {
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public string Diagnosis { get; set; }
]        public string Notes { get; set; }
    }
}

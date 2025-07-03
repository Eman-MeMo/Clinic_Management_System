using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public string Diagnosis { get; set; }

        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

    }
}

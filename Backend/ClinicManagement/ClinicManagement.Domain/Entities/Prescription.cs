using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class Prescription: BaseEntity
    {
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string? Notes { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }
        public Session Session { get; set; }
       
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class Attendance: BaseEntity
    {
        public bool IsPresent { get; set; }
        public string? Notes {  get; set; }
        
        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public Patient Patient { get; set; }
        
        [ForeignKey("Session")]
        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}

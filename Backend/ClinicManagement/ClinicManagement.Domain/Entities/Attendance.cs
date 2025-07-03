using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public bool IsPresent { get; set; }
        public DateTime Date { get; set; }
        
        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public Patient Patient { get; set; }
        
        [ForeignKey("Session")]
        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}

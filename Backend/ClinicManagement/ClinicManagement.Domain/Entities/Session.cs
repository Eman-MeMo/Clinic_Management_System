using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class Session: BaseEntity
    {
        public DateTime? ActualStartTime { get; set; }   
        public DateTime? ActualEndTime { get; set; }
        public SessionStatus Status { get; set; }
        public string? Notes { get; set; }

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public Attendance Attendance { get; set; }
        public ICollection<SessionService> SessionServices { get; set; } = new List<SessionService>();
        public Bill Bill { get; set; }
        public Prescription Prescription { get; set; }

    }
}

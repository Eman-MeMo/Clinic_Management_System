using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class Patient:AppUser
    {
        public Gender gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string NationID { get ; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}

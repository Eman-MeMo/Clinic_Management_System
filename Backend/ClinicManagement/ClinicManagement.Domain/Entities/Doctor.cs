using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class Doctor:AppUser
    {
        [ForeignKey("Specialization")]
        public int SpecializationId { get; set; }
        public Specialization Specialization { get; set; }
        public ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}

using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.SessionDTOs
{
    public class SessionDto
    {
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        [EnumDataType(typeof(SessionStatus))]
        public SessionStatus Status { get; set; } 
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Doctor ID is required.")]
        public string DoctorId { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Appointment ID is required.")]
        public int AppointmentId { get; set; }
    }
}

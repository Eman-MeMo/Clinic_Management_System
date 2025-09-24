using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.AppointmentDTOs
{
    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "Appointment date is required.")]
        [FutureDate(ErrorMessage = "Appointment date must be in the future.")]
        public DateTime Date { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [StringLength(500, ErrorMessage = "Notes must be less than 500 characters.")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Doctor ID is required.")]
        public string DoctorId { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public string PatientId { get; set; }
    }
}

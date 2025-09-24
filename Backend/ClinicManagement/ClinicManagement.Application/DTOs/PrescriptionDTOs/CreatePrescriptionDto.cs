using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.PrescriptionDTOs
{
    public class CreatePrescriptionDto
    {
        [Required(ErrorMessage = "Medication Name is required!")]
        [StringLength(50, ErrorMessage = "Medication Name cannot exceed 50 characters.")]
        public string MedicationName { get; set; }
        [Required(ErrorMessage = "Dosage is required.")]
        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters.")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "Diagnosis is required.")]
        [StringLength(200, ErrorMessage = "Diagnosis cannot exceed 200 characters.")]
        public string Diagnosis { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Session ID is required.")]
        public int SessionId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.PrescriptionDTOs
{
    public class PrescriptionDto
    {
        [Required(ErrorMessage ="Id is required!")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Medication Name is required!")]
        [StringLength(50,ErrorMessage = "Medication Name cannot exceed 50 characters.")]
        public string MedicationName { get; set; }
        [Required(ErrorMessage = "Dosage is required.")]
        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters.")]
        public string Dosage { get; set; }
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Session ID is required.")]
        public int SessionId { get; set; }
    }
}

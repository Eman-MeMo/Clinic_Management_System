using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.MedicalRecordDTOs
{
    public class MedicalRecordDto
    {
        [Required(ErrorMessage = "Record ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Diagnosis is required.")]
        [StringLength(200, ErrorMessage = "Diagnosis cannot exceed 200 characters.")]
        public string Diagnosis { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public string PatientId { get; set; }
    }
}

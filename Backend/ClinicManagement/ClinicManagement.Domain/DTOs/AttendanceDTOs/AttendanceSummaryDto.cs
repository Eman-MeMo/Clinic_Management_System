using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.AttendanceDTOs
{
    public class AttendanceSummaryDto
    {
        [Required(ErrorMessage = "Doctor ID is required.")]
        public string DoctorId { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalPatients { get; set; }

        [Range(0, int.MaxValue)]
        public int PresentCount { get; set; }

        [Range(0, int.MaxValue)]
        public int AbsentCount { get; set; }
    }
}

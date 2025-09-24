using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.AttendanceDTOs
{
    public class AttendanceDto
    {
        [Required(ErrorMessage = "Id is required!")]
        public int Id {  get; set; }

        [Required(ErrorMessage = "Attendance Status is required!")]
        public bool IsPresent { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Patient Id is required!")]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Session Id is required!")]
        public int SessionId { get; set; }
    }
}

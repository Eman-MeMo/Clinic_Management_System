using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.WorkScheduleDTOs
{
    public class WorkScheduleDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "End time is required.")]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "Day of week is required.")]
        [EnumDataType(typeof(DayOfWeek), ErrorMessage = "Invalid Day of week!")]
        public DayOfWeek DayOfWeek { get; set; }
        [Required(ErrorMessage = "IsAvailable is required.")]
        public bool IsAvailable { get; set; }
        [Required(ErrorMessage = "Doctor ID is required.")]
        public string DoctorId { get; set; }
    }
}

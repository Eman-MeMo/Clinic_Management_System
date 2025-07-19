using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.SessionServiceDTOs
{
    public class SessionServiceDto
    {
        [Required(ErrorMessage = "Session ID is required.")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Service ID is required.")]
        public int ServiceId { get; set; }
    }
}

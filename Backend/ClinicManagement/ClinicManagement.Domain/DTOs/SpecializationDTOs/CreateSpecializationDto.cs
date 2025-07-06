using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.SpecializationDTOs
{
    public class CreateSpecializationDto
    {
        [Required(ErrorMessage = "Specialization name is required.")]
        [StringLength(100, ErrorMessage = "Specialization Name cannot be longer than 100 characters.")]
        public string Name { get; set; }
    }
}

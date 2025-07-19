using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.BillDTOs
{
    public class BillDto
    {
        [Required(ErrorMessage = "Bill ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Payment status is required.")]
        public bool IsPaid { get; set; }

        [Required(ErrorMessage = "Session ID is required.")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public string PatientId { get; set; }
    }
}

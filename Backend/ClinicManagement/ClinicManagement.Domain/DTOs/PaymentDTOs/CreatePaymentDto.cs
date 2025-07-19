using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.DTOs.PaymentDTOs
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Payment Method is required.")]
        [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Invalid payment method.")]
        public PaymentMethod PaymentMethod { get; set; }
        [Required(ErrorMessage = "Bill ID is required.")]
        public int BillId { get; set; }
    }
}

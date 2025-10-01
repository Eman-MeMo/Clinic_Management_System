using ClinicManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Payments.CreatePayment
{
    public class CreatePaymentCommand:IRequest<int>
    {
        public decimal Amount { get; set; }
        public Domain.Enums.PaymentMethod PaymentMethod { get; set; }
        public int BillId { get; set; }
    }
}

using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<int> CreatePayment(int billId, PaymentMethod method);
    }
}

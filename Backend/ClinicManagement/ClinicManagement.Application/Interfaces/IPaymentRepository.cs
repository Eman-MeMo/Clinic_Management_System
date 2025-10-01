using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IPaymentRepository:IGenericRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetByPatientAsync(string patientId);
        Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<bool> GetStatusAsync(int billId);
        Task<Payment> GetByBillIdAsync(int billId);
    }
}

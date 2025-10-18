using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ClinicDbContext db) : base(db) { }
        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await db.Payments.AsNoTracking()
                .Where(p => p.Date >= start && p.Date <= end)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByPatientAsync(string patientId)
        {
            return await db.Payments.AsNoTracking()
                .Include(p => p.Bill)
                .ThenInclude(b => b.Patient)
                .Where(p => p.Bill.PatientId == patientId)
                .ToListAsync();
        }
        public async Task<Payment> GetByBillIdAsync(int billId)
        {
            return await db.Payments
                .FirstOrDefaultAsync(p => p.BillId == billId);
        }

        public async Task<bool> GetStatusAsync(int billId)
        {
            return await db.Payments.Include(p => p.Bill)
                .AnyAsync(p => p.BillId == billId && p.Bill.IsPaid);
        }
    }
}

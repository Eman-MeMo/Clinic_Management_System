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
    public class BillRepository:GenericRepository<Bill>, IBillRepository
    {
        public BillRepository(ClinicDbContext db) : base(db){ }
        public async Task<IEnumerable<Bill>> GetUnpaidBillsByPatientAsync(string patientId)
        {
            return await db.Bills.AsNoTracking()
                .Where(b => b.PatientId == patientId && !b.IsPaid)
                .ToListAsync();
        }
        public async Task<IEnumerable<Bill>> GetBySessionAsync(int sessionId)
        {
            return await db.Bills.AsNoTracking()
                .Where(b => b.SessionId == sessionId)
                .ToListAsync();
        }
    }
}

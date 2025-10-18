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
    public class SessionServiceRepository : ISessionServiceRepository
    {
        private readonly ClinicDbContext db;

        public SessionServiceRepository(ClinicDbContext _db)
        {
            db = _db;
        }

        public async Task AddAsync(SessionService sessionService)
        {
            await db.Set<SessionService>().AddAsync(sessionService);
        }
        public void Delete(SessionService sessionService)
        {
            db.Set<SessionService>().Remove(sessionService);
        }

        public async Task<bool> ExistsAsync(int sessionId, int serviceId)
        {
            return await db.Set<SessionService>()
                           .AnyAsync(ss => ss.SessionId == sessionId && ss.ServiceId == serviceId);
        }
        public async Task<SessionService> GetByIdAsync(int sessionId, int serviceId)
        {
            return await db.Set<SessionService>()
                           .FirstOrDefaultAsync(ss => ss.SessionId == sessionId && ss.ServiceId == serviceId);
        }

        public async Task<IEnumerable<SessionService>> GetAllBySessionIdAsync(int sessionId)
        {
            return await db.Set<SessionService>().AsNoTracking()
                           .Include(ss => ss.Service)
                           .Where(ss => ss.SessionId == sessionId)
                           .ToListAsync();
        }
    }

}

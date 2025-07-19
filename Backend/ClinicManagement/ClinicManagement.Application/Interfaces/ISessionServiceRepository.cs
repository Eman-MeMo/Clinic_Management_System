using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface ISessionServiceRepository
    {
        Task<IEnumerable<SessionService>> GetAllBySessionIdAsync(int sessionId);
        Task<SessionService> GetByIdAsync(int sessionId,int serviceId);
        Task AddAsync(SessionService sessionService);
        public void Delete(SessionService sessionService);
        public Task<bool> ExistsAsync(int sessionId, int serviceId);

    }
}

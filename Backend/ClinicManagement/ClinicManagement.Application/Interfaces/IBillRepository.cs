using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IBillRepository:IGenericRepository<Bill>
    {
        Task<IEnumerable<Bill>> GetUnpaidBillsByPatientAsync(string patientId);
        Task<IEnumerable<Bill>> GetBySessionAsync(int sessionId);
    }
}

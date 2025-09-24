using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClinicManagement.Application.Interfaces
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription>
    {
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(string patientId);
        Task<IEnumerable<Prescription>> GetBySessionIdAsync(int sessionId);
        Task<IEnumerable<Prescription>> GetByDoctorIdAsync(string doctorId);
    }
}

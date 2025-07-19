using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IMedicalRecordRepository:IGenericRepository<MedicalRecord>
    {
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(string patientId);
        Task<MedicalRecord> GetLatestRecordAsync(string patientId);
        Task <IEnumerable<MedicalRecord>> GetByDateAsync(DateTime date);
    }
}

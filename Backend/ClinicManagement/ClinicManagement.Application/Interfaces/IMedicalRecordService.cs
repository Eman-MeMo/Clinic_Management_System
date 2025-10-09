using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<int> CreateMedicalRecordAsync(string? Notes, string Diagnosis, int prescriptionId);
    }
}

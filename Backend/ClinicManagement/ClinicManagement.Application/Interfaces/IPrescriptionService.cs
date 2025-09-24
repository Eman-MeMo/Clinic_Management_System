using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IPrescriptionService
    {
        Task CreateMedicalRecoredAsync(string? Notes, string Diagnosis, int prescriptionId);
    }
}

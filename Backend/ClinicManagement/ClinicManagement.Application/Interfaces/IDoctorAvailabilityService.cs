using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IDoctorAvailabilityService
    {
        Task<bool> IsDoctorAvailableAsync(string doctorId, DateTime date, int? appointmentIdToExclude = null);
    }
}

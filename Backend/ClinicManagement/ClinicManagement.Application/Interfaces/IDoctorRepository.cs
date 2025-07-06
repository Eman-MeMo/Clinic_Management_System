using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IDoctorRepository : IUserRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetAllBySpecializationAsync(string specialization);
        Task<IEnumerable<Doctor>> GetAvailableDoctorsAtAsync(DateTime targetTime);
    }
}

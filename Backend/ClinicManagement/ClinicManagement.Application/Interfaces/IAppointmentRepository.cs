using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAllByDoctorIdAsync(string doctorId);
        Task<IEnumerable<Appointment>> GetAllByPatientIdAsync(string pateintId);
        Task<bool> IsDoctorAvailableAsync(string doctorId, DateTime date, int? appointmentIdToExclude = null);

    }
}

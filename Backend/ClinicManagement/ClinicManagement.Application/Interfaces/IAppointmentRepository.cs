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
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<IEnumerable<Appointment>> GetAllByDoctorIdAsync(string doctorId);
        Task<IEnumerable<Appointment>> GetAllByPatientIdAsync(string pateintId);
        Task<Appointment> GetByIdAsync(int id);
        Task AddAsync(Appointment entity);
        void Delete(Appointment entity);
        void Update(Appointment entity);
        Task<bool> IsDoctorAvailableAsync(string doctorId, DateTime date, int? appointmentIdToExclude = null);

    }
}

using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IAppointmentRepository:IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAllByDoctorIdAsync(string doctorId);
        Task<bool> HasAppointmentForDoctorAtAsync(string doctorId, DateTime date, int? appointmentIdToExclude = null);
        Task<IEnumerable<Appointment>> GetAllByPatientIdAsync(string pateintId);
    }
}

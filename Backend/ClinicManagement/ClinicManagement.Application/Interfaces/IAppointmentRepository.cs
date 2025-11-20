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
        Task<bool> HasAppointmentForDoctorAtDateAsync(string doctorId, DateTime date, int? appointmentIdToExclude = null);
        Task<bool> HasAppointmentForDoctorAsync(string doctorId);
        Task<IEnumerable<Appointment>> GetAllByPatientIdAsync(string pateintId);
        Task CancelAllAppointmentsForPatient(string patientId);
        Task<bool> CanPatientBook(string patientId, DateTime date);
    }
}

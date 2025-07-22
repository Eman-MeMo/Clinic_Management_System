using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IDoctorRepository DoctorRepository { get; }
        IPatientRepository PatientRepository { get; }
        IUserRepository<Admin> AdminRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IAttendanceRepository AttendanceRepository { get; }
        IBillRepository BillRepository { get; }
        IMedicalRecordRepository MedicalRecordRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IPrescriptionRepository PrescriptionRepository { get; }
        IGenericRepository<Service> ServiceRepository { get; }
        ISessionRepository SessionRepository { get; }
        ISessionServiceRepository SessionServiceRepository { get; }
        IGenericRepository<Specialization> SpecializationRepository { get; }
        IWorkScheduleRepository WorkScheduleRepository { get; }
        Task<int> SaveChangesAsync();
    }
}

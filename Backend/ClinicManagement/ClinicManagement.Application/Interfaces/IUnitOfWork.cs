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
        IGenericRepository<Attendance> AttendanceRepository { get; }
        IGenericRepository<AuditLog> AuditLogRepository { get; }
        IGenericRepository<Bill> BillRepository { get; }
        IGenericRepository<MedicalRecord> MedicalRecordRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        IGenericRepository<Prescription> PrescriptionRepository { get; }
        IGenericRepository<Service> ServiceRepository { get; }
        IGenericRepository<Session> SessionRepository { get; }
        IGenericRepository<Specialization> SpecializationRepository { get; }
        IGenericRepository<WorkSchedule> WorkScheduleRepository { get; }
        Task<int> SaveChangesAsync();
    }
}

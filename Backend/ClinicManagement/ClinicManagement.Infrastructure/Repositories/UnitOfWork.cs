using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext db;
        private IAppointmentRepository AppointmentRepo;
        private IGenericRepository<Attendance> AttendanceRepo;
        private IGenericRepository<AuditLog> AuditLogRepo;
        private IGenericRepository<Bill> BillRepo;
        private IGenericRepository<MedicalRecord> MedicalRecordRepo;
        private IGenericRepository<Payment> PaymentRepo;
        private IGenericRepository<Prescription> PrescriptionRepo;
        private IGenericRepository<Service> ServiceRepo;
        private IGenericRepository<Session> SessionRepo;
        private IGenericRepository<Specialization> SpecializationRepo;
        private IGenericRepository<WorkSchedule> WorkScheduleRepo;

        public UnitOfWork(ClinicDbContext context)
        {
            db = context;
        }
        public IAppointmentRepository AppointmentRepository
        {
            get
            {
                if(AppointmentRepo == null)
                    AppointmentRepo = new AppointmentRepository(db);
                return AppointmentRepo;
            }
        }

        public IGenericRepository<Attendance> AttendanceRepository
        {
            get
            {
                if (AttendanceRepo == null)
                    AttendanceRepo = new GenericRepository<Attendance>(db);
                return AttendanceRepo;
            }
        }

        public IGenericRepository<AuditLog> AuditLogRepository
        {
            get
            {
                if (AuditLogRepo == null)
                    AuditLogRepo = new GenericRepository<AuditLog>(db);
                return AuditLogRepo;
            }
        }

        public IGenericRepository<Bill> BillRepository {
            get
            {
                if (BillRepo == null)
                    BillRepo = new GenericRepository<Bill>(db);
                return BillRepo;
            }
        }

        public IGenericRepository<MedicalRecord> MedicalRecordRepository
        {
            get
            {
                if (MedicalRecordRepo == null)
                    MedicalRecordRepo = new GenericRepository<MedicalRecord>(db);
                return MedicalRecordRepo;
            }
        }

        public IGenericRepository<Payment> PaymentRepository
        {
            get
            {
                if (PaymentRepo == null)
                    PaymentRepo = new GenericRepository<Payment>(db);
                return PaymentRepo;
            }
        }

        public IGenericRepository<Prescription> PrescriptionRepository {
            get
            {
                if (PrescriptionRepo == null)
                    PrescriptionRepo = new GenericRepository<Prescription>(db);
                return PrescriptionRepo;
            }
        }

        public IGenericRepository<Service> ServiceRepository
        {
            get
            {
                if (ServiceRepo == null)
                    ServiceRepo = new GenericRepository<Service>(db);
                return ServiceRepo;
            }
        }

        public IGenericRepository<Session> SessionRepository
        {
            get
            {
                if (SessionRepo == null)
                    SessionRepo = new GenericRepository<Session>(db);
                return SessionRepo;
            }
        }

        public IGenericRepository<Specialization> SpecializationRepository
        {
            get
            {
                if (SpecializationRepo == null)
                    SpecializationRepo = new GenericRepository<Specialization>(db);
                return SpecializationRepo;
            }
        }

        public IGenericRepository<WorkSchedule> WorkScheduleRepository
        {
            get
            {
                if (WorkScheduleRepo == null)
                    WorkScheduleRepo = new GenericRepository<WorkSchedule>(db);
                return WorkScheduleRepo;
            }
        }
        public async Task<int> SaveChangesAsync()
        {
            return await db.SaveChangesAsync();
        }
    }
}

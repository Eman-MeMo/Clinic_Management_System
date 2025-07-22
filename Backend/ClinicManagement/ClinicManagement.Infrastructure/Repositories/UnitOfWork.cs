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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext db;
        private IDoctorRepository DoctorRepo;
        private IPatientRepository PatientRepo;
        private IUserRepository<Admin> AdminRepo;
        private IAppointmentRepository AppointmentRepo;
        private IAttendanceRepository AttendanceRepo;
        private IBillRepository BillRepo;
        private IMedicalRecordRepository MedicalRecordRepo;
        private IPaymentRepository PaymentRepo;
        private IPrescriptionRepository PrescriptionRepo;
        private IGenericRepository<Service> ServiceRepo;
        private ISessionServiceRepository SessionServiceRepo;
        private ISessionRepository SessionRepo;
        private IGenericRepository<Specialization> SpecializationRepo;
        private IWorkScheduleRepository WorkScheduleRepo;

        public UnitOfWork(ClinicDbContext context)
        {
            db = context;
        }
        public IDoctorRepository DoctorRepository
        {
            get
            {
                if (DoctorRepo == null)
                    DoctorRepo= new DoctorRepository(db);
                return DoctorRepo;
            }
        }
        public IPatientRepository PatientRepository
        {
            get
            {
                if (PatientRepo == null)
                    PatientRepo = new PatientRepository(db);
                return PatientRepo;
            }
        }
        public IUserRepository<Admin> AdminRepository
        {
            get
            {
                if (AdminRepo == null)
                    return new UserRepository<Admin>(db);
                return AdminRepo;
            }
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

        public IAttendanceRepository AttendanceRepository
        {
            get
            {
                if (AttendanceRepo == null)
                    AttendanceRepo = new AttendanceRepository(db);
                return AttendanceRepo;
            }
        }
        public IBillRepository BillRepository {
            get
            {
                if (BillRepo == null)
                    BillRepo = new BillRepository(db);
                return BillRepo;
            }
        }

        public IMedicalRecordRepository MedicalRecordRepository
        {
            get
            {
                if (MedicalRecordRepo == null)
                    MedicalRecordRepo = new MedicalRecordRepository(db);
                return MedicalRecordRepo;
            }
        }

        public IPaymentRepository PaymentRepository
        {
            get
            {
                if (PaymentRepo == null)
                    PaymentRepo = new PaymentRepository(db);
                return PaymentRepo;
            }
        }

        public IPrescriptionRepository PrescriptionRepository {
            get
            {
                if (PrescriptionRepo == null)
                    PrescriptionRepo = new PrescriptionRepository(db);
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

        public ISessionRepository SessionRepository
        {
            get
            {
                if (SessionRepo == null)
                    SessionRepo = new SessionRepository(db);
                return SessionRepo;
            }
        }
        public ISessionServiceRepository SessionServiceRepository
        {
            get
            {
                if (SessionServiceRepo == null)
                    SessionServiceRepo = new SessionServiceRepository(db);
                return SessionServiceRepo;
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

        public IWorkScheduleRepository WorkScheduleRepository
        {
            get
            {
                if (WorkScheduleRepo == null)
                    WorkScheduleRepo = new WorkScheduleRepository(db);
                return WorkScheduleRepo;
            }
        }
        public async Task<int> SaveChangesAsync()
        {
            return await db.SaveChangesAsync();
        }
    }
}

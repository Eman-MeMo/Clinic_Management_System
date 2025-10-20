using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Data
{
    public class ClinicDbContext : IdentityDbContext<AppUser>
    {
        private readonly ILogger<ClinicDbContext> logger;
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options, ILogger<ClinicDbContext> _logger) : base(options)
        {
            logger = _logger;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Get all tracked changes
                var entries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added ||
                                e.State == EntityState.Modified ||
                                e.State == EntityState.Deleted);

                foreach (var entry in entries)
                {
                    string action = entry.State.ToString();
                    string entityName = entry.Entity.GetType().Name;

                    logger.LogInformation($"Entity: {entityName}, Action: {action}, Time: {DateTime.Now}");

                    if (entry.State == EntityState.Modified)
                    {
                        foreach (var prop in entry.OriginalValues.Properties)
                        {
                            var original = entry.OriginalValues[prop];
                            var current = entry.CurrentValues[prop];
                            if (!Equals(original, current))
                            {
                                logger.LogInformation($"Property {prop.Name}: {original} => {current}");
                            }
                        }
                    }
                }
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during SaveChanges()");
                throw;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().ToTable("AppUsers");
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<Doctor>().ToTable("Doctors");
            modelBuilder.Entity<Patient>().ToTable("Patients");

            modelBuilder.Entity<SessionService>()
                .HasKey(ss => new { ss.SessionId, ss.ServiceId });

            modelBuilder.Entity<Bill>()
               .Property(b => b.Amount)
               .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");


        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionService> SessionServices { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
    }
}

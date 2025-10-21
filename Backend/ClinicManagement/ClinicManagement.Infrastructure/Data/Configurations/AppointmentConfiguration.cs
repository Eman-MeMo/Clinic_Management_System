using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasData(
                // Completed appointments
                new Appointment
                {
                    Id = 1,
                    Date = new DateTime(2025, 10, 15, 10, 0, 0),
                    Status = AppointmentStatus.Confirmed,
                    Notes = "Regular heart checkup",
                    DoctorId = "D1",
                    PatientId = "P1"
                },
                new Appointment
                {
                    Id = 2,
                    Date = new DateTime(2025, 10, 16, 11, 0, 0),
                    Status = AppointmentStatus.Confirmed,
                    Notes = "Skin allergy treatment",
                    DoctorId = "D2",
                    PatientId = "P2"
                },

                // Upcoming appointment
                new Appointment
                {
                    Id = 3,
                    Date = new DateTime(2025, 10, 25, 9, 30, 0),
                    Status = AppointmentStatus.Scheduled,
                    Notes = "Dental cleaning session",
                    DoctorId = "D3",
                    PatientId = "P3"
                },

                // Cancelled appointment
                new Appointment
                {
                    Id = 4,
                    Date = new DateTime(2025, 10, 10, 12, 0, 0),
                    Status = AppointmentStatus.Cancelled,
                    Notes = "Patient couldn’t attend",
                    DoctorId = "D1",
                    PatientId = "P2"
                },

                // Another upcoming appointment
                new Appointment
                {
                    Id = 5,
                    Date = new DateTime(2025, 10, 27, 14, 0, 0),
                    Status = AppointmentStatus.Scheduled,
                    Notes = "Neurological consultation",
                    DoctorId = "D2",
                    PatientId = "P1"
                }
            );
        }
    }
}

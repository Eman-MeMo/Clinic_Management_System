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
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasData(
                new Session
                {
                    Id = 1,
                    ActualStartTime = new DateTime(2025, 10, 15, 9, 0, 0),
                    ActualEndTime = new DateTime(2025, 10, 15, 9, 30, 0),
                    Status = SessionStatus.Confirmed,
                    Notes = "Session confirmed for ECG and blood pressure check.",
                    AppointmentId = 1,
                    DoctorId = "D1",
                    PatientId = "P1"
                },
                new Session
                {
                    Id = 2,
                    ActualStartTime = new DateTime(2025, 10, 16, 11, 0, 0),
                    ActualEndTime = new DateTime(2025, 10, 16, 11, 45, 0),
                    Status = SessionStatus.Confirmed,
                    Notes = "Confirmed session for skin treatment and allergy testing.",
                    AppointmentId = 2,
                    DoctorId = "D2",
                    PatientId = "P2"
                },
                new Session
                {
                    Id = 3,
                    ActualStartTime = new DateTime(2025, 10, 17, 10, 30, 0),
                    ActualEndTime = new DateTime(2025, 10, 17, 11, 0, 0),
                    Status = SessionStatus.Cancelled,
                    Notes = "Session cancelled due to patient unavailability.",
                    AppointmentId = 3,
                    DoctorId = "D3",
                    PatientId = "P3"
                },
                new Session
                {
                    Id = 4,
                    ActualStartTime = new DateTime(2025, 10, 18, 14, 0, 0),
                    ActualEndTime = new DateTime(2025, 10, 18, 14, 30, 0),
                    Status = SessionStatus.Scheduled,
                    Notes = "Teeth cleaning session scheduled for next visit.",
                    AppointmentId = 4,
                    DoctorId = "D1",
                    PatientId = "P2"
                },
                new Session
                {
                    Id = 5,
                    ActualStartTime = new DateTime(2025, 10, 19, 13, 0, 0),
                    ActualEndTime = new DateTime(2025, 10, 19, 13, 40, 0),
                    Status = SessionStatus.Confirmed,
                    Notes = "Neurological check completed successfully. Patient advised to rest.",
                    AppointmentId = 5,
                    DoctorId = "D2",
                    PatientId = "P1"
                }
            );
        }
    }
}

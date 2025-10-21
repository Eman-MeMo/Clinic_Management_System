using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasData(
                new Attendance
                {
                    Id = 1,
                    SessionId = 1,     
                    PatientId = "P1",
                    IsPresent = true,
                    Notes = "Patient attended on time and participated actively."
                },
                new Attendance
                {
                    Id = 2,
                    SessionId = 2,    
                    PatientId = "P2",
                    IsPresent = true,
                    Notes = "Patient arrived 5 minutes late but session completed successfully."
                },
                new Attendance
                {
                    Id = 3,
                    SessionId = 3,    
                    PatientId = "P3",
                    IsPresent = false,
                    Notes = "Patient did not attend; session was cancelled."
                },
                new Attendance
                {
                    Id = 4,
                    SessionId = 4,     
                    PatientId = "P2",
                    IsPresent = false,
                    Notes = "Upcoming session; attendance will be recorded later."
                },
                new Attendance
                {
                    Id = 5,
                    SessionId = 5,    
                    PatientId = "P1",
                    IsPresent = true,
                    Notes = "Patient attended and completed neurological check successfully."
                }
            );
        }
    }
}

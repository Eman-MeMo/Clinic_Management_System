using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
    {
        public void Configure(EntityTypeBuilder<WorkSchedule> builder)
        {
            builder.HasData(
                new WorkSchedule
                {
                    Id = 1,
                    DoctorId = "D1",
                    StartTime = new DateTime(1, 1, 1, 9, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 15, 0, 0),
                    DayOfWeek = DayOfWeek.Sunday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 2,
                    DoctorId = "D2",
                    StartTime = new DateTime(1, 1, 1, 10, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 16, 0, 0),
                    DayOfWeek = DayOfWeek.Monday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 3,
                    DoctorId = "D3",
                    StartTime = new DateTime(1, 1, 1, 11, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 17, 0, 0),
                    DayOfWeek = DayOfWeek.Tuesday,
                    IsAvailable = true
                }
            );
        }
    }
}

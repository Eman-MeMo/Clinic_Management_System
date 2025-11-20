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
                   DayOfWeek = DayOfWeek.Thursday,
                   IsAvailable = true
               },
                new WorkSchedule
                {
                    Id = 2,
                    DoctorId = "D1",
                    StartTime = new DateTime(1, 1, 1, 9, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 15, 0, 0),
                    DayOfWeek = DayOfWeek.Monday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 3,
                    DoctorId = "D1",
                    StartTime = new DateTime(1, 1, 1, 12, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 18, 0, 0),
                    DayOfWeek = DayOfWeek.Wednesday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 4,
                    DoctorId = "D1",
                    StartTime = new DateTime(1, 1, 1, 10, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 14, 0, 0),
                    DayOfWeek = DayOfWeek.Friday,
                    IsAvailable = false 
                },
                new WorkSchedule
                {
                    Id = 5,
                    DoctorId = "D2",
                    StartTime = new DateTime(1, 1, 1, 10, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 16, 0, 0),
                    DayOfWeek = DayOfWeek.Monday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 6,
                    DoctorId = "D2",
                    StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 14, 0, 0),
                    DayOfWeek = DayOfWeek.Tuesday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 7,
                    DoctorId = "D2",
                    StartTime = new DateTime(1, 1, 1, 12, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 18, 0, 0),
                    DayOfWeek = DayOfWeek.Wednesday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 8,
                    DoctorId = "D2",
                    StartTime = new DateTime(1, 1, 1, 9, 30, 0),
                    EndTime = new DateTime(1, 1, 1, 15, 30, 0),
                    DayOfWeek = DayOfWeek.Thursday,
                    IsAvailable = false
                },
                new WorkSchedule
                {
                    Id = 9,
                    DoctorId = "D3",
                    StartTime = new DateTime(1, 1, 1, 11, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 17, 0, 0),
                    DayOfWeek = DayOfWeek.Tuesday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 10,
                    DoctorId = "D3",
                    StartTime = new DateTime(1, 1, 1, 13, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 19, 0, 0),
                    DayOfWeek = DayOfWeek.Thursday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 11,
                    DoctorId = "D3",
                    StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 12, 0, 0),
                    DayOfWeek = DayOfWeek.Saturday,
                    IsAvailable = true
                },
                new WorkSchedule
                {
                    Id = 12,
                    DoctorId = "D3",
                    StartTime = new DateTime(1, 1, 1, 14, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 20, 0, 0),
                    DayOfWeek = DayOfWeek.Sunday,
                    IsAvailable = false 
                }
            );
        }
    }
}

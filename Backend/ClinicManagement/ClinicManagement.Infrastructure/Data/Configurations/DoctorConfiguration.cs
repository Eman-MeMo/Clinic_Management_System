using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            var hasher = new PasswordHasher<Doctor>();

            var doctor1 = new Doctor
            {
                Id = "D1",
                FirstName = "Ahmed",
                LastName = "Hassan",
                UserName = "ahmed.hassan@clinic.com",
                NormalizedUserName = "AHMED.HASSAN@CLINIC.COM",
                Email = "ahmed.hassan@clinic.com",
                NormalizedEmail = "AHMED.HASSAN@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = "11111111-1111-1111-1111-111111111111",
                ConcurrencyStamp = "22222222-2222-2222-2222-222222222222",
                SpecializationId = 1
            };
            doctor1.PasswordHash = hasher.HashPassword(doctor1, "Doctor@123");

            var doctor2 = new Doctor
            {
                Id = "D2",
                FirstName = "Sara",
                LastName = "Nabil",
                UserName = "sara.nabil@clinic.com",
                NormalizedUserName = "SARA.NABIL@CLINIC.COM",
                Email = "sara.nabil@clinic.com",
                NormalizedEmail = "SARA.NABIL@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = "33333333-3333-3333-3333-333333333333",
                ConcurrencyStamp = "44444444-4444-4444-4444-444444444444",
                SpecializationId = 2
            };
            doctor2.PasswordHash = hasher.HashPassword(doctor2, "Doctor@123");

            var doctor3 = new Doctor
            {
                Id = "D3",
                FirstName = "Omar",
                LastName = "Khaled",
                UserName = "omar.khaled@clinic.com",
                NormalizedUserName = "OMAR.KHALED@CLINIC.COM",
                Email = "omar.khaled@clinic.com",
                NormalizedEmail = "OMAR.KHALED@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = "55555555-5555-5555-5555-555555555555",
                ConcurrencyStamp = "66666666-6666-6666-6666-666666666666",
                SpecializationId = 3
            };
            doctor3.PasswordHash = hasher.HashPassword(doctor3, "Doctor@123");

            builder.HasData(doctor1, doctor2, doctor3);
        }
    }
}

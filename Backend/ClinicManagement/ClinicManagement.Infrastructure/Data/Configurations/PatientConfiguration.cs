using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            var hasher = new PasswordHasher<Patient>();

            var patient1 = new Patient
            {
                Id = "P1",
                FirstName = "Fatma",
                LastName = "Ali",
                UserName = "fatma.ali@clinic.com",
                NormalizedUserName = "FATMA.ALI@CLINIC.COM",
                Email = "fatma.ali@clinic.com",
                NormalizedEmail = "FATMA.ALI@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = "11111111-1111-1111-1111-111111111111",
                ConcurrencyStamp = "22222222-2222-2222-2222-222222222222",
                gender = Gender.Female,
                DateOfBirth = new DateOnly(1995, 5, 12),
                NationID = "29505121501043"
            };
            patient1.PasswordHash = hasher.HashPassword(patient1, "Patient@123");

            var patient2 = new Patient
            {
                Id = "P2",
                FirstName = "Mohamed",
                LastName = "Saad",
                UserName = "mohamed.saad@clinic.com",
                NormalizedUserName = "MOHAMED.SAAD@CLINIC.COM",
                Email = "mohamed.saad@clinic.com",
                NormalizedEmail = "MOHAMED.SAAD@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = "33333333-3333-3333-3333-333333333333",
                ConcurrencyStamp = "44444444-4444-4444-4444-444444444444",
                gender = Gender.Male,
                DateOfBirth = new DateOnly(1988, 8, 22),
                NationID = "28808221501043"
            };
            patient2.PasswordHash = hasher.HashPassword(patient2, "Patient@123");

            var patient3 = new Patient
            {
                Id = "P3",
                FirstName = "Eman",
                LastName = "Hussein",
                UserName = "eman.hussein@clinic.com",
                NormalizedUserName = "EMAN.HUSSEIN@CLINIC.COM",
                Email = "eman.hussein@clinic.com",
                NormalizedEmail = "EMAN.HUSSEIN@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = "55555555-5555-5555-5555-555555555555",
                ConcurrencyStamp = "66666666-6666-6666-6666-666666666666",
                gender = Gender.Female,
                DateOfBirth = new DateOnly(2000, 3, 15),
                NationID = "30003151501043"
            };
            patient3.PasswordHash = hasher.HashPassword(patient3, "Patient@123");

            builder.HasData(patient1, patient2, patient3);
        }
    }
}

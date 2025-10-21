using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            var hasher = new PasswordHasher<IdentityUser>();

            var admin = new IdentityUser
            {
                Id = "A1",
                UserName = "admin@clinic.com",
                NormalizedUserName = "ADMIN@CLINIC.COM",
                Email = "admin@clinic.com",
                NormalizedEmail = "ADMIN@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

            var doctor1 = new IdentityUser
            {
                Id = "D1",
                UserName = "ahmed.hassan@clinic.com",
                NormalizedUserName = "AHMED.HASSAN@CLINIC.COM",
                Email = "ahmed.hassan@clinic.com",
                NormalizedEmail = "AHMED.HASSAN@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            };
            doctor1.PasswordHash = hasher.HashPassword(doctor1, "Doctor@123");

            var doctor2 = new IdentityUser
            {
                Id = "D2",
                UserName = "sara.nabil@clinic.com",
                NormalizedUserName = "SARA.NABIL@CLINIC.COM",
                Email = "sara.nabil@clinic.com",
                NormalizedEmail = "SARA.NABIL@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            };
            doctor2.PasswordHash = hasher.HashPassword(doctor2, "Doctor@123");

            var doctor3 = new IdentityUser
            {
                Id = "D3",
                UserName = "omar.khaled@clinic.com",
                NormalizedUserName = "OMAR.KHALED@CLINIC.COM",
                Email = "omar.khaled@clinic.com",
                NormalizedEmail = "OMAR.KHALED@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            };
            doctor3.PasswordHash = hasher.HashPassword(doctor3, "Doctor@123");

            var patient1 = new IdentityUser
            {
                Id = "P1",
                UserName = "fatma.ali@clinic.com",
                NormalizedUserName = "FATMA.ALI@CLINIC.COM",
                Email = "fatma.ali@clinic.com",
                NormalizedEmail = "FATMA.ALI@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            };
            patient1.PasswordHash = hasher.HashPassword(patient1, "Patient@123");

            var patient2 = new IdentityUser
            {
                Id = "P2",
                UserName = "mohamed.saad@clinic.com",
                NormalizedUserName = "MOHAMED.SAAD@CLINIC.COM",
                Email = "mohamed.saad@clinic.com",
                NormalizedEmail = "MOHAMED.SAAD@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            };
            patient2.PasswordHash = hasher.HashPassword(patient2, "Patient@123");

            var patient3 = new IdentityUser
            {
                Id = "P3",
                UserName = "eman.hussein@clinic.com",
                NormalizedUserName = "EMAN.HUSSEIN@CLINIC.COM",
                Email = "eman.hussein@clinic.com",
                NormalizedEmail = "EMAN.HUSSEIN@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            };
            patient3.PasswordHash = hasher.HashPassword(patient3, "Patient@123");

            builder.HasData(admin, doctor1, doctor2, doctor3, patient1, patient2, patient3);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            var hasher = new PasswordHasher<Admin>();

            var admin = new Admin
            {
                Id = "A1",
                FirstName = "System",
                LastName = "Administrator",
                UserName = "admin@clinic.com",
                NormalizedUserName = "ADMIN@CLINIC.COM",
                Email = "admin@clinic.com",
                NormalizedEmail = "ADMIN@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = "11111111-1111-1111-1111-111111111111",
                ConcurrencyStamp = "22222222-2222-2222-2222-222222222222",
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

            builder.HasData(admin);
        }
    }
}

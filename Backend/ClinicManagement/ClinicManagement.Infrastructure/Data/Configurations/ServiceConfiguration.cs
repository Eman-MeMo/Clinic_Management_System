using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasData(
                new Service
                {
                    Id = 1,
                    Name = "Consultation",
                    Price = 300,
                    Duration = TimeSpan.FromMinutes(30)
                },
                new Service
                {
                    Id = 2,
                    Name = "ECG",
                    Price = 250,
                    Duration = TimeSpan.FromMinutes(20)
                },
                new Service
                {
                    Id = 3,
                    Name = "Skin Treatment",
                    Price = 500,
                    Duration = TimeSpan.FromMinutes(45)
                },
                new Service
                {
                    Id = 4,
                    Name = "Teeth Cleaning",
                    Price = 400,
                    Duration = TimeSpan.FromMinutes(40)
                },
                new Service
                {
                    Id = 5,
                    Name = "Neurological Check",
                    Price = 600,
                    Duration = TimeSpan.FromMinutes(50)
                }
            );
        }
    }
}

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
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasData(
                new Bill
                {
                    Id = 1,
                    SessionId = 1,
                    PatientId = "P1",
                    Amount = 550,
                    Date = new DateTime(2025, 10, 15),
                    IsPaid = true
                },
                new Bill
                {
                    Id = 2,
                    SessionId = 2,
                    PatientId = "P2",
                    Amount = 500,
                    Date = new DateTime(2025, 10, 16),
                    IsPaid = false
                },
                new Bill
                {
                    Id = 3,
                    SessionId = 5,
                    PatientId = "P1",
                    Amount = 600,
                    Date = new DateTime(2025, 10, 19),
                    IsPaid = true
                }
            );
        }
    }
}

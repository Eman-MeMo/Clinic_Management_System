using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasData(
                new Payment
                {
                    Id = 1,
                    BillId = 1,
                    Amount = 550,
                    Date = new DateTime(2025, 10, 15),
                    PaymentMethod = PaymentMethod.CreditCard
                },
                new Payment
                {
                    Id = 2,
                    BillId = 2,
                    Amount = 500,
                    Date = new DateTime(2025, 10, 16),
                    PaymentMethod = PaymentMethod.Cash
                },
                new Payment
                {
                    Id = 3,
                    BillId = 3,
                    Amount = 600,
                    Date = new DateTime(2025, 10, 19),
                    PaymentMethod = PaymentMethod.Visa
                }
            );
        }
    }
}

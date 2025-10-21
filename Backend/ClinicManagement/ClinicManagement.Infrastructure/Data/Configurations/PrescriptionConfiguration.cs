using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.HasData(
                new Prescription
                {
                    Id = 1,
                    SessionId = 1,
                    MedicationName = "Amlodipine 5mg",
                    Dosage = "Once daily after breakfast",
                    Notes = "Monitor blood pressure regularly."
                },
                new Prescription
                {
                    Id = 2,
                    SessionId = 2,
                    MedicationName = "Cetirizine 10mg",
                    Dosage = "Once daily before sleep",
                    Notes = "Avoid allergens."
                },
                new Prescription
                {
                    Id = 3,
                    SessionId = 3,
                    MedicationName = "Chlorhexidine mouthwash",
                    Dosage = "Twice daily after meals",
                    Notes = "Rinse for 30 seconds each time."
                },
                new Prescription
                {
                    Id = 4,
                    SessionId = 5,
                    MedicationName = "Ibuprofen 200mg",
                    Dosage = "Twice daily after meals",
                    Notes = "For headache relief."
                }
            );
        }
    }
}

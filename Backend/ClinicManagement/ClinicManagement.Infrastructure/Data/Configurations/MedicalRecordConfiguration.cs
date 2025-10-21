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
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.HasData(
                new MedicalRecord
                {
                    Id = 1,
                    PatientId = "P1",
                    DoctorId = "D1",
                    Diagnosis = "Hypertension",
                    Date = new DateTime(2025, 10, 15),
                    Notes = "Patient advised to reduce salt intake."
                },
                new MedicalRecord
                {
                    Id = 2,
                    PatientId = "P2",
                    DoctorId = "D2",
                    Diagnosis = "Skin Allergy",
                    Date = new DateTime(2025, 10, 16),
                    Notes = "Allergy confirmed, prescribed antihistamines."
                },
                new MedicalRecord
                {
                    Id = 3,
                    PatientId = "P3",
                    DoctorId = "D3",
                    Diagnosis = "Dental Plaque",
                    Date = new DateTime(2025, 10, 17),
                    Notes = "Recommended dental cleaning and follow-up."
                },
                new MedicalRecord
                {
                    Id = 4,
                    PatientId = "P1",
                    DoctorId = "D2",
                    Diagnosis = "Migraine",
                    Date = new DateTime(2025, 10, 19),
                    Notes = "Prescribed rest and hydration after neurological session."
                }
            );
        }
    }
}

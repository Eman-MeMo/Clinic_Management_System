using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Infrastructure.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasData(
                new Patient
                {
                    gender = Gender.Female,
                    DateOfBirth = new DateOnly(1995, 5, 12),
                    NationID = "29505121501043"
                },
                new Patient
                {
                    gender = Gender.Male,
                    DateOfBirth = new DateOnly(1988, 8, 22),
                    NationID = "28808221501043"
                },
                new Patient
                {
                    gender = Gender.Female,
                    DateOfBirth = new DateOnly(2000, 3, 15),
                    NationID = "30003151501043"
                }
            );
        }
    }
}

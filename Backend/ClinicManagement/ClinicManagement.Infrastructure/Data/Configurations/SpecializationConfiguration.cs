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
    public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.HasData(
                new Specialization { Id = 1, Name = "Cardiology" },
                new Specialization { Id = 2, Name = "Dermatology" },
                new Specialization { Id = 3, Name = "Dentistry" },
                new Specialization { Id = 4, Name = "Neurology" }
            );
        }
    }
}

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
    public class SessionServiceConfiguration : IEntityTypeConfiguration<SessionService>
    {
        public void Configure(EntityTypeBuilder<SessionService> builder)
        {
            builder.HasData(
                new SessionService { SessionId = 1, ServiceId = 1 },
                new SessionService { SessionId = 1, ServiceId = 2 },
                new SessionService { SessionId = 2, ServiceId = 3 }
            );
        }
    }
}

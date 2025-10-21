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
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string> { UserId = "A1", RoleId = "1" },
                new IdentityUserRole<string> { UserId = "D1", RoleId = "2" },
                new IdentityUserRole<string> { UserId = "D2", RoleId = "2" },
                new IdentityUserRole<string> { UserId = "D3", RoleId = "2" },
                new IdentityUserRole<string> { UserId = "P1", RoleId = "3" },
                new IdentityUserRole<string> { UserId = "P2", RoleId = "3" },
                new IdentityUserRole<string> { UserId = "P3", RoleId = "3" }
            );
        }
    }
}

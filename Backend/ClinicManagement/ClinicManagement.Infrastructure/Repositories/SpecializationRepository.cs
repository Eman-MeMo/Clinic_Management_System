using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class SpecializationRepository:GenericRepository<Specialization>, ISpecializationRepository
    {
        public SpecializationRepository(ClinicDbContext db) : base(db) { }
        public async Task AddByNameAsync(string name)
        {
            var exists = await db.Specializations
                                 .AnyAsync(s => s.Name == name);
            if (exists)
            {
                throw new Exception("This Service already exists!");
            }

            var specialization = new Specialization { Name = name };
            db.Set<Specialization>().Add(specialization);
            await db.SaveChangesAsync();
        }

    }
}

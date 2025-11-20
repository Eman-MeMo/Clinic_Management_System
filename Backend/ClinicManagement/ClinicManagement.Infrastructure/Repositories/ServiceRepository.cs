using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class ServiceRepository:GenericRepository<Service>,IServiceRepository
    {
        public ServiceRepository(ClinicDbContext _db) : base(_db)
        {
        }

        public new async Task AddAsync(Service entity)
        {
            var service = db.Services.FirstOrDefault(s=> s.Name == entity.Name);
            if (service != null)
                throw new Exception("This Service is already exist");

            await base.AddAsync(entity);
        }
    }
}

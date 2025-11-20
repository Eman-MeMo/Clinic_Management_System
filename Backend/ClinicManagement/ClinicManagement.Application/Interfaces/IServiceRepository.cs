using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Task AddAsync(Service entity);
    }
}

using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface ISpecializationRepository:IGenericRepository<Specialization>
    {
        public Task AddByNameAsync(string name);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IUserDeactivationStrategy
    {
        bool CanHandle(string userType);
        Task DeactivateAsync(string userId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IAuditLoggerService
    {
        Task LogAsync(string action, string tableName, string details, string userId);
    }
}

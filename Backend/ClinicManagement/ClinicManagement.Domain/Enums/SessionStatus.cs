using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Enums
{
    public enum SessionStatus
    {
        Scheduled,
        Done,
        Cancelled,
        NoShow, // Patient did not show up for the session
    }
}

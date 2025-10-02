using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Attendances.MarkPresent
{
    public class MarkPresentCommand:IRequest<int>
    {
        public int SessionId { get; set; }
        public string PatientId { get; set; }
        public string? Notes { get; set; }
    }
}

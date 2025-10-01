using ClinicManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Sessions.EndSession
{
    public class EndSessionCommand:IRequest<Unit>
    {
        public int SessionId { get; set; }
        public SessionStatus Status { get; set; }
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Sessions.StartSession
{
    public class StartSessionCommand:IRequest<int>
    {
        public int AppointmentId { get; set; }
    }
}

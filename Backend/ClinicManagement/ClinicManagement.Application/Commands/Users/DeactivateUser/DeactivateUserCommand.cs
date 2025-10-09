using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.DeactivateUser
{
    public class DeactivateUserCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public string UserType { get; set; } // Doctor, Patient, Admin
    }

}

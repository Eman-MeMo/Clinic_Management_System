using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.LoginUser
{
    public class LoginUserCommand:IRequest<object>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

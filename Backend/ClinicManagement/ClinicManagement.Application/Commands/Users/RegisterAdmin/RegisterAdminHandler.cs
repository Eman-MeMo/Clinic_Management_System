using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.RegisterAdmin
{
    public class RegisterAdminHandler:IRequestHandler<RegisterAdminCommand, object>
    {
        private readonly IAccountService userService;
        public RegisterAdminHandler(IAccountService _userService)
        {
            userService = _userService;
        }
        public Task<object> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            if(request== null)
                throw new ArgumentNullException(nameof(request));

            return userService.RegisterAdminAsync(request.FirstName, request.LastName, request.PhoneNumber, request.Email, request.Password, request.ConfirmPassword);
        }
    }
}

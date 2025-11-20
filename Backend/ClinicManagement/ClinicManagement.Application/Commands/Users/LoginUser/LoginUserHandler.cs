using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, object>
    {
        private readonly IAccountService accountService;
        public LoginUserHandler(IAccountService _accountService)
        {
            accountService = _accountService;
        }
        public async Task<object> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await accountService.LoginAsync(request.Email,request.Password);
        }
    }
}

using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, Unit>
    {
        private readonly IAccountService accountService;
        public LoginUserHandler(IAccountService _accountService)
        {
            accountService = _accountService;
        }
        public async Task<Unit> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await accountService.LoginAsync(request.Email,request.Password);
            return Unit.Value;
        }
    }
}

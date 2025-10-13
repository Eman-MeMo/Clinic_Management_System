using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.RegisterDoctor
{
    public class RegisterDoctorHandler : IRequestHandler<RegisterDoctorCommand, Object>
    {
        private readonly IAccountService _userService;
        public RegisterDoctorHandler(IAccountService userService)
        {
            _userService = userService;
        }
        public async Task<object> Handle(RegisterDoctorCommand request, CancellationToken cancellationToken)
        {
            if(request== null)
                throw new ArgumentNullException(nameof(request));

            return await _userService.RegisterDoctorAsync(request.FirstName, request.LastName, request.PhoneNumber, request.Email, request.Password, request.ConfirmPassword, request.SpecializationId);
        }
    }
}

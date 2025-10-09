using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.RegisterPatient
{
    public class RegisterPatientHandler : IRequestHandler<RegisterPatientCommand, object>
    {
        private readonly IUserService userService;
        public RegisterPatientHandler(IUserService _userService) 
        {
            userService = _userService;
        }
        public async Task<object> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            return await userService.RegisterPatientAsync(request.FirstName,request.Lastname,request.PhoneNumber,request.Email,request.Password,request.ConfirmPassword,request.NationID,request.gender,request.DateOfBirth);
        }
    }
}

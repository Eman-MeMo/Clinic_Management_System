using ClinicManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.RegisterPatient
{
    public class RegisterPatientCommand:IRequest<object>
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Gender gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string NationID { get; set; }

    }
}

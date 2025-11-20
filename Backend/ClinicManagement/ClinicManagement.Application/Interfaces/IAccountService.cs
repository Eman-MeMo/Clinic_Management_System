using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IAccountService
    {
        Task<object> LoginAsync(string  Email,string Password);
        Task<object> RegisterPatientAsync(string FirstName, string Lastname, string PhoneNumber, string Email, string Password, string ConfirmPassword, string NationId, Gender gender, DateOnly dateOfBirth);
        Task<object> RegisterDoctorAsync(string FirstName, string Lastname, string PhoneNumber, string Email, string Password, string ConfirmPassword, int SpecializationId);
        Task<object> RegisterAdminAsync(string FirstName,string Lastname,string PhoneNumber,string Email,string Password,string ConfirmPassword);
    }
}

using ClinicManagement.Application.Commands.Users.LoginUser;
using ClinicManagement.Application.Commands.Users.RegisterAdmin;
using ClinicManagement.Application.Commands.Users.RegisterDoctor;
using ClinicManagement.Application.Commands.Users.RegisterPatient;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AccountDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator mediator; 

        public AccountController(IMediator _mediator)
        {
            mediator = _mediator;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("Patient_Register")]
        public async Task<IActionResult> PatientRegister([FromBody] RegisterPatientCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("Doctor_Register")]
        public async Task<IActionResult> DoctorRegister([FromBody] RegisterDoctorCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("Admin_Register")]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterAdminCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}

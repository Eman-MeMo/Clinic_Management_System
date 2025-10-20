using AutoMapper;
using ClinicManagement.Application.Commands.Users.DeactivateUser;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AdminDTOs;
using ClinicManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService<Admin> adminService;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public AdminController(IUserService<Admin> _adminService, IMapper _mapper, IMediator _mediator)
        {
            adminService = _adminService;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await adminService.GetAllAsync();
            if (admins == null || !admins.Any())
            {
                return NotFound("No admins found.");
            }
            var adminDtos = mapper.Map<IEnumerable<AdminDto>>(admins);
            return Ok(adminDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            var admin = await adminService.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            var adminDto = mapper.Map<AdminDto>(admin);
            return Ok(adminDto);
        }

        [HttpGet("Email/{email}")]
        public async Task<IActionResult> GetAdminByEmail(string email)
        {
            var admin = await adminService.GetByEmailAsync(email);
            if (admin == null)
            {
                return NotFound($"Admin with Email {email} not found.");
            }
            var adminDto = mapper.Map<AdminDto>(admin);
            return Ok(adminDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] AdminDto adminDto)
        {
            if (adminDto == null)
            {
                return BadRequest("Admin data is null.");
            }
            var admin = await adminService.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            mapper.Map(adminDto, admin);
            await adminService.Update(admin);

            var resultDto = mapper.Map<AdminDto>(admin);
            return Ok(resultDto);
        }

        [HttpPut("Deactivate/{id}")]
        public async Task<IActionResult> DeactivateAdmin(string id)
        {
            var admin = await adminService.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }

            await mediator.Send(new DeactivateUserCommand { UserId = id, UserType = "Admin" });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var admin = await adminService.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            await adminService.Delete(admin);
            return NoContent();
        }
    }
}

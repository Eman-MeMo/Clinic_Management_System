using AutoMapper;
using ClinicManagement.Application.Commands.Bills.CreateBill;
using ClinicManagement.Application.Commands.Bills.MarkBillAsPaid;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Bills.GetUnpaidBillsByPatient;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.BillDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public BillController(IUnitOfWork _unitOfWork, IMapper _mapper,IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBills([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var bills = unitOfWork.BillRepository.GetAllAsQueryable();
            var totalCount = await bills.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await bills.Skip(paginationSkip).Take(pageSize).ToListAsync();

            var billDtos = mapper.Map<List<BillDto>>(items);
            var result = new PaginatedResultDto<BillDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = billDtos
            };
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid bill ID.");
            }

            var bill = await unitOfWork.BillRepository.GetByIdAsync(id);
            if (bill == null)
            {
                return NotFound($"Bill with ID {id} not found.");
            }
            return Ok(mapper.Map<BillDto>(bill));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillCommand createBillCommand)
        {
            var billId= await mediator.Send(createBillCommand);
            return CreatedAtAction(nameof(GetBillById), new { id = billId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] BillDto BillDto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid bill ID.");
            }

            if (BillDto == null)
            {
                return BadRequest("Bill data is null.");
            }

            var existingBill = await unitOfWork.BillRepository.GetByIdAsync(id);
            if (existingBill == null)
            {
                return NotFound($"Bill with ID {id} not found.");
            }

            mapper.Map(BillDto, existingBill);
            unitOfWork.BillRepository.Update(existingBill);
            await unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid bill ID.");
            }

            var bill = await unitOfWork.BillRepository.GetByIdAsync(id);
            if (bill == null)
            {
                return NotFound($"Bill with ID {id} not found.");
            }

            unitOfWork.BillRepository.Delete(bill);
            await unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("unpaid/{patientId}")]
        public async Task<IActionResult> GetUnpaidBillsByPatient(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest("Patient ID is required.");
            }

            var unpaidBills = await mediator.Send(new GetUnpaidBillsByPatientQuery { PatientId = patientId });
            return Ok(unpaidBills);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetBillsBySession(int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest("Invalid session ID.");
            }

            var bills = await unitOfWork.BillRepository.GetBySessionAsync(sessionId);
            if (bills == null || !bills.Any())
            {
                return NotFound($"No bills found for session ID {sessionId}.");
            }
            return Ok(mapper.Map<IEnumerable<BillDto>>(bills));
        }

        [HttpPut("mark-paid/{billId}")]
        public async Task<IActionResult> MarkBillAsPaid(int billId)
        {
            if (billId <= 0)
            {
                return NotFound("Invalid bill ID.");
            }

            var result = await mediator.Send(new MarkBillAsPaidCommand { Id = billId });

            if (!result)
            {
                return BadRequest("Bill could not be marked as paid. Either it doesn’t exist, is already paid, or payment is invalid.");
            }
            return NoContent();
        }
    }
}

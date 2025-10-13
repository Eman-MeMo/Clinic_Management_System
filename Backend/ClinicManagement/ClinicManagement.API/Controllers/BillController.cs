using AutoMapper;
using ClinicManagement.Application.Commands.Bills.CreateBill;
using ClinicManagement.Application.Commands.Bills.MarkBillAsPaid;
using ClinicManagement.Application.Interfaces;
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
        private readonly ILogger<BillController> logger;
        private readonly IMediator mediator;

        public BillController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<BillController> _logger,IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBills([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Fetching all bills - PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
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
            logger.LogInformation("Returning {Count} bills", billDtos.Count);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillById(int id)
        {
            logger.LogInformation("Fetching bill with ID: {Id}", id);
            if (id <= 0)
            {
                logger.LogWarning("Invalid bill ID: {Id}", id);
                return BadRequest("Invalid bill ID.");
            }

            var bill = await unitOfWork.BillRepository.GetByIdAsync(id);
            if (bill == null)
            {
                logger.LogWarning("Bill with ID {Id} not found", id);
                return NotFound($"Bill with ID {id} not found.");
            }

            logger.LogInformation("Bill with ID {Id} retrieved", id);
            return Ok(mapper.Map<BillDto>(bill));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillCommand createBillCommand)
        {
            logger.LogInformation("Creating new bill");
            var billId= await mediator.Send(createBillCommand);

            logger.LogInformation("Bill created with ID {BillId}", billId);
            return CreatedAtAction(nameof(GetBillById), new { id = billId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] BillDto BillDto)
        {
            logger.LogInformation("Updating bill with ID: {Id}", id);
            if (id <= 0)
            {
                logger.LogWarning("Invalid bill ID: {Id}", id);
                return BadRequest("Invalid bill ID.");
            }

            if (BillDto == null)
            {
                logger.LogWarning("BillDto is null for update operation");
                return BadRequest("Bill data is null.");
            }

            var existingBill = await unitOfWork.BillRepository.GetByIdAsync(id);
            if (existingBill == null)
            {
                logger.LogWarning("Bill with ID {Id} not found", id);
                return NotFound($"Bill with ID {id} not found.");
            }

            mapper.Map(BillDto, existingBill);
            unitOfWork.BillRepository.Update(existingBill);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Bill with ID {Id} updated", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            logger.LogInformation("Deleting bill with ID: {Id}", id);
            if (id <= 0)
            {
                logger.LogWarning("Invalid bill ID: {Id}", id);
                return BadRequest("Invalid bill ID.");
            }

            var bill = await unitOfWork.BillRepository.GetByIdAsync(id);
            if (bill == null)
            {
                logger.LogWarning("Bill with ID {Id} not found", id);
                return NotFound($"Bill with ID {id} not found.");
            }

            unitOfWork.BillRepository.Delete(bill);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Bill with ID {Id} deleted", id);
            return NoContent();
        }

        [HttpGet("unpaid/{patientId}")]
        public async Task<IActionResult> GetUnpaidBillsByPatient(string patientId)
        {
            logger.LogInformation("Fetching unpaid bills for patient ID: {PatientId}", patientId);
            if (string.IsNullOrEmpty(patientId))
            {
                logger.LogWarning("Patient ID is null or empty");
                return BadRequest("Patient ID is required.");
            }

            var unpaidBills = await unitOfWork.BillRepository.GetUnpaidBillsByPatientAsync(patientId);
            if (unpaidBills == null || !unpaidBills.Any())
            {
                logger.LogWarning("No unpaid bills found for patient ID {PatientId}", patientId);
                return NotFound($"No unpaid bills found for patient ID {patientId}.");
            }

            logger.LogInformation("Found {Count} unpaid bills for patient ID {PatientId}", unpaidBills.Count(), patientId);
            return Ok(mapper.Map<IEnumerable<BillDto>>(unpaidBills));
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetBillsBySession(int sessionId)
        {
            logger.LogInformation("Fetching bills for session ID: {SessionId}", sessionId);
            if (sessionId <= 0)
            {
                logger.LogWarning("Invalid session ID: {SessionId}", sessionId);
                return BadRequest("Invalid session ID.");
            }

            var bills = await unitOfWork.BillRepository.GetBySessionAsync(sessionId);
            if (bills == null || !bills.Any())
            {
                logger.LogWarning("No bills found for session ID {SessionId}", sessionId);
                return NotFound($"No bills found for session ID {sessionId}.");
            }

            logger.LogInformation("Found {Count} bills for session ID {SessionId}", bills.Count(), sessionId);
            return Ok(mapper.Map<IEnumerable<BillDto>>(bills));
        }

        [HttpPut("mark-paid/{billId}")]
        public async Task<IActionResult> MarkBillAsPaid(int billId)
        {
            logger.LogInformation("Marking bill with ID {BillId} as paid", billId);

            if (billId <= 0)
            {
                logger.LogWarning("Invalid bill ID: {BillId}", billId);
                return BadRequest("Invalid bill ID.");
            }

            var result = await mediator.Send(new MarkBillAsPaidCommand { Id = billId });

            if (!result)
            {
                logger.LogWarning("Failed to mark bill {BillId} as paid", billId);
                return BadRequest("Bill could not be marked as paid. Either it doesn’t exist, is already paid, or payment is invalid.");
            }

            logger.LogInformation("Bill with ID {BillId} marked as paid successfully", billId);
            return NoContent();
        }
    }
}

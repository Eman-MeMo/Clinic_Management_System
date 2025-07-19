using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.BillDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public BillController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBills([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var bills =  unitOfWork.BillRepository.GetAllAsQueryable();
            var totalCount = await bills.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await bills
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

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
        public async Task<IActionResult> CreateBill([FromBody] CraeteBillDto craeteBillDto)
        {
            if (craeteBillDto == null)
            {
                return BadRequest("Bill data is null.");
            }
            var patient = await unitOfWork.PatientRepository.GetByIdAsync(craeteBillDto.PatientId);
            if (patient == null)
            {
                return NotFound($"Patient with ID {craeteBillDto.PatientId} not found.");
            }
            var session = await unitOfWork.SessionRepository.GetByIdAsync(craeteBillDto.SessionId);
            if (session == null)
            {
                return NotFound($"Session with ID {craeteBillDto.SessionId} not found.");
            }
            var existingBill = await unitOfWork.BillRepository.GetBySessionAsync(craeteBillDto.SessionId);
            if (existingBill.Any())
            {
                return Conflict($"A bill already exists for session ID {craeteBillDto.SessionId}.");
            }
            var bill = mapper.Map<Bill>(craeteBillDto);
            await unitOfWork.BillRepository.AddAsync(bill);
            await unitOfWork.SaveChangesAsync();

            // Log the creation of the bill
            await auditLogger.LogAsync("Bill Created", "Bill", $"Bill for Patient ID {bill.PatientId} created by user {UserId}", UserId);
            return CreatedAtAction(nameof(GetBillById), new { id = bill.Id }, mapper.Map<BillDto>(bill));
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

            // Log the update of the bill
            await auditLogger.LogAsync("Bill Updated", "Bill", $"Bill with ID {id} updated by user {UserId}", UserId);
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

            // Log the deletion of the bill
            await auditLogger.LogAsync("Bill Deleted", "Bill", $"Bill with ID {id} deleted by user {UserId}", UserId);
            return NoContent();
        }
        [HttpGet("unpaid/{patientId}")]
        public async Task<IActionResult> GetUnpaidBillsByPatient(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest("Patient ID is required.");
            }
            var unpaidBills = await unitOfWork.BillRepository.GetUnpaidBillsByPatientAsync(patientId);
            if (unpaidBills == null || !unpaidBills.Any())
            {
                return NotFound($"No unpaid bills found for patient ID {patientId}.");
            }
            return Ok(mapper.Map<IEnumerable<BillDto>>(unpaidBills));
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
                return BadRequest("Invalid bill ID.");
            }

            var bill = await unitOfWork.BillRepository.GetByIdAsync(billId);
            if (bill == null)
            {
                return NotFound($"Bill with ID {billId} not found.");
            }

            if (bill.IsPaid)
            {
                return BadRequest("Bill is already marked as paid.");
            }

            var payment = await unitOfWork.PaymentRepository
                .GetPaymentByBillIdAsync(billId); 

            if (payment == null || payment.Amount != bill.Amount)
            {
                return BadRequest("Cannot mark bill as paid. Payment not found or amount doesn't match the bill total.");
            }

            bill.IsPaid = true;
            await unitOfWork.SaveChangesAsync();

            // Log the action of marking the bill as paid
            await auditLogger.LogAsync(
                "Bill Marked as Paid",
                "Bill",
                $"Bill with ID {billId} marked as paid based on matching payment by user {UserId}",
                UserId);

            return NoContent();
        }

    }
}

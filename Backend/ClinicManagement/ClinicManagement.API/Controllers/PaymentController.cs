using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PaymentDTOs;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public PaymentController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var payments =  unitOfWork.PaymentRepository.GetAllAsQueryable();
            var totalCount = await payments.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await payments
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

            var paymentDtos = mapper.Map<List<PaymentDto>>(items);
            var result = new PaginatedResultDto<PaymentDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = paymentDtos
            };
            return Ok(result);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment ID.");
            }
            var payment = await unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }
            return Ok(mapper.Map<PaymentDto>(payment));
        }
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            if (createPaymentDto == null)
            {
                return BadRequest("Invalid payment data.");
            }
            var bill = await unitOfWork.BillRepository.GetByIdAsync(createPaymentDto.BillId);
            if (bill == null)
            {
                return NotFound($"Bill with ID {createPaymentDto.BillId} not found.");
            }
            if (bill.IsPaid)
            {
                return BadRequest($"Bill with ID {createPaymentDto.BillId} is already marked as paid.");
            }
            if (createPaymentDto.Amount != bill.Amount)
                return BadRequest("Payment amount must match the bill amount.");

            var payment = mapper.Map<Domain.Entities.Payment>(createPaymentDto);
            await unitOfWork.PaymentRepository.AddAsync(payment);
            bill.IsPaid = true; 
            await unitOfWork.SaveChangesAsync();

            // Update the bill status to paid
            await auditLogger.LogAsync("Payment Created", "Payment", $"Payment for Bill ID {payment.BillId} created by user {UserId}", UserId);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, mapper.Map<PaymentDto>(payment));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] PaymentDto PaymentDto)
        {
            if (PaymentDto == null || id <= 0)
            {
                return BadRequest("Invalid payment data.");
            }
            if (id != PaymentDto.Id)
            {
                return BadRequest("Payment ID mismatch.");
            }
            var existingPayment = await unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (existingPayment == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }
            mapper.Map(PaymentDto, existingPayment);
            unitOfWork.PaymentRepository.Update(existingPayment);
            await unitOfWork.SaveChangesAsync();

            // Log the update action
            await auditLogger.LogAsync("Payment Updated", "Payment", $"Payment ID {id} updated by user {UserId}", UserId);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment ID.");
            }
            var existingPayment = await unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (existingPayment == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }
            unitOfWork.PaymentRepository.Delete(existingPayment);
            await unitOfWork.SaveChangesAsync();

            // Log the deletion action
            await auditLogger.LogAsync("Payment Deleted", "Payment", $"Payment ID {id} deleted by user {UserId}", UserId);
            return NoContent();
        }
        [HttpGet("by-patient/{patientId}")]
        public async Task<IActionResult> GetPaymentsByPatient(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest("Invalid patient ID.");
            }
            var payments = await unitOfWork.PaymentRepository.GetByPatientAsync(patientId);
            if (payments == null || !payments.Any())
            {
                return NotFound($"No payments found for patient with ID {patientId}.");
            }
            return Ok(mapper.Map<IEnumerable<PaymentDto>>(payments));
        }
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetPaymentsByDateRange(DateTime start, DateTime end)
        {
            if (start > end)
            {
                return BadRequest("Start date cannot be after end date.");
            }
            var payments = await unitOfWork.PaymentRepository.GetByDateRangeAsync(start, end);
            if (payments == null || !payments.Any())
            {
                return NotFound("No payments found in the specified date range.");
            }
            return Ok(mapper.Map<IEnumerable<PaymentDto>>(payments));
        }
        [HttpGet("status/{billId}")]
        public async Task<IActionResult> GetPaymentStatus(int billId)
        {
            if (billId <= 0)
            {
                return BadRequest("Invalid bill ID.");
            }
            var isPaid = await unitOfWork.PaymentRepository.GetStatusAsync(billId);
            return Ok(new { BillId = billId, IsPaid = isPaid });
        }
    }
}

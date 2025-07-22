using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PaymentDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<PaymentController> logger;

        public PaymentController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<PaymentController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Getting all payments - Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
            var payments = unitOfWork.PaymentRepository.GetAllAsQueryable();
            var totalCount = await payments.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await payments.Skip(paginationSkip).Take(pageSize).ToListAsync();

            var paymentDtos = mapper.Map<List<PaymentDto>>(items);
            var result = new PaginatedResultDto<PaymentDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = paymentDtos
            };
            logger.LogInformation("Retrieved {Count} payments", paymentDtos.Count);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            logger.LogInformation("Fetching payment with ID {Id}", id);
            if (id <= 0)
            {
                logger.LogWarning("Invalid payment ID: {Id}", id);
                return BadRequest("Invalid payment ID.");
            }
            var payment = await unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                logger.LogWarning("Payment with ID {Id} not found", id);
                return NotFound($"Payment with ID {id} not found.");
            }
            logger.LogInformation("Payment with ID {Id} retrieved successfully", id);
            return Ok(mapper.Map<PaymentDto>(payment));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            logger.LogInformation("Creating payment for bill ID {BillId}", createPaymentDto?.BillId);
            if (createPaymentDto == null)
            {
                logger.LogWarning("Payment creation failed: data is null");
                return BadRequest("Invalid payment data.");
            }

            var bill = await unitOfWork.BillRepository.GetByIdAsync(createPaymentDto.BillId);
            if (bill == null)
            {
                logger.LogWarning("Bill with ID {BillId} not found", createPaymentDto.BillId);
                return NotFound($"Bill with ID {createPaymentDto.BillId} not found.");
            }

            if (bill.IsPaid)
            {
                logger.LogWarning("Bill with ID {BillId} is already paid", createPaymentDto.BillId);
                return BadRequest($"Bill with ID {createPaymentDto.BillId} is already marked as paid.");
            }

            if (createPaymentDto.Amount != bill.Amount)
            {
                logger.LogWarning("Mismatch between payment and bill amount for Bill ID {BillId}", createPaymentDto.BillId);
                return BadRequest("Payment amount must match the bill amount.");
            }

            var payment = mapper.Map<Domain.Entities.Payment>(createPaymentDto);
            await unitOfWork.PaymentRepository.AddAsync(payment);
            bill.IsPaid = true;
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Payment created successfully for Bill ID {BillId}", createPaymentDto.BillId);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, mapper.Map<PaymentDto>(payment));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] PaymentDto PaymentDto)
        {
            logger.LogInformation("Updating payment with ID {Id}", id);
            if (PaymentDto == null || id <= 0)
            {
                logger.LogWarning("Invalid update data for payment ID {Id}", id);
                return BadRequest("Invalid payment data.");
            }
            if (id != PaymentDto.Id)
            {
                logger.LogWarning("Payment ID mismatch: route ID = {RouteId}, body ID = {BodyId}", id, PaymentDto.Id);
                return BadRequest("Payment ID mismatch.");
            }

            var existingPayment = await unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (existingPayment == null)
            {
                logger.LogWarning("Payment with ID {Id} not found for update", id);
                return NotFound($"Payment with ID {id} not found.");
            }

            mapper.Map(PaymentDto, existingPayment);
            unitOfWork.PaymentRepository.Update(existingPayment);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Payment with ID {Id} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            logger.LogInformation("Deleting payment with ID {Id}", id);
            if (id <= 0)
            {
                logger.LogWarning("Invalid payment ID: {Id}", id);
                return BadRequest("Invalid payment ID.");
            }

            var existingPayment = await unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (existingPayment == null)
            {
                logger.LogWarning("Payment with ID {Id} not found for deletion", id);
                return NotFound($"Payment with ID {id} not found.");
            }

            unitOfWork.PaymentRepository.Delete(existingPayment);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Payment with ID {Id} deleted successfully", id);
            return NoContent();
        }

        [HttpGet("by-patient/{patientId}")]
        public async Task<IActionResult> GetPaymentsByPatient(string patientId)
        {
            logger.LogInformation("Fetching payments for patient ID {PatientId}", patientId);
            if (string.IsNullOrEmpty(patientId))
            {
                logger.LogWarning("Invalid patient ID.");
                return BadRequest("Invalid patient ID.");
            }

            var payments = await unitOfWork.PaymentRepository.GetByPatientAsync(patientId);
            if (payments == null || !payments.Any())
            {
                logger.LogWarning("No payments found for patient ID {PatientId}", patientId);
                return NotFound($"No payments found for patient with ID {patientId}.");
            }

            logger.LogInformation("{Count} payments found for patient ID {PatientId}", payments.Count(), patientId);
            return Ok(mapper.Map<IEnumerable<PaymentDto>>(payments));
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetPaymentsByDateRange(DateTime start, DateTime end)
        {
            logger.LogInformation("Fetching payments from {Start} to {End}", start, end);
            if (start > end)
            {
                logger.LogWarning("Invalid date range: start date is after end date.");
                return BadRequest("Start date cannot be after end date.");
            }

            var payments = await unitOfWork.PaymentRepository.GetByDateRangeAsync(start, end);
            if (payments == null || !payments.Any())
            {
                logger.LogWarning("No payments found between {Start} and {End}", start, end);
                return NotFound("No payments found in the specified date range.");
            }

            logger.LogInformation("{Count} payments found in date range", payments.Count());
            return Ok(mapper.Map<IEnumerable<PaymentDto>>(payments));
        }

        [HttpGet("status/{billId}")]
        public async Task<IActionResult> GetPaymentStatus(int billId)
        {
            logger.LogInformation("Checking payment status for bill ID {BillId}", billId);
            if (billId <= 0)
            {
                logger.LogWarning("Invalid bill ID: {BillId}", billId);
                return BadRequest("Invalid bill ID.");
            }

            var isPaid = await unitOfWork.PaymentRepository.GetStatusAsync(billId);
            logger.LogInformation("Bill ID {BillId} payment status: {Status}", billId, isPaid ? "Paid" : "Unpaid");
            return Ok(new { BillId = billId, IsPaid = isPaid });
        }
    }
}

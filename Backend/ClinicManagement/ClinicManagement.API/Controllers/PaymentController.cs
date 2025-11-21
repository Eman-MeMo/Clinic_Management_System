using AutoMapper;
using ClinicManagement.Application.Commands.Payments.CreatePayment;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Payments.GetPaymentsByDateRange;
using ClinicManagement.Application.Queries.Payments.GetPaymentsByPatient;
using ClinicManagement.Application.Queries.Payments.GetPaymentStatus;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PaymentDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Patient")]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public PaymentController(IUnitOfWork _unitOfWork, IMapper _mapper, IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
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
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
        {
            if (command == null)
            {
                return BadRequest("Invalid payment data.");
            }
            var paymnentId= await mediator.Send(command);
            var payment = await unitOfWork.PaymentRepository.GetByIdAsync(paymnentId);
            var paymentDto = mapper.Map<PaymentDto>(payment);
            return CreatedAtAction(nameof(GetPaymentById), new { id = paymnentId},paymentDto);
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
            return NoContent();
        }

        [HttpGet("by-patient/{patientId}")]
        public async Task<IActionResult> GetPaymentsByPatient(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest("Invalid patient ID.");
            }

            var payments = await mediator.Send(new GetPaymentsByPatientQuery { PatientId = patientId });
            return Ok(payments);
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetPaymentsByDateRange(DateTime start, DateTime end)
        {
            if (start > end)
            {
                return BadRequest("Start date cannot be after end date.");
            }

            var payments = await mediator.Send(new GetPaymentsByDateRangeQuery { start = start, end = end });
            return Ok(payments);
        }

        [HttpGet("status/{billId}")]
        public async Task<IActionResult> GetPaymentStatus(int billId)
        {
            if (billId <= 0)
            {
                return BadRequest("Invalid bill ID.");
            }

            var isPaid = await mediator.Send(new GetPaymentStatusQuery { billId = billId });
            return Ok(new { BillId = billId, IsPaid = isPaid });
        }
    }
}

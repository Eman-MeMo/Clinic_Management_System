using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.BillDTOs;
using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services
{
    public class BillingService:IBillingService
    {
        private readonly IUnitOfWork unitOfWork;
        public BillingService(IUnitOfWork _unitOfWork) {
            unitOfWork= _unitOfWork;
        }

        public async Task<bool> MarkAsPaidAsync(int billId)
        {
            var bill = await unitOfWork.BillRepository.GetByIdAsync(billId);
            if (bill == null || bill.IsPaid)
                return false;

            var payment = await unitOfWork.PaymentRepository.GetByBillIdAsync(billId);

            if (payment == null || payment.Amount != bill.Amount)
            {
                return false;
            }

            bill.IsPaid = true;
            await unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<int> CreateBillAsync(string patientId, int sessionId, decimal amount)
        {

            var patient = await unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                throw new InvalidOperationException($"Patient with ID {patientId} not found.");
            }

            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                throw new InvalidOperationException($"Session with ID {sessionId} not found.");
            }

            var existingBill = await unitOfWork.BillRepository.GetBySessionAsync(sessionId);
            if (existingBill.Any())
            {
                throw new InvalidOperationException($"A bill already exists for session ID {sessionId}.");
            }
            var bill = new Bill
                {
                    PatientId = patientId,
                    SessionId = sessionId,
                    Amount = amount,
                    IsPaid = false,
                    Date = DateTime.UtcNow
                };
            await unitOfWork.BillRepository.AddAsync(bill);
            await unitOfWork.SaveChangesAsync();
            return bill.Id;
        }
    }
}

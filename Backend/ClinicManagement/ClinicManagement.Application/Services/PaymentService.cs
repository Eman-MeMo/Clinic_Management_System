using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.PaymentDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Services
{
    public class PaymentService:IPaymentService
    {
        private readonly IUnitOfWork unitOfWork;
        public PaymentService(IUnitOfWork _unitOfWork) {
            unitOfWork = _unitOfWork;
        }
        public async Task<int> CreatePayment(int billId,decimal amount,PaymentMethod method)
        {
            
            var bill = await unitOfWork.BillRepository.GetByIdAsync(billId);
            if (bill == null)
                throw new InvalidOperationException("Bill not found.");

            if (bill.IsPaid)
                throw new InvalidOperationException("Bill is already marked as paid.");

            if(amount != bill.Amount)
                throw new InvalidOperationException("Payment amount must match the bill amount.");


            var payment = new Payment
            {
                BillId = billId,
                Amount = amount,
                Date = DateTime.UtcNow,
                PaymentMethod = method
            };

            await unitOfWork.PaymentRepository.AddAsync(payment);
            bill.IsPaid = true;
            await unitOfWork.SaveChangesAsync();
            return payment.Id;
        }
    }
}

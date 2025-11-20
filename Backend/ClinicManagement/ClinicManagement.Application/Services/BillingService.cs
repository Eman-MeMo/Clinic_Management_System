using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services
{
    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork unitOfWork;

        public BillingService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public async Task<bool> MarkAsPaidAsync(int billId)
        {
            var bill = await unitOfWork.BillRepository.GetByIdAsync(billId);

            if (bill == null || bill.IsPaid)
                return false;

            var payment = await unitOfWork.PaymentRepository.GetByBillIdAsync(billId);

            if (payment == null || payment.Amount != bill.Amount)
                return false;

            bill.IsPaid = true;
            await unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<int> CreateBillAsync(string patientId, int sessionId)
        {
            var patient = await unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
                throw new InvalidOperationException($"Patient with ID {patientId} not found.");

            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                throw new InvalidOperationException($"Session with ID {sessionId} not found.");

            var bills = await unitOfWork.BillRepository.GetBySessionAsync(sessionId);
            if (bills.Any())
                throw new InvalidOperationException("A bill already exists for this session.");

            var sessionServices = await unitOfWork.SessionServiceRepository.GetAllBySessionIdAsync(sessionId);
            if (!sessionServices.Any())
                throw new InvalidOperationException("No services selected for this session. Cannot create bill.");

            decimal amount = sessionServices.Sum(s => s.Service.Price);

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
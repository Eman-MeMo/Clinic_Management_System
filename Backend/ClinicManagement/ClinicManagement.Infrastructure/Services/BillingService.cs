using ClinicManagement.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
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
    }
}

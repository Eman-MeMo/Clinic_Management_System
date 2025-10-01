using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Bills.MarkBillAsPaid
{
    public class MarkBillAsPaidHandler : IRequestHandler<MarkBillAsPaidCommand, bool>
    {
        private readonly IBillingService billingService;
        public MarkBillAsPaidHandler(IBillingService _billingService)
        {
            billingService = _billingService;
        }
        public async Task<bool> Handle(MarkBillAsPaidCommand request, CancellationToken cancellationToken)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            return await billingService.MarkAsPaidAsync(request.Id);
        }
    }
}

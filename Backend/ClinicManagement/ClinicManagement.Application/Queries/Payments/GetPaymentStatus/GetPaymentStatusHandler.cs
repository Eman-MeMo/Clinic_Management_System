using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Payments.GetPaymentStatus
{
    public class GetPaymentStatusHandler:IRequestHandler<GetPaymentStatusQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        public GetPaymentStatusHandler(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<bool> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await unitOfWork.PaymentRepository.GetStatusAsync(request.billId);
        }
    }
}

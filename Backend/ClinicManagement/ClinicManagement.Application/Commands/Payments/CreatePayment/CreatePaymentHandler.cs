using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Payments.CreatePayment
{
    public class CreatePaymentHandler:IRequestHandler<CreatePaymentCommand, int>
    {
        private readonly IPaymentService paymentService;
        public CreatePaymentHandler(Interfaces.IPaymentService _paymentService)
        {
            paymentService = _paymentService;
        }
        public async Task<int> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            var paymentId = await paymentService.CreatePayment(request.BillId, request.PaymentMethod);
            return paymentId;
        }
    }
}

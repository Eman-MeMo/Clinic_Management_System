using ClinicManagement.Application.Commands.Payments.CreatePayment;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Payments
{
    public class CreatePaymentHandlerTest
    {
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly CreatePaymentHandler _handler;

        public CreatePaymentHandlerTest()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _handler = new CreatePaymentHandler(_paymentServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CallsCreatePaymentAndReturnsId()
        {
            var command = new CreatePaymentCommand
            {
                BillId = 1,
                PaymentMethod = PaymentMethod.CreditCard
            };

            _paymentServiceMock
                .Setup(p => p.CreatePayment(command.BillId, command.PaymentMethod))
                .ReturnsAsync(101);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(101, result);
            _paymentServiceMock.Verify(p =>
                p.CreatePayment(command.BillId, command.PaymentMethod), Times.Once);
        }

        [Fact]
        public async Task Handle_RequestIsNull_ThrowsArgumentNullException()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));

            Assert.Equal("request", ex.ParamName);
            Assert.Contains("Request cannot be null", ex.Message);
        }
    }
}
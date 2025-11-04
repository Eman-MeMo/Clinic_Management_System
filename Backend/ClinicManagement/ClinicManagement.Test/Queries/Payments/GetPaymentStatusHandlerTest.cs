using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Payments.GetPaymentStatus;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.Payments
{
    public class GetPaymentStatusHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GetPaymentStatusHandler _handler;

        public GetPaymentStatusHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new GetPaymentStatusHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsTrue()
        {
            var query = new GetPaymentStatusQuery { billId = 1 };
            _unitOfWorkMock.Setup(u => u.PaymentRepository.GetStatusAsync(query.billId))
                           .ReturnsAsync(true);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.PaymentRepository.GetStatusAsync(query.billId), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsFalse()
        {
            var query = new GetPaymentStatusQuery { billId = 2 };
            _unitOfWorkMock.Setup(u => u.PaymentRepository.GetStatusAsync(query.billId))
                           .ReturnsAsync(false);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result);
            _unitOfWorkMock.Verify(u => u.PaymentRepository.GetStatusAsync(query.billId), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}
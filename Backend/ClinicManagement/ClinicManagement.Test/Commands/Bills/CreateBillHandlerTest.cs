using ClinicManagement.Application.Commands.Bills.CreateBill;
using ClinicManagement.Application.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Bills
{
    public class CreateBillHandlerTest
    {
        private readonly Mock<IBillingService> _billingServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateBillHandler _handler;

        public CreateBillHandlerTest()
        {
            _billingServiceMock = new Mock<IBillingService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateBillHandler(_billingServiceMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsBillId()
        {
            var command = new CreateBillCommand
            {
                PatientId = "P1",
                SessionId = 2,
                Amount = 500
            };

            _billingServiceMock
                .Setup(s => s.CreateBillAsync(command.PatientId, command.SessionId, command.Amount))
                .ReturnsAsync(10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(10, result);
            _billingServiceMock.Verify(s => s.CreateBillAsync("P1", 2, 500), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            CreateBillCommand command = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Application.Interfaces;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using ClinicManagement.Infrastructure.Repositories;

namespace ClinicManagement.Test.Services
{
    public class PaymentServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBillRepository> _billRepoMock;
        private readonly Mock<IPaymentRepository> _paymentRepoMock;
        private readonly IPaymentService _service;

        public PaymentServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _billRepoMock = new Mock<IBillRepository>();
            _paymentRepoMock = new Mock<IPaymentRepository>();

            _unitOfWorkMock.Setup(u => u.BillRepository).Returns(_billRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.PaymentRepository).Returns(_paymentRepoMock.Object);

            _service = new PaymentService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreatePayment_BillNotFound_ThrowsException()
        {
            _billRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Bill)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreatePayment(1, 100, PaymentMethod.Cash));
        }

        [Fact]
        public async Task CreatePayment_BillAlreadyPaid_ThrowsException()
        {
            var bill = new Bill { Id = 1, Amount = 100, IsPaid = true };
            _billRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bill);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreatePayment(1, 100, PaymentMethod.Cash));
        }

        [Fact]
        public async Task CreatePayment_AmountMismatch_ThrowsException()
        {
            var bill = new Bill { Id = 1, Amount = 100, IsPaid = false };
            _billRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bill);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreatePayment(1, 50, PaymentMethod.Cash));
        }

        [Fact]
        public async Task CreatePayment_ValidPayment_AddsPaymentAndMarksBillPaid()
        {
            var bill = new Bill { Id = 1, Amount = 100, IsPaid = false };
            _billRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bill);
            _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var paymentId = await _service.CreatePayment(1, 100, PaymentMethod.Cash);

            Assert.True(paymentId >= 0);
            Assert.True(bill.IsPaid); // Bill marked as paid
            _paymentRepoMock.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}

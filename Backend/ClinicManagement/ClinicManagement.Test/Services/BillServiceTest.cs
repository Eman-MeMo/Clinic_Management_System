using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Services;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClinicManagement.Application.Interfaces;

namespace ClinicManagement.Test.Services
{
    public class BillingServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBillRepository> _billRepoMock;
        private readonly Mock<IPaymentRepository> _paymentRepoMock;
        private readonly Mock<IPatientRepository> _patientRepoMock;
        private readonly Mock<ISessionRepository> _sessionRepoMock;
        private readonly Mock<ISessionServiceRepository> _sessionServiceRepoMock;
        private readonly IBillingService _service;

        public BillingServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _billRepoMock = new Mock<IBillRepository>();
            _paymentRepoMock = new Mock<IPaymentRepository>();
            _patientRepoMock = new Mock<IPatientRepository>();
            _sessionRepoMock = new Mock<ISessionRepository>();
            _sessionServiceRepoMock = new Mock<ISessionServiceRepository>();

            _unitOfWorkMock.Setup(u => u.BillRepository).Returns(_billRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.PaymentRepository).Returns(_paymentRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.PatientRepository).Returns(_patientRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SessionRepository).Returns(_sessionRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SessionServiceRepository).Returns(_sessionServiceRepoMock.Object);

            _service = new BillingService(_unitOfWorkMock.Object);
        }

        #region MarkAsPaidAsync Tests
        [Fact]
        public async Task MarkAsPaidAsync_BillNotFound_ReturnsFalse()
        {
            _billRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Bill)null);

            var result = await _service.MarkAsPaidAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task MarkAsPaidAsync_BillAlreadyPaid_ReturnsFalse()
        {
            _billRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Bill { IsPaid = true });

            var result = await _service.MarkAsPaidAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task MarkAsPaidAsync_NoPaymentOrAmountMismatch_ReturnsFalse()
        {
            var bill = new Bill { Id = 1, Amount = 100, IsPaid = false };
            _billRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bill);
            _paymentRepoMock.Setup(r => r.GetByBillIdAsync(1)).ReturnsAsync(new Payment { Amount = 50 });

            var result = await _service.MarkAsPaidAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task MarkAsPaidAsync_ValidPayment_SetsIsPaidTrue()
        {
            var bill = new Bill { Id = 1, Amount = 100, IsPaid = false };
            var payment = new Payment { Id = 1, BillId = 1, Amount = 100 };

            _billRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bill);
            _paymentRepoMock.Setup(r => r.GetByBillIdAsync(1)).ReturnsAsync(payment);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.MarkAsPaidAsync(1);

            Assert.True(result);
            Assert.True(bill.IsPaid);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region CreateBillAsync Tests
        [Fact]
        public async Task CreateBillAsync_PatientNotFound_ThrowsException()
        {
            _patientRepoMock.Setup(r => r.GetByIdAsync("P1")).ReturnsAsync((Patient)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBillAsync("P1", 1));
        }

        [Fact]
        public async Task CreateBillAsync_SessionNotFound_ThrowsException()
        {
            _patientRepoMock.Setup(r => r.GetByIdAsync("P1")).ReturnsAsync(new Patient { Id = "P1" });
            _sessionRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Session)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBillAsync("P1", 1));
        }

        [Fact]
        public async Task CreateBillAsync_ExistingBill_ThrowsException()
        {
            _patientRepoMock.Setup(r => r.GetByIdAsync("P1")).ReturnsAsync(new Patient { Id = "P1" });
            _sessionRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Session { Id = 1 });
            _billRepoMock.Setup(r => r.GetBySessionAsync(1)).ReturnsAsync(new List<Bill> { new Bill() }.AsQueryable());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBillAsync("P1", 1));
        }

        [Fact]
        public async Task CreateBillAsync_NoServices_ThrowsException()
        {
            _patientRepoMock.Setup(r => r.GetByIdAsync("P1")).ReturnsAsync(new Patient { Id = "P1" });
            _sessionRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Session { Id = 1 });
            _billRepoMock.Setup(r => r.GetBySessionAsync(1)).ReturnsAsync(Enumerable.Empty<Bill>().AsQueryable());
            _sessionServiceRepoMock.Setup(r => r.GetAllBySessionIdAsync(1)).ReturnsAsync(new List<SessionService>());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBillAsync("P1", 1));
        }

        [Fact]
        public async Task CreateBillAsync_ValidData_AddsBillAndSaves()
        {
            var patient = new Patient { Id = "P1" };
            var session = new Session { Id = 1 };
            var services = new List<SessionService>
            {
                new SessionService { Service = new Service { Price = 50 } },
                new SessionService { Service = new Service { Price = 100 } }
            };

            _patientRepoMock.Setup(r => r.GetByIdAsync("P1")).ReturnsAsync(patient);
            _sessionRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(session);
            _billRepoMock.Setup(r => r.GetBySessionAsync(1)).ReturnsAsync(Enumerable.Empty<Bill>().AsQueryable());
            _sessionServiceRepoMock.Setup(r => r.GetAllBySessionIdAsync(1)).ReturnsAsync(services);
            _billRepoMock.Setup(r => r.AddAsync(It.IsAny<Bill>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var billId = await _service.CreateBillAsync("P1", 1);

            Assert.True(billId >= 0);
            _billRepoMock.Verify(r => r.AddAsync(It.Is<Bill>(b => b.Amount == 150)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
        #endregion
    }
}
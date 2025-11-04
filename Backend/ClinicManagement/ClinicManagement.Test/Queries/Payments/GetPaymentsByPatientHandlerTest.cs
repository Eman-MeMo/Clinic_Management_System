using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Payments.GetPaymentsByPatient;
using ClinicManagement.Domain.DTOs.PaymentDTOs;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.Payments
{
    public class GetPaymentsByPatientHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetPaymentsByPatientHandler _handler;

        public GetPaymentsByPatientHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetPaymentsByPatientHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMappedPayments()
        {
            var query = new GetPaymentsByPatientQuery { PatientId = "P1" };
            var payments = new List<Payment> { new Payment { Id = 1 }, new Payment { Id = 2 } };
            var mappedPayments = new List<PaymentDto> { new PaymentDto(), new PaymentDto() };

            _unitOfWorkMock.Setup(u => u.PaymentRepository.GetByPatientAsync(query.PatientId))
                           .ReturnsAsync(payments);
            _mapperMock.Setup(m => m.Map<IEnumerable<PaymentDto>>(payments))
                       .Returns(mappedPayments);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(mappedPayments, result);
            _unitOfWorkMock.Verify(u => u.PaymentRepository.GetByPatientAsync(query.PatientId), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyList()
        {
            var query = new GetPaymentsByPatientQuery { PatientId = "P2" };
            _unitOfWorkMock.Setup(u => u.PaymentRepository.GetByPatientAsync(query.PatientId))
                           .ReturnsAsync(new List<Payment>());
            _mapperMock.Setup(m => m.Map<IEnumerable<PaymentDto>>(It.IsAny<IEnumerable<Payment>>()))
                       .Returns(new List<PaymentDto>());

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Empty(result);
        }
    }
}

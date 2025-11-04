using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Payments.GetPaymentsByDateRange;
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
    public class GetPaymentsByDateRangeHandlerTest
    {
        private readonly Mock<IUnitOfWork> mockUnitOfWork;
        private readonly Mock<IMapper> mockMapper;
        private readonly GetPaymentsByDateRangeHandler handler;

        public GetPaymentsByDateRangeHandlerTest()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockMapper = new Mock<IMapper>();
            handler = new GetPaymentsByDateRangeHandler(mockUnitOfWork.Object, mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidDateRange_ReturnsMappedPayments()
        {
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 1, 31);
            var query = new GetPaymentsByDateRangeQuery { start = startDate, end = endDate };

            var payments = new List<Payment>
            {
                new Payment { Id = 1, Amount = 100, Date = new DateTime(2025, 1, 5) },
                new Payment { Id = 2, Amount = 200, Date = new DateTime(2025, 1, 20) }
            };

            var mappedDtos = new List<PaymentDto>
            {
                new PaymentDto { Id = 1, Amount = 100 },
                new PaymentDto { Id = 2, Amount = 200 }
            };

            mockUnitOfWork
                .Setup(u => u.PaymentRepository.GetByDateRangeAsync(startDate, endDate))
                .ReturnsAsync(payments);

            mockMapper
                .Setup(m => m.Map<IEnumerable<PaymentDto>>(payments))
                .Returns(mappedDtos);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, ((List<PaymentDto>)result).Count);
            Assert.Collection(result,
                item => Assert.Equal(100, item.Amount),
                item => Assert.Equal(200, item.Amount));
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            GetPaymentsByDateRangeQuery? query = null;
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyList()
        {
            var query = new GetPaymentsByDateRangeQuery
            {
                start = new DateTime(2025, 2, 1),
                end = new DateTime(2025, 2, 28)
            };

            mockUnitOfWork
                .Setup(u => u.PaymentRepository.GetByDateRangeAsync(query.start, query.end))
                .ReturnsAsync(new List<Payment>());

            mockMapper
                .Setup(m => m.Map<IEnumerable<PaymentDto>>(It.IsAny<IEnumerable<Payment>>()))
                .Returns(new List<PaymentDto>());

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
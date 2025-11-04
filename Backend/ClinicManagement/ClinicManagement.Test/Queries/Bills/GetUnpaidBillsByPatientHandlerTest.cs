using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Bills.GetUnpaidBillsByPatient;
using ClinicManagement.Domain.DTOs.BillDTOs;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.Bills
{
    public class GetUnpaidBillsByPatientHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetUnpaidBillsByPatientHandler _handler;

        public GetUnpaidBillsByPatientHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetUnpaidBillsByPatientHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedBills_WhenValidRequest()
        {
            var patientId = "p1";
            var query = new GetUnpaidBillsByPatientQuery() { PatientId=patientId};
            var bills = new List<Bill> { new Bill { Id = 1 }, new Bill { Id = 2 } };
            var mappedDtos = new List<BillDto> { new BillDto(), new BillDto() };

            _unitOfWorkMock.Setup(u => u.BillRepository.GetUnpaidBillsByPatientAsync(patientId))
                           .ReturnsAsync(bills);
            _mapperMock.Setup(m => m.Map<IEnumerable<BillDto>>(bills))
                       .Returns(mappedDtos);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, ((List<BillDto>)result).Count);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}
using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Attendances.GetPatientAttendanceHistory;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.Attendances
{
    public class GetPatientAttendanceHistoryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetPatientAttendanceHistoryHandler _handler;

        public GetPatientAttendanceHistoryHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetPatientAttendanceHistoryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedAttendances_WhenValidRequest()
        {
            var patientId = "P1";
            var query = new GetPatientAttendanceHistoryQuery() { PatientId=patientId};
            var attendances = new List<Attendance> { new Attendance { Id = 1 }, new Attendance { Id = 2 } };
            var mappedDtos = new List<AttendanceDto> { new AttendanceDto(), new AttendanceDto() };

            _unitOfWorkMock.Setup(u => u.AttendanceRepository.GetPatientAttendanceHistoryAsync(patientId))
                           .ReturnsAsync(attendances);
            _mapperMock.Setup(m => m.Map<IEnumerable<AttendanceDto>>(attendances))
                       .Returns(mappedDtos);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, ((List<AttendanceDto>)result).Count);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}
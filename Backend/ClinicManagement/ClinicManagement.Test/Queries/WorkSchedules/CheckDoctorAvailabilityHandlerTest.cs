using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.WorkSchedules.CheckDoctorAvailability;
using ClinicManagement.Infrastructure.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.WorkSchedules
{
    public class CheckDoctorAvailabilityHandlerTests
    {
        private readonly Mock<IDoctorAvailabilityService> doctorAvailabilityServiceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly CheckDoctorAvailabilityHandler handler;

        public CheckDoctorAvailabilityHandlerTests()
        {
            doctorAvailabilityServiceMock = new Mock<IDoctorAvailabilityService>();
            mapperMock = new Mock<IMapper>();
            handler = new CheckDoctorAvailabilityHandler(doctorAvailabilityServiceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenDoctorIsAvailable()
        {
            var query = new CheckDoctorAvailabilityQuery
            {
                DoctorId = "D1",
                AppointmentDateTime = DateTime.Now
            };

            doctorAvailabilityServiceMock
                .Setup(s => s.IsDoctorAvailableAsync(query.DoctorId, query.AppointmentDateTime, null))
                .ReturnsAsync(true);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.True(result);
            doctorAvailabilityServiceMock.Verify(s => s.IsDoctorAvailableAsync(query.DoctorId, query.AppointmentDateTime, null), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenDoctorIsNotAvailable()
        {
            var query = new CheckDoctorAvailabilityQuery
            {
                DoctorId = "D1",
                AppointmentDateTime = DateTime.Now
            };

            doctorAvailabilityServiceMock
                .Setup(s => s.IsDoctorAvailableAsync(query.DoctorId, query.AppointmentDateTime, null))
                .ReturnsAsync(false);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.False(result);
            doctorAvailabilityServiceMock.Verify(s => s.IsDoctorAvailableAsync(query.DoctorId, query.AppointmentDateTime,null), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }
    }
}
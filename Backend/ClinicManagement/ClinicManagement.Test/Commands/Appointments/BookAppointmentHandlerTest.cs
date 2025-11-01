using AutoMapper;
using ClinicManagement.Application.Commands.Appointments.BookAppointment;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Appointments
{
    public class BookAppointmentHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly Mock<IDoctorAvailabilityService> _availabilityServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BookAppointmentHandler _handler;

        public BookAppointmentHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _availabilityServiceMock = new Mock<IDoctorAvailabilityService>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.AppointmentRepository).Returns(_appointmentRepoMock.Object);

            _handler = new BookAppointmentHandler(_unitOfWorkMock.Object, _availabilityServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_DoctorAvailable_ReturnsAppointmentId()
        {
            var command = new BookAppointmentCommand
            {
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.UtcNow.AddDays(1),
                Notes = "Follow-up"
            };

            var appointment = new Appointment { Id = 123, DoctorId = "D1", PatientId = "P1" };

            _availabilityServiceMock.Setup(s => s.IsDoctorAvailableAsync(command.DoctorId, command.Date,null))
                .ReturnsAsync(true);

            _mapperMock.Setup(m => m.Map<Appointment>(It.IsAny<BookAppointmentCommand>()))
                .Returns(appointment);

            _appointmentRepoMock.Setup(r => r.AddAsync(It.IsAny<Appointment>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(123, result);
            _availabilityServiceMock.Verify(s => s.IsDoctorAvailableAsync(command.DoctorId, command.Date,null), Times.Once);
            _appointmentRepoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DoctorNotAvailable_ThrowsInvalidOperationException()
        {
            var command = new BookAppointmentCommand
            {
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.UtcNow.AddDays(1)
            };

            _availabilityServiceMock.Setup(s => s.IsDoctorAvailableAsync(command.DoctorId, command.Date,null))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));
        }
    }
}

using AutoMapper;
using ClinicManagement.Application.Commands.Appointments.UpdateAppointment;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Appointments
{
    public class UpdateAppointmentHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDoctorAvailabilityService> _availabilityServiceMock;
        private readonly UpdateAppointmentHandler _handler;

        public UpdateAppointmentHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _mapperMock = new Mock<IMapper>();
            _availabilityServiceMock = new Mock<IDoctorAvailabilityService>();

            _unitOfWorkMock.Setup(u => u.AppointmentRepository).Returns(_appointmentRepoMock.Object);

            _handler = new UpdateAppointmentHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _availabilityServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_AppointmentNotFound_ThrowsKeyNotFoundException()
        {
            var command = new UpdateAppointmentCommand
            {
                Id = 1,
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(1)
            };

            _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.Id))
                .ReturnsAsync((Appointment)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DoctorNotAvailable_ThrowsInvalidOperationException()
        {
            var command = new UpdateAppointmentCommand
            {
                Id = 2,
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(1)
            };

            var appointment = new Appointment { Id = 2, DoctorId = "D1", PatientId = "P1" };

            _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.Id))
                .ReturnsAsync(appointment);

            _availabilityServiceMock.Setup(s =>
                s.IsDoctorAvailableAsync(command.DoctorId, command.Date, command.Id))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesAppointmentSuccessfully()
        {
            var command = new UpdateAppointmentCommand
            {
                Id = 3,
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(1),
                Notes = "Updated notes"
            };

            var appointment = new Appointment { Id = 3 };

            _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.Id))
                .ReturnsAsync(appointment);

            _availabilityServiceMock.Setup(s =>
                s.IsDoctorAvailableAsync(command.DoctorId, command.Date, command.Id))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            _mapperMock.Verify(m => m.Map(command, appointment), Times.Once);
            _appointmentRepoMock.Verify(r => r.Update(appointment), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(Unit.Value, result);
        }
    }
}

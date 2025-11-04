using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Appointments.GetAppointmentsByPatient;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.Appointments
{
    public class GetAppointmentsByPatientHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAppointmentsByPatientHandler _handler;

        public GetAppointmentsByPatientHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.AppointmentRepository)
                           .Returns(_appointmentRepoMock.Object);

            _handler = new GetAppointmentsByPatientHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidPatientId_ReturnsMappedAppointments()
        {
            var patientId = "p1";
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, DoctorId = "d1", PatientId = patientId, Date = DateTime.UtcNow },
                new Appointment { Id = 2, DoctorId = "d2", PatientId = patientId, Date = DateTime.UtcNow.AddDays(1) }
            };

            var appointmentDtos = new List<AppointmentDto>
            {
                new AppointmentDto { Id = 1, DoctorId = "d1", PatientId = patientId },
                new AppointmentDto { Id = 2, DoctorId = "d2", PatientId = patientId }
            };

            _appointmentRepoMock.Setup(r => r.GetAllByPatientIdAsync(patientId))
                                .ReturnsAsync(appointments);

            _mapperMock.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments))
                       .Returns(appointmentDtos);

            var query = new GetAppointmentsByPatientQuery { PatientId = patientId };

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(patientId, result.First().PatientId);

            _appointmentRepoMock.Verify(r => r.GetAllByPatientIdAsync(patientId), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<AppointmentDto>>(appointments), Times.Once);
        }

        [Fact]
        public async Task Handle_NoAppointments_ReturnsEmptyList()
        {
            var patientId = "pe";
            var appointments = new List<Appointment>();

            _appointmentRepoMock.Setup(r => r.GetAllByPatientIdAsync(patientId))
                                .ReturnsAsync(appointments);

            _mapperMock.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments))
                       .Returns(new List<AppointmentDto>());

            var query = new GetAppointmentsByPatientQuery { PatientId = patientId };

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
            _appointmentRepoMock.Verify(r => r.GetAllByPatientIdAsync(patientId), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            GetAppointmentsByPatientQuery query = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}

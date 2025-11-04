using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Appointments.GetAppointmentsByDoctor;
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
    public class GetAppointmentsByDoctorHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAppointmentsByDoctorHandler _handler;

        public GetAppointmentsByDoctorHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.AppointmentRepository)
                           .Returns(_appointmentRepoMock.Object);

            _handler = new GetAppointmentsByDoctorHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidDoctorId_ReturnsMappedAppointments()
        {
            var doctorId = "D1";
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, DoctorId = doctorId, PatientId = "p1", Date = DateTime.UtcNow },
                new Appointment { Id = 2, DoctorId = doctorId, PatientId = "p", Date = DateTime.UtcNow.AddDays(1) }
            };

            var appointmentDtos = new List<AppointmentDto>
            {
                new AppointmentDto { Id = 1, DoctorId = doctorId, PatientId = "p1" },
                new AppointmentDto { Id = 2, DoctorId = doctorId, PatientId = "p2" }
            };

            _appointmentRepoMock.Setup(r => r.GetAllByDoctorIdAsync(doctorId))
                                .ReturnsAsync(appointments);

            _mapperMock.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments))
                       .Returns(appointmentDtos);

            var query = new GetAppointmentsByDoctorQuery { DoctorId = doctorId };

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("p1", result.First().PatientId);

            _appointmentRepoMock.Verify(r => r.GetAllByDoctorIdAsync(doctorId), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<AppointmentDto>>(appointments), Times.Once);
        }

        [Fact]
        public async Task Handle_NoAppointments_ReturnsEmptyList()
        {
            var doctorId = "DE";
            var appointments = new List<Appointment>();

            _appointmentRepoMock.Setup(r => r.GetAllByDoctorIdAsync(doctorId))
                                .ReturnsAsync(appointments);

            _mapperMock.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments))
                       .Returns(new List<AppointmentDto>());

            var query = new GetAppointmentsByDoctorQuery { DoctorId = doctorId };

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
            _appointmentRepoMock.Verify(r => r.GetAllByDoctorIdAsync(doctorId), Times.Once);
        }
    }
}
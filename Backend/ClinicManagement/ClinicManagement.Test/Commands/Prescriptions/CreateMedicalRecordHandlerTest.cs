using ClinicManagement.Application.Commands.Prescriptions.CreateMedicalRecord;
using ClinicManagement.Application.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Prescriptions
{
    public class CreateMedicalRecordHandlerTest
    {
        private readonly Mock<IMedicalRecordService> _medicalRecordServiceMock;
        private readonly CreateMedicalRecordHandler _handler;

        public CreateMedicalRecordHandlerTest()
        {
            _medicalRecordServiceMock = new Mock<IMedicalRecordService>();
            _handler = new CreateMedicalRecordHandler(_medicalRecordServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMedicalRecordId()
        {
            var command = new CreateMedicalRecordCommand
            {
                Notes = "Patient stable",
                Diagnosis = "Flu",
                PrescriptionId = 1
            };

            _medicalRecordServiceMock
                .Setup(x => x.CreateMedicalRecordAsync(command.Notes, command.Diagnosis, command.PrescriptionId))
                .ReturnsAsync(10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(10, result);
            _medicalRecordServiceMock.Verify(x =>
                x.CreateMedicalRecordAsync(command.Notes, command.Diagnosis, command.PrescriptionId), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}
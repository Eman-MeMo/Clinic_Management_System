using ClinicManagement.Application.Commands.WorkSchedules.UpdateWorkSchedule;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.WorkSchedules
{
    public class UpdateWorkScheduleHandlerTest
    {
        private readonly Mock<IUnitOfWork> mockUnitOfWork;
        private readonly Mock<IWorkScheduleRepository> mockRepository;
        private readonly UpdateWorkScheduleHandler handler;

        public UpdateWorkScheduleHandlerTest()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockRepository = new Mock<IWorkScheduleRepository>();
            mockUnitOfWork.Setup(u => u.WorkScheduleRepository).Returns(mockRepository.Object);
            handler = new UpdateWorkScheduleHandler(mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateWorkSchedule_WhenValidRequest()
        {
            var existing = new WorkSchedule
            {
                Id = 1,
                DoctorId = "D2",
                DayOfWeek = DayOfWeek.Monday,
                StartTime = DateTime.Today.AddHours(8),
                EndTime = DateTime.Today.AddHours(16),
                IsAvailable = true
            };
            var request = new UpdateWorkScheduleCommand
            {
                Id = 1,
                DoctorId = "D2",
                DayOfWeek = DayOfWeek.Tuesday,
                StartTime = DateTime.Today.AddHours(9),
                EndTime = DateTime.Today.AddHours(17),
                IsAvailable = false
            };
            mockRepository.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(existing);

            await handler.Handle(request, CancellationToken.None);

            mockRepository.Verify(r => r.Update(It.IsAny<WorkSchedule>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenEndTimeBeforeStartTime()
        {
            var request = new UpdateWorkScheduleCommand
            {
                Id = 1,
                DoctorId = "D2",
                DayOfWeek = DayOfWeek.Monday,
                StartTime = DateTime.Today.AddHours(10),
                EndTime = DateTime.Today.AddHours(8),
                IsAvailable = true
            };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenWorkScheduleNotFound()
        {
            var request = new UpdateWorkScheduleCommand
            {
                Id = 1,
                DoctorId = "D2",
                DayOfWeek = DayOfWeek.Monday,
                StartTime = DateTime.Today.AddHours(8),
                EndTime = DateTime.Today.AddHours(16),
                IsAvailable = true
            };
            mockRepository.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((WorkSchedule)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(request, CancellationToken.None));
        }
    }
}
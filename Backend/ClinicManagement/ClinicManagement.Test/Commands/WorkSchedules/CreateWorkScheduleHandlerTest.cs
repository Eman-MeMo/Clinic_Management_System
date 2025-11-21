using ClinicManagement.Application.Commands.WorkSchedules.CreateWorkSchedule;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.Test.Commands.WorkSchedules
{
    public class CreateWorkScheduleHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IWorkScheduleRepository> _workScheduleRepoMock;
        private readonly CreateWorkScheduleHandler _handler;

        public CreateWorkScheduleHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _workScheduleRepoMock = new Mock<IWorkScheduleRepository>();

            _unitOfWorkMock.Setup(u => u.WorkScheduleRepository).Returns(_workScheduleRepoMock.Object);
            _handler = new CreateWorkScheduleHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsWorkScheduleId()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Monday,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(5),
                IsAvailable = true
            };

            _workScheduleRepoMock
                .Setup(r => r.GetScheduleByDoctorAndDayAsync(command.DoctorId, command.DayOfWeek))
                .ReturnsAsync(new List<WorkSchedule>());

            var workSchedule = new WorkSchedule { Id = 42 };
            _workScheduleRepoMock
                .Setup(r => r.AddAsync(It.IsAny<WorkSchedule>()))
                .Callback<WorkSchedule>(ws => ws.Id = workSchedule.Id)
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            _workScheduleRepoMock.Verify(r => r.AddAsync(It.IsAny<WorkSchedule>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(workSchedule.Id, result);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EndTimeBeforeStartTime_ThrowsArgumentException()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Tuesday,
                StartTime = DateTime.Now.AddHours(5),
                EndTime = DateTime.Now.AddHours(1),
                IsAvailable = false
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_OverlappingSchedule_ThrowsException()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(4),
                IsAvailable = true
            };

            var existingSchedule = new WorkSchedule
            {
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = DateTime.Now.AddHours(3),
                EndTime = DateTime.Now.AddHours(5)
            };

            _workScheduleRepoMock
                .Setup(r => r.GetScheduleByDoctorAndDayAsync(command.DoctorId, command.DayOfWeek))
                .ReturnsAsync(new List<WorkSchedule> { existingSchedule });

            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}

using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.WorkSchedules.GetScheduleByDoctorAndDay;
using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.WorkSchedules
{
    public class GetScheduleByDoctorAndDayHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly GetScheduleByDoctorAndDayHandler handler;

        public GetScheduleByDoctorAndDayHandlerTests()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            mapperMock = new Mock<IMapper>();
            handler = new GetScheduleByDoctorAndDayHandler(unitOfWorkMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedWorkScheduleDtos_WhenSchedulesExist()
        {
            var doctorId = "D1";
            var day = DayOfWeek.Monday;
            var query = new GetScheduleByDoctorAndDayQuery { DoctorId = doctorId, Day = day };

            var workSchedules = new List<WorkSchedule>
            {
                new WorkSchedule { Id = 1, DoctorId = doctorId, DayOfWeek = day },
                new WorkSchedule { Id = 2, DoctorId = doctorId, DayOfWeek = day }
            };

                    var workScheduleDtos = new List<WorkScheduleDto>
            {
                new WorkScheduleDto { Id = 1, DoctorId = doctorId },
                new WorkScheduleDto { Id = 2, DoctorId = doctorId }
            };

            unitOfWorkMock
                .Setup(u => u.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(doctorId, day))
                .ReturnsAsync(workSchedules);

            mapperMock
                .Setup(m => m.Map<IEnumerable<WorkScheduleDto>>(workSchedules))
                .Returns(workScheduleDtos);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            unitOfWorkMock.Verify(u => u.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(doctorId, day), Times.Once);
            mapperMock.Verify(m => m.Map<IEnumerable<WorkScheduleDto>>(workSchedules), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenScheduleNotFound()
        {
            var doctorId = "D1";
            var day = DayOfWeek.Tuesday;
            var query = new GetScheduleByDoctorAndDayQuery { DoctorId = doctorId, Day = day };

            unitOfWorkMock
                .Setup(u => u.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(doctorId, day))
                .ReturnsAsync(new List<WorkSchedule>()); 

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            unitOfWorkMock.Verify(u => u.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(doctorId, day), Times.Once);
            mapperMock.Verify(m => m.Map<WorkScheduleDto>(It.IsAny<WorkSchedule>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }
    }
}
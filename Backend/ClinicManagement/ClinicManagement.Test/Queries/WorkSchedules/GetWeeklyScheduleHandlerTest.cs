using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.WorkSchedules.GetWeeklySchedule;
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
    public class GetWeeklyScheduleHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetWeeklyScheduleHandler _handler;

        public GetWeeklyScheduleHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetWeeklyScheduleHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsWorkScheduleDto()
        {
            var doctorId = "D1";
            var request = new GetWeeklyScheduleQuery { DoctorId = doctorId };

            var workScheduleList = new List<WorkSchedule> {
                  new WorkSchedule
                    {
                        DoctorId = doctorId,
                        StartTime = new DateTime(2025, 11, 3, 9, 0, 0),
                        EndTime = new DateTime(2025, 11, 3, 17, 0, 0),
                        DayOfWeek = DayOfWeek.Monday,
                        IsAvailable = true
                    }
            };
            var workScheduleDtoList = new List<WorkScheduleDto> {
                new WorkScheduleDto
            {
                DoctorId = doctorId,
                StartTime = new DateTime(2025, 11, 3, 9, 0, 0),
                EndTime = new DateTime(2025, 11, 3, 17, 0, 0),
                DayOfWeek = DayOfWeek.Monday,
                IsAvailable = true,
            }
            };

            _unitOfWorkMock
                .Setup(u => u.WorkScheduleRepository.GetWeeklyScheduleAsync(doctorId))
                .ReturnsAsync(workScheduleList);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<WorkScheduleDto>>(workScheduleList))
                .Returns(workScheduleDtoList);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(doctorId, result.First().DoctorId);
            Assert.Equal(DayOfWeek.Monday, result.First().DayOfWeek);
            Assert.True(result.First().IsAvailable);

            _unitOfWorkMock.Verify(u => u.WorkScheduleRepository.GetWeeklyScheduleAsync(doctorId), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<WorkScheduleDto>>(workScheduleList), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNoSchedules_ReturnsEmptyList()
        {
            var doctorId = "D1";
            var request = new GetWeeklyScheduleQuery { DoctorId = doctorId };

            _unitOfWorkMock
                .Setup(u => u.WorkScheduleRepository.GetWeeklyScheduleAsync(doctorId))
                .ReturnsAsync(new List<WorkSchedule>()); 

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            _unitOfWorkMock.Verify(u => u.WorkScheduleRepository.GetWeeklyScheduleAsync(doctorId), Times.Once);
            _mapperMock.Verify(m => m.Map<WorkScheduleDto>(It.IsAny<WorkSchedule>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}
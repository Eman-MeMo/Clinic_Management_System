using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;
using ClinicManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.Test.Services
{
    public class DoctorAvailabilityServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IWorkScheduleRepository> _workScheduleRepoMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly Mock<IDoctorRepository> _doctorRepoMock;
        private readonly DoctorAvailabilityService _service;

        public DoctorAvailabilityServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _workScheduleRepoMock = new Mock<IWorkScheduleRepository>();
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _doctorRepoMock = new Mock<IDoctorRepository>();

            _unitOfWorkMock.Setup(u => u.WorkScheduleRepository).Returns(_workScheduleRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.AppointmentRepository).Returns(_appointmentRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.DoctorRepository).Returns(_doctorRepoMock.Object);

            _service = new DoctorAvailabilityService(_unitOfWorkMock.Object);
        }
        #region IsDoctorAvailableAsync Tests
        [Fact]
        public async Task IsDoctorAvailableAsync_DoctorNotWorking_ReturnsFalse()
        {
            var doctorId = "D1";
            var date = DateTime.Today.AddHours(10);

            _workScheduleRepoMock.Setup(r => r.GetScheduleByDoctorAndDayAsync(doctorId, date.DayOfWeek))
                .ReturnsAsync(new List<WorkSchedule>()); // No schedule

            var result = await _service.IsDoctorAvailableAsync(doctorId, date);

            Assert.False(result);
        }

        [Fact]
        public async Task IsDoctorAvailableAsync_DoctorHasConflict_ReturnsFalse()
        {
            var doctorId = "D1";
            var date = DateTime.Today.AddHours(10);

            _workScheduleRepoMock.Setup(r => r.GetScheduleByDoctorAndDayAsync(doctorId, date.DayOfWeek))
                .ReturnsAsync(new List<WorkSchedule>
                {
                    new WorkSchedule
                    {
                        IsAvailable = true,
                        StartTime = date.Date.AddHours(9),
                        EndTime = date.Date.AddHours(17)
                    }
                });

            _appointmentRepoMock.Setup(r => r.HasAppointmentForDoctorAtDateAsync(doctorId, date, null))
                .ReturnsAsync(true); // Conflict exists

            var result = await _service.IsDoctorAvailableAsync(doctorId, date);

            Assert.False(result);
        }

        [Fact]
        public async Task IsDoctorAvailableAsync_DoctorAvailableNoConflict_ReturnsTrue()
        {
            var doctorId = "D1";
            var date = DateTime.Today.AddHours(10);

            _workScheduleRepoMock.Setup(r => r.GetScheduleByDoctorAndDayAsync(doctorId, date.DayOfWeek))
                .ReturnsAsync(new List<WorkSchedule>
                {
                    new WorkSchedule
                    {
                        IsAvailable = true,
                        StartTime = date.Date.AddHours(9),
                        EndTime = date.Date.AddHours(17)
                    }
                });

            _appointmentRepoMock.Setup(r => r.HasAppointmentForDoctorAtDateAsync(doctorId, date, null))
                .ReturnsAsync(false); // No conflict

            var result = await _service.IsDoctorAvailableAsync(doctorId, date);

            Assert.True(result);
        }
        #endregion

        #region GetAvailableDoctorsAtAsync Tests
        [Fact]
        public async Task GetAvailableDoctorsAtAsync_ReturnsOnlyAvailableDoctors_InMemory()
        {
            var options = new DbContextOptionsBuilder<ClinicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ClinicDbContext(options, null)) 
            {
                var targetTime = DateTime.Today.AddHours(10);


                var doctor1 = new Doctor
                {
                    Id = "D1",
                    FirstName = "John",
                    LastName = "Doe",
                    IsAvaible = true,
                    WorkSchedules = new List<WorkSchedule>
                    {
                        new WorkSchedule
                        {
                            IsAvailable = true,
                            DayOfWeek = targetTime.DayOfWeek,
                            StartTime = targetTime.Date.AddHours(9),
                            EndTime = targetTime.Date.AddHours(17)
                        }
                    },
                    Appointments = new List<Appointment>()
                };

                var doctor2 = new Doctor
                {
                    Id = "D2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    IsAvaible = false
                };

                var doctor3 = new Doctor
                {
                    Id = "D3",
                    FirstName = "Alice",
                    LastName = "Brown",
                    IsAvaible = true,
                    WorkSchedules = new List<WorkSchedule>
                    {
                        new WorkSchedule
                        {
                            IsAvailable = true,
                            DayOfWeek = targetTime.DayOfWeek,
                            StartTime = targetTime.Date.AddHours(8),
                            EndTime = targetTime.Date.AddHours(9)
                        }
                    }
                };


                context.Doctors.AddRange(doctor1, doctor2, doctor3);
                context.SaveChanges();

                var unitOfWork = new UnitOfWork(context);
                var service = new DoctorAvailabilityService(unitOfWork);

                var result = await service.GetAvailableDoctorsAtAsync(targetTime);

                Assert.Single(result);
                Assert.Equal("D1", result.First().Id);
            }
        }
        #endregion
    }
}

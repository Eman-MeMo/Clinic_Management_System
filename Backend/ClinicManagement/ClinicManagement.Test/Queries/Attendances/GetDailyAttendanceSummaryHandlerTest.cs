using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Attendances.GetDailyAttendanceSummary;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.Attendances
{
    public class GetDailyAttendanceSummaryHandlerTest
    {
        private readonly Mock<IAttendanceService> _mockService;
        private readonly GetDailyAttendanceSummaryHandler _handler;

        public GetDailyAttendanceSummaryHandlerTest()
        {
            _mockService = new Mock<IAttendanceService>();
            _handler = new GetDailyAttendanceSummaryHandler(_mockService.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSummary_WhenValidRequest()
        {
            var query = new GetDailyAttendanceSummaryQuery();
            var expected = new AttendanceSummaryDto
            {
                Date = DateTime.Today,
                TotalPatients = 10,
                PresentCount = 8,
                AbsentCount = 2
            };

            _mockService.Setup(s => s.GetDailySummaryReportAsync(query.Date))
                        .ReturnsAsync(expected);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(expected.TotalPatients, result.TotalPatients);
        }
    }
}
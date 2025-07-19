using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClinicManagement.Application.Interfaces
{
    public interface IWorkScheduleRepository:IGenericRepository<WorkSchedule>
    {
        Task<IEnumerable<WorkSchedule>> GetScheduleByDoctorAndDayAsync(string doctorId, DayOfWeek dayOfWeek);
        Task<bool> CheckAvailabilityAsync(string doctorId, DateTime dateTime);
        Task<IEnumerable<WorkSchedule>> GetWeeklyScheduleAsync(string doctorId);
    }
}

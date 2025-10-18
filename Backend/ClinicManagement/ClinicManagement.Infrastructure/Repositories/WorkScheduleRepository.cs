using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class WorkScheduleRepository : GenericRepository<WorkSchedule>, IWorkScheduleRepository
    {
        public WorkScheduleRepository(ClinicDbContext db) : base(db)
        {
        }
        public Task<bool> CheckAvailabilityAsync(string doctorId, DateTime day)
        {
            return db.WorkSchedules
                .Where(ws => ws.DoctorId == doctorId && ws.DayOfWeek == day.DayOfWeek &&
                             ws.IsAvailable &&
                             day.TimeOfDay >= ws.StartTime.TimeOfDay &&
                             day.TimeOfDay <= ws.EndTime.TimeOfDay)
                .AnyAsync();
        }

        public async Task<IEnumerable<WorkSchedule>> GetScheduleByDoctorAndDayAsync(string doctorId, DayOfWeek dayOfWeek)
        {
            return await db.WorkSchedules.AsNoTracking()
                .Where(ws => ws.DoctorId == doctorId && ws.DayOfWeek == dayOfWeek)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkSchedule>> GetWeeklyScheduleAsync(string doctorId)
        {
            return await db.WorkSchedules.AsNoTracking()
                .Where(ws => ws.DoctorId == doctorId)
                .OrderBy(ws => ws.DayOfWeek)
                .ToListAsync();
        }
    }
}

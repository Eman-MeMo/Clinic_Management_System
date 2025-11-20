using ClinicManagement.Application.Commands.WorkSchedules.CreateWorkSchedule;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using MediatR;

public class CreateWorkScheduleHandler : IRequestHandler<CreateWorkScheduleCommand, int>
{
    private readonly IUnitOfWork unitOfWork;

    public CreateWorkScheduleHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateWorkScheduleCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Validate time
        if (request.EndTime <= request.StartTime)
            throw new ArgumentException("End time must be later than start time.");

        // Get all schedules for that doctor & same day
        var schedules = await unitOfWork.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(request.DoctorId, request.DayOfWeek);

        // Check overlap
        bool isOverlap = schedules.Any(w =>
            request.StartTime < w.EndTime &&
            request.EndTime > w.StartTime
        );

        if (isOverlap)
            throw new Exception("This work schedule overlaps with an existing schedule.");

        // Create new schedule
        var workSchedule = new WorkSchedule
        {
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            DayOfWeek = request.DayOfWeek,
            IsAvailable = request.IsAvailable,
            DoctorId = request.DoctorId
        };

        await unitOfWork.WorkScheduleRepository.AddAsync(workSchedule);
        await unitOfWork.SaveChangesAsync();

        return workSchedule.Id;
    }
}

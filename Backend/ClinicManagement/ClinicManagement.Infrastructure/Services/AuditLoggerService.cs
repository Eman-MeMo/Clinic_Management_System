using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class AuditLoggerService : IAuditLoggerService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<AuditLoggerService> logger;

    public AuditLoggerService(IUnitOfWork unitOfWork, ILogger<AuditLoggerService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task LogAsync(string action, string tableName, string details, string userId)
    {
        logger.LogInformation($"{action} action performed on {tableName} by User ID: {userId}. Details: {details}");

        await unitOfWork.AuditLogRepository.AddAsync(new AuditLog
        {
            Action = action,
            TableName = tableName,
            Timestamp = DateTime.UtcNow,
            Details = details,
            AppUserId = userId
        });

        await unitOfWork.SaveChangesAsync();
    }
}

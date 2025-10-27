using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Application.Mapper;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ClinicManagement.Infrastructure.Repositories;
using ClinicManagement.Application.Interfaces;
using Serilog;
using Serilog.Formatting.Compact;
using ClinicManagement.Application.Behaviors;
using MediatR;
using ClinicManagement.Application.Commands.Appointments.BookAppointment;
using FluentValidation;
using ClinicManagement.Application.Strategies;
using ClinicManagement.Application.Services;
using ClinicManagement.Infrastructure.Services;
using ClinicManagement.API.Middlewares;

namespace ClinicManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog for logging from appsettings.json
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .CreateLogger();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure EF Core with SQL Server
            builder.Services.AddDbContext<ClinicDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ClinicManagementDb")));

            // Configure Identity
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ClinicDbContext>()
            .AddDefaultTokenProviders();

            // AutoMapper
            builder.Services.AddMappingProfiles();

            // MediatR
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(BookAppointmentCommand).Assembly));

            // FluentValidation
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                    )
                };
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Dependency Injection for Repositories and Services
                // Repositories
                builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
                builder.Services.AddScoped<IPatientRepository, PatientRepository>();
                builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
                builder.Services.AddScoped<IUserRepository<AppUser>, UserRepository<AppUser>>();
                builder.Services.AddScoped<IAttendanceRepository,AttendanceRepository>();
                builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
                builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
                builder.Services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
                builder.Services.AddScoped<IBillRepository, BillRepository>();
                builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
                builder.Services.AddScoped<ISessionServiceRepository, SessionServiceRepository>();
                builder.Services.AddScoped<ISessionRepository, SessionRepository>();
                builder.Services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();
                builder.Services.AddScoped<IAttendanceService, AttendanceService>();
                builder.Services.AddScoped<IAccountService, AccountService>();
                builder.Services.AddScoped<ISessionManagementService, SessionManagementService>();
                builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
                builder.Services.AddScoped<IPaymentService, PaymentService>();
                builder.Services.AddScoped<IBillingService, BillingService>();
                builder.Services.AddScoped<DoctorAvailabilityService>();
                builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

                // For Stragegy Pattern
                builder.Services.AddScoped<IUserDeactivationStrategy, DoctorDeactivationStrategy>();
                builder.Services.AddScoped<IUserDeactivationStrategy, PatientDeactivationStrategy>();


            // Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure Serilog as the logging provider
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Global exception handler middleware
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
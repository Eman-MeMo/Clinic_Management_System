using ClinicManagement.Application.Behaviors;
using ClinicManagement.Application.Commands.Appointments.BookAppointment;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Services;
using ClinicManagement.Application.Strategies;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;
using ClinicManagement.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ClinicManagement.Application.Mapper;
using System.Text;

namespace ClinicManagement.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLayer(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ClinicDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("ClinicManagementDb")));

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ClinicDbContext>()
            .AddDefaultTokenProviders();

            // Repositories
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<ISessionServiceRepository, SessionServiceRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(BookAppointmentCommand).Assembly));

            services.AddValidatorsFromAssembly(typeof(BookAppointmentCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddMappingProfiles();

            // Services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IBillingService, BillingService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();
            services.AddScoped<ISessionManagementService, SessionManagementService>();
            services.AddScoped(typeof(IUserService<>), typeof(UserService<>));
            services.AddScoped<DoctorAvailabilityService>();

            // Strategy Pattern
            services.AddScoped<IUserDeactivationStrategy, DoctorDeactivationStrategy>();
            services.AddScoped<IUserDeactivationStrategy, PatientDeactivationStrategy>();
            services.AddScoped<IUserDeactivationStrategy, AdminDeactivationStrategy>();

            return services;
        }
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
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
                   ValidIssuer =config["Jwt:Issuer"],
                   ValidAudience = config["Jwt:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(config["Jwt:Key"])
                   )
               };
           });
            return services;
        }
        public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;

        }
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            return services;
        }
        }
}

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
using Microsoft.OpenApi.Models;
using ClinicManagement.API.Extensions;

namespace ClinicManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Serilog
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .CreateLogger();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Host.UseSerilog();

            builder.Services.AddApplicationLayer();
            builder.Services.AddInfrastructureLayer(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddSwaggerWithAuth();
            builder.Services.AddCorsPolicy();

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
            app.UseMiddleware<ActiveUserMiddleware>();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.PrescriptionDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Services
{
    public class PrescriptionService: IPrescriptionService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMedicalRecordService medicalRecordService;
        public PrescriptionService(IUnitOfWork _unitOfWork,IMapper _mapper,IMedicalRecordService _medicalRecordService)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            medicalRecordService = _medicalRecordService;
        }
        public async Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto dto)
        {
            var session = await unitOfWork.SessionRepository.GetByIdWithAttendanceAsync(dto.SessionId);

            if (session == null)
                throw new Exception("Session not found.");

            if (session.Attendance == null || !session.Attendance.IsPresent)
                throw new Exception("Cannot create prescription for an absent patient.");

            if (session.Status != SessionStatus.Confirmed)
                throw new Exception("The session must be completed before writing a prescription.");

            var prescriptions = await unitOfWork.PrescriptionRepository.GetBySessionIdAsync(dto.SessionId);
            if (prescriptions.Any()) 
                throw new Exception("Prescription already exists for this session.");

            var prescription = mapper.Map<Prescription>(dto);

            await unitOfWork.PrescriptionRepository.AddAsync(prescription);
            await unitOfWork.SaveChangesAsync();

            await medicalRecordService.CreateMedicalRecordAsync(dto.Notes, dto.Diagnosis, prescription.Id);
            return mapper.Map<PrescriptionDto>(prescription);
        }
    }
}

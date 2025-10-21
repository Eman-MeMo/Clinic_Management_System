using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IUnitOfWork unitOfWork;
        public MedicalRecordService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public async Task<int> CreateMedicalRecordAsync(string? Notes, string Diagnosis, int prescriptionId)
        {
            var prescription = await unitOfWork.PrescriptionRepository.GetAllAsQueryable()
                .Include(p => p.Session)
                .ThenInclude(s => s.Appointment)
                .SingleOrDefaultAsync(p => p.Id == prescriptionId);

            if (prescription == null || prescription.Session == null || prescription.Session.Appointment == null)
                throw new Exception("Prescription or related session/appointment not found.");

            var medicalRecord = new MedicalRecord
            {
                PatientId = prescription.Session.PatientId,
                DoctorId = prescription.Session.DoctorId,
                Date = prescription.Session.Appointment.Date.Date,
                Notes = Notes,
                Diagnosis = Diagnosis
            };
            await unitOfWork.MedicalRecordRepository.AddAsync(medicalRecord);
            await unitOfWork.SaveChangesAsync();
            return medicalRecord.Id;
        }
    }
}

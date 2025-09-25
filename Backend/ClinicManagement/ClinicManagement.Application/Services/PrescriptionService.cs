using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ClinicManagement.Infrastructure.Services
{
    public class PrescriptionService: IPrescriptionService
    {
        private readonly IUnitOfWork unitOfWork;
        public PrescriptionService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task CreateMedicalRecoredAsync(string? Notes, string Diagnosis, int prescriptionId)
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
                Date = prescription.Session.Appointment.Date.Date,
                Notes = Notes,
                Diagnosis = Diagnosis
            };
            await unitOfWork.MedicalRecordRepository.AddAsync(medicalRecord);
            await unitOfWork.SaveChangesAsync();
        }
    }
}

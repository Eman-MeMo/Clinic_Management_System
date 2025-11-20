using ClinicManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Strategies
{
    public class PatientDeactivationStrategy : IUserDeactivationStrategy
    {
        private readonly IUnitOfWork unitOfWork;

        public PatientDeactivationStrategy(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public bool CanHandle(string userType) => userType.Equals("Patient", StringComparison.OrdinalIgnoreCase);
        public async Task DeactivateAsync(string patientId)
        {
            var patient = await unitOfWork.PatientRepository.GetByIdAsync(patientId);
            patient.IsActive = false;
            unitOfWork.PatientRepository.Update(patient);

            await unitOfWork.AppointmentRepository.CancelAllAppointmentsForPatient(patientId);
        }
    }

}

using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Services
{
    public class PatientService:UserService<Patient>, IPatientService
    {
        private readonly IPatientRepository patientRepository;

        public PatientService(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
            : base(patientRepository, unitOfWork)
        {
            this.patientRepository = patientRepository;
        }
        public async Task<Patient> GetByNationalIdAsync(string nationId)
        {
            return await patientRepository.GetByNationalIdAsync(nationId);
        }
    }
}

using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Services
{
    public class DoctorService : UserService<Doctor>, IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
            : base(doctorRepository, unitOfWork)
        {
            this.doctorRepository = doctorRepository;
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId)
        {
            if (specializationId <= 0)
            {
                throw new ArgumentException("Specialization ID must be a positive integer.", nameof(specializationId));
            }
            return await doctorRepository.GetAllBySpecializationAsync(specializationId);
        }
    }
}

using ClinicManagement.Application.Interfaces;
namespace ClinicManagement.Application.Strategies
{
    public class DoctorDeactivationStrategy : IUserDeactivationStrategy
    {
        private readonly IUnitOfWork unitOfWork;

        public DoctorDeactivationStrategy(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public bool CanHandle(string userType) => userType.Equals("Doctor", StringComparison.OrdinalIgnoreCase);
        public async Task DeactivateAsync(string doctorId)
        {
            var hasFutureAppointments = await unitOfWork.AppointmentRepository.HasAppointmentForDoctorAsync(doctorId);
            if (hasFutureAppointments)
                throw new InvalidOperationException("Doctor has upcoming appointments.");

            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(doctorId);
            doctor.IsActive = false;
            unitOfWork.DoctorRepository.Update(doctor);
            await unitOfWork.SaveChangesAsync();
        }
    }
}

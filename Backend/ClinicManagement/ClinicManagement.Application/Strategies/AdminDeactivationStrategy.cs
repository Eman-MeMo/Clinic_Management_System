using ClinicManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Strategies
{
    public class AdminDeactivationStrategy : IUserDeactivationStrategy
    {
        private readonly IUnitOfWork unitOfWork;

        public AdminDeactivationStrategy(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public bool CanHandle(string userType) => userType.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        public async Task DeactivateAsync(string adminId)
        {
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(adminId);
            admin.IsActive = false;
            unitOfWork.AdminRepository.Update(admin);
            await unitOfWork.SaveChangesAsync();
        }
    }
}

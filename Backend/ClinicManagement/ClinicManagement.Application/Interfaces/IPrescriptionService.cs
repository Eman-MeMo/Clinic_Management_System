using ClinicManagement.Domain.DTOs.PrescriptionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto dto);
    }
}

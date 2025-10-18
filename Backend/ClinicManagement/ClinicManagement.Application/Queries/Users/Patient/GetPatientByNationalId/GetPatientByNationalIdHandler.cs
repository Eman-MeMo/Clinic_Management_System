using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Users.Patient.GetPatientByNationalId
{
    public class GetPatientByNationalIdHandler : IRequestHandler<GetPatientByNationalIdQuery, PatientDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetPatientByNationalIdHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public Task<PatientDto> Handle(GetPatientByNationalIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var patient = unitOfWork.PatientRepository.GetByNationalIdAsync(request.NationalId);
            return mapper.Map<Task<PatientDto>>(patient);
        }
    }
}

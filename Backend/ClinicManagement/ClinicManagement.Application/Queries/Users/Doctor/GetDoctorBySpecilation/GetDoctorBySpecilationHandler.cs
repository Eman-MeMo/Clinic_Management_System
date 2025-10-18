using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.DoctorDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Users.Doctor.GetDoctorBySpecilation
{
    public class GetDoctorBySpecilationHandler:IRequestHandler<GetDoctorBySpecilationQuery, IEnumerable<DoctorDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetDoctorBySpecilationHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<DoctorDto>> Handle(GetDoctorBySpecilationQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var doctors = await unitOfWork.DoctorRepository.GetAllBySpecializationAsync(request.SpecializationId);
            return mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
    }
}

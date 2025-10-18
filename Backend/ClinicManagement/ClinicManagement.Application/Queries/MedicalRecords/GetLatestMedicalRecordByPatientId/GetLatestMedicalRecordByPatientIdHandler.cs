using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.MedicalRecords.GetLatestMedicalRecordByPatientId
{
    public class GetLatestMedicalRecordByPatientIdHandler:IRequestHandler<GetLatestMedicalRecordByPatientIdQuery, MedicalRecordDto>  
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetLatestMedicalRecordByPatientIdHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        public async Task<MedicalRecordDto> Handle(GetLatestMedicalRecordByPatientIdQuery request, CancellationToken cancellationToken)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            var medicalRecord = await unitOfWork.MedicalRecordRepository.GetLatestRecordAsync(request.PatientId);
            return mapper.Map<MedicalRecordDto>(medicalRecord);
        }
    }
}

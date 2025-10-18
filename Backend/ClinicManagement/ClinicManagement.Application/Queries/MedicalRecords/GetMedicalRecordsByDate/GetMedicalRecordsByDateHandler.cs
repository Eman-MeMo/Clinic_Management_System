using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.MedicalRecords.GetMedicalRecordsByDate
{
    public class GetMedicalRecordsByDateHandler:IRequestHandler<GetMedicalRecordsByDateQuery,IEnumerable<MedicalRecordDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetMedicalRecordsByDateHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<MedicalRecordDto>> Handle(GetMedicalRecordsByDateQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var records = await unitOfWork.MedicalRecordRepository.GetByDateAsync(request.date);
            return mapper.Map<IEnumerable<MedicalRecordDto>>(records);
        }
    }
}

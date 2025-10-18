using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.BillDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Bills.GetUnpaidBillsByPatient
{
    public class GetUnpaidBillsByPatientHandler:IRequestHandler<GetUnpaidBillsByPatientQuery, IEnumerable<BillDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetUnpaidBillsByPatientHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<BillDto>> Handle(GetUnpaidBillsByPatientQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var bills = await unitOfWork.BillRepository.GetUnpaidBillsByPatientAsync(request.PatientId);
            return mapper.Map<IEnumerable<BillDto>>(bills);
        }
    }
}

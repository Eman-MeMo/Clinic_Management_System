using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.PaymentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Payments.GetPaymentsByPatient
{
    public class GetPaymentsByPatientHandler : IRequestHandler<GetPaymentsByPatientQuery, IEnumerable<PaymentDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetPaymentsByPatientHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentsByPatientQuery request, CancellationToken cancellationToken)
        {
            if(request == null) 
                throw new ArgumentNullException(nameof(request));

            var payments= await unitOfWork.PaymentRepository.GetByPatientAsync(request.PatientId);
            return mapper.Map<IEnumerable<PaymentDto>>(payments);
        }
    }
}

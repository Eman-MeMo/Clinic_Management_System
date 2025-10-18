using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.PaymentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Payments.GetPaymentsByDateRange
{
    public class GetPaymentsByDateRangeHandler:IRequestHandler<GetPaymentsByDateRangeQuery,IEnumerable<PaymentDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetPaymentsByDateRangeHandler(IUnitOfWork _unitOfWork,IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentsByDateRangeQuery request, CancellationToken cancellationToken)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            var paymnets= await unitOfWork.PaymentRepository.GetByDateRangeAsync(request.start, request.end);
            return mapper.Map<IEnumerable<PaymentDto>>(paymnets);
        }
    }
}

using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.BillDTOs;
using ClinicManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Bills.CreateBill
{
    public class CreateBillHandler:IRequestHandler<CreateBillCommand,int>
    {
        private readonly IBillingService billingService;
        private readonly IUnitOfWork unitOfWork;
        public CreateBillHandler(IBillingService _billingService,IUnitOfWork _unitOfWork) { 
            billingService = _billingService;
            unitOfWork = _unitOfWork;
        }

        public async Task<int> Handle(CreateBillCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await billingService.CreateBillAsync(request.PatientId, request.SessionId);
        }
    }
}

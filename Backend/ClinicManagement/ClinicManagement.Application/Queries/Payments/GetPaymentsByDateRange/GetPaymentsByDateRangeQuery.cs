using ClinicManagement.Domain.DTOs.PaymentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Payments.GetPaymentsByDateRange
{
    public class GetPaymentsByDateRangeQuery:IRequest<IEnumerable<PaymentDto>>
    {
        public DateTime start {  get; set; }
        public DateTime end { get; set; }
    }
}

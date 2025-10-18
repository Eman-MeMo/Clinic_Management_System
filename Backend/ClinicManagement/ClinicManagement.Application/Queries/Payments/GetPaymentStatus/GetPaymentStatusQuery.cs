using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Payments.GetPaymentStatus
{
    public class GetPaymentStatusQuery:IRequest<bool>
    {
        public int billId { get; set; }
    }
}

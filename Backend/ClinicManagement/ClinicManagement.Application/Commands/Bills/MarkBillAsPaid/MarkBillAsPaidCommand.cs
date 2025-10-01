using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Bills.MarkBillAsPaid
{
    public class MarkBillAsPaidCommand:IRequest<bool>
    {
        public int Id { get; set; }
    }
}

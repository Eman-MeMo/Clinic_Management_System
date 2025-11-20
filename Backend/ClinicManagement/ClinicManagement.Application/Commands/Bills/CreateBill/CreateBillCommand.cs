using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Bills.CreateBill
{
    public class CreateBillCommand:IRequest<int>
    {
        public int SessionId { get; set; }
        public string PatientId { get; set; }
    }
}

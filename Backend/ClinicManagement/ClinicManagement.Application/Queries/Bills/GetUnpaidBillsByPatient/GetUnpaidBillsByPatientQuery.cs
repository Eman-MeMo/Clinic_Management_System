using ClinicManagement.Domain.DTOs.BillDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Bills.GetUnpaidBillsByPatient
{
    public class GetUnpaidBillsByPatientQuery:IRequest<IEnumerable<BillDto>>
    {
        public string PatientId { get; set; }
    }
}

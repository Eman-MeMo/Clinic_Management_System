using ClinicManagement.Domain.DTOs.PaymentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Payments.GetPaymentsByPatient
{
    public class GetPaymentsByPatientQuery:IRequest<IEnumerable<PaymentDto>>
    {
        public string PatientId { get; set; }
    }
}

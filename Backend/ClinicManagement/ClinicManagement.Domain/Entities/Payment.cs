using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class Payment: BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        [ForeignKey("Bill")]
        public int BillId { get; set; }
        public Bill Bill { get; set; }

    }
}

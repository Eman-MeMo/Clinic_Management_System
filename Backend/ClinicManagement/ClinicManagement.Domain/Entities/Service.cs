using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class Service: BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public ICollection<SessionService> SessionServices { get; set; } = new List<SessionService>();
    }
}

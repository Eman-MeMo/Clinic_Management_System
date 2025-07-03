using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entities
{
    public class AuditLog: BaseEntity
    {
        public string Action { get; set; } 
        public string TableName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }

        [ForeignKey("AppUser")]
        public string AppUserId { get; set; } 
        public AppUser AppUser { get; set; }
    }
}

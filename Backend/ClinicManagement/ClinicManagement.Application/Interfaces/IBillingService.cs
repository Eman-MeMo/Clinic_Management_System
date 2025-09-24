﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IBillingService
    {
        Task<bool> MarkAsPaidAsync(int billId);
    }
}

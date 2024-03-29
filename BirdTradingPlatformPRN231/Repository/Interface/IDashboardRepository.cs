﻿using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IDashboardRepository
    {
        public StoreDashboardDTO GetStoreDashboard(long currentUser);
        public CustomerDashboardDTO GetHomePageProducts();
    }
}

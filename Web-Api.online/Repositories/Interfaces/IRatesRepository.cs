﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Web_Api.online.Models.StoredProcedures;
using Web_Api.online.Models.Tables;

namespace Web_Api.online.Repositories
{
    public interface IRatesRepository
    {
        public Rate GetLastRates();

        public Task<List<spGetTickerRatesResult>> GetTickerInformationAsync();
    }
}

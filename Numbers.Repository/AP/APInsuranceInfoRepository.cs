using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
    public class APInsuranceInfoRepository : BaseRepository<APInsuranceInfo>, IAPInsuranceInfoRepository
    {
        public APInsuranceInfoRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

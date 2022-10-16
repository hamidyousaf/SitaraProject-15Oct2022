using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
   public class APCustomInfoRepository : BaseRepository<APCustomInfo>, IAPCustomInfoRepository
    {
        public APCustomInfoRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

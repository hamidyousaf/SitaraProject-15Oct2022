using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
   public class APOGPRepository : BaseRepository<APOGP>, IAPOGPRepository
    {
        public APOGPRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

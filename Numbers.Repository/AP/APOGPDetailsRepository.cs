using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
    public class APOGPDetailsRepository : BaseRepository<APOGPDetails>, IAPOGPDetailsRepository
    {
        public APOGPDetailsRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

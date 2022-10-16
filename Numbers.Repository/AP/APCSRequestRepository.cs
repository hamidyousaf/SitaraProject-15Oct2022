using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface.AP;

namespace Numbers.Repository.AP
{
    public class APCSRequestRepository : BaseRepository<APCSRequest>, IAPCSRequestRepository
    {
        public APCSRequestRepository(NumbersDbContext context) : base(context)
        {

        }


    }
}

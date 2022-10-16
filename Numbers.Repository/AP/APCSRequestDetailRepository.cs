using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;
using Numbers.Interface.AP;

namespace Numbers.Repository.AP
{
    public class APCSRequestDetailRepository : BaseRepository<APCSRequestDetail>, IAPCSRequestDetailRepository
    {
        public APCSRequestDetailRepository(NumbersDbContext context) : base(context)
        {

        }


    }
}

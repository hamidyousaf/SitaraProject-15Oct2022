using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
    public class APLCRepository : BaseRepository<APLC>, IAPLCRepository
    {
        public APLCRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

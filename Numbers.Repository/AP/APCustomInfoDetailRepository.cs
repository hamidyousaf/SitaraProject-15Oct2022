using Numbers.Entity.Models;
using Numbers.Interface.AP;
using Numbers.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
   public class APCustomInfoDetailRepository : BaseRepository<APCustomInfoDetails>,IAPCustomInfoDetailRepository
    {
        public APCustomInfoDetailRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

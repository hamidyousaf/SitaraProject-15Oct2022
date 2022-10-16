using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
   public class APIRNRepository: BaseRepository<APIRN>, IAPIRNRepository
    {
        public APIRNRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

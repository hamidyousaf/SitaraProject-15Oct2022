using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
   public  class APIGPRepository:BaseRepository<APIGP>,IAPIGPRepository
    {
        public APIGPRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
   public class APIGPDetailsRepository:BaseRepository<APIGPDetails>,IAPIGPDetailsRepository
    {
        public APIGPDetailsRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

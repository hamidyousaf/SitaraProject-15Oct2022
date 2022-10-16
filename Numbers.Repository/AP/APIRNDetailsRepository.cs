using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
    public class APIRNDetailsRepository : BaseRepository<APIRNDetails>, IAPIRNDetailsRepository
    {
        public APIRNDetailsRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

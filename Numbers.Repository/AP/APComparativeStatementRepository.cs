using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface.AP;

namespace Numbers.Repository.AP
{
    public class APComparativeStatementRepository : BaseRepository<APComparativeStatement>, IAPComparativeStatementRepository
    {
        public APComparativeStatementRepository(NumbersDbContext context) : base(context)
        {

        }


    }
}

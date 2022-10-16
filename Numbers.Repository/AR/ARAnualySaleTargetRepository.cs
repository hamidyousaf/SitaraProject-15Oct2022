using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AR
{
    public class ARAnualySaleTargetRepository : BaseRepository<ARAnnualSaleTargets>, IARAnnuallySaleTarget
    {
        public ARAnualySaleTargetRepository(NumbersDbContext context) : base(context)
        {

        }

    }
}

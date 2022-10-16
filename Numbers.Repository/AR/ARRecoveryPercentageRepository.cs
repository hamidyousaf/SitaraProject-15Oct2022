using Numbers.Entity.Models;
using Numbers.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AR
{
    public class ARRecoveryPercentageRepository : BaseRepository<ARRecoveryPercentage>, IARRecoveryPercentageRepository
    {
        public ARRecoveryPercentageRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

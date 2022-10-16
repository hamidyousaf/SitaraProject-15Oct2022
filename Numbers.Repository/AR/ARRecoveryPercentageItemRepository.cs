using Numbers.Entity.Models;
using Numbers.Interface.AR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AR
{
    public class ARRecoveryPercentageItemRepository : BaseRepository<ARRecoveryPercentageItem>, IARRecoveryPercentageItemRepository
    {
        public ARRecoveryPercentageItemRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}
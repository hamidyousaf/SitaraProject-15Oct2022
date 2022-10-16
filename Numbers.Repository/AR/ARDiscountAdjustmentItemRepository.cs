using Numbers.Entity.Models;
using Numbers.Interface;
using Numbers.Interface.AR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AR
{
   public class ARDiscountAdjustmentItemRepository : BaseRepository<ARDiscountAdjustmentItem>, IARDiscountAdjustmentItemRepository
    {
        public ARDiscountAdjustmentItemRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

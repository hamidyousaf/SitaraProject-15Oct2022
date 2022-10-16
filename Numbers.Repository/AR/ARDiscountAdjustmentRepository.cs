using Numbers.Entity.Models;
using Numbers.Interface.AR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AR
{
    public class ARDiscountAdjustmentRepository : BaseRepository<ARDiscountAdjustment>, IARDiscountAdjustmentRepository
    {
        public ARDiscountAdjustmentRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}
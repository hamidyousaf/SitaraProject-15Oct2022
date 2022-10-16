using System;
using System.Collections.Generic;
using System.Text;
using Numbers.Entity.Models;
using Numbers.Interface.AR;
namespace Numbers.Repository.AR
{
    public class ARCustomerDiscountAdjustmentRepository:BaseRepository<ARCustomerDiscountAdjustment>,IARCustomerDiscountAdjustmentRepository
    {
        public ARCustomerDiscountAdjustmentRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

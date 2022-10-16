using System;
using System.Collections.Generic;
using System.Text;
using Numbers.Entity.Models;
using Numbers.Interface.AR;
namespace Numbers.Repository.AR
{
    public class ARCustomerAdjustmentItemRepository:BaseRepository<ARCustomerAdjustmentItem>,IARCustomerAdjustmentItemRepository
    {
        public ARCustomerAdjustmentItemRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

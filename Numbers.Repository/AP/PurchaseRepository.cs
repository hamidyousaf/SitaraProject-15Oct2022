using Numbers.Entity.Models;
using Numbers.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
    public class PurchaseRepository : BaseRepository<APPurchase>, IAPPurchaseRepository
    {
        public PurchaseRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

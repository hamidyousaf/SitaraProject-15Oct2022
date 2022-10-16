using System;
using System.Collections.Generic;
using System.Text;
using Numbers.Interface.AP;
using Numbers.Entity.Models;

namespace Numbers.Repository.AP
{
   public class APPurchaseRequisitionRepository : BaseRepository<APPurchaseRequisition>,IAPPurchaseRequisitionRepository 
    {
        public APPurchaseRequisitionRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

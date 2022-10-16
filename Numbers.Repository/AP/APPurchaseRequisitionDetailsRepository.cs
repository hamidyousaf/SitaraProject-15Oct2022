using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Numbers.Interface.AP;

namespace Numbers.Repository.AP
{
  public  class APPurchaseRequisitionDetailsRepository:BaseRepository<APPurchaseRequisitionDetails>,IAPPurchaseRequisitionDetailsRepository
    {
        public APPurchaseRequisitionDetailsRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

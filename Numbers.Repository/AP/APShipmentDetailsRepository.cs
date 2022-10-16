using Numbers.Entity.Models;
using Numbers.Interface.AP;
using Numbers.Repository;
using Numbers.Repository.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
    public class APShipmentDetailsRepository : BaseRepository<APShipmentDetail>, IAPShipmentDetailsRepository
    {
        public APShipmentDetailsRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

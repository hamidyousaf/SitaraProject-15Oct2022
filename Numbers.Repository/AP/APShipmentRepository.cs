using Numbers.Entity.Models;
using Numbers.Interface;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
    public class APShipmentRepository : BaseRepository<APShipment>, IAPShipmentRepository
    {
        public APShipmentRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}
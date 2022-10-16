using Numbers.Entity.Models;
using Numbers.Interface;
using Numbers.Interface.AR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AR
{
   public class ARDiscountItemRepository: BaseRepository<ARDiscountItem>, IARDiscountItemRepository
    {
        public ARDiscountItemRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

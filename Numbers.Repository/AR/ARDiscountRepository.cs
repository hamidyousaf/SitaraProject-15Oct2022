using Numbers.Entity.Models;
using Numbers.Interface.AR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AR
{
    public class ARDiscountRepository : BaseRepository<ARDiscount>, IARDiscountRepository
    {
        public ARDiscountRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}
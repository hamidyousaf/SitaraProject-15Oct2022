using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AR
{
    public class ARSalePersonItemCategoryRepository : BaseRepository<ARSalePersonItemCategory>, IARSalePersonItemCategory
    {
        public ARSalePersonItemCategoryRepository(NumbersDbContext context) : base(context)
        {

        }

    }
}

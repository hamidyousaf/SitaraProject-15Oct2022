using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AR
{
    public class ARSalePersonCityRepository : BaseRepository<ARSalePersonCity>, IARSalePersonCity
    {
        public ARSalePersonCityRepository(NumbersDbContext context) : base(context)
        {

        }

    }
}

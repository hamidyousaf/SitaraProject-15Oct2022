using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.Inventory
{
    public class ItemPricingsRepository : BaseRepository<ItemPricings>, IItemPricingsRepository
    {
        public ItemPricingsRepository(NumbersDbContext context) : base(context)
        {

        }


    }
}

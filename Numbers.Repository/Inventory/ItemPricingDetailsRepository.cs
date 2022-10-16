using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Interface;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Numbers.Repository.Inventory
{
    public class ItemPricingDetailsRepository : BaseRepository<ItemPricingDetails>, IItemPricingDetailsRepository
    {
        public ItemPricingDetailsRepository(NumbersDbContext context) : base(context)
        {

        }


    }
}


using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numbers.Repository.Inventory
{
    public class AdjustmentRepo
    {
        private readonly NumbersDbContext _dbContext;
        public AdjustmentRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<InvAdjustment> GetAll(int companyId)
        {
            IEnumerable<InvAdjustment> listRepo = _dbContext.InvAdjustments.Include(w => w.WareHouse)
                                                 .Where(a => !a.IsDeleted && a.CompanyId == companyId).ToList();
            return listRepo;
        }

        //public static InvAdjustmentItem[] GetAdjustmentItems()
        //{

        //}
    }
}

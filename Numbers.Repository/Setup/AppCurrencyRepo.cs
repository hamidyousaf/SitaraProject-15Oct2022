using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numbers.Repository.Setup
{
    public class AppCurrencyRepo
    {

        private static readonly NumbersDbContext _dbContext;
        static AppCurrencyRepo()
        {
            _dbContext = new NumbersDbContext();
        }
        public static List<AppCurrency> GetCurrencies()
        {
            List<AppCurrency> list = new List<AppCurrency>();
            list = _dbContext.AppCurrencies.Where(c => c.IsActive).OrderBy(c => c.Sequence).OrderBy(c => c.Sequence).ToList();
            return list;
        }
        
    }
}

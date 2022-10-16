using Numbers.Repository.Helpers;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Numbers.Repository.Setup
{
    class AppConfigRepo : SessionBase
    {
        public static IEnumerable<AppCompanyConfigBase> GetConfigurations()
        {
            IEnumerable<AppCompanyConfigBase> config = _dbContext.AppCompanyConfigBases.Where(x => x.CompanyId == _companyId);
            return config;
        }
        public static AppCompanyConfig GetConfigById(int id)
        {
            AppCompanyConfig config = _dbContext.AppCompanyConfigs.Find(id);//. .Where(e => e.Id == id).FirstOrDefault();
            return config;
        }

    }
}

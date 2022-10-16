using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.Helpers
{
    public class SessionBase
    {
        protected static readonly string _userId = AppSession.GetUserId;
        protected static readonly int _companyId = AppSession.GetCompanyId;
        protected static NumbersDbContext _dbContext = new NumbersDbContext(AppSession.GetHttpContextAccessor);
    }
}

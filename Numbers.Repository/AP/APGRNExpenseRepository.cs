using Numbers.Entity.Models;
using Numbers.Interface.AP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AP
{
    public class APGRNExpenseRepository : BaseRepository<APGRNExpense>, IAPGRNExpenseRepository
    {
        public APGRNExpenseRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AppModule
{
    public class Sys_Rules_ApprovalRepository : BaseRepository<Sys_Rules_Approval>, ISys_Rules_ApprovalRepository
    {
        public Sys_Rules_ApprovalRepository(NumbersDbContext context) : base(context)
        {

        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AppModule
{
    public class SysApprovalGroupDetailsRepository : BaseRepository<SysApprovalGroupDetails>, ISysApprovalGroupDetailsRepository
    {
        public SysApprovalGroupDetailsRepository(NumbersDbContext context) : base(context)
        {

        }

    }
}

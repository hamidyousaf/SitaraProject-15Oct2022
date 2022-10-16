using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AppModule
{
    public class SysOrganizationRepository : BaseRepository<SysOrganization>, ISysOrganizationRepository
    {
        public SysOrganizationRepository(NumbersDbContext context) : base(context)
        {

        }


    }
}
